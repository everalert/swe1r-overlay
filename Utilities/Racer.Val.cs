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
            readonly public static Dictionary<byte, string> pods = new Dictionary<byte, string>()
            {
                { 0,  "Anakin Skywalker" },
                { 1,  "Teemto Pagalies" },
                { 2,  "Sebulba" },
                { 3,  "Ratts Tyerell" },
                { 4,  "Aldar Beedo" },
                { 5,  "Mawhonic" },
                { 6,  "Ark 'Bumpy' Roose" },
                { 7,  "Wan Sandage" },
                { 8,  "Mars Guo" },
                { 9,  "Ebe Endocott" },
                { 10, "Dud Bolt" },
                { 11, "Gasgano" },
                { 12, "Clegg Holdfast" },
                { 13, "Elan Mak" },
                { 14, "Neva Kee" },
                { 15, "Bozzie Baranta" },
                { 16, "Boles Roor" },
                { 17, "Ody Mandrell" },
                { 18, "Fud Sang" },
                { 19, "Ben Quadinaros" },
                { 20, "Slide Paramita" },
                { 21, "Toy Dampner" },
                { 22, "'Bullseye' Navior" }
            };

            readonly public static Dictionary<byte, string> tracks = new Dictionary<byte, string>()
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

            readonly public static Dictionary<byte, string> worlds = new Dictionary<byte, string>()
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
