﻿using System;
using System.Threading.Tasks;

namespace SLAM.Models {

    using Events;
    using Data.Readers;
    using Data.Writers;
    using Map;

    public class Model : IDisposable {

        public event ModelUpdatedEvent OnModelUpdated;

        private DataProvider reader;
        private BaseWriter   writer;
        private BaseMapper   mapper;

        private FramesConverter framesConverter;

        public string CurrentState { get; private set; } = "Ready";
        public bool Ready { get; private set; } = true;
        public int FramesCount { get { return reader.TotalFrames; } }

        public int MapActualWidth { get { return mapper.ActualWidth; } }
        public int MapActualHeight { get { return mapper.ActualHeight; } }

        public Model(ModelUpdatedEvent onModelUpdated) {
            OnModelUpdated += onModelUpdated;
            MapperSwitchOffline();
        }

        public void MapperSwitchOnline() {
            reader = new KinectDeviceReader();
            Initialize();
        }

        public void MapperSwitchOffline() {
            reader = new KinectFileReader();
            Initialize();
        }

        private void Initialize() {
            mapper = new MatrixBasedMapper(reader);
            framesConverter = new FramesConverter(mapper);
        }

        private void ChangeState(string newModelState, bool lockModel = false) {
            CurrentState = newModelState;
            Ready = !lockModel;
            OnModelUpdated?.Invoke();
        }

        public Task CalculateFramesCountAsync() {
            Task calculateFramesCountTask = new Task(() => {
                ChangeState("Calculate Frames Count", true);
                (reader as FileReader)?.CalculateFramesCount();
                ChangeState("Ready");
            });
            calculateFramesCountTask.Start();
            return calculateFramesCountTask;
        }

        public bool Start(string fileName) {
            return (bool)(reader as FileReader)?.Start(fileName);
        }

        public void MoveToPosition(int frameIndex) {
            (reader as FileReader)?.MoveToPosition(frameIndex);
        }

        public void Stop() {
            reader?.Stop();
        }

        public byte[] GetActualMapFrame() {
            return framesConverter.GetActualMapFrame();
        }

        public Task<byte[]> GetActualMapFrameAsync() {
            Task<byte[]> getActualMapFrame = new Task<byte[]>(() => {
                ChangeState("Calculate Map", true);
                byte[] result = framesConverter.GetActualMapFrame();
                ChangeState("Ready");
                return result;                
            });
            getActualMapFrame.Start();
            return getActualMapFrame;
        }

        public byte[] GetActualTopDepthFrame() {
            return framesConverter.GetActualTopDepthFrame();
        }

        public byte[] GetActualFrontDepthFrame() {
            return framesConverter.GetActualFrontDepthFrame();
        }

        public void Dispose() {

        }
    }
}