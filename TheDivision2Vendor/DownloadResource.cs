using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheDivision2Vendor
{
    public static class DownloadResource
    {
        public static async Task<bool> Download()
        {
            return await Task.Run(async () =>
            {
                var gearJson = string.Empty;
                var weaponJson = string.Empty;
                var modJson = string.Empty;
                bool failed = false;
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var hct = new HttpClient();
                    gearJson = await hct.GetStringAsync("https://rubenalamina.mx/division/gear.json");
                    weaponJson = await hct.GetStringAsync("https://rubenalamina.mx/division/weapons.json");
                    modJson = await hct.GetStringAsync("https://rubenalamina.mx/division/mods.json");
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
                    File.WriteAllText(Config.GetThisSaturdayGear(), JsonConvert.SerializeObject(gearJ, Formatting.Indented));
                    File.WriteAllText(Config.GetThisSaturdayWeapons(), JsonConvert.SerializeObject(weaponJ, Formatting.Indented));
                    File.WriteAllText(Config.GetThisSaturdayMods(), JsonConvert.SerializeObject(modJ, Formatting.Indented));
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
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var hct = new HttpClient();
                        hct.DefaultRequestHeaders.Add("User-Agent", "The Division 2 Vendor C# App");
                        var json = await hct.GetStringAsync("https://api.github.com/repos/IceLitty/TheDivision2Vendor/releases");
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
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var hct = new HttpClient();
                    json = await hct.GetStringAsync("https://raw.githubusercontent.com/IceLitty/TheDivision2Vendor/master/TheDivision2Vendor/Trans.json");
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.Popup, LogType.Warn, "下载翻译文件失败。", e);
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
                            if (int.Parse(oj["version"].ToString()) > int.Parse(j["version"].ToString()))
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
