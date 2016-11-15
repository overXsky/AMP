using AnyListen.Music.Download;

namespace AnyListen.Music.Track
{
    public interface IDownloadable
    {
        string DownloadParameter { get; set; }
        string DownloadFilename { get; set; }
        DownloadMethod DownloadMethod { get;}
        bool CanDownload { get; }
        int DownloadBitrate { get; set; }
        int LossPrefer { get; set; }
    }
}
