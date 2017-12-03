using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Player : IComparable<Player>
    {
        //string _id;
        string _name;
        string _hexcolor;
        int _wrcount;
        int _uwrcount;
        int _gain;
        string _url;

        public List<Record> Records = new List<Record>();

        /*public Player(string id, string name, string hexcolor)
        {
            _id = id;
            _name = name;
            _hexcolor = hexcolor;
        }*/

        public Player(string name, string hexcolor, string url)
        {
            _name = name;
            _hexcolor = hexcolor;
            _url = url;
        }

        /*public string ID
        {
            get { return _id; }
            set { _id = value; }
        }*/

        public string Name
        {
            get { return _name; }
        }

        public string HexColor
        {
            get { return _hexcolor; }
        }

        public string URL
        {
            get { return _url; }
        }

        public int WRCount
        {
            get { return _wrcount; }
            set { _wrcount = value; }
        }

        public int UntiedCount
        {
            get { return _uwrcount; }
            set { _uwrcount = value; }
        }

        public int PointsGain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        public int CompareTo(Player other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
