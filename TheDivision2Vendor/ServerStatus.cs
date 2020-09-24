using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TheDivision2Vendor
{
    public static class ServerStatus
    {
        private static string apiUrl = "https://game-status-api.ubisoft.com/v1/instances?appIds=6c6b8cd7-d901-4cd5-8279-07ba92088f06,6f220906-8a24-4b6a-a356-db5498501572,7d9bbf16-d76d-43e1-9e82-1e64b4dd5543,42e81559-1fbc-42cd-bd12-e42460f9aaeb";
        public static List<string> Status = new List<string>();

        public static async Task<List<string>> GetStatus()
        {
            return await Task.Run(async () =>
            {
                string response = null;
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var hct = new HttpClient();
                    response = await hct.GetStringAsync(apiUrl);
                }
                catch (Exception e)
                {
                    Logger.Put(LogPopType.File, LogType.Warn, "访问育碧服务器API失败", e);
                }
                if (response == null)
                {
                    Status = new List<string>() { "", "访问育碧服务器API失败" };
                }
                else
                {
                    List<ServerStatue> status = null;
                    try { status = JsonConvert.DeserializeObject<List<ServerStatue>>(response); } catch (Exception) { }
                    if (status == null || status.Count < 1)
                    {
                        Status = new List<string>() { "", "访问育碧服务器API失败" };
                    }
                    else
                    {
                        var strs = new List<string>() { "" };
                        foreach (var item in status)
                        {
                            var nowstr = "";
                            switch (item.Platform.ToLower())
                            {
                                case "pc":
                                    nowstr += "PC: ";
                                    break;
                                case "ps4":
                                    nowstr += "PS4: ";
                                    break;
                                case "xboxone":
                                    nowstr += "Xbox: ";
                                    break;
                                case "stadia":
                                    nowstr += "Stadia: ";
                                    break;
                                default:
                                    break;
                            }
                            var serverMaintance = false;
                            var serverProblem = false;
                            if (item.Maintenance != null && !string.IsNullOrWhiteSpace(item.Maintenance))
                            {
                                serverMaintance = true;
                                nowstr += "维护";
                                nowstr = "§y" + nowstr;
                                Logger.Put(LogPopType.File, LogType.Info, $"游戏服务器正在维护：{item.Maintenance} ({item.Platform})");
                            }
                            else
                            {
                                switch (item.Status.ToLower())
                                {
                                    case "online":
                                        nowstr += "运行";
                                        nowstr = "§g" + nowstr;
                                        break;
                                    case "interrupted":
                                        serverProblem = true;
                                        nowstr += "故障";
                                        nowstr = "§o" + nowstr;
                                        break;
                                    case "degraded":
                                        serverProblem = true;
                                        nowstr += "断电";
                                        nowstr = "§r" + nowstr;
                                        break;
                                    default:
                                        Logger.Put(LogPopType.File, LogType.Info, $"检查服务器状态时遇到意外Status代号：{item.Status}");
                                        break;
                                }
                            }
                            if (serverMaintance)
                            {
                                TitleFunc.serverMaintance = true;
                                TitleFunc.serverProblem = false;
                            }
                            else if (serverProblem)
                            {
                                TitleFunc.serverMaintance = false;
                                TitleFunc.serverProblem = true;
                            }
                            else
                            {
                                TitleFunc.serverMaintance = false;
                                TitleFunc.serverProblem = false;
                            }
                            strs.Add(nowstr);
                        }
                        Status = strs;
                    }
                }
                return Status;
            });
        }
    }

    public class ServerStatue
    {
        [JsonProperty("Platform")]
        public string Platform { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Maintenance")]
        public string Maintenance { get; set; }
    }
}
