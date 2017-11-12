using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Record
    {
        Player _player;
        Type _rectype;

        string _date;
        string _time;
        string _level;
        string _video;
        //string _timecut;

        int _points;
        int _gain;

        bool _oldwr;

        public Record(string p_date, Player p_player, string p_time, string p_video, int p_points, int p_gain, Type p_type)
        {
            _date = p_date;
            _video = p_video;
            _points = p_points;
            _player = p_player;
            _rectype = p_type;
            _gain = p_gain;
            //_timecut = p_timecut;

            _level = LevelHelper.GetLevelAndDifficultyFromTime(p_time);
            _time = p_time.Replace("-", "");
        }

        public string Date
        {
            get { return _date; }
        }

        public Player Player
        {
            get { return _player; }
        }

        public string Time
        {
            get { return _time; }
        }

        public string VideoLink
        {
            get { return _video; }
        }

        public string Level
        {
            get { return _level; }
        }

        public int Points
        {
            get { return _points; }
        }

        public int PointsGain
        {
            get { return _gain; }
        }

        public Type RecordType
        {
            get { return _rectype; }
            set { _rectype = value; }
        }

        public bool IsOldWR
        {
            get { return _oldwr; }
            set { _oldwr = value; }
        }

        public enum Type
        {
            Regular,
            WR,
            UWR
        }

        public bool IsWR
        {
            get { return _rectype == Type.WR ? true : false; }
        }

        public bool IsUWR
        {
            get { return _rectype == Type.UWR ? true : false; }
        }

        public bool HasVideo
        {
            get { return string.IsNullOrEmpty(VideoLink) ? false : true; }
        }

        public int CompareTo(Record other)
        {
            return _rectype.CompareTo(other._rectype);
        }
    }
}
