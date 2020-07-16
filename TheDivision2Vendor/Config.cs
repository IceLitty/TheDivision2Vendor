using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheDivision2Vendor
{
    public static class Config
    {
        public static readonly string ConfigDir = Path.Combine(AppContext.BaseDirectory, "config");
        public static readonly string Configs = Path.Combine(AppContext.BaseDirectory, "config/Config.json");
        private static JObject Conf;
        private static readonly string _defaultConf = "{\"checkUpdate\": true, \"checkServerStatus\": true, \"bestFilterThreshold\":0.95, \"bestFilterUpToMax\":-1, \"bestFilterUpToMaxPercent\":1, \"barLength\": 1, \"ignoreSetsMainAttrIsUtility\":true}";
        private static readonly JObject _defaultConfObj = (JObject)JsonConvert.DeserializeObject(_defaultConf);
        public static readonly string Log = Path.Combine(AppContext.BaseDirectory, "config/Log.log");
        public static readonly string D2Dir = Path.Combine(AppContext.BaseDirectory, "resource");
        public static readonly List<ConfigD2> D2Dirs = new List<ConfigD2>();
        public static readonly List<string> D2Talents = new List<string>();
        public static readonly List<string> D2TalentsFrom = new List<string>();
        public static readonly List<string> D2Brands = new List<string>();
        public static readonly List<string> D2BrandsColor = new List<string>();

        static Config()
        {
            if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
            if (!File.Exists(Configs))
                using(var sw = File.CreateText(Configs))
                {
                    var jo = JObject.Parse(_defaultConf);
                    sw.WriteLine(JsonConvert.SerializeObject(jo, Formatting.Indented));
                }
            try
            {
                Conf = JObject.Parse(File.ReadAllText(Configs));
            }
            catch (Exception)
            {
                Conf = JObject.Parse(_defaultConf);
            }
            if (!File.Exists(Log))
                using (var sw = File.CreateText(Log))
                {
                    sw.Write("");
                }
            if (!Directory.Exists(D2Dir)) Directory.CreateDirectory(D2Dir);
            FlushD2Dir();
        }

        public static string GetValueConf(string key)
        {
            var r = Conf[key];
            if (r == null)
            {
                Conf[key] = _defaultConfObj[key];
                File.WriteAllText(Configs, JsonConvert.SerializeObject(Conf, Formatting.Indented));
                return GetValueConf(key);
            }
            return r.ToString();
        }

        public static string GetThisSaturdayGear()
        {
            var dir = Path.Combine(D2Dir, Util.GetThisTuesday().ToString("yyyyMMdd"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, "gear.json");
        }

        public static string GetThisSaturdayWeapons()
        {
            var dir = Path.Combine(D2Dir, Util.GetThisTuesday().ToString("yyyyMMdd"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, "weapons.json");
        }

        public static string GetThisSaturdayMods()
        {
            var dir = Path.Combine(D2Dir, Util.GetThisTuesday().ToString("yyyyMMdd"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, "mods.json");
        }

        public static void FlushD2Dir()
        {
            D2Dirs.Clear();
            foreach (var directoryInfo in Directory.GetDirectories(D2Dir))
            {
                var c = new ConfigD2()
                {
                    Path = directoryInfo,
                    Gear = Path.Combine(directoryInfo, "gear.json"),
                    Weapon = Path.Combine(directoryInfo, "weapons.json"),
                    Mod = Path.Combine(directoryInfo, "mods.json"),
                };
                //c.Vaild();
                D2Dirs.Insert(0, c);
            }
        }
    }

    public class ConfigD2
    {
        public bool isVaild { get; set; } = false;
        public string Path { get; set; }
        public string Gear { get; set; }
        public string Weapon { get; set; }
        public string Mod { get; set; }
        public List<D2Empty> d2Best { get; set; } = new List<D2Empty>();
        public List<D2Gear> d2Gears
        {
            get { if (isVaild) return d2Gears_shadow; else { Vaild(); return d2Gears_shadow; }}
            set { d2Gears_shadow = value; }
        }
        private List<D2Gear> d2Gears_shadow = new List<D2Gear>();
        public List<D2Weapon> d2Weapons
        {
            get { if (isVaild) return d2Weapons_shadow; else { Vaild(); return d2Weapons_shadow; }}
            set { d2Weapons_shadow = value; }
        }
        private List<D2Weapon> d2Weapons_shadow = new List<D2Weapon>();
        public List<D2Mod> d2Mods
        {
            get { if (isVaild) return d2Mods_shadow; else { Vaild(); return d2Mods_shadow; }}
            set { d2Mods_shadow = value; }
        }
        private List<D2Mod> d2Mods_shadow = new List<D2Mod>();

        public void Vaild()
        {
            if (!isVaild)
            {
                isVaild = true;
                try { d2Gears = JsonConvert.DeserializeObject<List<D2Gear>>(File.ReadAllText(Gear)); } catch (Exception) { }
                try { d2Weapons = JsonConvert.DeserializeObject<List<D2Weapon>>(File.ReadAllText(Weapon)); } catch (Exception) { }
                try { d2Mods = JsonConvert.DeserializeObject<List<D2Mod>>(File.ReadAllText(Mod)); } catch (Exception) { }
            }
        }
    }
}
