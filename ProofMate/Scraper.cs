using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Diagnostics;
using System.IO;


namespace ConsoleApp2
{
    public class Scraper
    {
        const int point_threshold = 60;
        const string waiting1 = "Sit tight while it gets generated, mate..";
        const string waiting2 = "Grabbing records from the table..";
        string deadline = "";

        Game GE = new Game("GE");
        Game PD = new Game("PD");

        public void FetchRecords(int year, int month, string _deadline)
        {
            deadline = _deadline;
            Stopwatch s = new Stopwatch();
            s.Start();

            Console.WriteLine(waiting1);

            //Console.WriteLine(year + "/" + month);
            //return;

            //Load the HTML page.
            //Load the CSS file containing player IDs + their hex colors.

            HtmlDocument doc = null;
            string UserColors;
            try
            {
                doc = Parser.GetHistory(year, month);
                UserColors = Parser.UserColors;
            }
            catch (Exception ex) { Console.WriteLine("Sounds like you have an network error. Here's the full message:\n\n" + ex.ToString()); }

            Console.WriteLine(waiting2);
            var p = doc.DocumentNode.SelectSingleNode("//table");
            for (int j = 1; j < p.SelectNodes(".//tr").Count; j++)
            {
                //Select the entire row.
                var rec = p.SelectNodes(".//tr")[j];

                //Individual properties of the row.
                var points = Parser.GetPoints(rec);

                //If the time is worth less than 60 points, we don't need to proofcall it so move onto the next record.
                if (points < point_threshold)
                    continue;

                var date = Parser.GetDate(rec);
                var player = Parser.GetPlayer(rec);
                var time = Parser.GetTime(rec);
                var type = Parser.GetPRType(rec);
                var pointgain = Parser.GetPointGain(rec);
                //var timecut = Parser.GetTimeCut(rec);

                //This is empty if there is no video link.
                var vidLink = Parser.GetVideoLink(rec);

                Record R = new Record(date, player, time, vidLink, points, pointgain, type);

                if (LevelHelper.IsGELevel(time))
                    AddRecord(GE, R);
                else
                    AddRecord(PD, R);
            }

            GenerateHTML(GE);
            GenerateHTML(PD);
            s.Stop();

            Console.WriteLine($"done in {s.Elapsed}!");

        }

        public void AddRecord(Game game, Record record)
        {
            Player player = record.Player;

            //If the player doesn't exist, add him or her to the game.
            if (game.Players.FindIndex(a => a.Name == player.Name) == -1)
                game.Players.Add(new Player(player.Name, player.HexColor, player.URL));

            //Now get the fresh, new index of the player.
            int index = game.Players.FindIndex(a => a.Name == player.Name);

            //Check if the player has improved a time on the level. If so skip the previous.
            //But only skip it if it wasn't a world record. WRs/UWRs are relevant, so they'll still be posted.

            var AlreadyExists = game.Players[index].Records.FindIndex(a => a.Level == record.Level) != -1;
            var IsntWR = record.RecordType == Record.Type.Regular;
            var IsWR = !IsntWR;

            if (AlreadyExists && IsntWR)
                return;

            else if (AlreadyExists && IsWR)
                record.IsOldWR = true;

            Console.WriteLine($"Adding {record.Time} by {player.Name}");

            game.Players[index].Records.Add(record);
        }

