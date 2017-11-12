using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class WR
    {
        int _wrcount;
        int _untiedcount;

        public int WRCount
        {
            get { return _wrcount; }
            set { _wrcount = value; }
        }

        public int UntiedCount
        {
            get { return _untiedcount; }
            set { _untiedcount = value; }
        }
    }
}
