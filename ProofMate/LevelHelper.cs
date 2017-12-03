using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class LevelHelper
    {
        static string[] LevelsGE = { "Dam", "Facility", "Runway", "Surface 1", "Bunker 1", "Silo", "Frigate", "Surface 2", "Bunker 2", "Statue",
            "Archives", "Streets", "Depot", "Train", "Jungle", "Control", "Caverns", "Cradle", "Aztec", "Egypt" };

        //static string[] LevelsPD = { "Defection", "Investigation", "Extraction", "Villa", "Chicago", "G5", "Infiltration", "Rescue", "Escape", "Air Base",
        //   "Air Force One", "Crash Site", "Pelagic II", "Deep Sea", "CI", "Attack Ship", "Skedar Ruins", "MBR", "Maian SOS", "WAR!", "The Duel" };

        public static string GetLevelAndDifficultyFromTime(string time)
        {
            string level = GetLevelFromTime(time);
            string difficulty = GetDifficulty(time);

            return string.Concat(level, " ", difficulty);
        }

        private static string GetLevelFromTime(string rec)
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
            //We have the level and difficulty, but we only want to check the level name.
            level = GetLevelFromTime(level);

            return LevelsGE.Any(level.Contains) ? true : false;
        }

        //If it's not a GE level, it HAS to be a PD one!
        /*public static bool IsPDLevel(string level)
        {
            level = GetLevelFromTime(level);
            return LevelsPD.Any(level.Contains) ? true : false;
        }*/
    }
}