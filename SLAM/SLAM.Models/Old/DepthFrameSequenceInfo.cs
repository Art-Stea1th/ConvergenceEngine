using System.IO;


namespace SLAM.Models.Old {

    public sealed class DepthFrameSequenceInfo {

        public float NominalFocalLengthInPixels { get; private set; }
        public float NominalInverseFocalLengthInPixels { get; private set; }

        public float NominalHorizontalFOV { get; private set; }
        public float NominalVerticalFOV { get; private set; }
        public float NominalDiagonalFOV { get; private set; }

        public int MinDepth { get; private set; }
        public int MaxDepth { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        // ---

        public int DepthRange { get { return MaxDepth - MinDepth; } }

        public int Length { get { return Width * Height; } }

        public int FirstFramePosition { get { return sizeof(float) * 5 + sizeof(int) * 4; } }

        public int BytesPerFrame { get { return Length * sizeof(short); } }

        public bool IsCorrect {
            get {

                if (NominalFocalLengthInPixels <= 0.0f) { return false; }
                if (NominalInverseFocalLengthInPixels <= 0.0f) { return false; }

                if (NominalHorizontalFOV <= 0.0f) { return false; }
                if (NominalVerticalFOV <= 0.0f) { return false; }
                if (NominalDiagonalFOV <= 0.0f) { return false; }

                if (MinDepth < 0) { return false; }
                if (MaxDepth < 0 || MaxDepth > 8000) { return false; }

                if (Width < 0 || Width > 640) { return false; }
                if (Height < 0 || Height > 480) { return false; }

                return true;
            }
        }

        public DepthFrameSequenceInfo(BinaryReader reader) {

            long savedPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;

            NominalFocalLengthInPixels = reader.ReadSingle();
            NominalInverseFocalLengthInPixels = reader.ReadSingle();

            NominalHorizontalFOV = reader.ReadSingle();
            NominalVerticalFOV = reader.ReadSingle();
            NominalDiagonalFOV = reader.ReadSingle();

            MinDepth = reader.ReadInt32();
            MaxDepth = reader.ReadInt32();

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            reader.BaseStream.Position = savedPosition;
        }
    }
}