using System.Windows;


namespace SLAM.Models.Data.Adapters {

    using Readers;

    internal interface IDataAdapter {

        DataProvider DataProvider { get; }

        Point[] GetAdapted(byte[] rawFrameBuffer);
    }
}