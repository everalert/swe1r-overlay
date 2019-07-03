using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWE1R
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

            readonly public static Dictionary<byte, PodStats> pod_base_stats = new Dictionary<byte, PodStats>()
            {
                {0,new PodStats(0.5f,300f,110f,3f,490f,30f,60f,200f,13f,9f,4.99f,0.4f,50f,0.6f,5f)},
                {1,new PodStats(0.5f,260f,90f,1.7f,479f,30f,80f,195f,14f,6f,4.99f,0.43f,50f,0.5f,8f)},
                {2,new PodStats(0.38f,228f,95f,3.2f,600f,38f,50f,185f,9f,2f,4.99f,0.19f,80f,0.3f,7f)},
                {3,new PodStats(0.35f,238f,90f,4f,520f,33f,80f,300f,12f,5f,4.99f,0.3f,55f,0.45f,7f)},
                {4,new PodStats(0.3f,250f,85f,4f,517f,35f,85f,360f,10.5f,4.5f,4.99f,0.25f,68f,0.4f,7f)},
                {5,new PodStats(0.36f,224f,100f,3.75f,480f,41f,80f,350f,13f,7f,4.99f,0.2f,60f,0.48f,7f)},
                {6,new PodStats(0.3f,202f,85f,1f,485f,30f,80f,210f,10.5f,6.5f,6.5f,0.1f,70f,0.25f,6f)},
                {7,new PodStats(0.8f,294f,95f,1.9f,480f,25f,70f,180f,9f,3f,7f,0.19f,60f,0.5f,7f)},
                {8,new PodStats(0.6f,288f,100f,2.3f,540f,30f,85f,315f,7.5f,2.1f,6f,0.35f,70f,0.5f,10f)},
                {9,new PodStats(0.6f,294f,100f,2.5f,500f,40f,70f,190f,15.2f,11f,4.99f,0.45f,45f,0.7f,4.8f)},
                {10,new PodStats(0.54f,215f,80f,3f,505f,35f,90f,230f,8.6f,2.5f,4.99f,0.2f,70f,0.35f,5.5f)},
                {11,new PodStats(0.43f,238f,82f,3.3f,510f,43f,83f,310f,12.5f,1.7f,4.99f,0.4f,63f,0.43f,4.2f)},
                {12,new PodStats(0.5f,252f,89f,1.75f,495f,45f,80f,303f,11.5f,5f,6f,0.31f,55f,0.43f,7f)},
                {13,new PodStats(0.3f,224f,95f,3.75f,480f,40f,70f,360f,10f,2.5f,4.99f,0.3f,53f,0.5f,6f)},
                {14,new PodStats(0.8f,230f,115f,1f,480f,30f,70f,280f,11.5f,3.3f,4.99f,0.32f,55f,0.6f,7f)},
                {15,new PodStats(0.33f,294f,90f,2.1f,485f,42f,83f,275f,11.8f,3.5f,4.99f,0.3f,60f,0.55f,7f)},
                {16,new PodStats(0.3f,280f,83f,2.85f,590f,35f,85f,390f,9.5f,2.7f,4.99f,0.18f,62f,0.3f,7f)},
                {17,new PodStats(0.45f,238f,90f,1.8f,475f,30f,65f,240f,11f,4.4f,5f,0.4f,57f,0.6f,5.2f)},
                {18,new PodStats(0.35f,245f,90f,2.85f,490f,30f,75f,250f,12f,6.5f,4.99f,0.39f,53f,0.55f,7f)},
                {19,new PodStats(0.45f,203f,89f,3f,575f,40f,95f,400f,8f,2f,4.99f,0.28f,73f,0.45f,7f)},
                {20,new PodStats(0.43f,297f,120f,1.95f,475f,30f,80f,200f,16f,12f,4.99f,0.63f,40f,0.8f,7f)},
                {21,new PodStats(0.5f,270f,86f,1.75f,485f,25f,70f,240f,12.5f,10f,4.99f,0.5f,40f,0.65f,7f)},
                {22,new PodStats(0.7f,322f,120f,1.8f,480f,25f,70f,300f,15f,11f,4.99f,0.55f,45f,0.77f,7f)}
            };

            readonly public static Dictionary<string, UpgradeStats> pod_upgrades = new Dictionary<string, UpgradeStats>()
            {
                {"anti_skid",     new UpgradeStats(0.05f,0.1f,0.15f,0.2f,0.25f) },
                {"turn_response", new UpgradeStats(116,242,348,464,578) },
                {"acceleration",  new UpgradeStats(0.14f,0.28f,0.42f,0.56f,0.70f) },
                {"max_speed",     new UpgradeStats(40,80,120,160,200) },
                {"air_brake_inv", new UpgradeStats(0.08f,0.17f,0.26f,0.35f,0.44f) },
                {"cool_rate",     new UpgradeStats(1.6f,3.2f,4.8f,6.4f,8.0f) },
                {"repair_rate",   new UpgradeStats(0.1f,0.2f,0.3f,0.4f,0.45f) }
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

            public struct PodStats
            {
                public float anti_skid, turn_response, max_turn_rate, acceleration, max_speed,
                    air_brake_inv, decel_inv, boost_thrust, heat_rate, cool_rate,
                    hover_height, repair_rate, bump_mass, dmg_immunity, isect_radius;

                public PodStats(float AS, float TR, float MTR, float A, float MS,
                    float ABI, float DI, float BT, float HR, float CR,
                    float HH, float RR, float BM, float DmI, float ISR)
                {
                    anti_skid = AS > 0 ? AS : 0;
                    turn_response = TR > 0 ? TR : 0;
                    max_turn_rate = MTR > 0 ? MTR : 0;
                    acceleration = A > 0 ? A : 0;
                    max_speed = MS > 0 ? MS : 0;
                    air_brake_inv = ABI > 0 ? ABI : 0;
                    decel_inv = DI > 0 ? DI : 0;
                    boost_thrust = BT > 0 ? BT : 0;
                    heat_rate = HR > 0 ? HR : 0;
                    cool_rate = CR > 0 ? CR : 0;
                    hover_height = HH > 0 ? HH : 0;
                    repair_rate = RR > 0 ? RR : 0;
                    bump_mass = BM > 0 ? BM : 0;
                    dmg_immunity = DmI > 0 ? DmI : 0;
                    isect_radius = ISR > 0 ? ISR : 0;
                }
            }
            public struct UpgradeStats
            {
                public float level1, level2, level3, level4, level5;

                public UpgradeStats(float lv1, float lv2, float lv3, float lv4, float lv5)
                {
                    level1 = lv1 > 0 ? lv1 : 0;
                    level2 = lv2 > 0 ? lv2 : 0;
                    level3 = lv3 > 0 ? lv3 : 0;
                    level4 = lv4 > 0 ? lv4 : 0;
                    level5 = lv5 > 0 ? lv5 : 0;
                }
            }
        }
    }
}
