using System;
using System.Collections.Generic;
using System.Text;

namespace TheDivision2Vendor
{
    public static class TextSpawner
    {
        public static List<string> GearsList(int stdoutIndex, D2Gear gear)
        {
            var l = new List<string>();
            var name = Translate.Name(gear.name);
            var brand = Translate.Brand(gear.brand);
            var attribute = Translate.AttrValAndText(gear.attributes);
            AttrValType? mod = Translate.Mod(gear.mods);
            var talent = Translate.Talents(gear.talents);
            l.Add(stdoutIndex + ". " + name);
            l.Add(brand);
            var attrstr = String.Empty;
            var attrstrmain = new List<string>();
            var attrstroff = new List<string>();
            foreach (var attr in attribute)
            {
                var tmp = String.Empty;
                if (attr.val >= attr.valMax)
                {
                    if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                        tmp = Translate.TransAttrValType(attr.valType);
                    else
                        tmp = Translate.TransAttrValType(attr.valType) + "(满)";
                }
                else tmp = Translate.TransAttrValType(attr.valType);
                if (attr.isMainAttr) attrstrmain.Add(tmp);
                else attrstroff.Add(tmp);
            }
            attrstr += "主属性：" + String.Join(" ", attrstrmain);
            l.Add(attrstr);
            attrstr = "次属性：" + String.Join(" ", attrstroff);
            l.Add(attrstr);
            var modAndTal = new StringBuilder();
            if (mod.HasValue) modAndTal.Append("模组：" + Translate.TransAttrValType(mod.Value) + "§- ");
            if (!String.IsNullOrWhiteSpace(talent)) modAndTal.Append("天赋：" + talent);
            if (modAndTal.Length > 0) l.Add(modAndTal.ToString());
            return l;
            // 装备名称 品牌套组名称 核心属性数值 次属性是否满值 改造模块颜色
        }

        public static List<string> GearsLarge(int stdoutIndex, D2Gear gear, int barLength)
        {
            var l = new List<string>();
            var name = Translate.Name(gear.name);
            var rarity = Translate.Rarity(gear.rarity);
            var brand = Translate.Brand(gear.brand);
            var brandDesc = Translate.BrandDesc(brand);
            var attribute = Translate.AttrValAndText(gear.attributes);
            AttrValType? mod = Translate.Mod(gear.mods);
            var talent = Translate.Talents(gear.talents);
            var talentDesc = Translate.TalentsDesc(talent);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(stdoutIndex + ". " + rarity + "§w " + name);
            l.Add(format + brand);
            l.Add(format + gear.armor + " 装甲 " + Translate.Vendor(gear.vendor));
            l.Add("");
            var lattrmain = new List<string>();
            var lattroff = new List<string>();
            foreach (var attr in attribute)
            {
                if (attr.val == 0 || attr.valMax == 0) continue;
                var tmp = new List<string>();
                var str = attr.val >= attr.valMax ? "§o▲§w +" : "+";
                if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                    str = "+";
                tmp.Add(str + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + "§w / " +
                    attr.valMax + (attr.type == AttributeType.Percent ? "%" : ""));
                var percent = (int) Math.Floor(attr.val / attr.valMax * barLength) / 2;
                var percent2 = (int) Math.Floor((barLength - (double)percent * 2) / 2);
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
            if (!String.IsNullOrWhiteSpace(talent))
            {
                l.Add("");
                l.Add("天赋：" + talent);
                if (!String.IsNullOrWhiteSpace(talentDesc))
                    foreach (string str in talentDesc.Split('\n'))
                        l.Add(str);
            }
            if (!String.IsNullOrWhiteSpace(brandDesc))
            {
                l.Add("");
                l.Add(brand + " 套装效果：");
                foreach (string str in brandDesc.Split('\n'))
                    l.Add(format + str);
            }
            return l;
            // 装备名称 色泽名称 栏位图标 装甲/伤害值 装备分数
            // 品牌套组名称 品牌套组三件套效果
            // 核心属性、值、条
            // 次属性、值、条
            // 改造模块颜色
            // 价格
        }

        public static List<string> WeaponsList(int stdoutIndex, D2Weapon weapon)
        {
            var l = new List<string>();
            var name = Translate.Name(weapon.name);
            var attribute = Translate.AttrValAndText(weapon.attribute1, weapon.attribute2, weapon.attribute3);
            var talent = Translate.Talents(weapon.talent);
            l.Add(stdoutIndex + ". " + name);
            //l.Add("伤害：" + weapon.dmg + " 射速：" + weapon.rpm + " 弹夹：" + weapon.mag);
            l.Add("");
            var attrstr = String.Empty;
            var attrstrmain = new List<string>();
            var attrstroff = new List<string>();
            foreach (var attr in attribute)
            {
                var tmp = String.Empty;
                if (attr.val >= attr.valMax)
                {
                    if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                        tmp = Translate.TransAttrValType(attr.valType);
                    else
                        tmp = Translate.TransAttrValType(attr.valType) + "(满)";
                }
                else tmp = Translate.TransAttrValType(attr.valType);
                if (attr.isMainAttr) attrstrmain.Add(tmp);
                else attrstroff.Add(tmp);
            }
            attrstr += "主属性：" + String.Join(" ", attrstrmain);
            l.Add(attrstr);
            attrstr = "次属性：" + String.Join(" ", attrstroff);
            l.Add(attrstr);
            if (!String.IsNullOrWhiteSpace(talent)) l.Add("天赋：" + talent);
            return l;
        }

        public static List<string> WeaponsLarge(int stdoutIndex, D2Weapon weapon, int barLength)
        {
            var l = new List<string>();
            var name = Translate.Name(weapon.name);
            var rarity = Translate.Rarity(weapon.rarity);
            var attribute = Translate.AttrValAndText(weapon.attribute1, weapon.attribute2, weapon.attribute3);
            var talent = Translate.Talents(weapon.talent);
            var talentDesc = Translate.TalentsDesc(talent);
            var format = "".PadLeft((stdoutIndex + ". ").Length);
            l.Add(stdoutIndex + ". " + rarity + "§w " + name);
            l.Add(format + weapon.dmg + " 伤害 射速：" + weapon.rpm + " 弹夹：" + weapon.mag);
            l.Add(format + "商人：" + Translate.Vendor(weapon.vendor));
            l.Add("");
            var lattrmain = new List<string>();
            var lattroff = new List<string>();
            foreach (var attr in attribute)
            {
                if (attr.val == 0 || attr.valMax == 0) continue;
                var tmp = new List<string>();
                var str = attr.val >= attr.valMax ? "§o▲§w +" : "+";
                if (attr.isMainAttr && attr.valType == AttrValType.Utility)
                    str = "+";
                tmp.Add(str + attr.val + (attr.type == AttributeType.Percent ? "%" : "") + " " + attr.desc + "§w / " +
                    attr.valMax + (attr.type == AttributeType.Percent ? "%" : ""));
                var percent = (int)Math.Floor(attr.val / attr.valMax * barLength) / 2;
                var percent2 = (int)Math.Floor((barLength - (double) percent * 2) / 2);
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
            if (!String.IsNullOrWhiteSpace(talent))
            {
                l.Add("");
                l.Add("天赋：" + talent);
                if (!String.IsNullOrWhiteSpace(talentDesc))
                    foreach (string str in talentDesc.Split('\n'))
                        l.Add(str);
            }
            return l;
        }
    }
}
