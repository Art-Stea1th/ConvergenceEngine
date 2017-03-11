using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace ConvergenceEngine.Models.IO {

    using Infrastructure;
    using Infrastructure.Interfaces;

    public sealed class KinectFileReader : IDataProvider {

        private readonly SequenceInfo sequenceInfo;
        private const int rightBadAreaSize = 8;

        private ConcurrentQueue<Tuple<List<Point>, short[,]>> frames; // ~ 610 kb
        private const int bufferLimit = 64;               // 610 * 64 ~ 38.125 mb

        private DateTime nextFrameRedyInvokeLast;
        private TimeSpan nextFrameRedyInvokeInterval;
        private double fps;
        private bool allRead;

        public event Action<IEnumerable<Point>> OnNextDepthLineReady;
        public event Action<short[,]> OnNextFullFrameReady;
        public event Action<DataProviderStates> OnStateChanged;

        public DataProviderStates State { get; private set; }

        public int FrameWidth { get { return sequenceInfo.Width; } }
        public int FrameHeight { get { return sequenceInfo.Height; } }

        public int MinDepth { get { return sequenceInfo.MinDepth; } }
        public int MaxDepth { get { return sequenceInfo.MaxDepth; } }

        public double FPS { get { return fps; } set { fps = LimitedValue(value, 0.5, 60.0); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double LimitedValue(double value, double min, double max) {
            return value < min ? min : value > max ? max : value;
        }

        public static KinectFileReader CreateReader(string fileName) {
            var sequenceInfo = SequenceInfo.Read(fileName);
            if (sequenceInfo == null) {
                return null;
            }
            if (sequenceInfo.IsValid) {
                return new KinectFileReader(sequenceInfo);
            }
            return null;
        }

        private KinectFileReader(SequenceInfo sequenceInfo) {
            this.sequenceInfo = sequenceInfo;
            FPS = 30.0;
            Initialize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeState(DataProviderStates newState) {
            if (newState == DataProviderStates.Stopped) {
                Initialize();
            }
            OnStateChanged?.Invoke(State = newState);
        }

        private void Initialize() {
            frames = new ConcurrentQueue<Tuple<List<Point>, short[,]>>();
            State = DataProviderStates.Stopped; allRead = false;
        }

        public void Start() {
            if (State == DataProviderStates.Stopped) {
                new Thread(() => FillBufferProcess()).Start();
                new Thread(() => GenerateSequenceProcess()).Start();
                ChangeState(DataProviderStates.Started);
            }
        }

        public void Stop() {
            ChangeState(DataProviderStates.Stopped);
        }

        private void FillBufferProcess() {
            using (var stream = new FileStream(sequenceInfo.FileName, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    reader.BaseStream.Position = sequenceInfo.FirstFramePosition;
                    while (State == DataProviderStates.Started && !allRead) {
                        FillBuffers(reader);
                        Thread.Sleep(10);
                    }
                }
            }
        }

        private void FillBuffers(BinaryReader reader) {

            int bytesPerFrame = sequenceInfo.BytesPerFrame;
            byte[] nextRawBuffer = new byte[bytesPerFrame];

            while (frames.Count < bufferLimit && !allRead) {
                if (reader.Read(nextRawBuffer, 0, bytesPerFrame) == bytesPerFrame) {
                    HorizontalMirror(nextRawBuffer);
                    short[,] nextDepthFrame = DepthsFrameFrom(nextRawBuffer);
                    List<Point> nextDepthLine = new List<Point>(DepthLineFrom(nextDepthFrame));
                    frames.Enqueue(new Tuple<List<Point>, short[,]>(nextDepthLine, nextDepthFrame));
                }
                else {
                    allRead = true;
                }                
            }
        }

        private void GenerateSequenceProcess() {

            Tuple<List<Point>, short[,]> nextFrame = null;

            while (State == DataProviderStates.Started && (frames.Count > 0 || !allRead)) {
                if (DateTime.Now >= nextFrameRedyInvokeLast + nextFrameRedyInvokeInterval) {
                    while (!frames.TryDequeue(out nextFrame)) {
                        Thread.Sleep(nextFrameRedyInvokeInterval.Milliseconds / 10);
                    }
                    NextFrameRedyInvoke(nextFrame.Item1, nextFrame.Item2);
                }
                Thread.Sleep(nextFrameRedyInvokeInterval.Milliseconds / 10);
            }
            ChangeState(DataProviderStates.Stopped);
        }

        private void NextFrameRedyInvoke(List<Point> nextLine, short[,] nextFrame) {
            OnNextDepthLineReady?.Invoke(nextLine);
            OnNextFullFrameReady?.Invoke(nextFrame);
            nextFrameRedyInvokeLast = DateTime.Now;
            nextFrameRedyInvokeInterval = TimeSpan.FromSeconds(1.0 / fps);
        }

        private void HorizontalMirror(byte[] frame) {

            int width  = sequenceInfo.Width * sizeof(short);
            int height = sequenceInfo.Height;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width / 2; ++x) {

                    int linearIndexNormal = GetLinearIndex(x, y, width);
                    int linearIndexInverse = GetLinearIndex((width - 1 - x), y, width);

                    byte tmp = frame[linearIndexNormal];
                    frame[linearIndexNormal] = frame[linearIndexInverse];
                    frame[linearIndexInverse] = tmp;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLinearIndex(int x, int y, int width) {
            return width * y + x;
        }

        private short[,] DepthsFrameFrom(byte[] rawFrame) {

            short[,] depthFrame = new short[sequenceInfo.Width - rightBadAreaSize, sequenceInfo.Height];

            for (int y = 0; y < sequenceInfo.Height; ++y) {
                for (int x = 0; x < sequenceInfo.Width - rightBadAreaSize; ++x) {
                    int i = GetLinearIndex(x * sizeof(short), y, sequenceInfo.Width * sizeof(short));
                    short nextDepth = rawFrame[i];
                    nextDepth <<= 8;
                    nextDepth |= (short)rawFrame[i + 1]; // <-- depth short construct
                    nextDepth >>= 3;                     // <-- remove 3 unused low bits & return
                    depthFrame[x, y] = nextDepth;
                }
            }
            return depthFrame;
        }

        private IEnumerable<Point> DepthLineFrom(short[,] depthFrame) {

            int width = depthFrame.GetLength(0);
            int y = depthFrame.GetLength(1) / 2;

            for (int x = 0; x < width; ++x) {
                int z = depthFrame[x, y];
                if (z < sequenceInfo.MinDepth || z > sequenceInfo.MaxDepth) {
                    continue;
                }
                yield return PerspectiveToRectangle(x, y, z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point PerspectiveToRectangle(double x, double y, double z) {
            return new Point((x - 320.0) * (0.003501 * 0.5) * (z * 0.1), z * 0.1);
        }

        public void Dispose() {
            Stop();
        }
    }
}