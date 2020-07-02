using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheDivision2Vendor;

namespace ConsoleTest
{
    class Program
    {
        private static System.Threading.Timer timerScreenSizeChange;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            Console.WindowWidth = 159;
            Console.WindowHeight = 37;
            Console.TreatControlCAsInput = true;
            Logs();
            TitleFunc.Init();
            if (Config.D2Dirs.Count > 0) Controller.nowFileIndex = 0;
            WelcomeScreen();
            _ = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.Q:
                            case ConsoleKey.Backspace:
                            case ConsoleKey.Escape:
                                Controller.TurnBack();
                                break;
                            case ConsoleKey.Spacebar:
                            case ConsoleKey.Enter:
                                if (Controller.contents.Count > Controller.nowChoose)
                                    if (Controller.contents[Controller.nowChoose].action != null)
                                    {
                                        if (!Controller.lockUpdate)
                                        {
                                            Controller.lockUpdate = true;
                                            //Controller.contents[Controller.nowChoose].action.Invoke();
                                            try { Controller.contents[Controller.nowChoose].action.Invoke(); } catch (Exception) { }
                                            Controller.lockUpdate = false;
                                        }
                                    }
                                break;
                            case ConsoleKey.W:
                            case ConsoleKey.UpArrow:
                                if (Controller.nowChoose > 0)
                                {
                                    Controller.nowChoose--;
                                    if (Controller.pageState == PageState.History && Controller.nowFileIndex > 0) Controller.nowFileIndex--;
                                    Controller.Flush(true);
                                }
                                break;
                            case ConsoleKey.S:
                            case ConsoleKey.DownArrow:
                                if (Controller.nowChoose < Controller.contents.Count - 1)
                                {
                                    Controller.nowChoose++;
                                    if (Controller.pageState == PageState.History && Controller.nowFileIndex < Controller.contents.Count - 1) Controller.nowFileIndex++;
                                    Controller.Flush(false);
                                }
                                break;
                            case ConsoleKey.A:
                            case ConsoleKey.LeftArrow:
                                if (!Controller.lockLeftRightWhenHistoryEntry)
                                {
                                    if (Controller.contentInLine != 1 && Controller.nowLeft2RightIndex > 0)
                                    {
                                        Controller.nowLeft2RightIndex--;
                                        Controller.Flush(null, true);
                                    }
                                }
                                break;
                            case ConsoleKey.D:
                            case ConsoleKey.RightArrow:
                                if (!Controller.lockLeftRightWhenHistoryEntry)
                                {
                                    if (Controller.contentInLine != 1 && Controller.nowLeft2RightIndex < Controller.contentInLine - 1)
                                    {
                                        Controller.nowLeft2RightIndex++;
                                        Controller.Flush(null, true);
                                    }
                                }
                                break;
                            case ConsoleKey.PageUp:
                                if (Controller.nowChoose > 0)
                                {
                                    Controller.lockScrollUp = 0;
                                    Controller.lockScrollDown = 0;
                                    Controller.nowChoose = Math.Max(Controller.nowChoose - Controller.CanShow(), 0);
                                    Controller.Flush(true);
                                }
                                break;
                            case ConsoleKey.PageDown:
                                if (Controller.nowChoose < Controller.contents.Count - 1)
                                {
                                    Controller.lockScrollUp = 0;
                                    Controller.lockScrollDown = 4;
                                    Controller.nowChoose = Math.Min(Controller.nowChoose + Controller.CanShow(), Controller.contents.Count - 1);
                                    Controller.Flush(false);
                                }
                                break;
                            default:
                                break;
                        }
                        Console.CursorVisible = false;
                    }
                    catch (Exception) { }
                }
            });
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            timerScreenSizeChange = new System.Threading.Timer(state =>
            {
                try
                {
                    if (width != Console.WindowWidth || height != Console.WindowHeight)
                    {
                        width = Console.WindowWidth;
                        height = Console.WindowHeight;
                        Controller.lockScrollUp = 0;
                        Controller.lockScrollDown = 0;
                        Controller.Flush(null);
                    }
                }
                catch (Exception) { }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            CheckUpdate();
            await Task.Delay(-1);
        }

        private static void Logs()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        Log l = Logger.Take().GetAwaiter().GetResult();
                        switch (l.popup)
                        {
                            case LogPopType.Title:
                                Console.Title = l.msg;
                                break;
                            case LogPopType.File:
                                string msg = l.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "");
                                File.AppendAllLinesAsync(Config.Log, new List<string>() { msg }, Encoding.UTF8);
                                break;
                            case LogPopType.Popup:
                                if (l.e == null) MessageBox.Show(l.msg, "...", MessageBoxButtons.OK);
                                else MessageBox.Show(l.msg + "\r\n" + l.e.StackTrace, "?!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                        }
                    }
                    catch (Exception) { }
                }
            });
        }

        public static string FileHint()
        {
            if (Config.D2Dirs.Count == 0) return "本地未找到任何资源文件，请联网获取。";
            if (Controller.nowFileIndex < 0 || Config.D2Dirs.Count < Controller.nowFileIndex) return "文件读取下标错误：" + Controller.nowFileIndex;
            var sp = Config.D2Dirs[Controller.nowFileIndex].Path.Split("\\");
            var t = sp[sp.Length - 1];
            var ca = t.ToCharArray();
            if (ca.Length != 8) return "读取文件夹错误：" + t;
            var dt = DateTime.Parse($"{ca[0]}{ca[1]}{ca[2]}{ca[3]}.{ca[4]}{ca[5]}.{ca[6]}{ca[7]} AM 08:00:00");
            string[] day = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = day[Convert.ToInt32(dt.DayOfWeek.ToString("d"))].ToString();
            int weekYear = Util.GetWeekOfYear(dt);
            var text = dt.ToString("MM月dd日") + " " + week + " 第" + weekYear + "周";
            return text;
        }

        public static void WelcomeScreen()
        {
            Controller.nowChoose = 0;
            Controller.nowFileIndex = 0;
            Controller.nowLeft2RightIndex = 0;
            Controller.contentInLine = 1;
            TitleFunc.pageStr = "主菜单";
            Controller.pageState = PageState.Main;
            Controller.pageWhat = PageWhat.None;
            Controller.lockScrollUp = 0;
            Controller.lockScrollDown = 1;
            Controller.lockLeftRightWhenHistoryEntry = false;
            Controller.contents.Clear();
            Controller.contents.Add(new Content() { action = Controller.ShowBest, lines = new List<List<string>>() { new List<string>() { "", "推荐装备", "现算法不考虑天赋/具名/套装影响，装备额外考虑词条颜色统一", "半数以上词条优质(>=95%)则进入推荐，可配置文件自行更改", "" } } });
            Controller.contents.Add(new Content() { action = Controller.UpdateResources, lines = new List<List<string>>() { new List<string>() { "", "更新数据源（数据源可能会有延期情况）", "最后资源日期：" + FileHint(), "数据源是人工输入，故会出现录入错误，请谅解。" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowGears, lines = new List<List<string>>() { new List<string>() { "", "查看防具", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowWeapons, lines = new List<List<string>>() { new List<string>() { "", "查看武器", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowMods, lines = new List<List<string>>() { new List<string>() { "", "查看模组（秒/米单位不显示）", "" } } });
            Controller.contents.Add(new Content() { action = Controller.OpenHistory, lines = new List<List<string>>() { new List<string>() { "", "查看已保存的往期数据", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowBrandsUseShower, lines = new List<List<string>>() { new List<string>() { "", "列出套装效果（同屏显示）", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowAllBrands, lines = new List<List<string>>() { new List<string>() { "", "列出套装效果（逐一显示）", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowAllTalents, lines = new List<List<string>>() { new List<string>() { "", "列出全部天赋", "" } } });
            Controller.contents.Add(new Content() { action = Controller.Exit, lines = new List<List<string>>() { new List<string>() { "", "退出", "" } } });
            Controller.shower.lines = Shower.GetDefaultMsg();
            Controller.shower.color = Color.Default;
            Controller.Flush(null);
        }

        private static void CheckUpdate()
        {
            _ = Task.Run(() =>
            {
                string val = DownloadResource.CheckUpdate().GetAwaiter().GetResult();
                if (string.IsNullOrWhiteSpace(val)) Shower.newestVersion = "更新检查失败，请检查网络设置。";
                else if ("false".Equals(val)) Shower.newestVersion = "已关闭检查更新功能。";
                else
                {
                    Shower.newestVersion = "最新版本：" + val;
                    string[] d = val.Substring(1, val.Length - 1).Split(".");
                    try
                    {
                        if (int.Parse(d[0]) > Assembly.GetEntryAssembly().GetName().Version.Major || int.Parse(d[1]) > Assembly.GetEntryAssembly().GetName().Version.Minor)
                        {
                            TitleFunc.updateStr = "【有更新：" + val + "】";
                        }
                    }
                    catch (Exception) { }
                }
            });
        }
    }
}
