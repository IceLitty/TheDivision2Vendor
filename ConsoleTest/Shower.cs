using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTest
{
    class Shower
    {
        public List<string> lines = new List<string>();
        public Color color = Color.Default;

        public List<string> Print(int row, int line)
        {
            if (lines.Count == 0) lines = GetDefaultMsg();
            var sbl = new List<string>();
            for (int hIndex = 0; hIndex < line; hIndex++)
            {
                if (hIndex == 0)
                {
                    var sb = new StringBuilder();
                    for (int wIndex = 0; wIndex < row - 1; wIndex++)
                    {
                        if (wIndex == 0) sb.Append("┏");
                        else if (wIndex == row - 2) sb.Append("┓");
                        else sb.Append("━");
                    }
                    sbl.Add(Content.GetColorS(color) + sb.ToString());
                }
                else if (hIndex == line - 1)
                {
                    var sb = new StringBuilder();
                    for (int wIndex = 0; wIndex < row - 1; wIndex++)
                    {
                        if (wIndex == 0) sb.Append("┗");
                        else if (wIndex == row - 2) sb.Append("┛");
                        else sb.Append("━");
                    }
                    sbl.Add(Content.GetColorS(color) + sb.ToString());
                }
                else
                {
                    string text = String.Empty;
                    if (lines.Count >= hIndex) text = lines[hIndex - 1];
                    int length = 0;// String.IsNullOrWhiteSpace(text) ? 0 : Encoding.UTF8.GetBytes(text).Length;
                    // fix utf8 other lang char added 1 length problem?
                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        bool isColor = false;
                        foreach (char c in text)
                        {
                            if (c == '§') isColor = true;
                            else
                            {
                                if (isColor) isColor = false;
                                else
                                {
                                    int l = Encoding.UTF8.GetBytes(c.ToString()).Length;
                                    if (l > 1) l -= 1;
                                    length += l;
                                }
                            }
                        }
                    }
                    // row == spIndex; spIndex - 3 == each line text can be length; -1 is for space append each line.
                    int canRange = row - 3 - 1;
                    if (length > canRange)
                    {
                        var ss = new StringBuilder();
                        int nowLength = 0;
                        foreach (char c in text)
                        {
                            int l = Encoding.UTF8.GetBytes(c.ToString()).Length;
                            if (nowLength + l > canRange)
                            {
                                for (int i = 0; i < canRange - nowLength; i++)
                                    ss.Append(" ");
                                break;
                            }
                            ss.Append(c);
                            nowLength += l;
                        }
                        text = ss.ToString();
                    }
                    else
                        for (int i = 0; i < canRange - length; i++)
                            text += " ";
                    var sb = new StringBuilder();
                    sb.Append(Content.GetColorS(color) + "┃ ");
                    sb.Append(Content.GetColorS(Color.Default) + text);
                    sb.Append(Content.GetColorS(color) + "┃");
                    sbl.Add(sb.ToString());
                }
            }
            return sbl;
        }

        public static List<string> GetDefaultMsg()
        {
            return new List<string>() { "操作说明：", "WSAD/方向箭头 控制上下左右选择", "上页/下页 控制上下翻页", "空格/回车 确认", "Esc/Q 返回上一步" };
        }
    }
}
