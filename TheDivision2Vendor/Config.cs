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
        public static readonly string Log = Path.Combine(AppContext.BaseDirectory, "config/Log.log");
        public static readonly string D2Dir = Path.Combine(AppContext.BaseDirectory, "resource");
        public static readonly List<ConfigD2> D2Dirs = new List<ConfigD2>();

        static Config()
        {
            if (!Directory.Exists(ConfigDir)) Directory.CreateDirectory(ConfigDir);
            if (!File.Exists(Configs))
                using(var sw = File.CreateText(Configs))
                {
                    var jo = JObject.Parse("{}");
                    sw.WriteLine(JsonConvert.SerializeObject(jo, Formatting.Indented));
                }
            if (!File.Exists(Log))
                using (var sw = File.CreateText(Log))
                {
                    sw.Write("");
                }
            if (!Directory.Exists(D2Dir)) Directory.CreateDirectory(D2Dir);
            FlushD2Dir();
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
                c.Vaild();
                D2Dirs.Insert(0, c);
            }
        }
    }

    public class ConfigD2
    {
        public string Path { get; set; }
        public string Gear { get; set; }
        public string Weapon { get; set; }
        public string Mod { get; set; }
        public List<D2Gear> d2Gears { get; set; } = new List<D2Gear>();
        public List<D2Weapon> d2Weapons { get; set; } = new List<D2Weapon>();

        public void Vaild()
        {
            try { d2Gears = JsonConvert.DeserializeObject<List<D2Gear>>(File.ReadAllText(Gear)); } catch (Exception) { }
            try { d2Weapons = JsonConvert.DeserializeObject<List<D2Weapon>>(File.ReadAllText(Weapon)); } catch (Exception) { }
            //try { } catch (Exception) { }
        }
    }
}
