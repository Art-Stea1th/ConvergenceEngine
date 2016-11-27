using System.IO;


namespace SLAM.Models {

    public sealed class DepthFrameSequenceInfo {

        public float NominalFocalLengthInPixels { get; private set; }
        public float NominalInverseFocalLengthInPixels { get; private set; }

        public float NominalHorizontalFieldOfView { get; private set; }
        public float NominalVerticalFieldOfView { get; private set; }
        public float NominalDiagonalFieldOfView { get; private set; }

        public int MinDepth { get; private set; }
        public int MaxDepth { get; private set; }

        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }

        // ---

        public int FirstFramePosition { get { return sizeof(float) * 5 + sizeof(int) * 4; } }

        public int FrameLength { get { return FrameWidth * FrameHeight; } }

        public int BytesPerFrame { get { return FrameLength * sizeof(short); } }

        public bool IsCorrect {
            get {

                if (NominalFocalLengthInPixels <= 0.0f) { return false; }
                if (NominalInverseFocalLengthInPixels <= 0.0f) { return false; }

                if (NominalHorizontalFieldOfView <= 0.0f) { return false; }
                if (NominalVerticalFieldOfView <= 0.0f) { return false; }
                if (NominalDiagonalFieldOfView <= 0.0f) { return false; }

                if (MinDepth < 0) { return false; }
                if (MaxDepth < 0 || MaxDepth > 8000) { return false; }

                if (FrameWidth < 0 || FrameWidth > 640) { return false; }
                if (FrameHeight < 0 || FrameHeight > 480) { return false; }

                return true;
            }
        }

        public DepthFrameSequenceInfo(BinaryReader reader) {

            long savedPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;

            NominalFocalLengthInPixels = reader.ReadSingle();
            NominalInverseFocalLengthInPixels = reader.ReadSingle();

            NominalHorizontalFieldOfView = reader.ReadSingle();
            NominalVerticalFieldOfView = reader.ReadSingle();
            NominalDiagonalFieldOfView = reader.ReadSingle();

            MinDepth = reader.ReadInt32();
            MaxDepth = reader.ReadInt32();

            FrameWidth = reader.ReadInt32();
            FrameHeight = reader.ReadInt32();

            reader.BaseStream.Position = savedPosition;
        }
    }
}