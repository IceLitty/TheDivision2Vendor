using System.Collections.Generic;

namespace TheDivision2Vendor
{
    public static class TheBest
    {
        public static List<D2Empty> GetBest(List<D2Gear> gears = null, List<D2Weapon> weapons = null, List<D2Mod> mods = null)
        {
            var list = new List<D2Empty>();
            // 3词条判断时需要2词条高属性，2词条判断时只要1词条高属性 + 如果有词条满属性直接推荐 + 默认天赋合理
            if (gears == null) gears = new List<D2Gear>();
            if (weapons == null) weapons = new List<D2Weapon>();
            if (mods == null) mods = new List<D2Mod>();
            foreach (var o in gears)
            {
                var purple = false;
                if (Translate.RarityS(o.rarity).Equals("§p")) purple = true;
                var coreStr = string.Empty;
                if (o.core != null && !string.IsNullOrEmpty(o.core)) coreStr = o.core + "<br/>";
                var attr = Translate.AttrValAndText(coreStr + o.attributes);
                if (CanAdd(attr, true, purple)) list.Add(o);
            }
            foreach (var o in weapons)
            {
                var purple = false;
                if (Translate.RarityS(o.rarity).Equals("§p")) purple = true;
                var attr = Translate.AttrValAndText(o.attribute1, o.attribute2, o.attribute3);
                if (CanAdd(attr, false, purple)) list.Add(o);
            }
            // 由于最高值不可信，故仅采用无条模组直接推荐、无最高值记录则忽略、否则大于等于最高值的80%方可推荐
            foreach (var o in mods)
            {
                var name = Translate.Name(o.name);
                var attrs = Translate.AttrValAndTextMods(o.attributes, name.Equals(o.name) ? null : name);
                foreach (var attr in attrs)
                {
                    if (attr.valMax == 0)
                    {
                        list.Add(o);
                        break;
                    }
                    else if (attr.valMax == Translate.ATTRVALMAXDEFAULT) continue;
                    else if (attr.val >= attr.valMax * 0.8)
                    {
                        if (Translate.RarityS(o.rarity).Equals("§p")) continue;
                        list.Add(o);
                        break;
                    }
                }
            }
            return list;
        }

        private static bool CanAdd(List<Attribute> attr, bool isGear, bool purple)
        {
            if (purple)
            {
                foreach (var a in attr)
                {
                    if (isGear && a.isMainAttr && a.valType == AttrValType.Utility) continue;
                    if (a.val >= a.valMax)
                        return true;
                }
                return false;
            }
            else
            {
                var counter = 0;
                foreach (var a in attr)
                {
                    if (isGear && a.isMainAttr && a.valType == AttrValType.Utility) continue;
                    if (a.val >= a.valMax)
                    {
                        return true;
                    }
                    if (a.val >= a.valMax * 0.8) counter++;
                }
                switch (attr.Count)
                {
                    case 1:
                        if (counter > 0) return true;
                        break;
                    case 2:
                        if (counter > 1) return true;
                        break;
                    case 3:
                        if (counter > 2) return true;
                        break;
                    default:
                        break;
                }
                return CanAddTwice(attr, isGear);
            }
        }

        private static bool CanAddTwice(List<Attribute> attr, bool isGear)
        {
            var counter = 0;
            foreach (var a in attr)
            {
                if (isGear && a.isMainAttr && a.valType == AttrValType.Utility) continue;
                if (a.val >= a.valMax)
                {
                    return true;
                }
                if (a.val >= a.valMax * 0.7) counter++;
            }
            return counter >= attr.Count;
        }
    }
}
