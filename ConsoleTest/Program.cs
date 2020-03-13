using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheDivision2Vendor;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            Console.TreatControlCAsInput = true;
            Logs();
            MainFunc.Init();
            if (Config.D2Dirs.Count > 0) Controller.nowFileIndex = 0;
            WelcomeScreen();
            //var a = Config.D2Dirs;
            //Console.Title += "";
            _ = Task.Factory.StartNew(() =>
              {
                  while (true)
                  {
                      try
                      {
                          switch (Console.ReadKey(true).Key)
                          {
                              case ConsoleKey.Q:
                              case ConsoleKey.Escape:
                                  if (Controller.contents.Count > Controller.nowChoose)
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
            _ = Task.Factory.StartNew(() =>
            {
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;
                _ = new System.Threading.Timer(state =>
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
            });
            await Task.Delay(-1);
        }

        private static void Logs()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
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
            Controller.pageState = PageState.Main;
            Controller.pageWhat = PageWhat.None;
            Controller.lockScrollUp = 0;
            Controller.lockScrollDown = 1;
            Controller.lockLeftRightWhenHistoryEntry = false;
            Controller.contents.Clear();
            Controller.contents.Add(new Content() { action = Controller.ShowBest, lines = new List<List<string>>() { new List<string>() { "", "推荐装备", "现算法不考虑天赋和具名影响，只考虑装备非主电其余的优质(>=80%)词条数量" } } });
            Controller.contents.Add(new Content() { action = Controller.UpdateResources, lines = new List<List<string>>() { new List<string>() { "", "更新数据源", "最后资源日期：" + FileHint(), "数据源是人工输入，故会出现录入错误，请谅解。" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowGears, lines = new List<List<string>>() { new List<string>() { "", "查看防具", "" } } });
            Controller.contents.Add(new Content() { action = Controller.ShowWeapons, lines = new List<List<string>>() { new List<string>() { "", "查看武器", "" } } });
            Controller.contents.Add(new Content() { action = Controller.OpenHistory, lines = new List<List<string>>() { new List<string>() { "", "查看已保存的往期数据", "" } } });
            Controller.contents.Add(new Content() { action = Controller.Exit, lines = new List<List<string>>() { new List<string>() { "", "退出", "" } } });
            Controller.shower.lines = Shower.GetDefaultMsg();
            Controller.shower.color = Color.Default;
            Controller.Flush(null);
        }
    }
}
