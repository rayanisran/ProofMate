using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Scraper s = new Scraper();

            if (args.Length == 0)
            {
                DateTime dt = DateTime.Now.AddMonths(-1);
                string deadline = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.AddMonths(2).Month);
                s.FetchRecords(dt.Year, dt.Month, deadline);
            }

            else if (args.Length == 1)
            {
                if (Regex.IsMatch(args[0], @"\d{4}/\d{2}"))
                {
                    //Make sure it's not into the future.
                    int year = int.Parse(args[0].Substring(0, 4));
                    int month = int.Parse(args[0].Substring(5));

                    string deadline = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month + 2);

                    if (IsFuture(year, month))
                        Console.WriteLine("You can't proof call the future!");
                    else
                        s.FetchRecords(year, month, deadline);
                }
                else
                    Console.WriteLine("Format must be YYYY/MM.");
            }
        }

        public static bool IsFuture(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
            return dt > DateTime.Now ? true : false;          
        }
    }
}
