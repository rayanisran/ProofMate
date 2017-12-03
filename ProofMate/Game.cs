using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Game
    {
        public List<Player> Players = new List<Player>();
        int _totalwrs;
        int _totaluwrs;
        string _name;

        public Game(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public int TotalWRs
        {
            get { return _totalwrs; }
            set { _totalwrs = value; }
        }

        public int TotalUntieds
        {
            get { return _totaluwrs; }
            set { _totaluwrs = value; }
        }

        //Returns 0 if a player doesn't exist.
        /*public int GetPlayerIndex(Player player)
        {
            int index = this.Players.FindIndex(a => a.Name == player.Name);

            return index != -1 ? index : 0;
        }*/
    }
}
