using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length != 1)
            //    return;

            //if (args[0].ToLower() == "!proof")
            Scraper s = new Scraper();
            s.GenProof();
        }
    }
}
