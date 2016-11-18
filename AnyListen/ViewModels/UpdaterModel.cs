using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AnyListen.Music.Track.WebApi.AnyListen;
using AnyListen.Settings;
using AnyListen.ViewModelBase;
using Newtonsoft.Json.Linq;

namespace AnyListen.ViewModels
{
    public class UpdaterModel : PropertyChangedBase
    {
        
        public UpdaterModel()
        {
            if (AnyListenSettings.Instance.Config.CheckForAnyListenUpdates)
            {
                var date = AnyListenSettings.Instance.Config.LastUpdateTime;
                if (DateTime.Now - date > new TimeSpan(1,0,0))
                {
                    Task.Factory.StartNew((() =>
                    {
                        var html = new WebClient {Encoding = Encoding.UTF8}.DownloadString("http://vip.itwusun.com/soft/version");
                        if (string.IsNullOrEmpty(html)) return;
                        var json = JObject.Parse(html);
                        var ver = json["version"].ToString();
                        var crtVer = CommonHelper.GetCurrentAssemblyVersion();
                        UpdateFound = Convert.ToInt32(ver.Replace(".","")) > Convert.ToInt32(crtVer.Replace(".", ""));
                        DownUrl = string.IsNullOrEmpty(json["downUrl"].ToString()) ? "https://github.com/AnyListen/AMP/releases" : json["downUrl"].ToString();
                        AnyListenSettings.Instance.Config.LastUpdateTime = DateTime.Now;
                    }));
                }
            }
        }

        private bool _updatefound;
        public bool UpdateFound
        {
            get { return _updatefound; }
            set
            {
                SetProperty(value, ref _updatefound);
            }
        }

        public string DownUrl { get; set; }
    }
}