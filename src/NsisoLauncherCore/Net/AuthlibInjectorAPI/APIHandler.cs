﻿using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NsisoLauncherCore.Util.Checker;

namespace NsisoLauncherCore.Net.AuthlibInjectorAPI
{
    public class APIHandler
    {
        public async Task<DownloadTask> GetLatestAICoreDownloadTask(DownloadSource source, string downloadTo,
            NetRequester requester)
        {
            string apiBase;
            switch (source)
            {
                case DownloadSource.Mojang:
                    apiBase = "https://authlib-injector.yushi.moe/artifact/latest.json";
                    break;
                case DownloadSource.BMCLAPI:
                    apiBase = "https://bmclapi2.bangbang93.com/mirrors/authlib-injector/artifact/latest.json";
                    break;
                default:
                    apiBase = "https://authlib-injector.yushi.moe/artifact/latest.json";
                    break;
            }

            var jsonRespond = await requester.Client.GetAsync(apiBase);
            string json = null;
            if (jsonRespond.IsSuccessStatusCode) json = await jsonRespond.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) return null;
            var jobj = JObject.Parse(json);
            var downloadURL = jobj.Value<string>("download_url");
            var sha256 = jobj["checksums"].Value<string>("sha256");
            var downloadTask = new DownloadTask("AuthlibInjector核心", new StringUrl(downloadURL), downloadTo);
            downloadTask.Checker = new SHA256Checker
            {
                CheckSum = sha256,
                FilePath = downloadTo
            };
            return downloadTask;
        }
    }
}