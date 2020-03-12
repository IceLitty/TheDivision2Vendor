using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheDivision2Vendor
{
    public static class Translate
    {
        private static JObject trans = null;

        static Translate()
        {
            var transJsonRes = typeof(MainFunc).Assembly.GetManifestResourceStream("TheDivision2Vendor.Trans.json");
            var transJson = String.Empty;
            using (var sr = new StreamReader(transJsonRes, Encoding.UTF8)) transJson = sr.ReadToEndAsync().GetAwaiter().GetResult();
            trans = (JObject) JsonConvert.DeserializeObject(transJson);
        }

        public static string Type(string en)
        {
            try
            {
                return trans["type"][en].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("type类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string Rarity(string en)
        {
            try
            {
                return trans["rarity"][en].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("rarity类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string RarityS(string en)
        {
            try
            {
                string cn = trans["rarity"][en].ToString();
                if (cn.StartsWith("§") && cn.Length > 1)
                    return cn.Substring(0, 2);
                else return "§w";
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("rarity类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string Vendor(string en)
        {
            try
            {
                return trans["vendor"][en].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("vendor类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string Brand(string en)
        {
            try
            {
                return trans["brand"][en].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("brand类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string BrandDesc(string cn)
        {
            try
            {
                return trans["brandDesc"][cn].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("brandDesc类型找不到语言文本: {0}", cn));
                return String.Empty;
            }
        }

        public static string Talents(string en)
        {
            if (en.Equals("-")) return String.Empty;
            try
            {
                en = en.Replace("<span class=\"icon-active\"></span>", "");
                if (en.StartsWith("Perfect "))
                {
                    en = en.Substring(8);
                    return "完美" + trans["talents"][en].ToString();
                }
                return trans["talents"][en].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("talents类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string TalentsDesc(string cn)
        {
            try
            {
                return trans["talentsDescription"][cn].ToString();
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("talentsDescription类型找不到语言文本: {0}", cn));
                return String.Empty;
            }
        }

        public static List<Attribute> AttrValAndText(string en, string en2 = null, string en3 = null)
        {
            var isGear = en2 == null && en3 == null;
            if (!String.IsNullOrWhiteSpace(en2) && !en2.Equals("-")) en += "<br/>" + en2;
            if (!String.IsNullOrWhiteSpace(en3) && !en3.Equals("-")) en += "<br/>" + en3;
            string sss = en.Replace("<span class=\"icon-utility\"></span>", "")
                .Replace("<span class=\"icon-offensive\"></span>", "")
                .Replace("<span class=\"icon-defensive\"></span>", "")
                .Replace("<br/>", "\n");
            string[] l = sss.Split('\n');
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].EndsWith("   ")) l[i] = l[i].Substring(0, l[i].Length - 3);
                if (l[i].EndsWith("  ")) l[i] = l[i].Substring(0, l[i].Length - 2);
                if (l[i].EndsWith(" ")) l[i] = l[i].Substring(0, l[i].Length - 1);
            }
            var list = new List<Attribute>();
            for (int i = 0; i < l.Length; i++)
            {
                //var num = l[i].Split(' ')[0].Replace(",", "");
                //var text = l[i].Replace(num + " ", "");
                var li = l[i].ToCharArray();
                var ind = 0;
                if (li.Length == 0) continue;
                for (int j = 0; j < li.Length; j++)
                {
                    switch (li[j])
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '.':
                        case ',':
                        case '%':
                            break;
                        default:
                            ind = j;
                            break;
                    }
                    if (ind != 0) break;
                }
                var at = AttributeType.Direct;
                double avl = 0;
                string adesc;
                if (ind == 0)
                {
                    at = AttributeType.Direct;
                    avl = 0;
                    adesc = l[i];
                }
                else
                {
                    var val = l[i].Substring(0, ind).Replace(",", "");
                    var text = l[i].Substring(ind);
                    if (text.StartsWith("   ")) text = text.Substring(3);
                    if (text.StartsWith("  ")) text = text.Substring(2);
                    if (text.StartsWith(" ")) text = text.Substring(1);
                    at = val.Contains("%") ? AttributeType.Percent : AttributeType.Direct;
                    avl = Double.Parse(val.Replace("%", ""));
                    adesc = text;
                }
                var cn = String.Empty;
                double mval = 0;
                bool isMainAttr = false;
                AttrWeaponType? cnt = null;
                var type = AttrValType.Unknown;
                if (isGear)
                {
                    try
                    {
                        cn = trans["attributesGearMain"][adesc].ToString();
                        isMainAttr = true;
                    }
                    catch (Exception)
                    {
                        try { cn = trans["attributesGear"][adesc].ToString(); }
                        catch (Exception)
                        {
                            cn = adesc;
                            Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesGear类型找不到语言文本: {0}", adesc));
                        }
                    }
                    try { mval = Double.Parse(trans["attributesValMaxGear"][cn].ToString()); } catch (Exception) { }
                    try
                    {
                        switch (trans["attributesColorGear"][cn].ToString())
                        {
                            case "§r":
                                type = AttrValType.Offensive;
                                break;
                            case "§c":
                                type = AttrValType.Defensive;
                                break;
                            case "§y":
                                type = AttrValType.Utility;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    try
                    {
                        cn = trans["attributesWeaponMain"][adesc].ToString();
                        isMainAttr = true;
                        try
                        {
                            cnt = (AttrWeaponType) Enum.Parse(typeof(AttrWeaponType), cn);
                        }
                        catch (Exception) { }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            cn = trans["attributesWeapon"][adesc].ToString();
                            try
                            {
                                foreach (Attribute attr in list)
                                {
                                    if (attr.weaponTest.HasValue)
                                    {
                                        switch (attr.weaponTest.Value)
                                        {
                                            case AttrWeaponType.步枪伤害:
                                                if (cn.Equals("爆击伤害")) isMainAttr = true;
                                                break;
                                            case AttrWeaponType.突击步枪伤害:
                                                if (cn.Equals("生命值伤害")) isMainAttr = true;
                                                break;
                                            case AttrWeaponType.射手步枪伤害:
                                                if (cn.Equals("爆头伤害")) isMainAttr = true;
                                                break;
                                            case AttrWeaponType.霰弹枪伤害:
                                                if (cn.Equals("对装甲伤害")) isMainAttr = true;
                                                break;
                                            case AttrWeaponType.冲锋枪伤害:
                                                if (cn.Equals("爆击几率")) isMainAttr = true;
                                                break;
                                            case AttrWeaponType.轻机枪伤害:
                                                if (cn.Equals("对离开掩体目标的伤害")) isMainAttr = true;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                        catch (Exception)
                        {
                            cn = adesc;
                            Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesWeapon类型找不到语言文本: {0}", adesc));
                        }
                    }
                    try
                    {
                        mval = isMainAttr ?
                            Double.Parse(trans["attributesValMaxWeaponMain"][cn].ToString()) :
                            Double.Parse(trans["attributesValMaxWeapon"][cn].ToString());
                    } catch (Exception) { }
                    try
                    {
                        switch (trans["attributesColorWeapon"][cn].ToString())
                        {
                            case "§r":
                                type = AttrValType.Offensive;
                                break;
                            case "§c":
                                type = AttrValType.Defensive;
                                break;
                            case "§y":
                                type = AttrValType.Utility;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception) { }
                }
                list.Add(new Attribute()
                {
                    type = at,
                    val = avl,
                    desc = cn,
                    valMax = mval,
                    valType = type,
                    isMainAttr = isMainAttr,
                    weaponTest = cnt,
                });
            }
            return list;
        }

        public static string TransAttrValType(AttrValType en)
        {
            switch (en)
            {
                case AttrValType.Offensive:
                    return "§r火§-";
                case AttrValType.Defensive:
                    return "§c体§-";
                case AttrValType.Utility:
                    return "§y电§-";
                case AttrValType.Unknown:
                default:
                    return String.Empty;
            }
        }

        public static string Name(string en)
        {
            return en;
            //try
            //{
            //    return trans["names"][en].ToString();
            //}
            //catch (Exception)
            //{
            //    Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributes类型找不到语言文本: {0}", en));
            //    return en;
            //}
        }

        public static AttrValType? Mod(string en)
        {
            if (String.IsNullOrWhiteSpace(en) || en.Equals("-")) return null;
            try
            {
                return (AttrValType) Enum.Parse(typeof(AttrValType), en);
            }
            catch (Exception)
            {
                Logger.Put(LogPopType.File, LogType.Debug, String.Format("mods类型找不到语言文本: {0}", en));
                return null;
            }
        }
    }

    public class Attribute
    {
        public AttributeType type { get; set; } = AttributeType.Direct;
        public double val { get; set; } = 0;
        public string desc { get; set; } = String.Empty;
        public double valMax { get; set; } = 1000000;
        public AttrValType valType { get; set; } = AttrValType.Unknown;
        public bool isMainAttr { get; set; } = false;
        public AttrWeaponType? weaponTest { get; set; } = null;
    }

    public enum AttrWeaponType
    {
        步枪伤害,
        突击步枪伤害,
        射手步枪伤害,
        霰弹枪伤害,
        冲锋枪伤害,
        轻机枪伤害,
        手枪伤害
    }

    public enum AttributeType
    {
        Percent,
        Direct
    }

    public enum AttrValType
    {
        Unknown,
        Offensive,
        Defensive,
        Utility
    }
}
