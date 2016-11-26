using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SLAM.Models {

    public delegate void ModelUpdatedEvent();

    public sealed class Model : IDisposable {

        private DataReader reader;
        private Mapper mapper;

        public event ModelUpdatedEvent OnModelUpdated;

        public string CurrentState { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public string FullFileName { get { return reader.FullFileName; } }
        public int FramesCount { get { return reader.FramesCount; } }
        public int CurrentFrame { get { return reader.CurrentFrame; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            reader = new DataReader();
            mapper = new Mapper();
        }

        private void ChangeState(string newModelState, bool lockModel = false) {
            CurrentState = newModelState;
            Ready = !lockModel;
            OnModelUpdated?.Invoke();
        }

        public bool OpenFile(string fullFileName) {
            return reader.OpenFile(fullFileName);
        }

        public Task CalculateFramesCount() {
            Task calculateFramesCountTask = new Task(() => {
                ChangeState("Calculate Frames Count", true);
                reader.CalculateFramesCount();
                Thread.Sleep(333);
                ChangeState("Ready");
            });
            calculateFramesCountTask.Start();
            return calculateFramesCountTask;
        }

        public byte[] GetFullFrame(int frameIndex) {

            byte[] rawData = reader.ReadFrame(frameIndex);

            if (rawData != null) {

                byte[] resultFrame = new byte[reader.FrameInfo.FrameLength * sizeof(int)];

                int fullDepth = reader.FrameInfo.MaxDepth - reader.FrameInfo.MinDepth;
                double intencityStep = 192.0 / fullDepth;

                int colorPixelIndex = 0;
                for (int i = 0; i < reader.FrameInfo.FrameLength; ++i) {

                    short depth = rawData[i * sizeof(short) + 1];
                    short depthLowByte = rawData[i * sizeof(short)];

                    depth <<= 8;
                    depth |= depthLowByte;

                    depth >>= 3; // !! remove unused bits

                    byte intensity = (byte)(255 - Math.Round(depth * intencityStep));

                    if (depth < reader.FrameInfo.MinDepth) {
                        resultFrame[colorPixelIndex++] = 192;
                        resultFrame[colorPixelIndex++] = 128;
                        resultFrame[colorPixelIndex++] = 0;
                    }
                    else if (depth > reader.FrameInfo.MaxDepth) {
                        resultFrame[colorPixelIndex++] = 32;
                        resultFrame[colorPixelIndex++] = 0;
                        resultFrame[colorPixelIndex++] = 0;
                    }
                    else {
                        resultFrame[colorPixelIndex++] = intensity;
                        resultFrame[colorPixelIndex++] = intensity;
                        resultFrame[colorPixelIndex++] = intensity;
                    }
                    ++colorPixelIndex;

                }
                return resultFrame;
            }
            return null;
        }

        public void CloseFile() {
            reader.CloseFile();
        }

        public void Dispose() {
            reader.Dispose();
        }
    }
}