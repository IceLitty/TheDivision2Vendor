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
        public static JObject trans = null;
        public static int ATTRVALMAXDEFAULT = 88888888;

        static Translate()
        {
            var transJsonRes = typeof(TitleFunc).Assembly.GetManifestResourceStream("TheDivision2Vendor.Trans.json");
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("type类型找不到语言文本: {0}", en));
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("rarity类型找不到语言文本: {0}", en));
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("rarity类型找不到语言文本: {0}", en));
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("vendor类型找不到语言文本: {0}", en));
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("brand类型找不到语言文本: {0}", en));
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
                if (!String.IsNullOrWhiteSpace(cn)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("brandDesc类型找不到语言文本: {0}", cn));
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
                else if (en.StartsWith("Perfectly "))
                {
                    en = en.Substring(10);
                    return "完美" + trans["talents"][en].ToString();
                }
                return trans["talents"][en].ToString();
            }
            catch (Exception)
            {
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("talents类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string[] TalentsDesc(string cn, bool normal = true)
        {
            try
            {
                var result = string.Empty;
                if (cn.StartsWith("完美"))
                {
                    var cn2 = cn.Substring(2);
                    result = trans["talentsDescription"][cn2].ToString();
                }
                else result = trans["talentsDescription"][cn].ToString();
                if (result.StartsWith("\n"))
                {
                    if (normal)
                    {
                        result = result.Substring(1, result.Length - 1);
                        var index = result.IndexOf("\n");
                        if (index != -1)
                        {
                            result = result.Substring(index + 1, result.Length - index - 1);
                        }
                    }
                    else
                        result = result.Substring(1, result.Length - 1);
                }
                return result.Split('\n');
            }
            catch (Exception)
            {
                if (!String.IsNullOrWhiteSpace(cn)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("talentsDescription类型找不到语言文本: {0}", cn));
                return new string[0];
            }
        }

        public static List<Attribute> AttrValAndText(string en, string en2 = null, string en3 = null)
        {
            var isGear = en2 == null && en3 == null;
            if (!String.IsNullOrWhiteSpace(en2) && !en2.Equals("-")) en += "<br/>" + en2;
            if (!String.IsNullOrWhiteSpace(en3) && !en3.Equals("-")) en += "<br/>" + en3;
            string sss = en.Replace("<span class=\"icon-utility\"></span>", "")
                .Replace("<span class=\"icon-weapons\"></span>", "")
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
                AttributeType at;
                double avl;
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
                    // fix some typo
                    val = val.Replace("..", ".");
                    // end
                    var text = l[i].Substring(ind);
                    if (text.StartsWith("   ")) text = text.Substring(3);
                    if (text.StartsWith("  ")) text = text.Substring(2);
                    if (text.StartsWith(" ")) text = text.Substring(1);
                    at = val.Contains("%") ? AttributeType.Percent : AttributeType.Direct;
                    avl = Double.Parse(val.Replace("%", ""));
                    adesc = text;
                }
                string cn;
                double mval = ATTRVALMAXDEFAULT;
                bool isMainAttr = false;
                AttrWeaponType? cnt = null;
                var type = AttrValType.Unknown;
                if (isGear)
                {
                    try
                    {
                        cn = trans["attributesMain"][adesc].ToString();
                        isMainAttr = true;
                    }
                    catch (Exception)
                    {
                        try { cn = trans["attributesOff"][adesc].ToString(); }
                        catch (Exception)
                        {
                            cn = adesc;
                            if (!String.IsNullOrWhiteSpace(adesc)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesGear类型找不到语言文本: {0}", adesc));
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
                        cn = trans["attributesMain"][adesc].ToString();
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
                            cn = trans["attributesOff"][adesc].ToString();
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
                            if (!String.IsNullOrWhiteSpace(adesc)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesWeapon类型找不到语言文本: {0}", adesc));
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

        public static List<Attribute> AttrValAndTextMods(string en, string modNameCn)
        {
            string sss = en.Replace("<br/>", "\n");
            string[] l = sss.Split('\n');
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].EndsWith("   ")) l[i] = l[i].Substring(0, l[i].Length - 3);
                if (l[i].EndsWith("  ")) l[i] = l[i].Substring(0, l[i].Length - 2);
                if (l[i].EndsWith(" ")) l[i] = l[i].Substring(0, l[i].Length - 1);
                if (l[i].StartsWith(" ")) l[i] = l[i].Substring(1, l[i].Length - 1);
            }
            var list = new List<Attribute>();
            var a = new Attribute();
            string tmp = l[0];
            if (l.Length > 1)
            {
                a.modsUseful = tmp;
                tmp = l[1];
            }
            var li = tmp.ToCharArray();
            var ind = 0;
            if (li.Length == 0) return list;
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
                    case 'm':
                    case 's':
                        break;
                    default:
                        ind = j;
                        break;
                }
                if (ind != 0) break;
            }
            AttributeType at;
            double avl;
            string adesc;
            if (ind == 0)
            {
                at = AttributeType.Direct;
                avl = 0;
                adesc = tmp;
            }
            else
            {
                var val = tmp.Substring(0, ind).Replace(",", "");
                // fix some typo
                val = val.Replace("..", ".");
                // end
                var text = tmp.Substring(ind);
                if (text.StartsWith("   ")) text = text.Substring(3);
                if (text.StartsWith("  ")) text = text.Substring(2);
                if (text.StartsWith(" ")) text = text.Substring(1);
                at = val.Contains("%") ? AttributeType.Percent : AttributeType.Direct;
                avl = Double.Parse(val.Replace("%", "").Replace("m", "").Replace("s", ""));
                adesc = text;
            }
            string cn;
            string cnWoColor = string.Empty;
            double mval = ATTRVALMAXDEFAULT;
            try
            {
                cn = trans["attributesMod"][adesc].ToString();
                cnWoColor = cn.StartsWith("§") ? cn.Substring(2, cn.Length - 2) : cn;
            }
            catch (Exception)
            {
                cn = adesc;
                if (!String.IsNullOrWhiteSpace(adesc)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesMod类型找不到语言文本: {0}", adesc));
            }
            if (!String.IsNullOrWhiteSpace(a.modsUseful))
            {
                try
                {
                    a.modsUseful = trans["attributesMod"][a.modsUseful].ToString();
                }
                catch (Exception)
                {
                    if (!String.IsNullOrWhiteSpace(a.modsUseful)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributesMod类型找不到语言文本: {0}", a.modsUseful));
                    a.modsUseful = null;
                }
            }
            if (!String.IsNullOrWhiteSpace(modNameCn))
            {
                if (modNameCn.StartsWith("攻击协定"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["攻击协定"][cnWoColor].ToString()); } catch (Exception) { }
                else if (modNameCn.StartsWith("攻击系统"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["攻击系统"][cnWoColor].ToString()); } catch (Exception) { }
                else if (modNameCn.StartsWith("防御协定"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["防御协定"][cnWoColor].ToString()); } catch (Exception) { }
                else if (modNameCn.StartsWith("防御系统"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["防御系统"][cnWoColor].ToString()); } catch (Exception) { }
                else if (modNameCn.StartsWith("性能协定"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["性能协定"][cnWoColor].ToString()); } catch (Exception) { }
                else if (modNameCn.StartsWith("性能系统"))
                    try { mval = Double.Parse(trans["attributesValMaxMod"]["性能系统"][cnWoColor].ToString()); } catch (Exception) { }
                else if (!String.IsNullOrEmpty(a.modsUseful))
                    try { mval = Double.Parse(trans["attributesValMaxMod"][a.modsUseful][cnWoColor].ToString()); } catch (Exception) { }
            }
            a.type = at;
            a.val = avl;
            a.desc = cn;
            a.valMax = mval;
            list.Add(a);
            return list;
        }

        public static string Slot(string en)
        {
            try
            {
                return trans["slot"][en].ToString();
            }
            catch (Exception)
            {
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("slot类型找不到语言文本: {0}", en));
                return en;
            }
        }

        public static string Name(string en, bool isWeapon = false)
        {
            if (isWeapon)
            {
                try
                {
                    return trans["names"][en].ToString();
                }
                catch (Exception)
                {
                    return en.Replace("First Wave", "第一波")
                        .Replace("Officer's", "军官的")
                        .Replace("Black Market", "黑市")
                        .Replace("Military ", "军规 ")
                        .Replace("Tactical ", "战术 ")
                        .Replace(" Tactical", " 战术")
                        .Replace("Classic ", "经典 ")
                        .Replace("Converted ", "改装 ")
                        .Replace("Covert ", "改装 ")
                        .Replace("Police ", "警用 ")
                        .Replace("Custom ", "定制 ")
                        .Replace("Lightweight ", "轻型 ");
                }
            }
            else
            {
                try
                {
                    return trans["names"][en].ToString();
                }
                catch (Exception)
                {
                    if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("attributes类型找不到语言文本: {0}", en));
                    return en;
                }
            }
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
                if (!String.IsNullOrWhiteSpace(en)) Logger.Put(LogPopType.File, LogType.Debug, String.Format("mods类型找不到语言文本: {0}", en));
                return null;
            }
        }

        public static AttrModType AttrModFromGearOrSkill(bool isTranslated, string cn, string attributes)
        {
            var str = attributes.Replace("<br/>", "\n").Split('\n')[0];
            if (str.StartsWith("   ")) str = str.Substring(3);
            if (str.StartsWith("  ")) str = str.Substring(2);
            if (str.StartsWith(" ")) str = str.Substring(1);
            var strCn = string.Empty;
            try { strCn = trans["attributesMod"][str].ToString(); }
            catch (Exception) { strCn = string.Empty; }
            if (!String.IsNullOrWhiteSpace(strCn))
                return AttrModType.Skill;
            else
            {
                if (!isTranslated) return AttrModType.Unknown;
                if (cn.StartsWith("攻击协定") || cn.StartsWith("攻击系统") ||
                    cn.StartsWith("防御协定") || cn.StartsWith("防御系统") ||
                    cn.StartsWith("性能协定") || cn.StartsWith("性能系统"))
                    return AttrModType.Gear;
                return AttrModType.Unknown;
            }
        }
    }

    public class Attribute
    {
        public AttributeType type { get; set; } = AttributeType.Direct;
        public double val { get; set; } = 0;
        public string desc { get; set; } = String.Empty;
        public double valMax { get; set; } = Translate.ATTRVALMAXDEFAULT;
        public AttrValType valType { get; set; } = AttrValType.Unknown;
        public bool isMainAttr { get; set; } = false;
        public AttrWeaponType? weaponTest { get; set; } = null;
        public string modsUseful { get; set; } = String.Empty;
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

    public enum AttrModType
    {
        Unknown,
        Gear,
        Skill
    }
}
