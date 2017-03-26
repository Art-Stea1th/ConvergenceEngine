using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ConvergenceEngine.Models.IO {

    using Infrastructure;
    using Infrastructure.Interfaces;

    public sealed class KinectFileReader : IDataProvider {

        private readonly SequenceInfo _sequenceInfo;
        private const int _rightBadAreaSize = 8;

        private ConcurrentQueue<(short[,] frame, List<Point> line)> _frames; // ~ 610 kb
        private const int _bufferLimit = 64;                     // 610 * 64 ~ 38.125 mb

        private DateTime _nextFrameRedyInvokeLast;
        private TimeSpan _nextFrameRedyInvokeInterval;
        private double _fps;
        private bool _allRead;

        public event Action<IEnumerable<Point>> OnNextDepthLineReady;
        public event Action<short[,]> OnNextFullFrameReady;
        public event Action<DataProviderStates> OnStateChanged;

        public DataProviderStates State { get; private set; }

        public int FrameWidth => _sequenceInfo.Width;
        public int FrameHeight => _sequenceInfo.Height;

        public int MinDepth => _sequenceInfo.MinDepth;
        public int MaxDepth => _sequenceInfo.MaxDepth;

        public double FPS { get => _fps; set => _fps = LimitedValue(value, 0.5, 60.0); }

        private double LimitedValue(double value, double min, double max) {
            return value < min ? min : value > max ? max : value;
        }

        public static KinectFileReader CreateReader(string fileName) {
            var sequenceInfo = SequenceInfo.Read(fileName);
            return sequenceInfo == null ? null : sequenceInfo.IsValid ? new KinectFileReader(sequenceInfo) : null;
        }

        private KinectFileReader(SequenceInfo sequenceInfo) {
            _sequenceInfo = sequenceInfo;
            FPS = 30.0;
            Initialize();
        }

        private void ChangeState(DataProviderStates newState) {
            if (newState == DataProviderStates.Stopped) {
                Initialize();
            }
            OnStateChanged?.Invoke(State = newState);
        }

        private void Initialize() {
            _frames = new ConcurrentQueue<(short[,] Frame, List<Point> Line)>();
            State = DataProviderStates.Stopped; _allRead = false;
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
            using (var stream = new FileStream(_sequenceInfo.FileName, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    reader.BaseStream.Position = _sequenceInfo.FirstFramePosition;
                    while (State == DataProviderStates.Started && !_allRead) {
                        FillBuffers(reader);
                        Thread.Sleep(10);
                    }
                }
            }
        }

        private void FillBuffers(BinaryReader reader) {

            int bytesPerFrame = _sequenceInfo.BytesPerFrame;
            var nextRawBuffer = new byte[bytesPerFrame];

            while (_frames.Count < _bufferLimit && !_allRead) {
                if (reader.Read(nextRawBuffer, 0, bytesPerFrame) == bytesPerFrame) {
                    HorizontalMirror(nextRawBuffer);
                    var nextDepthFrame = DepthsFrameFrom(nextRawBuffer);
                    _frames.Enqueue((frame: nextDepthFrame, line: DepthLineFrom(nextDepthFrame).ToList()));
                }
                else {
                    _allRead = true;
                }
            }
        }

        private void GenerateSequenceProcess() {

            while (State == DataProviderStates.Started && (_frames.Count > 0 || !_allRead)) {
                if (DateTime.Now >= _nextFrameRedyInvokeLast + _nextFrameRedyInvokeInterval
                    && _frames.TryDequeue(out var nextFrame)) {
                    NextFrameRedyInvoke(nextFrame.frame, nextFrame.line);
                }
                else {
                    Thread.Sleep(_nextFrameRedyInvokeInterval.Milliseconds / 10);
                }
            }
            ChangeState(DataProviderStates.Stopped);
        }

        private void NextFrameRedyInvoke(short[,] nextFrame, List<Point> nextLine) {
            OnNextFullFrameReady?.Invoke(nextFrame);
            OnNextDepthLineReady?.Invoke(nextLine);
            _nextFrameRedyInvokeLast = DateTime.Now;
            _nextFrameRedyInvokeInterval = TimeSpan.FromSeconds(1.0 / _fps);
        }

        private void HorizontalMirror(byte[] frame) {

            int width  = _sequenceInfo.Width * sizeof(short);
            int height = _sequenceInfo.Height;

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

        private int GetLinearIndex(int x, int y, int width) => width * y + x;

        private short[,] DepthsFrameFrom(byte[] rawFrame) {

            var depthFrame = new short[_sequenceInfo.Width - _rightBadAreaSize, _sequenceInfo.Height];

            for (int y = 0; y < _sequenceInfo.Height; ++y) {
                for (int x = 0; x < _sequenceInfo.Width - _rightBadAreaSize; ++x) {
                    int i = GetLinearIndex(x * sizeof(short), y, _sequenceInfo.Width * sizeof(short));
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
                if (z < _sequenceInfo.MinDepth || z > _sequenceInfo.MaxDepth) {
                    continue;
                }
                yield return PerspectiveToRectangle(x, y, z);
            }
        }

        private Point PerspectiveToRectangle(double x, double y, double z) {
            return new Point((x - 320.0) * (0.003501 * 0.5) * (z * 0.1), z * 0.1);
        }

        public void Dispose() => Stop();
    }
}