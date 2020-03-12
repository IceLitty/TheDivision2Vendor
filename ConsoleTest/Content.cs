using System;
using System.Collections.Generic;
using System.Text;
using TheDivision2Vendor;

namespace ConsoleTest
{
    class Content
    {
        public List<List<string>> lines = new List<List<string>>();
        public List<Color> theme = new List<Color>();
        public Action action = null;

        public List<string> Print(int row, int line, bool selected)
        {
            if (lines.Count == 0) return new List<string>();
            if (theme.Count < lines.Count)
                for (int i = 0; i < lines.Count; i++)
                {
                    if (theme.Count < i + 1)
                        theme.Add(Color.Default);
                }
            if (Controller.contentInLine != 1)
            {
                if (lines.Count % 2 != 0) lines.Add(new List<string>());
                if (theme.Count % 2 != 0) theme.Add(Color.Default);
                var each = (int) Math.Ceiling((double) row / Controller.contentInLine);
                var linesAll = new List<List<string>>();
                for (int i = 0; i < Controller.contentInLine; i++)
                    if (lines.Count > i)
                    {
                        var sel = selected;
                        if (sel)
                            if (i == Controller.nowLeft2RightIndex) sel = true;
                            else sel = false;
                        linesAll.Add(PrintPriv(each, line, sel, i));
                    }
                var newLines = new List<string>();
                if (linesAll.Count == 0) return newLines;
                for (int i = 0; i < line; i++)
                {
                    var eachLine = new StringBuilder();
                    foreach (var ll in linesAll)
                        if (ll.Count > i)
                            eachLine.Append(ll[i]);
                    if (eachLine.Length != 0) newLines.Add(eachLine.ToString());
                }
                return newLines;
            }
            else return PrintPriv(row, line, selected);
        }

        private List<string> PrintPriv(int row, int line, bool selected, int useLinesWhat = 0)
        {
            Color color = selected ? ReverseColor(theme[useLinesWhat]) : theme[useLinesWhat];
            var sbl = new List<string>();
            Color frameColor = GetFrameColor(color);
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
                    sbl.Add(GetColorS(frameColor) + sb.ToString());
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
                    sbl.Add(GetColorS(frameColor) + sb.ToString());
                }
                else
                {
                    string text = String.Empty;
                    if (lines[useLinesWhat].Count >= hIndex) text = lines[useLinesWhat][hIndex - 1];
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
                    sb.Append(GetColorS(frameColor) + "┃ ");
                    sb.Append(GetColorS(color) + text);
                    sb.Append(GetColorS(frameColor) + "┃");
                    sbl.Add(sb.ToString());
                }
            }
            return sbl;
        }

        public static string GetColorS(Color color)
        {
            switch (color)
            {
                case Color.Purple:
                    return "§p";
                case Color.Orange:
                    return "§o";
                case Color.Named:
                    return "§n";
                case Color.Green:
                    return "§g";
                case Color.PurpleReverse:
                    return "§q";
                case Color.OrangeReverse:
                    return "§0";
                case Color.NamedReverse:
                    return "§u";
                case Color.GreenReverse:
                    return "§8";
                case Color.DefaultReverse:
                    return "§b";
                case Color.Red:
                    return "§r";
                case Color.Blue:
                    return "§c";
                case Color.Yellow:
                    return "§y";
                case Color.Default:
                default:
                    return "§w";
            }
        }

        public static Color GetColorFs(string s)
        {
            switch (s)
            {
                case "§p":
                    return Color.Purple;
                case "§o":
                    return Color.Orange;
                case "§n":
                    return Color.Named;
                case "§g":
                    return Color.Green;
                case "§q":
                    return Color.PurpleReverse;
                case "§0":
                    return Color.OrangeReverse;
                case "§u":
                    return Color.NamedReverse;
                case "§8":
                    return Color.GreenReverse;
                case "§b":
                    return Color.DefaultReverse;
                case "§r":
                    return Color.Red;
                case "§c":
                    return Color.Blue;
                case "§y":
                    return Color.Yellow;
                case "§w":
                default:
                    return Color.Default;
            }
        }

        public static Color ReverseColor(Color color)
        {
            switch (color)
            {
                case Color.Default:
                    return Color.DefaultReverse;
                case Color.Purple:
                    return Color.PurpleReverse;
                case Color.Orange:
                    return Color.OrangeReverse;
                case Color.Named:
                    return Color.NamedReverse;
                case Color.Green:
                    return Color.GreenReverse;
                case Color.DefaultReverse:
                    return Color.Default;
                case Color.PurpleReverse:
                    return Color.Purple;
                case Color.OrangeReverse:
                    return Color.Orange;
                case Color.NamedReverse:
                    return Color.Named;
                case Color.GreenReverse:
                    return Color.Green;
                default:
                    return color;
            }
        }

        public static Color GetFrameColor(Color color)
        {
            switch (color)
            {
                case Color.DefaultReverse:
                    return Color.Default;
                case Color.PurpleReverse:
                    return Color.Purple;
                case Color.OrangeReverse:
                    return Color.Orange;
                case Color.NamedReverse:
                    return Color.Named;
                case Color.GreenReverse:
                    return Color.Green;
                default:
                    return color;
            }
        }

        public static string RemoveColor(string str)
        {
            return str.Replace("§p", "")
                .Replace("§o", "")
                .Replace("§n", "")
                .Replace("§g", "")
                .Replace("§q", "")
                .Replace("§0", "")
                .Replace("§u", "")
                .Replace("§8", "")
                .Replace("§b", "")
                .Replace("§r", "")
                .Replace("§c", "")
                .Replace("§y", "")
                .Replace("§w", "");
        }
    }

    public enum Color
    {
        Default,
        Purple,
        Orange,
        Named,
        Green,
        DefaultReverse,
        PurpleReverse,
        OrangeReverse,
        NamedReverse,
        GreenReverse,
        Red,
        Blue,
        Yellow
    }
}
