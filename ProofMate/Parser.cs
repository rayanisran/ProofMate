using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    static class Parser
    {
        private static string UrlRankings = "https://rankings.the-elite.net";
        private static string UrlHistory = "https://rankings.the-elite.net/history";
        private static string UrlUserCSS = "https://rankings.the-elite.net/css/users.1510033127.css";
        public static string UserColors = GetCSS(UrlUserCSS);

        public static HtmlDocument GetHistory(int year, int month)
        {
            HtmlWeb page = new HtmlWeb();
            HtmlDocument doc = page.Load(string.Concat(UrlHistory, "/", year, "/", month));
            return doc;
        }

        private static string GetCSS(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        public static string GetDate(HtmlNode RecordRow)
        {
            return RecordRow.ChildNodes[1].InnerText;
        }

        public static Player GetPlayer(HtmlNode RecordRow)
        {
            string id = GetUserId(RecordRow);
            string name = RecordRow.ChildNodes[3].InnerText;
            string hex = GetColorFromUserId(id);

            return new Player(name, hex);
            //return RecordRow.ChildNodes[3].InnerText;
        }

        public static string GetTime(HtmlNode RecordRow)
        {
            //So the time in the table has a lot of whitespace..
            //This makes it in the format: Aztec - Agent - 1:54.

            string time = RecordRow.ChildNodes[5].InnerText;
            time = Regex.Replace(time, @"\s{2,}", "").Replace("(untied!)", "").Trim();

            return time;
        }

        private static bool HasVideo(HtmlNode RecordRow)
        {
            return RecordRow.ChildNodes[7].InnerText == "Video" ? true : false;
        }

        public static int GetPoints(HtmlNode RecordRow)
        {
            string txt = RecordRow.ChildNodes[9].InnerText;
            return txt == "Unknown" ? 0 : int.Parse(txt);
        }

        public static string GetOldTime(HtmlNode RecordRow)
        {
            return RecordRow.ChildNodes[11].InnerText;
        }

        public static string GetTimeCut(HtmlNode RecordRow)
        {
            return RecordRow.ChildNodes[13].InnerText;
        }

        private static string GetUserId(HtmlNode RecordRow)
        {
            return Regex.Match(RecordRow.ChildNodes[3].InnerHtml, "u(\\d+)").Groups[1].Value;
        }

        private static string GetColorFromUserId(string Id)
        {
            return Regex.Match(UserColors, "u" + Id + "(?!\\d).*?{color:(#.*?)}").Groups[1].Value;
        }

        public static int GetPointGain(HtmlNode RecordRow)
        {
            string txt = RecordRow.ChildNodes[15].InnerText;
            return txt == "Unknown" ? 0 : int.Parse(txt);
        }

        private static bool IsWR(HtmlNode RecordRow)
        {
            return GetPoints(RecordRow) == 100 ? true : false;
        }

        private static bool IsUntied(HtmlNode RecordRow)
        {
            return RecordRow.ChildNodes[5].InnerText.Contains("untied") ? true : false;
        }

        public static Record.Type GetPRType(HtmlNode RecordRow)
        {
            return IsUntied(RecordRow) ? Record.Type.UWR : IsWR(RecordRow) ? Record.Type.WR : Record.Type.Regular;
        }

        public static string GetVideoLink(HtmlNode RecordRow)
        {
            if (!HasVideo(RecordRow))
                return "";

            var URL = string.Concat(UrlRankings, Regex.Match(RecordRow.ChildNodes[5].InnerHtml, "href=\"(.*?)\"").Groups[1]);
            var UsersPRPage = Parser.GetCSS(URL);
            var link = Regex.Match(UsersPRPage, "<a href=\"(.*)\">(Watch on .*|Download video)</a>").Groups[1].Value;

            return link;
        }
    }
}
