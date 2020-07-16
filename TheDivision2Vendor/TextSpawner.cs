using System;
using System.Collections.Generic;
using System.Text;

namespace TheDivision2Vendor
{
    public static class TextSpawner
    {
        public static List<string> GearsList(int stdoutIndex, D2Gear gear)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(gear.name);
                var slot = Translate.Slot(gear.slot);
                var brand = Translate.Brand(gear.brand);
                var coreStr = string.Empty;
                if (gear.core != null && !string.IsNullOrEmpty(gear.core)) coreStr = gear.core + "<br/>";
                var attribute = Translate.AttrValAndText(coreStr + gear.attributes);
                AttrValType? mod = Translate.Mod(gear.mods);
                var talent = Translate.Talents(gear.talents);
                l.Add(stdoutIndexStr + ". " + name);
                l.Add("[" + slot + "] " + brand);
                var attrstr = string.Empty;
                var attrstrmain = new List<string>();
                var attrstroff = new List<string>();
                foreach (var attr in attribute)
                {
                    string tmp;
                    if (attr.val == 0) tmp = Translate.TransAttrValType(attr.valType);
                    else
                    {
                        if (attr.val >= attr.valMax && attr.val / attr.valMax < 3)
                        {
                            if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                                tmp = Translate.TransAttrValType(attr.valType);
                            else
                                tmp = Translate.TransAttrValType(attr.valType) + "[满]";
                        }
                        else tmp = Translate.TransAttrValType(attr.valType);
                    }
                    if (attr.isMainAttr) attrstrmain.Add(tmp);
                    else attrstroff.Add(tmp);
                }
                attrstr += "主属性：" + string.Join(" ", attrstrmain);
                l.Add(attrstr);
                attrstr = "次属性：" + string.Join(" ", attrstroff);
                l.Add(attrstr);
                var modAndTal = new StringBuilder();
                if (mod.HasValue) modAndTal.Append("模组：" + Translate.TransAttrValType(mod.Value) + "§- ");
                if (!String.IsNullOrWhiteSpace(talent)) modAndTal.Append("天赋：" + talent);
                if (modAndTal.Length > 0) l.Add(modAndTal.ToString());
                return l;
                // 装备名称 品牌套组名称 核心属性数值 次属性是否满值 改造模块颜色
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + gear.name);
                l.Add("");
                l.Add("");
                l.Add("数据源异常");
                return l;
            }
        }

        public static List<string> GearsLarge(int stdoutIndex, D2Gear gear, int barLength)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(gear.name);
                var rarity = Translate.Rarity(gear.rarity);
                var slot = Translate.Slot(gear.slot);
                var brand = Translate.Brand(gear.brand);
                var brandDesc = Translate.BrandDesc(brand);
                var coreStr = string.Empty;
                if (gear.core != null && !string.IsNullOrEmpty(gear.core)) coreStr = gear.core + "<br/>";
                var attribute = Translate.AttrValAndText(coreStr + gear.attributes);
                AttrValType? mod = Translate.Mod(gear.mods);
                var talent = Translate.Talents(gear.talents);
                var talentDesc = Translate.TalentsDesc(talent);
                var format = "".PadLeft((stdoutIndexStr + ". ").Length);
                l.Add(stdoutIndexStr + ". " + name);
                l.Add(format + rarity + "§w [" + slot + "] " + brand);
                l.Add(format + gear.armor + " 装甲 " + Translate.Vendor(gear.vendor));
                l.Add("");
                var lattrmain = new List<string>();
                var lattroff = new List<string>();
                foreach (var attr in attribute)
                {
                    if (attr.val == 0 || attr.valMax == 0) continue;
                    var valMax = attr.valMax == Translate.ATTRVALMAXDEFAULT ? "???" : attr.valMax.ToString();
                    var tmp = new List<string>();
                    var str = attr.val >= attr.valMax ? "§o▲§w +" : "+";
                    if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                        str = "+";
                    var checkValError = attr.val > (attr.valMax * 1.67) ? " §y数值异常？§w" : "";
                    tmp.Add(str + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + "§w / " +
                        valMax + (attr.type == AttributeType.Percent ? "%" : "") + checkValError);
                    var bl = int.Parse(Config.GetValueConf("barLength"));
                    var percent = (int)Math.Floor(attr.val / attr.valMax * (barLength - 1)) / bl;
                    var percent2 = (int)Math.Floor(((barLength - 1) - (double)percent * bl) / bl);
                    if (percent2 < 0)
                    {
                        percent += percent2;
                        percent2 = 0;
                    }
                    var color = "";
                    switch (attr.valType)
                    {
                        case AttrValType.Offensive:
                            color = "§r";
                            break;
                        case AttrValType.Defensive:
                            color = "§c";
                            break;
                        case AttrValType.Utility:
                            color = "§y";
                            break;
                        case AttrValType.Unknown:
                        default:
                            color = "§w";
                            break;
                    }
                    var barStr = new StringBuilder();
                    for (int i = 0; i < percent; i++)
                        barStr.Append("█");
                    for (int i = 0; i < percent2; i++)
                        barStr.Append("▁");
                    tmp.Add(color + barStr.ToString());
                    if (attr.isMainAttr) lattrmain.AddRange(tmp);
                    else lattroff.AddRange(tmp);
                }
                l.Add("主要属性：");
                l.AddRange(lattrmain);
                l.Add("");
                l.Add("次要属性：");
                l.AddRange(lattroff);
                if (mod.HasValue)
                {
                    l.Add("");
                    l.Add("模组槽位：" + Translate.TransAttrValType(mod.Value));
                }
                if (!string.IsNullOrWhiteSpace(talent))
                {
                    l.Add("");
                    l.Add("天赋：" + talent);
                    if (talentDesc.Length != 0)
                        foreach (string str in talentDesc)
                            l.Add(str);
                }
                if (!string.IsNullOrWhiteSpace(brandDesc))
                {
                    l.Add("");
                    l.Add(brand + " 套装效果：");
                    foreach (string str in brandDesc.Split('\n'))
                        l.Add(str);
                }
                return l;
                // 装备名称 色泽名称 栏位图标 装甲/伤害值 装备分数
                // 品牌套组名称 品牌套组三件套效果
                // 核心属性、值、条
                // 次属性、值、条
                // 改造模块颜色
                // 价格
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + gear.name);
                l.Add("");
                l.Add("");
                l.Add("数据源信息无法正常显示");
                l.Add("可能是源录入错误");
                l.Add("可自行打开当周json文件排错");
                return l;
            }
        }

        public static List<string> WeaponsList(int stdoutIndex, D2Weapon weapon)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(weapon.name, true);
                var attribute = Translate.AttrValAndText(weapon.attribute1, weapon.attribute2, weapon.attribute3);
                var type = Translate.WeaponAttrToType(attribute);
                var talent = Translate.Talents(weapon.talent);
                l.Add(stdoutIndexStr + ". " + name);
                //l.Add("伤害：" + weapon.dmg + " 射速：" + weapon.rpm + " 弹夹：" + weapon.mag);
                l.Add(string.IsNullOrWhiteSpace(type) ? "" : $"[{type}]");
                var attrstr = string.Empty;
                var attrstrmain = new List<string>();
                var attrstroff = new List<string>();
                foreach (var attr in attribute)
                {
                    string tmp;
                    if (attr.val == 0) tmp = Translate.TransAttrValType(attr.valType);
                    else
                    {
                        if (attr.val >= attr.valMax && attr.val / attr.valMax < 3)
                        {
                            if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                                tmp = Translate.TransAttrValType(attr.valType);
                            else
                                tmp = Translate.TransAttrValType(attr.valType) + "[满]";
                        }
                        else tmp = Translate.TransAttrValType(attr.valType);
                    }
                    if (attr.isMainAttr) attrstrmain.Add(tmp);
                    else attrstroff.Add(tmp);
                }
                attrstr += "主属性：" + string.Join(" ", attrstrmain);
                l.Add(attrstr);
                attrstr = "次属性：" + string.Join(" ", attrstroff);
                l.Add(attrstr);
                if (!string.IsNullOrWhiteSpace(talent)) l.Add("天赋：" + talent);
                return l;
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + weapon.name);
                l.Add("");
                l.Add("");
                l.Add("数据源异常");
                return l;
            }
        }

        public static List<string> WeaponsLarge(int stdoutIndex, D2Weapon weapon, int barLength)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(weapon.name, true);
                var rarity = Translate.Rarity(weapon.rarity);
                var attribute = Translate.AttrValAndText(weapon.attribute1, weapon.attribute2, weapon.attribute3);
                var talent = Translate.Talents(weapon.talent);
                var talentDesc = Translate.TalentsDesc(talent);
                var format = "".PadLeft((stdoutIndexStr + ". ").Length);
                l.Add(stdoutIndexStr + ". " + rarity + "§w " + name);
                l.Add(format + weapon.dmg + " 伤害");
                l.Add(format + "射速：" + weapon.rpm + " 弹夹：" + weapon.mag);
                l.Add(format + "商人：" + Translate.Vendor(weapon.vendor));
                l.Add("");
                var lattrmain = new List<string>();
                var lattroff = new List<string>();
                foreach (var attr in attribute)
                {
                    if (attr.val == 0 || attr.valMax == 0) continue;
                    var valMax = attr.valMax == Translate.ATTRVALMAXDEFAULT ? "???" : attr.valMax.ToString();
                    var tmp = new List<string>();
                    var str = attr.val >= attr.valMax ? "§o▲§w +" : "+";
                    if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                        str = "+";
                    var checkValError = attr.val > (attr.valMax * 1.67) ? " §y数值异常？§w" : "";
                    tmp.Add(str + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + "§w / " +
                        valMax + (attr.type == AttributeType.Percent ? "%" : "") + checkValError);
                    var bl = int.Parse(Config.GetValueConf("barLength"));
                    var percent = (int)Math.Floor(attr.val / attr.valMax * (barLength - 1)) / bl;
                    var percent2 = (int)Math.Floor(((barLength - 1) - (double)percent * bl) / bl);
                    if (percent2 < 0)
                    {
                        percent += percent2;
                        percent2 = 0;
                    }
                    var color = "";
                    switch (attr.valType)
                    {
                        case AttrValType.Offensive:
                            color = "§r";
                            break;
                        case AttrValType.Defensive:
                            color = "§c";
                            break;
                        case AttrValType.Utility:
                            color = "§y";
                            break;
                        case AttrValType.Unknown:
                        default:
                            color = "§w";
                            break;
                    }
                    var barStr = new StringBuilder();
                    for (int i = 0; i < percent; i++)
                        barStr.Append("█");
                    for (int i = 0; i < percent2; i++)
                        barStr.Append("▁");
                    tmp.Add(color + barStr.ToString());
                    if (attr.isMainAttr) lattrmain.AddRange(tmp);
                    else lattroff.AddRange(tmp);
                }
                l.Add("主要属性：");
                l.AddRange(lattrmain);
                l.Add("");
                l.Add("次要属性：");
                l.AddRange(lattroff);
                if (!string.IsNullOrWhiteSpace(talent))
                {
                    l.Add("");
                    l.Add("天赋：" + talent);
                    if (talentDesc.Length != 0)
                        foreach (string str in talentDesc)
                            l.Add(str);
                }
                return l;
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + weapon.name);
                l.Add("");
                l.Add("");
                l.Add("数据源信息无法正常显示");
                l.Add("可能是源录入错误");
                l.Add("可自行打开当周json文件排错");
                return l;
            }
        }

        public static List<string> ModsList(int stdoutIndex, D2Mod mod)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(mod.name);
                var whichType = Translate.AttrModFromGearOrSkill(!name.Equals(mod.name), name, mod.attributes);
                var attribute = Translate.AttrValAndTextMods(mod.attributes, whichType == AttrModType.Unknown ? null : name);
                l.Add(stdoutIndexStr + ". " + name);
                var format = "".PadLeft((stdoutIndexStr + ". ").Length);
                l.Add(attribute.Count > 0 && whichType == AttrModType.Skill ? format + attribute[0].modsUseful + "技能模组" : (whichType == AttrModType.Gear ? format + "装备模组" : format + "未知模组"));
                l.Add("");
                var attrSB = new StringBuilder();
                foreach (var attr in attribute)
                {
                    var suffix = attr.desc.StartsWith("§") ? attr.desc.Substring(0, 2) : "";
                    attrSB.Append(format + suffix + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + (attr.val >= attr.valMax ? "[满]" : "") + " ");
                }
                l.Add(attrSB.ToString());
                return l;
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + mod.name);
                l.Add("");
                l.Add("");
                l.Add("数据源异常");
                return l;
            }
        }

        public static List<string> ModsLarge(int stdoutIndex, D2Mod mod, int barLength)
        {
            var stdoutIndexStr = stdoutIndex.ToString().ToCharArray().Length < 2 ? "0" + stdoutIndex.ToString() : stdoutIndex.ToString();
            try
            {
                var l = new List<string>();
                var name = Translate.Name(mod.name);
                var whichType = Translate.AttrModFromGearOrSkill(!name.Equals(mod.name), name, mod.attributes);
                var rarity = Translate.Rarity(mod.rarity);
                var attribute = Translate.AttrValAndTextMods(mod.attributes, whichType == AttrModType.Unknown ? null : name);
                l.Add(stdoutIndexStr + ". " + rarity + "§w " + name);
                var format = "".PadLeft((stdoutIndexStr + ". ").Length);
                l.Add(format + (attribute.Count > 0 && whichType == AttrModType.Skill ? attribute[0].modsUseful + "技能模组" : (whichType == AttrModType.Gear ? "装备模组" : "未知模组")));
                l.Add(format + Translate.Vendor(mod.vendor));
                l.Add("");
                var lattr = new List<string>();
                foreach (var attr in attribute)
                {
                    if (attr.val == 0) continue;
                    var valMax = attr.valMax == Translate.ATTRVALMAXDEFAULT ? "???" : attr.valMax.ToString();
                    var tmp = new List<string>();
                    double p;
                    if (attr.valMax == 0)
                    {
                        tmp.Add("+" + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc);
                        p = 1;
                    }
                    else
                    {
                        tmp.Add("+" + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + "§w / " +
                            valMax + (attr.type == AttributeType.Percent ? "%" : ""));
                        p = attr.val / attr.valMax;
                    }
                    var bl = int.Parse(Config.GetValueConf("barLength"));
                    var percent = (int)Math.Floor(p * (barLength - 1)) / bl;
                    var percent2 = (int)Math.Floor(((barLength - 1) - (double)percent * bl) / bl);
                    if (percent2 < 0)
                    {
                        percent += percent2;
                        percent2 = 0;
                    }
                    var barStr = new StringBuilder();
                    for (int i = 0; i < percent; i++)
                        barStr.Append("█");
                    for (int i = 0; i < percent2; i++)
                        barStr.Append("▁");
                    tmp.Add("§w" + barStr.ToString());
                    lattr.AddRange(tmp);
                }
                l.Add("属性：");
                l.AddRange(lattr);
                return l;
            }
            catch (Exception)
            {
                var l = new List<string>();
                l.Add(stdoutIndexStr + ". " + mod.name);
                l.Add("");
                l.Add("");
                l.Add("数据源信息无法正常显示");
                l.Add("可能是源录入错误");
                l.Add("可自行打开当周json文件排错");
                return l;
            }
        }

        public static List<string> TalentsList(int stdoutIndex, string talentCn, string talentType)
        {
            var l = new List<string>();
            l.Add(stdoutIndex + ". " + talentCn);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(format + talentType);
            return l;
        }

        public static List<string> TalentsLarge(int stdoutIndex, string talentCn, string talentType)
        {
            var l = new List<string>();
            var talentDesc = Translate.TalentsDesc(talentCn, false);
            l.Add(stdoutIndex + ". " + talentCn);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(format + talentType);
            if (talentDesc.Length > 0)
            {
                l.Add("");
                foreach (string str in talentDesc)
                    l.Add(str);
            }
            return l;
        }

        public static List<string> BrandsList(int stdoutIndex, string brandCn, string brandType)
        {
            var l = new List<string>();
            l.Add(stdoutIndex + ". " + brandCn);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(format + brandType);
            return l;
        }

        public static List<string> BrandsLarge(int stdoutIndex, string brandCn, string brandType)
        {
            var l = new List<string>();
            var brandDesc = Translate.BrandDesc(brandCn);
            l.Add(stdoutIndex + ". " + brandCn);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(format + brandType);
            if (!string.IsNullOrWhiteSpace(brandDesc))
            {
                l.Add("");
                l.Add(brandCn + " 套装效果：");
                foreach (string str in brandDesc.Split('\n'))
                    l.Add(str);
            }
            return l;
        }

        public static string PadRight(string str, int maxLength)
        {
            int length = 0;
            int color = 0;
            foreach (char c in str)
            {
                if ('§'.Equals(c)) color++;
                else
                {
                    int l = Encoding.UTF8.GetBytes(c.ToString()).Length;
                    if (l > 1) l -= 1;
                    length += l;
                }
            }
            length -= color;
            length = Math.Max(0, length);
            var sb = new StringBuilder();
            sb.Append(str);
            for (int i = 0; i < maxLength - length; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
