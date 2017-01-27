using System.Windows;


namespace SLAM.Models.IO.Adapters {

    using Readers;

    internal interface IDataAdapter {

        DataProvider DataProvider { get; }

        Point[] GetAdapted(byte[] rawFrameBuffer);
    }
}