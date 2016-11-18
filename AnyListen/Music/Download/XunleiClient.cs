using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AnyListen.Music.Track.WebApi.AnyListen;

namespace AnyListen.Music.Download
{
    public class XunleiClient : IDisposable
    {
        private IntPtr _taskIntPtr;
        private readonly string _fileName;
        private Xl.DownTaskParam _stParam;
        public delegate void ProgressChangedHandle(object sender, ProgressChangedEventArgs e);
        public event ProgressChangedHandle XunleiProgressChanged;
        private int _progress;


        public XunleiClient(string fileName, string link)
        {
            _fileName = fileName;
            _stParam = new Xl.DownTaskParam
            {
                szTaskUrl = link,
                szSavePath = Path.Combine(Environment.CurrentDirectory, "ErrorSongs"),
                szFilename = DateTime.Now.Ticks + CommonHelper.GetFormat(fileName)
            };
            _taskIntPtr = Xl.XL_CreateTask(_stParam);
            Xl.XL_StartTask(_taskIntPtr);
        }

        public Task DownloadFileTaskAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            new Timer(self =>
            {
                Xl.DownTaskInfo downTaskInfo = new Xl.DownTaskInfo();
                Xl.XL_QueryTaskInfoEx(_taskIntPtr, downTaskInfo);
                var isError = false;
                switch (downTaskInfo.stat)
                {
                    case Xl.DownTaskStatus.TscDownload:
                        _progress = Convert.ToInt32(downTaskInfo.fPercent * 100);
                        break;
                    case Xl.DownTaskStatus.TscComplete:
                        ((IDisposable)self).Dispose();
                        _progress = 100;
                        var oriPath = Path.Combine(Environment.CurrentDirectory, "ErrorSongs", _stParam.szFilename);
                        if (File.Exists(_fileName))
                        {
                            var newFileName = _fileName.Replace(CommonHelper.GetFormat(_fileName),
                                new Random(DateTime.Now.Millisecond).Next(0, 1000) + CommonHelper.GetFormat(_fileName));
                            File.Move(_fileName, newFileName);
                        }
                        File.Move(oriPath, _fileName);
                        tcs.TrySetResult(true);
                        break;
                    case Xl.DownTaskStatus.TscStartpending:
                        _progress = 0;
                        break;
                    default:
                        isError = true;
                        _progress = 100;
                        ((IDisposable)self).Dispose();
                        tcs.TrySetResult(true);
                        break;
                }
                XunleiProgressChanged?.Invoke(null, new ProgressChangedEventArgs(_progress, isError));
            }).Change(0,500);
            return tcs.Task;
        }

        public void Dispose()
        {
            _taskIntPtr = IntPtr.Zero;
            _stParam = null;
        }
    }
}