using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheDivision2Vendor;

namespace ConsoleTest
{
    static class Controller
    {
        public static PageState pageState = PageState.Main;
        public static PageWhat pageWhat = PageWhat.None;
        public static List<Content> contents = new List<Content>();
        public static Shower shower = new Shower();
        public static int eachHeight = 7;
        public static int spIndex = Convert.ToInt32(Console.WindowWidth * 0.7f);
        public static int nowChoose = 0;
        public static int lockScrollUp = 0;
        public static int lockScrollDown = 1;
        public static bool lockUpdate = false;
        public static int nowFileIndex = -1;
        public static int contentInLine = 1;
        public static int nowLeft2RightIndex = 0;
        public static bool lockLeftRightWhenHistoryEntry = false;
        public static int realFileIndex = 0;
        public static ManualResetEvent flushDone = new ManualResetEvent(true);
        public static string errorPopup = string.Empty;

        public static int CanShow()
        {
            return (int) Math.Floor((float) Console.WindowHeight / eachHeight);
        }

        public static void ResetAndFlush()
        {
            nowChoose = 0;
            lockScrollUp = 0;
            lockScrollDown = 1;
            Flush(null);
        }

        public static void Flush(bool? upper, bool isLeftRight = false)
        {
            flushDone.WaitOne();
            flushDone.Reset();
            Console.SetCursorPosition(0, 0);
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            spIndex = Convert.ToInt32(width * 0.7f);
            int canShow = CanShow();
            var contentsStrs = new List<string>();
            var tmpContentsStrs = new List<string>();
            for (int index = 0; index < contents.Count; index++)
                tmpContentsStrs.AddRange(contents[index].Print(spIndex - 1, eachHeight, index == nowChoose));
            bool ifs;
            if (isLeftRight) ifs = true;
            else ifs = upper.HasValue;
            if (ifs && contents.Count * eachHeight > height)
            {
                if (isLeftRight) upper = false;
                if (upper.Value)
                {
                    lockScrollDown = Math.Max(1, lockScrollDown - 1);
                    if (lockScrollUp > 0)
                    {
                        int canShowOutLines = height - eachHeight * canShow;
                        int splitIndex = Math.Max(0, nowChoose - lockScrollUp);
                        int index = splitIndex * eachHeight + (eachHeight - canShowOutLines);
                        for (int i = index; i < Math.Min(index + canShow * eachHeight + canShowOutLines, tmpContentsStrs.Count); i++)
                            contentsStrs.Add(tmpContentsStrs[i]);
                    }
                    else
                    {
                        int canShowOutLines = height - eachHeight * canShow;
                        int index = nowChoose * eachHeight;
                        for (int i = index; i < Math.Min(index + canShow * eachHeight + canShowOutLines, tmpContentsStrs.Count); i++)
                            contentsStrs.Add(tmpContentsStrs[i]);
                    }
                    lockScrollUp = Math.Max(0, lockScrollUp - 1);
                }
                else
                {
                    if (!isLeftRight) lockScrollUp = Math.Min(canShow - 1, lockScrollUp + 1);
                    if (lockScrollDown < canShow)
                    {
                        int canShowOutLines = height - eachHeight * canShow;
                        int index = Math.Max(0, nowChoose - lockScrollDown) * eachHeight;
                        for (int i = index; i < Math.Min(index + canShow * eachHeight + canShowOutLines, tmpContentsStrs.Count); i++)
                            contentsStrs.Add(tmpContentsStrs[i]);
                    }
                    else
                    {
                        int canShowOutLines = height - eachHeight * canShow;
                        int splitIndex = nowChoose + 1 - canShow;
                        int index = splitIndex * eachHeight + (eachHeight - canShowOutLines);
                        for (int i = index; i < Math.Min(index + canShow * eachHeight + canShowOutLines, tmpContentsStrs.Count); i++)
                            contentsStrs.Add(tmpContentsStrs[i]);
                    }
                    if (!isLeftRight) lockScrollDown = Math.Min(canShow, lockScrollDown + 1);
                }
            }
            else contentsStrs = tmpContentsStrs;
            if (pageState == PageState.Entry || pageState == PageState.HistoryEntry)
            {
                if (Config.D2Dirs.Count <= realFileIndex)
                {
                    shower.lines = Shower.GetDefaultMsg();
                    shower.color = Color.Default;
                }
                int index = nowChoose * contentInLine + nowLeft2RightIndex;
                switch (pageWhat)
                {
                    case PageWhat.Best:
                        if (Config.D2Dirs[realFileIndex].d2Best.Count > index)
                        {
                            var o = Config.D2Dirs[realFileIndex].d2Best[index];
                            if (o.GetType() == typeof(D2Gear))
                            {
                                var oo = (D2Gear) o;
                                shower.lines = TextSpawner.GearsLarge(index + 1, oo, width - spIndex + 1 - 3);
                                shower.color = FormatProfile.Rarity2Color(oo.rarity);
                            }
                            else if (o.GetType() == typeof(D2Weapon))
                            {
                                var oo = (D2Weapon) o;
                                shower.lines = TextSpawner.WeaponsLarge(index + 1, oo, width - spIndex + 1 - 3);
                                shower.color = FormatProfile.Rarity2Color(oo.rarity);
                            }
                            else if (o.GetType() == typeof(D2Mod))
                            {
                                var oo = (D2Mod) o;
                                shower.lines = TextSpawner.ModsLarge(index + 1, oo, width - spIndex + 1 - 3);
                                shower.color = FormatProfile.Rarity2Color(oo.rarity);
                            }
                            else
                            {
                                shower.lines = new List<string>();
                                shower.color = Color.Default;
                            }
                        }
                        break;
                    case PageWhat.Gear:
                        if (Config.D2Dirs[realFileIndex].d2Gears.Count > index)
                        {
                            shower.lines = TextSpawner.GearsLarge(index + 1, Config.D2Dirs[realFileIndex].d2Gears[index], width - spIndex + 1 - 3);
                            shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Gears[index].rarity);
                        }
                        break;
                    case PageWhat.Weapon:
                        if (Config.D2Dirs[realFileIndex].d2Weapons.Count > index)
                        {
                            shower.lines = TextSpawner.WeaponsLarge(index + 1, Config.D2Dirs[realFileIndex].d2Weapons[index], width - spIndex + 1 - 3);
                            shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Weapons[index].rarity);
                        }
                        break;
                    case PageWhat.Mod:
                        if (Config.D2Dirs[realFileIndex].d2Mods.Count > index)
                        {
                            shower.lines = TextSpawner.ModsLarge(index + 1, Config.D2Dirs[realFileIndex].d2Mods[index], width - spIndex + 1 - 3);
                            shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Mods[index].rarity);
                        }
                        break;
                    case PageWhat.Talent:
                        if (Config.D2Talents.Count > index)
                        {
                            shower.lines = TextSpawner.TalentsLarge(index + 1, Config.D2Talents[index], Config.D2TalentsFrom[index]);
                            shower.color = Color.Default;
                        }
                        break;
                    case PageWhat.Brand:
                        if (Config.D2Brands.Count > index)
                        {
                            shower.lines = TextSpawner.BrandsLarge(index + 1, Config.D2Brands[index], Config.D2BrandsColor[index]);
                            shower.color = Config.D2BrandsColor[index] == "顶级" ? Color.Orange : (Config.D2BrandsColor[index] == "装备组" ? Color.Green : Color.Default);
                        }
                        break;
                    case PageWhat.BrandShower:
                        {
                            var list = new List<string>();
                            var nowColor = Color.Default;
                            var history = new List<string>();
                            foreach (var bb in Translate.trans["brand"].Children<JProperty>())
                            {
                                var b = Translate.trans["brand"][bb.Name].ToString();
                                if (String.IsNullOrWhiteSpace(b) || b.Equals("未知")) continue;
                                else if (b.Equals("__套装")) nowColor = Color.Orange;
                                else if (b.Equals("__装备组")) nowColor = Color.Green;
                                else if (!string.IsNullOrWhiteSpace(b) && !history.Contains(b))
                                {
                                    var ll = Translate.BrandDesc(b).Split('\n');
                                    var line = new StringBuilder();
                                    line.Append(nowColor == Color.Green ? "§g" : "§o");
                                    line.Append(TextSpawner.PadRight(b, 20));
                                    line.Append("§w：");
                                    foreach (var s in ll)
                                    {
                                        if (String.IsNullOrWhiteSpace(s)) continue;
                                        var f = s.Substring(0, 1);
                                        switch (f)
                                        {
                                            case "+":
                                            case "核":
                                            case "2":
                                            case "3":
                                            case "4":
                                                var ss = s.Replace("2件：", "").Replace("3件：", "").Replace("4件：", "");
                                                if (s.Contains("对装甲的伤害"))
                                                    ss = "§r" + ss + "§w";
                                                else if (s.Contains("武器控制力"))
                                                    ss = "§p" + ss + "§w";
                                                else if (s.Contains("生命") || s.Contains("防护") || s.Contains("降临") || s.Contains("装甲") || s.Contains("抗性"))
                                                    ss = "§c" + ss + "§w";
                                                else if (s.Contains("技能") || s.Contains("状态") || s.Contains("爆裂"))
                                                    ss = "§y" + ss + "§w";
                                                else if (s.Contains("爆击") || s.Contains("武器") || s.Contains("伤害") || s.Contains("枪"))
                                                    ss = "§r" + ss + "§w";
                                                else
                                                    ss = "§p" + ss + "§w";
                                                if (nowColor == Color.Green)
                                                    line.Append(TextSpawner.PadRight(ss, 40));
                                                else
                                                    line.Append(TextSpawner.PadRight(ss, 30));
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    list.Add(line.ToString());
                                    history.Add(b);
                                }
                            }
                            shower.lines = list;
                            shower.color = Color.Default;
                        }
                        break;
                    case PageWhat.None:
                    default:
                        shower.lines = new List<string>();
                        shower.color = Color.Default;
                        break;
                }
            }
            else if (pageState == PageState.ErrorPopup)
            {
                var list = new List<string>();
                foreach (string str in errorPopup.Replace("\r", "").Split("\n"))
                {
                    list.Add(str);
                }
                shower.lines = list;
                shower.color = Color.Default;
            }
            else
            {
                shower.lines = Shower.GetDefaultMsg();
                shower.color = Color.Default;
            }
            var showerStrs = pageWhat == PageWhat.BrandShower ? shower.Print(width - 1, height) : shower.Print(width - spIndex + 1, height);
            var all = new List<string>();
            if (pageWhat == PageWhat.BrandShower) all = showerStrs;
            else
            {
                for (int index = 0; index < height; index++)
                {
                    string line = String.Empty;
                    if (contentsStrs.Count > index) line += contentsStrs[index];
                    else for (int i = 0; i < spIndex - 3; i++) line += " ";
                    if (showerStrs.Count > index) line += showerStrs[index];
                    all.Add(line);
                }
            }
            //foreach (var s in all) Logger.Put(LogPopType.File, LogType.Debug, s);
            bool color = false;
            for (int lIndex = 0; lIndex < height; lIndex++)
            {
                string lineStr = all[lIndex];
                ConsoleColor? backup = null;
                ConsoleColor? backup2 = null;
                foreach (char lineChar in lineStr)
                {
                    if (lineChar == '§') color = true;
                    else if (color)
                    {
                        color = false;
                        switch (lineChar)
                        {
                            case 'p':
                                if (Console.BackgroundColor != ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.Black;
                                if (Console.ForegroundColor != ConsoleColor.Magenta) Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case 'o':
                                if (Console.BackgroundColor != ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.Black;
                                if (Console.ForegroundColor != ConsoleColor.DarkYellow) Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;
                            case 'n':
                                if (Console.BackgroundColor != ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.Black;
                                if (Console.ForegroundColor != ConsoleColor.Yellow) Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 'g':
                                if (Console.BackgroundColor != ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.Black;
                                if (Console.ForegroundColor != ConsoleColor.Green) Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 'q':
                                if (Console.BackgroundColor != ConsoleColor.DarkMagenta) Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                if (Console.ForegroundColor != ConsoleColor.White) Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case '0':
                                if (Console.BackgroundColor != ConsoleColor.DarkYellow) Console.BackgroundColor = ConsoleColor.DarkYellow;
                                if (Console.ForegroundColor != ConsoleColor.White) Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case 'u':
                                if (Console.BackgroundColor != ConsoleColor.Yellow) Console.BackgroundColor = ConsoleColor.Yellow;
                                if (Console.ForegroundColor != ConsoleColor.Black) Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case '8':
                                if (Console.BackgroundColor != ConsoleColor.DarkGreen) Console.BackgroundColor = ConsoleColor.DarkGreen;
                                if (Console.ForegroundColor != ConsoleColor.White) Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case 'b':
                                if (Console.BackgroundColor != ConsoleColor.Gray) Console.BackgroundColor = ConsoleColor.Gray;
                                if (Console.ForegroundColor != ConsoleColor.Black) Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case 'r':
                                backup = Console.ForegroundColor;
                                if (Console.ForegroundColor != ConsoleColor.Red) Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 'c':
                                backup = Console.ForegroundColor;
                                if (Console.ForegroundColor != ConsoleColor.Blue) Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 'y':
                                backup = Console.ForegroundColor;
                                if (Console.ForegroundColor != ConsoleColor.Yellow) Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case '-':
                                if (backup != null)
                                {
                                    if (Console.ForegroundColor != backup.Value) Console.ForegroundColor = backup.Value;
                                    backup = null;
                                }
                                break;
                            default:
                                if (Console.BackgroundColor != ConsoleColor.Black) Console.BackgroundColor = ConsoleColor.Black;
                                if (Console.ForegroundColor != ConsoleColor.White) Console.ForegroundColor = ConsoleColor.White;
                                break;
                        }
                    }
                    else if (lineChar == '[')
                    {
                        Console.Write(lineChar);
                        backup2 = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else if (lineChar == ']')
                    {
                        if (backup2.HasValue) Console.ForegroundColor = backup2.Value;
                        Console.Write(lineChar);
                    }
                    else Console.Write(lineChar);
                }
                if (lIndex < height - 1) Console.WriteLine();
            }
            flushDone.Set();
        }

        public static void TurnBack()
        {
            switch (pageState)
            {
                case PageState.HistoryEntry:
                    //break;
                case PageState.HistorySelected:
                    //break;
                case PageState.ErrorPopup:
                case PageState.History:
                case PageState.Entry:
                    Program.WelcomeScreen();
                    ResetAndFlush();
                    break;
                case PageState.Main:
                default:
                    break;
            }
        }

        public static void ShowBest()
        {
            if (Config.D2Dirs.Count < 1) return;
            realFileIndex = nowFileIndex * contentInLine + nowLeft2RightIndex;
            foreach (var c in contents)
            {
                if (c.action == ShowBest && Config.D2Dirs[realFileIndex].d2Best.Count == 0)
                {
                    c.lines[0][2] = "正在生成本期筛选结果……";
                    c.lines[0][3] = "";
                    Flush(null);
                    Config.D2Dirs[realFileIndex].d2Best = TheBest.GetBestTU10(Config.D2Dirs[realFileIndex].d2Gears, Config.D2Dirs[realFileIndex].d2Weapons, Config.D2Dirs[realFileIndex].d2Mods);
                    c.lines[0][2] = "筛选结果数量" + Config.D2Dirs[realFileIndex].d2Best.Count + (Config.D2Dirs[realFileIndex].d2Best.Count == 0 ? "" : "，正在加载…");
                    Flush(null);
                    break;
                }
            }
            if (Config.D2Dirs[realFileIndex].d2Best.Count == 0) return;
            lockLeftRightWhenHistoryEntry = false;
            TitleFunc.pageStr = nowFileIndex == 0 ? "最佳筛选页" : "历史最佳筛选页";
            pageState = nowFileIndex == 0 ? PageState.Entry : PageState.HistoryEntry;
            pageWhat = PageWhat.Best;
            contents.Clear();
            contentInLine = 2;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            foreach (var o in Config.D2Dirs[realFileIndex].d2Best)
            {
                var i = lines.Count - 1;
                if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                {
                    if (o.GetType() == typeof(D2Gear))
                    {
                        var oo = (D2Gear) o;
                        lines[i].Add(TextSpawner.GearsList(Config.D2Dirs[realFileIndex].d2Gears.IndexOf(oo) + 1, oo));
                        colors[i].Add(Content.GetColorFs(Translate.RarityS(oo.rarity)));
                    }
                    else if (o.GetType() == typeof(D2Weapon))
                    {
                        var oo = (D2Weapon) o;
                        lines[i].Add(TextSpawner.WeaponsList(Config.D2Dirs[realFileIndex].d2Weapons.IndexOf(oo) + 1, oo));
                        colors[i].Add(Content.GetColorFs(Translate.RarityS(oo.rarity)));
                    }
                    else if (o.GetType() == typeof(D2Mod))
                    {
                        var oo = (D2Mod) o;
                        lines[i].Add(TextSpawner.ModsList(Config.D2Dirs[realFileIndex].d2Mods.IndexOf(oo) + 1, oo));
                        colors[i].Add(Content.GetColorFs(
                            oo.rarity.Equals(Translate.Rarity(oo.rarity)) ?
                            Translate.RarityS("header-unknown") :
                            Translate.RarityS(("header-he".Equals(oo.rarity) || "header-purple".Equals(oo.rarity)) ? "header-purple" : "header-he")
                        ));
                    }
                    //else
                    //{
                    //    lines[i].Add(new List<string>());
                    //    colors[i].Add(Color.Default);
                    //}
                }
                else
                {
                    if (o.GetType() == typeof(D2Gear))
                    {
                        var oo = (D2Gear) o;
                        lines.Add(new List<List<string>>() { TextSpawner.GearsList(Config.D2Dirs[realFileIndex].d2Gears.IndexOf(oo) + 1, oo) });
                        colors.Add(new List<Color>() { Content.GetColorFs(Translate.RarityS(oo.rarity)) });
                    }
                    else if (o.GetType() == typeof(D2Weapon))
                    {
                        var oo = (D2Weapon) o;
                        lines.Add(new List<List<string>>() { TextSpawner.WeaponsList(Config.D2Dirs[realFileIndex].d2Weapons.IndexOf(oo) + 1, oo) });
                        colors.Add(new List<Color>() { Content.GetColorFs(Translate.RarityS(oo.rarity)) });
                    }
                    else if (o.GetType() == typeof(D2Mod))
                    {
                        var oo = (D2Mod) o;
                        lines.Add(new List<List<string>>() { TextSpawner.ModsList(Config.D2Dirs[realFileIndex].d2Mods.IndexOf(oo) + 1, oo) });
                        colors.Add(new List<Color>() { Content.GetColorFs(
                            oo.rarity.Equals(Translate.Rarity(oo.rarity)) ?
                            Translate.RarityS("header-unknown") :
                            Translate.RarityS(("header-he".Equals(oo.rarity) || "header-purple".Equals(oo.rarity)) ? "header-purple" : "header-he")
                        ) });
                    }
                    //else
                    //{
                    //    lines.Add(new List<List<string>>() { new List<string>() });
                    //    colors.Add(new List<Color>() { Color.Default });
                    //}
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            if (Config.D2Dirs[realFileIndex].d2Best.Count > 0)
            {
                var o = Config.D2Dirs[realFileIndex].d2Best[0];
                if (o.GetType() == typeof(D2Gear))
                {
                    var oo = (D2Gear) o;
                    shower.lines = TextSpawner.GearsLarge(1, oo, Console.WindowWidth - spIndex + 1 - 3);
                    shower.color = FormatProfile.Rarity2Color(oo.rarity);
                }
                else if (o.GetType() == typeof(D2Weapon))
                {
                    var oo = (D2Weapon) o;
                    shower.lines = TextSpawner.WeaponsLarge(1, oo, Console.WindowWidth - spIndex + 1 - 3);
                    shower.color = FormatProfile.Rarity2Color(oo.rarity);
                }
                else
                {
                    shower.lines = new List<string>();
                    shower.color = Color.Default;
                }
            }
            else
            {
                shower.lines = new List<string>();
                shower.color = Color.Default;
            }
            ResetAndFlush();
        }

        public static void UpdateResources()
        {
            if (contents.Count > 0 && contents[1].action == UpdateResources)
            {
                contents[1].lines[0][2] = "正在后台更新数据中……";
                Flush(null);
            }
            _ = Task.Run(() =>
            {
                bool isSuccess = DownloadResource.Download().GetAwaiter().GetResult();
                if (isSuccess && contents.Count > 0 && contents[1].action == UpdateResources)
                {
                    contents[1].lines[0][2] = "最后资源日期：" + Program.FileHint();
                    Flush(null);
                }
            });
        }

        public static void ShowGears()
        {
            foreach (var c in contents)
            {
                if (c.action == ShowGears)
                {
                    c.lines[0][2] = "正在加载中……请稍后";
                    Flush(null);
                    break;
                }
            }
            lockLeftRightWhenHistoryEntry = false;
            if (nowFileIndex < 0 || Config.D2Dirs.Count < 1) return;
            TitleFunc.pageStr = nowFileIndex == 0 ? "装备页" : "历史装备页";
            pageState = nowFileIndex == 0 ? PageState.Entry : PageState.HistoryEntry;
            pageWhat = PageWhat.Gear;
            realFileIndex = nowFileIndex * contentInLine + nowLeft2RightIndex;
            if (Config.D2Dirs.Count <= realFileIndex) return;
            contents.Clear();
            contentInLine = 2;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            foreach (var o in Config.D2Dirs[realFileIndex].d2Gears)
            {
                var i = lines.Count - 1;
                if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                {
                    lines[i].Add(TextSpawner.GearsList(Config.D2Dirs[realFileIndex].d2Gears.IndexOf(o) + 1, o));
                    colors[i].Add(Content.GetColorFs(Translate.RarityS(o.rarity)));
                }
                else
                {
                    lines.Add(new List<List<string>>() { TextSpawner.GearsList(Config.D2Dirs[realFileIndex].d2Gears.IndexOf(o) + 1, o) });
                    colors.Add(new List<Color>() { Content.GetColorFs(Translate.RarityS(o.rarity)) });
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            if (Config.D2Dirs[realFileIndex].d2Gears.Count > 0)
            {
                shower.lines = TextSpawner.GearsLarge(1, Config.D2Dirs[realFileIndex].d2Gears[0], Console.WindowWidth - spIndex + 1 - 3);
                shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Gears[0].rarity);
            }
            else
            {
                shower.lines = new List<string>();
                shower.color = Color.Default;
            }
            ResetAndFlush();
        }

        public static void ShowWeapons()
        {
            foreach (var c in contents)
            {
                if (c.action == ShowWeapons)
                {
                    c.lines[0][2] = "正在加载中……请稍后";
                    Flush(null);
                    break;
                }
            }
            lockLeftRightWhenHistoryEntry = false;
            if (nowFileIndex < 0 || Config.D2Dirs.Count < 1) return;
            TitleFunc.pageStr = nowFileIndex == 0 ? "武器页" : "历史武器页";
            pageState = nowFileIndex == 0 ? PageState.Entry : PageState.HistoryEntry;
            pageWhat = PageWhat.Weapon;
            realFileIndex = nowFileIndex * contentInLine + nowLeft2RightIndex;
            if (Config.D2Dirs.Count <= realFileIndex) return;
            contents.Clear();
            contentInLine = 2;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            foreach (var o in Config.D2Dirs[realFileIndex].d2Weapons)
            {
                var i = lines.Count - 1;
                if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                {
                    lines[i].Add(TextSpawner.WeaponsList(Config.D2Dirs[realFileIndex].d2Weapons.IndexOf(o) + 1, o));
                    colors[i].Add(Content.GetColorFs(Translate.RarityS(o.rarity)));
                }
                else
                {
                    lines.Add(new List<List<string>>() { TextSpawner.WeaponsList(Config.D2Dirs[realFileIndex].d2Weapons.IndexOf(o) + 1, o) });
                    colors.Add(new List<Color>() { Content.GetColorFs(Translate.RarityS(o.rarity)) });
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            if (Config.D2Dirs[realFileIndex].d2Weapons.Count > 0)
            {
                shower.lines = TextSpawner.WeaponsLarge(1, Config.D2Dirs[realFileIndex].d2Weapons[0], Console.WindowWidth - spIndex + 1 - 3);
                shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Weapons[0].rarity);
            }
            else
            {
                shower.lines = new List<string>();
                shower.color = Color.Default;
            }
            ResetAndFlush();
        }

        public static void ShowMods()
        {
            foreach (var c in contents)
            {
                if (c.action == ShowMods)
                {
                    c.lines[0][2] = "正在加载中……请稍后";
                    Flush(null);
                    break;
                }
            }
            lockLeftRightWhenHistoryEntry = false;
            if (nowFileIndex < 0 || Config.D2Dirs.Count < 1) return;
            TitleFunc.pageStr = nowFileIndex == 0 ? "模组页" : "历史模组页";
            pageState = nowFileIndex == 0 ? PageState.Entry : PageState.HistoryEntry;
            pageWhat = PageWhat.Mod;
            realFileIndex = nowFileIndex * contentInLine + nowLeft2RightIndex;
            if (Config.D2Dirs.Count <= realFileIndex) return;
            contents.Clear();
            contentInLine = 2;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            foreach (var o in Config.D2Dirs[realFileIndex].d2Mods)
            {
                var i = lines.Count - 1;
                if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                {
                    lines[i].Add(TextSpawner.ModsList(Config.D2Dirs[realFileIndex].d2Mods.IndexOf(o) + 1, o));
                    colors[i].Add(Content.GetColorFs(
                        o.rarity.Equals(Translate.Rarity(o.rarity)) ?
                        Translate.RarityS("header-unknown") :
                        Translate.RarityS(("header-he".Equals(o.rarity) || "header-purple".Equals(o.rarity)) ? "header-purple" : "header-he")
                    ));
                }
                else
                {
                    lines.Add(new List<List<string>>() { TextSpawner.ModsList(Config.D2Dirs[realFileIndex].d2Mods.IndexOf(o) + 1, o) });
                    colors.Add(new List<Color>() { Content.GetColorFs(
                            o.rarity.Equals(Translate.Rarity(o.rarity)) ?
                            Translate.RarityS("header-unknown") :
                            Translate.RarityS(("header-he".Equals(o.rarity) || "header-purple".Equals(o.rarity)) ? "header-purple" : "header-he")
                    ) });
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            if (Config.D2Dirs[realFileIndex].d2Mods.Count > 0)
            {
                shower.lines = TextSpawner.ModsLarge(1, Config.D2Dirs[realFileIndex].d2Mods[0], Console.WindowWidth - spIndex + 1 - 3);
                shower.color = FormatProfile.Rarity2Color(Config.D2Dirs[realFileIndex].d2Mods[0].rarity);
            }
            else
            {
                shower.lines = new List<string>();
                shower.color = Color.Default;
            }
            ResetAndFlush();
        }

        public static void OpenHistory()
        {
            if (Config.D2Dirs.Count < 1) return;
            TitleFunc.pageStr = "历史页";
            pageState = PageState.History;
            contents.Clear();
            nowFileIndex = 0;
            contentInLine = 2;
            nowLeft2RightIndex = 0;
            for (int i = 0; i < Config.D2Dirs.Count; i += contentInLine)
            {
                var ll = new List<List<string>>();
                for (int j = 0; j < contentInLine; j++)
                {
                    if (i + j >= Config.D2Dirs.Count) break;
                    var sp = Config.D2Dirs[i + j].Path.Split("\\");
                    var t = sp[sp.Length - 1];
                    var t2 = Program.FileHint();
                    ll.Add(new List<string>() { "", t, t2 });
                    nowFileIndex++;
                }
                contents.Add(new Content() { action = OpenHistoryEntry, lines = ll });
            }
            nowFileIndex = 0;
            ResetAndFlush();
        }

        public static void OpenHistoryEntry()
        {
            if (nowFileIndex < 0) return;
            realFileIndex = nowFileIndex * contentInLine + nowLeft2RightIndex;
            if (Config.D2Dirs.Count <= realFileIndex) return;
            TitleFunc.pageStr = "历史选择页";
            pageState = PageState.HistorySelected;
            lockLeftRightWhenHistoryEntry = true;
            contents.Clear();
            var sp = Config.D2Dirs[realFileIndex].Path.Split("\\");
            var t = sp[sp.Length - 1];
            contents.Add(new Content() { action = ShowBest, lines = new List<List<string>>() { new List<string>() { "", $"推荐装备 - {t}", "", "" } } });
            contents.Add(new Content() { action = ShowGears, lines = new List<List<string>>() { new List<string>() { "", $"查看防具 - {t}", "", "" } } });
            contents.Add(new Content() { action = ShowWeapons, lines = new List<List<string>>() { new List<string>() { "", $"查看武器 - {t}", "", "" } } });
            contents.Add(new Content() { action = ShowMods, lines = new List<List<string>>() { new List<string>() { "", $"查看模组 - {t}", "", "" } } });
            ResetAndFlush();
        }

        public static void ShowAllTalents()
        {
            TitleFunc.pageStr = "天赋页";
            pageState = PageState.Entry;
            pageWhat = PageWhat.Talent;
            contents.Clear();
            contentInLine = 4;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            var nowColor = Color.Default;
            var nowType = String.Empty;
            var ind = 1;
            var isFirstInit = Config.D2Talents.Count == 0;
            var history = new List<string>();
            foreach (var bb in Translate.trans["talents"].Children<JProperty>())
            {
                var b = Translate.trans["talents"][bb.Name].ToString();
                switch (b)
                {
                    case "__具名武器天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__通用武器天赋":
                        nowColor = Color.Purple;
                        nowType = b.Substring(2);
                        break;
                    case "__步枪天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__突击步枪天赋":
                        nowColor = Color.Purple;
                        nowType = b.Substring(2);
                        break;
                    case "__射手步枪天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__霰弹枪天赋":
                        nowColor = Color.Purple;
                        nowType = b.Substring(2);
                        break;
                    case "__冲锋枪天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__轻机枪天赋":
                        nowColor = Color.Purple;
                        nowType = b.Substring(2);
                        break;
                    case "__手枪天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__防弹衣天赋":
                        nowColor = Color.Orange;
                        nowType = b.Substring(2);
                        break;
                    case "__背包天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    case "__装备组背包天赋":
                        nowColor = Color.Green;
                        nowType = b.Substring(2);
                        break;
                    case "__装备组防弹衣天赋":
                        nowColor = Color.Default;
                        nowType = b.Substring(2);
                        break;
                    default:
                        if (!string.IsNullOrWhiteSpace(b) && !history.Contains(b))
                        {
                            var i = lines.Count - 1;
                            if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                            {
                                lines[i].Add(TextSpawner.TalentsList(ind, b, nowType));
                                colors[i].Add(nowColor);
                            }
                            else
                            {
                                lines.Add(new List<List<string>>() { TextSpawner.TalentsList(ind, b, nowType) });
                                colors.Add(new List<Color>() { nowColor });
                            }
                            if (isFirstInit)
                            {
                                Config.D2Talents.Add(b);
                                Config.D2TalentsFrom.Add(nowType);
                            }
                            history.Add(b);
                            ind++;
                        }
                        break;
                }
            }
            if (lines[lines.Count - 1].Count % contentInLine != 0)
            {
                var needAdd = contentInLine - lines[lines.Count - 1].Count % contentInLine;
                for (int idd = 0; idd < needAdd; idd++)
                {
                    lines[lines.Count - 1].Add(new List<string>());
                    colors[lines.Count - 1].Add(Color.Default);
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            shower.lines = TextSpawner.TalentsLarge(ind, Config.D2Talents[0], Config.D2TalentsFrom[0]);
            shower.color = Color.Default;
            ResetAndFlush();
        }

        public static void ShowBrandsUseShower()
        {
            TitleFunc.pageStr = "装备套组页";
            pageState = PageState.Entry;
            pageWhat = PageWhat.BrandShower;
            contents.Clear();
            contentInLine = 1;
            nowLeft2RightIndex = 0;
            ResetAndFlush();
        }

        public static void ShowAllBrands()
        {
            TitleFunc.pageStr = "装备套组页";
            pageState = PageState.Entry;
            pageWhat = PageWhat.Brand;
            contents.Clear();
            contentInLine = 4;
            nowLeft2RightIndex = 0;
            var lines = new List<List<List<string>>>();
            var colors = new List<List<Color>>();
            var nowColor = Color.Default;
            var ind = 1;
            var isFirstInit = Config.D2Brands.Count == 0;
            var history = new List<string>();
            foreach (var bb in Translate.trans["brand"].Children<JProperty>())
            {
                var b = Translate.trans["brand"][bb.Name].ToString();
                if (String.IsNullOrWhiteSpace(b) || b.Equals("未知")) continue;
                else if (b.Equals("__套装")) nowColor = Color.Orange;
                else if (b.Equals("__装备组")) nowColor = Color.Green;
                else if (!string.IsNullOrWhiteSpace(b) && !history.Contains(b))
                {
                    var i = lines.Count - 1;
                    var c = nowColor == Color.Orange ? "顶级" : (nowColor == Color.Green ? "装备组" : "");
                    if (lines.Count != 0 && lines[i].Count < Controller.contentInLine)
                    {
                        lines[i].Add(TextSpawner.BrandsList(ind, b, c));
                        colors[i].Add(nowColor);
                    }
                    else
                    {
                        lines.Add(new List<List<string>>() { TextSpawner.BrandsList(ind, b, c) });
                        colors.Add(new List<Color>() { nowColor });
                    }
                    if (isFirstInit)
                    {
                        Config.D2Brands.Add(b);
                        Config.D2BrandsColor.Add(c);
                    }
                    history.Add(b);
                    ind++;
                }
            }
            if (lines[lines.Count - 1].Count % contentInLine != 0)
            {
                var needAdd = contentInLine - lines[lines.Count - 1].Count % contentInLine;
                for (int idd = 0; idd < needAdd; idd++)
                {
                    lines[lines.Count - 1].Add(new List<string>());
                    colors[lines.Count - 1].Add(Color.Default);
                }
            }
            foreach (var i in lines)
                contents.Add(new Content() { lines = i, theme = colors[lines.IndexOf(i)] });
            shower.lines = TextSpawner.BrandsLarge(ind, Config.D2Brands[0], Config.D2BrandsColor[0]);
            shower.color = colors[0][0];
            ResetAndFlush();
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }
    }

    public enum PageState
    {
        Main,
        History,
        HistorySelected,
        HistoryEntry,
        Entry,
        ErrorPopup,
    }

    public enum PageWhat
    {
        None,
        Best,
        Gear,
        Weapon,
        Mod,
        Talent,
        Brand,
        BrandShower,
    }
}
