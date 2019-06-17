using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWE1R_Overlay.Utilities
{
    public partial class Racer
    {
        public struct Val
        {
            static Dictionary<byte, string> tracks = new Dictionary<byte, string>()
            {
                { 0,  "The Boonta Training Course" }, // Amateur
                { 16, "Mon Gazza Speedway" },
                { 2,  "Beedo's Wild Ride" },
                { 6,  "Aquilaris Classic" },
                { 22, "Malastare 100" },
                { 19, "Vengeance" },
                { 17, "Spice Mine Run" },
                { 7,  "Sunken City" },                // Semi-Pro
                { 3,  "Howler Gorge" },
                { 23, "Dug Derby" },
                { 9,  "Scrapper's Run" },
                { 18, "Zugga Challenge" },
                { 12, "Baroo Coast" },
                { 8,  "Bumpy's Breakers" },
                { 20, "Executioner" },                // Galactic
                { 24, "Sebulba's Legacy" },
                { 13, "Grabvine Gateway" },
                { 4,  "Andobi Mountain Run" },
                { 10, "Dethro's Revenge" },
                { 14, "Fire Mountain Rally" },
                { 1,  "The Boonta Classic" },
                { 5,  "Ando Prime Centrum" },         // Invitational
                { 11, "Abyss" },
                { 21, "The Gauntlet" },
                { 15, "Inferno" }
            };

            static Dictionary<byte, string> worlds = new Dictionary<byte, string>()
            {
                // based on track order, need to verify (don't have world addr yet)
                { 0,  "Tatooine" },
                { 1,  "Ando Prime" },
                { 2,  "Aquilaris" },
                { 3,  "Ord Ibanna" },
                { 4,  "Baroonda" },
                { 5,  "Mon Gazza" },
                { 6,  "Oovo IV" },
                { 7,  "Malastare" }
            };
        }
    }
}
