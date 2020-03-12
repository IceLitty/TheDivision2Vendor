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
                    Logger.Put(LogPopType.Popup, LogType.Warn, "从数据源获取装备信息失败", e);
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
                    Logger.Put(LogPopType.Popup, LogType.Warn, "保存数据源信息失败", e);
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
    }
}
