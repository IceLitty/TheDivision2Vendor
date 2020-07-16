using System;
using System.Collections.Generic;

namespace TheDivision2Vendor
{
    public static class TheBest
    {
        public static List<D2Empty> GetBestTU10(List<D2Gear> gears = null, List<D2Weapon> weapons = null, List<D2Mod> mods = null)
        {
            var nowThreshold = float.Parse(Config.GetValueConf("bestFilterThreshold"));
            var nowThresholdVal = float.Parse(Config.GetValueConf("bestFilterUpToMax"));
            var nowThresholdValPercent = float.Parse(Config.GetValueConf("bestFilterUpToMaxPercent"));
            var list = new List<D2Empty>();
            if (gears == null) gears = new List<D2Gear>();
            if (weapons == null) weapons = new List<D2Weapon>();
            if (mods == null) mods = new List<D2Mod>();
            var errorCount = 0;
            foreach (var o in gears)
            {
                try
                {
                    var coreStr = string.Empty;
                    if (o.core != null && !string.IsNullOrEmpty(o.core)) coreStr = o.core + "<br/>";
                    var attr = FilterAttribute(Translate.AttrValAndText(coreStr + o.attributes));
                    if (bool.Parse(Config.GetValueConf("ignoreSetsMainAttrIsUtility")))
                    {
                        if (attr.Count > 0 && attr.Count < 3 && attr[0].isMainAttr && attr[0].valType == AttrValType.Utility && attr[0].valMax == 1)
                        {
                            continue;
                        }
                    }
                    int counter = 0;
                    var colorList = new List<AttrValType>();
                    foreach (var an in attr)
                    {
                        if (an.val >= an.valMax * nowThreshold)
                        {
                            counter++;
                            colorList.Add(an.valType);
                        }
                        else
                        {
                            var c = an.valMax - an.val;
                            if (an.type == AttributeType.Direct)
                            {
                                if (nowThresholdVal >= 0 && c <= nowThresholdVal)
                                {
                                    counter++;
                                    colorList.Add(an.valType);
                                }
                            }
                            else
                            {
                                if (nowThresholdValPercent >= 0 && c <= nowThresholdValPercent)
                                {
                                    counter++;
                                    colorList.Add(an.valType);
                                }
                            }
                        }
                    }
                    if (counter >= attr.Count - 1)
                    {
                        if (colorList.Count <= 1)
                        {
                            list.Add(o);
                        }
                        else if (colorList.Count == attr.Count)
                        {
                            int maxVal = -1;
                            var r = RepeatItem<AttrValType>.GetRepeat(colorList);
                            foreach (var item in r)
                            {
                                if (item.Counter > maxVal)
                                {
                                    maxVal = item.Counter;
                                }
                            }
                            if (maxVal != -1 && maxVal >= attr.Count - 1)
                            {
                                list.Add(o);
                            }
                        }
                        else
                        {
                            int maxVal = -1;
                            var r = RepeatItem<AttrValType>.GetRepeat(colorList);
                            foreach (var item in r)
                            {
                                if (item.Counter > maxVal)
                                {
                                    maxVal = item.Counter;
                                }
                            }
                            if (maxVal != -1 && maxVal == colorList.Count)
                            {
                                list.Add(o);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    errorCount++;
                    Logger.Put(LogPopType.File, LogType.Warn, String.Format("筛选优质装备时出现错误，原名称为: {0}", o.name));
                }
            }
            foreach (var o in weapons)
            {
                try
                {
                    var attr = FilterAttribute(Translate.AttrValAndText(o.attribute1, o.attribute2, o.attribute3));
                    int counter = 0;
                    foreach (var an in attr)
                    {
                        if (an.val >= an.valMax * nowThreshold)
                        {
                            counter++;
                        }
                        else
                        {
                            var c = an.valMax - an.val;
                            if (an.type == AttributeType.Direct)
                            {
                                if (nowThresholdVal >= 0 && c <= nowThresholdVal)
                                {
                                    counter++;
                                }
                            }
                            else
                            {
                                if (nowThresholdValPercent >= 0 && c <= nowThresholdValPercent)
                                {
                                    counter++;
                                }
                            }
                        }
                    }
                    if (counter >= attr.Count - 1)
                    {
                        list.Add(o);
                    }
                }
                catch (Exception)
                {
                    errorCount++;
                    Logger.Put(LogPopType.File, LogType.Warn, String.Format("筛选优质武器时出现错误，原名称为: {0}", o.name));
                }
            }
            foreach (var o in mods)
            {
                try
                {
                    var name = Translate.Name(o.name);
                    var attr = FilterAttribute(Translate.AttrValAndTextMods(o.attributes, name.Equals(o.name) ? null : name));
                    int counter = 0;
                    foreach (var an in attr)
                    {
                        if (an.val >= an.valMax * nowThreshold)
                        {
                            counter++;
                        }
                        else
                        {
                            var c = an.valMax - an.val;
                            if (an.type == AttributeType.Direct)
                            {
                                if (nowThresholdVal >= 0 && c <= nowThresholdVal)
                                {
                                    counter++;
                                }
                            }
                            else
                            {
                                if (nowThresholdValPercent >= 0 && c <= nowThresholdValPercent)
                                {
                                    counter++;
                                }
                            }
                        }
                    }
                    if (counter == attr.Count)
                    {
                        list.Add(o);
                    }
                }
                catch (Exception)
                {
                    errorCount++;
                    Logger.Put(LogPopType.File, LogType.Warn, String.Format("筛选优质模组时出现错误，原名称为: {0}", o.name));
                }
            }
            TitleFunc.theBestErrorCount = errorCount;
            return list;
        }

        private static List<Attribute> FilterAttribute(List<Attribute> attrs)
        {
            var res = new List<Attribute>();
            foreach (var item in attrs)
            {
                if (item.valMax != Translate.ATTRVALMAXDEFAULT)
                {
                    res.Add(item);
                }
            }
            return res;
        }

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
