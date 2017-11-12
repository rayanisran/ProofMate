using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ConsoleApp2
{
    public class Scraper
    {
        static string urlHistory = "https://rankings.the-elite.net/history";
        static string urlUserCSS = "https://rankings.the-elite.net/css/users.1510033127.css";
        List<Player> players = new List<Player>();

        public void GenProof()
        {
            string s = "";

            //Load the HTML page.
            HtmlWeb page = new HtmlWeb();
            HtmlDocument doc = page.Load(urlHistory);

            //Load the CSS file for user colors so we can link them.
            using (WebClient client = new WebClient())
            {
                s = client.DownloadString(urlUserCSS);
            }

            var p = doc.DocumentNode.SelectSingleNode("//table");
            for (int j = 1; j < p.SelectNodes(".//tr").Count; j++)
            {
                //Select the entire row.
                var rec = p.SelectNodes(".//tr")[j];
                var points = rec.ChildNodes[9].InnerText;

                //If the time is worth less than 60 points, we don't need to proofcall it.
                if (points == "Unknown" || int.Parse(points) < 60)
                    continue;

                var date = rec.ChildNodes[1].InnerText;
                var time = rec.ChildNodes[5].InnerText;

                //This truly is a 140+ IQ line written by me. It trims the extra whitespace crap added to a time. 
                //The last one makes up for a missing space (untied!) that shows up on untieds.
                time = Regex.Replace(time, @"\s{2,}", "").Trim().Replace("(", " (");
                var levelname = time;
                time = time.Replace("-", "");

                //Get the name. But the name doesn't tell me it's a real name or alias.
                //So I check based on the level name.
                var name = rec.ChildNodes[3].InnerText;
                var (ge_name, pd_name) = LevelProps.IsGELevel(levelname) ? (name, "none") : ("none", name);

                //Get the hex color of the user from the CSS file.
                var id = Regex.Match(rec.ChildNodes[3].InnerHtml, "u(\\d+)").Groups[1].Value;
                var color = Regex.Match(s, "u" + id + ".*?{color:(#.*?)}").Groups[1].Value;

                //Just figure out some generic properties of the time.
                var hasVideo = rec.ChildNodes[7].InnerText.Length != 0 ? true : false;
                var isWR = int.Parse(points) == 100 ? true : false;
                var isUntied = isWR && time.Contains("untied") ? true : false;

                //If the player doesn't exist, add him or her to the list.
                int index = players.FindIndex(a => a.ID == id);
                if (index == -1)
                    players.Add(new Player(id, ge_name, pd_name, color));

                index = players.FindIndex(a => a.ID == id);

                //The proof call only checks the quickest time of a player on a level.
                //The nice thing about the structure of the table is that the latest one will always be the quickest.
                //So just checking if the player and level exists is the same is enough to skip the time.

                //If for GoldenEye players, the level already exists, skip it.
                //Otherwise add it.
                //if (players[index].Records.GE.FindIndex(a => a.LevelName == LevelProps.LevelName(levelname)) != -1)
                //    continue;

                Record r = new Record(date, name, time, levelname, hasVideo, isWR, isUntied, int.Parse(points));

                if (players[index].Records.GE.FindIndex(a => a.LevelName == LevelProps.LevelName(levelname)) == -1)
                    players[index].Records.GE.Add(r);

                if (players[index].Records.PD.FindIndex(a => a.LevelName == LevelProps.LevelName(levelname)) == -1)
                    players[index].Records.PD.Add(r);
            }

            //return;
            GenerateHTML(true);

            //GE.Players[i].Name

            //for (int i = 0; i < players.Count; i++)
            //{
            //    Console.WriteLine(players[i].Alias + ", " + players[i].HexColor);

            //    for (int j = 0; j < players[i].Records.PD.Count; j++)
            //        Console.WriteLine(players[i].Records.PD[j].Time);

            //    Console.WriteLine();
            //}
        }

        private void GenerateHTML(bool ge)
        {
            StringBuilder s = new StringBuilder();
            s.Append(@"Times that say [color=red][size=10pt][b]NEEDS PROOF[/b][/size][/color] need videos up by December 1st or they will be backrolled.
Also included are the point values, and whether the time was a[color=yellow]TWR[/color] or[color=orange]UWR[/color] when listed time was achieved.\n");

            for (int i = 0; i < players.Count; i++)
            {
                if (ge)
                    s.Append($@"[color={players[i].HexColor}][size=10pt][b]{players[i].RealName}[/b][/size][/color]\n");
                else
                    s.Append($@"[color={players[i].HexColor}][size=10pt][b]{players[i].Alias}[/b][/size][/color]\n");
                //for (int j = 0; j < players[i].Records.PD.Count; j++)
                //    Console.WriteLine(players[i].Records.PD[j].Time);

                //Console.WriteLine();
            }

            Console.WriteLine(s);

            //            [color=#7ddb8c][size=10pt][b]Ryan White[/b][/size][/color]
            //Silo 00 Agent 1:23 (94 points) - [color=limegreen]
            //        [size=10pt]
            //        [b]
            //        PROVEN[/ b][/ size][/ color]
            //Silo 00 Agent 1:26 - [color=purple] [b] [glow=white,2,300] NTSC-J UWR[/ glow][/b] [/color] - [color=limegreen] [size=10pt] [b] PROVEN[/ b][/ size][/ color]
        }
    }
}
