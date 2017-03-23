using System;
using System.IO;

namespace ConvergenceEngine.Models.IO {

    internal sealed class SequenceInfo {

        internal string FileName { get; private set; }

        internal readonly float FocalLengthInPixels;
        internal readonly float InverseFocalLengthInPixels;
        internal readonly float HorizontalFOV;
        internal readonly float VerticalFOV;
        internal readonly float DiagonalFOV;

        internal readonly int MinDepth;
        internal readonly int MaxDepth;
        internal readonly int Width;
        internal readonly int Height;

        internal int FirstFramePosition { get => sizeof(float) * 5 + sizeof(int) * 4; }

        internal int BytesPerFrame { get => FrameLength * sizeof(short); }
        internal int FrameLength { get => Width * Height; }

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
                        var result = new SequenceInfo(reader);
                        result.FileName = fileName;
                        return result;
                    }
                }
            }
            catch (Exception) { return null; }
        }

        private SequenceInfo(BinaryReader reader) {

            long savedPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;

            FocalLengthInPixels = reader.ReadSingle();
            InverseFocalLengthInPixels = reader.ReadSingle();

            HorizontalFOV = reader.ReadSingle();
            VerticalFOV = reader.ReadSingle();
            DiagonalFOV = reader.ReadSingle();

            MinDepth = reader.ReadInt32();
            MaxDepth = reader.ReadInt32();

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            reader.BaseStream.Position = savedPosition;
        }
    }
}