        //TODO: REWRITE THIS CODE
        private void GenerateHTML(Game game)
        {
            game.Players.Sort();

            StringBuilder s = new StringBuilder();
            s.Append($@"Times that say [color=red][size=10pt][b]NEEDS PROOF[/b][/size][/color] need videos up by {deadline} 1st or they will be backrolled.
Also included are the point values, and whether the time was a [color=yellow]TWR[/color] or [color=orange]UWR[/color] when listed time was achieved.");

            s.AppendLine();
            s.AppendLine();

            foreach (Player player in game.Players)
            {
                //string gameLink = game.Name == "GE" ? "goldeneye" : "perfect-dark";
                //string timespage = $@"https://rankings.the-elite.net/~{player.Name.Replace(" ", "+")}/" + gameLink;

                //s.AppendLine($@"[color={player.HexColor}][size=10pt][b][url={timespage}]{player.Name}[/url][/b][/size][/color]");

                string game_append = game.Name == "GE" ? "/goldeneye" : "/perfect-dark";

                s.AppendLine($@"[color={player.HexColor}][size=10pt][b][url={player.URL + game_append}]{player.Name}[/url][/b][/size][/color]");

                foreach (Record record in player.Records)
                {
                    string time = "";

                    if (!string.IsNullOrEmpty(record.VideoLink))
                        time = $@"[url={record.VideoLink}]{record.Time}[/url]";
                    else
                        time = $"{record.Time}";

                    string style = record.IsUWR ? $"[color=orange]{time}[/color] (Untied!)" :
                                   record.IsWR ? $"[color=yellow]{time}[/color] (TWR)" :
                                                     $@"{time} ({record.Points} points)";
                    string isProven = "";

                    //If a WR record has been improved, we don't need to proofcall it, so skip this.
                    if (!record.IsOldWR)
                        isProven = record.HasVideo ? " - [color=limegreen][size=10pt][b]PROVEN[/b][/size][/color]" :
                                                       " - [color=red][size=10pt][b]NEEDS PROOF[/b][/size][/color] ";

                    if (record.IsWR)
                    {
                        player.WRCount++;
                        game.TotalWRs++;
                    }

                    if (record.IsUWR)
                    {
                        player.UntiedCount++;
                        game.TotalUntieds++;
                    }

                    s.AppendLine(style + isProven);
                    player.PointsGain += record.PointsGain;
                }

                s.AppendLine($@"[i][color=#999][b]Total Points Gained: [/b]{player.PointsGain}[/color][/i]");
                //s.AppendLine($@"[i][color=#999][b]Total Time Cut: [/b]{player.TimeCut}[/color][/i]");
                s.AppendLine();
            }

            var mostPtsGain = game.Players.GroupBy(item => item.PointsGain).OrderByDescending(g => g.Key).First().ToList();
            var mostTWRs = game.Players.GroupBy(item => item.WRCount).OrderByDescending(g => g.Key).First().ToList();
            var mostUntieds = game.Players.GroupBy(item => item.UntiedCount).OrderByDescending(g => g.Key).First().ToList();

            s.AppendLine("----------------------------------------------------");
            s.AppendLine();

            s.AppendLine($@"{game.TotalWRs} [color=yellow]TWRs[/color] achieved last month!");
            s.Append($@"{game.TotalUntieds} [color=orange]UWRs[/color] achieved last month!");

            s.AppendLine();
            s.AppendLine();

            if (mostPtsGain.Count == 1)
                s.AppendLine("This month's [b][font=Georgia][size=12pt]Player Of The Month[/size][/font][/b] is awarded to " +
                    $@"[color={mostPtsGain[0].HexColor}][size=10pt][b]{mostPtsGain[0].Name}[/b][/size][/color] with [b]{mostPtsGain[0].PointsGain}[/b] points gained last month!");

            else
            {
                s.Append(@"This month's [b][font=Georgia][size=12pt]Players Of The Month[/size][/font][/b] are awarded to ");

                foreach (var x in mostPtsGain)
                {
                    if (!x.Equals(mostPtsGain.Last()))
                        s.Append($@"[color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color], ");
                    else
                        s.Append($@"and [color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color] ");
                }

                s.AppendLine($"who all gained [b]{mostPtsGain[0].PointsGain}[/b] points last month!");
            }

            if (mostTWRs.Count == 1 && mostTWRs[0].WRCount >= 1)
                s.AppendLine("This month's [b][font=Georgia][size=12pt]Outstanding Player[/size][/font][/b] is awarded to " +
                    $@"[color={mostTWRs[0].HexColor}][size=10pt][b]{mostTWRs[0].Name}[/b][/size][/color] who achieved [b]{mostTWRs[0].WRCount}[/b] TWR(s) last month!");

            else if (mostTWRs.Count > 1 && mostTWRs[0].WRCount >= 1)
            {
                s.Append(@"This month's [b][font=Georgia][size=12pt]Outstanding Players[/size][/font][/b] are awarded to ");

                foreach (var x in mostTWRs)
                {
                    if (!x.Equals(mostTWRs.Last()))
                        s.Append($@"[color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color], ");
                    else
                        s.Append($@"and [color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color] ");
                }

                s.AppendLine($"who all achieved [b]{mostTWRs[0].WRCount}[/b] TWR(s) last month!");
            }

            if (mostUntieds.Count == 1 && mostUntieds[0].UntiedCount >= 1)
                s.Append("This month's [b][font=Georgia][size=12pt]Greatest Contributor[/size][/font][/b] is awarded to " +
                    $@"[color={mostUntieds[0].HexColor}][size=10pt][b]{mostUntieds[0].Name}[/b][/size][/color] who achieved [b]{mostUntieds[0].UntiedCount}[/b] UWR(s) last month!");

            else if (mostUntieds.Count > 1 && mostUntieds[0].UntiedCount >= 1)
            {
                s.Append(@"This month's [b][font=Georgia][size=12pt]Greatest Contributors[/size][/font][/b] are awarded to ");

                foreach (var x in mostUntieds)
                {
                    if (!x.Equals(mostUntieds.Last()))
                        s.Append($@"[color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color], ");
                    else
                        s.Append($@"and [color={x.HexColor}][size=10pt][b]{x.Name}[/b][/size][/color] ");
                }

                s.Append($"who all achieved [b]{mostUntieds[0].UntiedCount}[/b] UWR(s) last month!");
            }

            var directory = AppDomain.CurrentDomain.BaseDirectory;

            File.WriteAllText(string.Concat(directory, $"{game.Name}.txt"), s.ToString());
        }
    }
}
