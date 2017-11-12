using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Player
    {
        string _name;
        string _alias;
        string _hexcolor;
        string _id;

        public Game Records = new Game();

        //public List<Record> GE = new List<Record>();
        //public List<Record> PD = new List<Record>();

        public Player(string id, string name, string alias, string hexcolor)
        {
            _id = id;
            _name = name ?? "None";
            _alias = alias ?? "None";
            _hexcolor = hexcolor;
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string RealName
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public string HexColor
        {
            get { return _hexcolor; }
            set { _hexcolor = value; }
        }
    }

    public static class LevelProps
    {
        static string[] LevelsGE = { "Dam", "Facility", "Runway", "Surface 1", "Bunker 1", "Silo", "Frigate", "Surface 2", "Bunker 2", "Statue",
            "Archives", "Streets", "Depot", "Train", "Jungle", "Control", "Caverns", "Cradle", "Aztec", "Egypt" };

        static string[] LevelsPD = { "Defection", "Investigation", "Extraction", "Villa", "Chicago", "G5", "Infiltration", "Rescue", "Escape", "Air Base",
            "Air Force One", "Crash Site", "Pelagic II", "Deep Sea", "CI", "Attack Ship", "Skedar Ruins", "MBR", "Maian SOS", "WAR!", "The Duel" };

        public static string LevelName(string rec)
        {
            string level = GetLevel(rec);
            string difficulty = GetDifficulty(rec);

            return string.Concat(level, " ", difficulty);
        }

        private static string GetLevel(string rec)
        {
            int firstIndex = rec.IndexOf('-');

            string level = rec.Substring(0, firstIndex).TrimEnd();
            return level;
        }

        private static string GetDifficulty(string rec)
        {
            int firstIndex = rec.IndexOf('-');
            int secondIndex = rec.IndexOf('-', rec.IndexOf('-') + 1);

            string difficulty = rec.Substring(firstIndex, secondIndex - firstIndex).Remove(0, 1);
            return difficulty;
        }

        public static bool IsGELevel(string level)
        {
            //We have the level and difficulty, but we only want to check for the level.
            level = GetLevel(level);

            return LevelsGE.Any(level.Contains) ? true : false;
        }

        public static bool IsPDLevel(string level)
        {
            level = GetLevel(level);
            return LevelsPD.Any(level.Contains) ? true : false;
        }
    }

    public class Game
    {
        public List<Record> GE = new List<Record>();
        public List<Record> PD = new List<Record>();
    }

    public struct Record
    {
        string _date;
        string _time;
        string _player;
        string _level;
        bool _video;
        bool _isWR;
        bool _isUntied;
        int _points;

        public Record(string p_date, string p_player, string p_time, string p_level, bool p_video, bool p_isWR, bool p_isUntied, int p_points)
        {
            _date = p_date;
            _time = p_time;
            _video = p_video;
            _points = p_points;
            _player = p_player;
            _isWR = p_isWR;
            _isUntied = p_isUntied;
            _level = p_level;
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public string LevelName
        {
            get { return LevelProps.LevelName(_level); }
        }

        public bool HasVideo
        {
            get { return _video; }
            set { _video = value; }
        }

        public bool IsWR
        {
            get { return _isWR; }
            set { _isWR = value; }
        }

        public bool IsUntied
        {
            get { return _isUntied; }
            set { _isUntied = value; }
        }

        public int Points
        {
            get { return _points; }
            set { _points = value; }
        }

        //public enum WRType
        //{
        //    Regular,
        //    WR,
        //    Untied,
        //}
    }
}
