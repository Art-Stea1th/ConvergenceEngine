using System;
using System.IO;

namespace ConvergenceEngine.Models.IO {

    internal sealed class SequenceInfo {

        internal string FileName { get; private set; }

        private readonly float _focalLengthInPixels, _inverseFocalLengthInPixels;
        private readonly float _horizontalFOV, _verticalFOV, _diagonalFOV;
        private readonly int _minDepth, _maxDepth, _width, _height;

        internal float FocalLengthInPixels => _focalLengthInPixels;
        internal float InverseFocalLengthInPixels => _inverseFocalLengthInPixels;

        internal float HorizontalFOV => _horizontalFOV;
        internal float VerticalFOV => _verticalFOV;
        internal float DiagonalFOV => _diagonalFOV;

        internal int MinDepth => _minDepth;
        internal int MaxDepth => _maxDepth;
        internal int Width => _width;
        internal int Height => _height;

        internal int FirstFramePosition => sizeof(float) * 5 + sizeof(int) * 4;

        internal int BytesPerFrame => FrameLength * sizeof(short);
        internal int FrameLength => Width * Height;

        internal bool IsValid {
            get {
                if (FocalLengthInPixels <= 0.0f) { return false; }
                if (InverseFocalLengthInPixels <= 0.0f) { return false; }

                if (HorizontalFOV <= 0.0f) { return false; }
                if (VerticalFOV <= 0.0f) { return false; }
                if (DiagonalFOV <= 0.0f) { return false; }

                if (MinDepth < 0) { return false; }
                if (MaxDepth < 0 || MaxDepth > 8000) { return false; }

                if (Width < 0 || Width > 640) { return false; }
                if (Height < 0 || Height > 480) { return false; }

                return true;
            }
        }

        

        internal static SequenceInfo Read(string fileName) {
            try {
                var fileInfo = new FileInfo(fileName);
                using (var stream = fileInfo.OpenRead()) {
                    using (var reader = new BinaryReader(stream)) {
                        var result = new SequenceInfo(reader) {
                            FileName = fileName
                        };
                        return result;
                    }
                }
            }
            catch (Exception) { return null; }
        }

        private SequenceInfo(BinaryReader reader) {

            long savedPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;

            _focalLengthInPixels = reader.ReadSingle();
            _inverseFocalLengthInPixels = reader.ReadSingle();

            _horizontalFOV = reader.ReadSingle();
            _verticalFOV = reader.ReadSingle();
            _diagonalFOV = reader.ReadSingle();

            _minDepth = reader.ReadInt32();
            _maxDepth = reader.ReadInt32();

            _width = reader.ReadInt32();
            _height = reader.ReadInt32();

            reader.BaseStream.Position = savedPosition;
        }
    }
}