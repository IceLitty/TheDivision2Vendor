using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheDivision2Vendor
{
    public static class DownloadResource
    {
        private static HttpClient httpClient;
        private static Regex pageVersionRegex;

        static DownloadResource()
        {
            pageVersionRegex = new Regex(@"/division/gear\.json\?\d+");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpClientHandler = new HttpClientHandler();
            if (bool.Parse(Config.GetValueConf("useProxy")))
            {
                var proxy = new WebProxy
                {
                    Address = new Uri(Config.GetValueConf("proxyAddress")),
                    BypassProxyOnLocal = false,
                };
                if (!string.IsNullOrWhiteSpace(Config.GetValueConf("proxyUsername")))
                {
                    proxy.UseDefaultCredentials = false;
                    var password = string.Empty;
                    if (!string.IsNullOrWhiteSpace(Config.GetValueConf("proxyPassword"))) {
                        password = Config.GetValueConf("proxyPassword");
                    }
                    proxy.Credentials = new NetworkCredential(Config.GetValueConf("proxyUsername"), password);
                }
                httpClientHandler.Proxy = proxy;
            }
            httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "The Division 2 Vendor C# App");
        }

        public static async Task<bool> Download()
        {
            return await Task.Run(async () =>
            {
                var mainPage = string.Empty;
                var dateVer = string.Empty;
                var gearJson = string.Empty;
                var weaponJson = string.Empty;
                var modJson = string.Empty;
                bool failed = false;
                try {
                    mainPage = await httpClient.GetStringAsync("https://rubenalamina.mx/the-division-weekly-vendor-reset");
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "从数据源获取页面信息失败。", e);
                    failed = true;
                }
                if (failed) return false;
                try {
                    MatchCollection matches = pageVersionRegex.Matches(mainPage);
                    if (matches.Count < 1) {
                        Logger.Put(LogPopType.File, LogType.Warn, "日期使用Regex获取失败。");
                        throw new VersionNotFoundException();
                    }
                    String result = matches[0].Value;
                    if (result == null) {
                        throw new VersionNotFoundException();
                    }
                    var split = result.Split('?');
                    result = split[split.Length - 1];
                    var dateTimeResult = DateTime.ParseExact(result, "yyyyMMdd", CultureInfo.InvariantCulture);
                    dateVer = result;
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "从页面信息获取最后更新日期失败。", e);
                    failed = true;
                }
                Logger.Put(LogPopType.File, LogType.Info, "当前更新的内容日期版本为：" + dateVer);
                try
                {
                    gearJson = await httpClient.GetStringAsync("https://rubenalamina.mx/division/gear.json?" + dateVer);
                    weaponJson = await httpClient.GetStringAsync("https://rubenalamina.mx/division/weapons.json?" + dateVer);
                    modJson = await httpClient.GetStringAsync("https://rubenalamina.mx/division/mods.json?" + dateVer);
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "从数据源获取装备信息失败。", e);
                    failed = true;
                }
                if (failed) return false;
                try
                {
                    var gearJ = (JArray)JsonConvert.DeserializeObject(gearJson);
                    var weaponJ = (JArray)JsonConvert.DeserializeObject(weaponJson);
                    var modJ = (JArray)JsonConvert.DeserializeObject(modJson);
                    File.WriteAllText(Config.GetGearPath(dateVer), JsonConvert.SerializeObject(gearJ, Formatting.Indented));
                    File.WriteAllText(Config.GetWeaponsPath(dateVer), JsonConvert.SerializeObject(weaponJ, Formatting.Indented));
                    File.WriteAllText(Config.GetModsPath(dateVer), JsonConvert.SerializeObject(modJ, Formatting.Indented));
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "保存数据源信息失败。", e);
                    failed = true;
                    return false;
                }
                if (!failed)
                {
                    Config.FlushD2Dir();
                    Logger.Put(LogPopType.Title, LogType.Info, "保存装备信息成功。");
                }
                return true;
            });
        }

        public static async Task<string> CheckUpdate()
        {
            return await Task.Run(async () =>
            {
                try
                {
                if (bool.Parse(Config.GetValueConf("checkUpdate")))
                    {
                        var json = await httpClient.GetStringAsync("https://api.github.com/repos/IceLitty/TheDivision2Vendor/releases");
                        var ja = JsonConvert.DeserializeObject<JArray>(json);
                        return ja[0]["tag_name"].ToString();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
                return "false";
            });
        }

        public static async Task<bool> DownloadTransJson(string path)
        {
            Logger.Put(LogPopType.Title, LogType.Info, "正在更新翻译文件……");
            return await Task.Run(async () =>
            {
                var json = string.Empty;
                bool failed = false;
                try
                {
                    json = await httpClient.GetStringAsync("https://raw.githubusercontent.com/IceLitty/TheDivision2Vendor/master/TheDivision2Vendor/Trans.json");
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "下载翻译文件失败：" + e.Message, e);
                    failed = true;
                }
                if (failed) return false;
                try
                {
                    var j = (JObject) JsonConvert.DeserializeObject(json);
                    try
                    {
                        if (File.Exists(path))
                        {
                            var oj = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(path));
                            if (int.Parse(j["version"].ToString()) > int.Parse(oj["version"].ToString()))
                                goto place;
                        }
                        else
                            goto place;
                    }
                    catch (Exception)
                    {
                        goto place;
                    }
                    goto ctn;
                    place:
                    File.WriteAllText(path, JsonConvert.SerializeObject(j, Formatting.Indented));
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "保存翻译文件失败。", e);
                    failed = true;
                    return false;
                }
                if (!failed)
                {
                    Config.FlushD2Dir();
                    Logger.Put(LogPopType.Title, LogType.Info, "下载并保存翻译文件成功，请再次点击刚才功能。");
                }
                ctn:
                return true;
            });
        }
    }
}
