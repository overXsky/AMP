using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace AnyListen.Music.Download
{
    public class XunleiDownloader
    {
        public static async Task DownloadAnyListenTrack(string link, string fileName, Action<double> progressChangedAction)
        {
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            using (var client = new XunleiClient(fileName,link))
            {
                client.XunleiProgressChanged += (s, e) => progressChangedAction.Invoke(e.ProgressPercentage);
                await client.DownloadFileTaskAsync();
            }
        }
    }
}