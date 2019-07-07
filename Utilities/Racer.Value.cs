using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWE1R
{
    public partial class Racer
    {
        readonly public struct Value
        {

            public static float CalculateUpgradedStat(byte vehicle, byte stat, byte level)
            {
                if (vehicle < 0 || vehicle > 22)
                    throw new Exception("Vehicle invalid.");
                switch (stat)
                {
                    case Upgrade.AntiSkid:
                        return Math.Max(Vehicle.Stats[vehicle].AntiSkid + Upgrade.Stats[stat].GetLevel(level), 1);
                    case Upgrade.TurnResponse:
                        return Vehicle.Stats[vehicle].TurnResponse + Upgrade.Stats[stat].GetLevel(level);
                    case Upgrade.Acceleration:
                        return Vehicle.Stats[vehicle].Acceleration - Upgrade.Stats[stat].GetLevel(level);
                    case Upgrade.MaxSpeed:
                        return Math.Max(Vehicle.Stats[vehicle].MaxSpeed + Upgrade.Stats[stat].GetLevel(level), 650);
                    case Upgrade.AirBrakeInv:
                        return Vehicle.Stats[vehicle].AirBrakeInv - Upgrade.Stats[stat].GetLevel(level);
                    case Upgrade.CoolRate:
                        return Vehicle.Stats[vehicle].CoolRate + Upgrade.Stats[stat].GetLevel(level);
                    case Upgrade.RepairRate:
                        return Math.Max(Vehicle.Stats[vehicle].RepairRate + Upgrade.Stats[stat].GetLevel(level), 1);
                    default:
                        throw new Exception("Stat invalid.");
                }
            }

            readonly public struct Vehicle
            {
                public const byte AnakinSkywalker = 0x00, 
                    TeemtoPagalies = 0x01,
                    Sebulba = 0x02,
                    RattsTyerell = 0x03,
                    AldarBeedo = 0x04,
                    Mawhonic = 0x05,
                    ArkBumpyRoose = 0x06,
                    WanSandage = 0x07,
                    MarsGuo = 0x08,
                    EbeEndocott = 0x09,
                    DudBolt = 0x0A,
                    Gasgano = 0x0B,
                    CleggHoldfast = 0x0C,
                    ElanMak = 0x0D,
                    NevaKee = 0x0E,
                    BozzieBaranta = 0x0F,
                    BolesRoor = 0x10,
                    OdyMandrell = 0x11,
                    FudSang = 0x12,
                    BenQuadinaros = 0x13,
                    SlideParamita = 0x14,
                    ToyDampner = 0x15,
                    BullseyeNavior = 0x16;

                readonly public static Dictionary<byte, string> Name = new Dictionary<byte, string>()
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

                readonly public static Dictionary<byte, VehicleStats> Stats = new Dictionary<byte, VehicleStats>()
                {
                    {0,new VehicleStats(0.5f,300f,110f,3f,490f,30f,60f,200f,13f,9f,4.99f,0.4f,50f,0.6f,5f)},
                    {1,new VehicleStats(0.5f,260f,90f,1.7f,479f,30f,80f,195f,14f,6f,4.99f,0.43f,50f,0.5f,8f)},
                    {2,new VehicleStats(0.38f,228f,95f,3.2f,600f,38f,50f,185f,9f,2f,4.99f,0.19f,80f,0.3f,7f)},
                    {3,new VehicleStats(0.35f,238f,90f,4f,520f,33f,80f,300f,12f,5f,4.99f,0.3f,55f,0.45f,7f)},
                    {4,new VehicleStats(0.3f,250f,85f,4f,517f,35f,85f,360f,10.5f,4.5f,4.99f,0.25f,68f,0.4f,7f)},
                    {5,new VehicleStats(0.36f,224f,100f,3.75f,480f,41f,80f,350f,13f,7f,4.99f,0.2f,60f,0.48f,7f)},
                    {6,new VehicleStats(0.3f,202f,85f,1f,485f,30f,80f,210f,10.5f,6.5f,6.5f,0.1f,70f,0.25f,6f)},
                    {7,new VehicleStats(0.8f,294f,95f,1.9f,480f,25f,70f,180f,9f,3f,7f,0.19f,60f,0.5f,7f)},
                    {8,new VehicleStats(0.6f,288f,100f,2.3f,540f,30f,85f,315f,7.5f,2.1f,6f,0.35f,70f,0.5f,10f)},
                    {9,new VehicleStats(0.6f,294f,100f,2.5f,500f,40f,70f,190f,15.2f,11f,4.99f,0.45f,45f,0.7f,4.8f)},
                    {10,new VehicleStats(0.54f,215f,80f,3f,505f,35f,90f,230f,8.6f,2.5f,4.99f,0.2f,70f,0.35f,5.5f)},
                    {11,new VehicleStats(0.43f,238f,82f,3.3f,510f,43f,83f,310f,12.5f,1.7f,4.99f,0.4f,63f,0.43f,4.2f)},
                    {12,new VehicleStats(0.5f,252f,89f,1.75f,495f,45f,80f,303f,11.5f,5f,6f,0.31f,55f,0.43f,7f)},
                    {13,new VehicleStats(0.3f,224f,95f,3.75f,480f,40f,70f,360f,10f,2.5f,4.99f,0.3f,53f,0.5f,6f)},
                    {14,new VehicleStats(0.8f,230f,115f,1f,480f,30f,70f,280f,11.5f,3.3f,4.99f,0.32f,55f,0.6f,7f)},
                    {15,new VehicleStats(0.33f,294f,90f,2.1f,485f,42f,83f,275f,11.8f,3.5f,4.99f,0.3f,60f,0.55f,7f)},
                    {16,new VehicleStats(0.3f,280f,83f,2.85f,590f,35f,85f,390f,9.5f,2.7f,4.99f,0.18f,62f,0.3f,7f)},
                    {17,new VehicleStats(0.45f,238f,90f,1.8f,475f,30f,65f,240f,11f,4.4f,5f,0.4f,57f,0.6f,5.2f)},
                    {18,new VehicleStats(0.35f,245f,90f,2.85f,490f,30f,75f,250f,12f,6.5f,4.99f,0.39f,53f,0.55f,7f)},
                    {19,new VehicleStats(0.45f,203f,89f,3f,575f,40f,95f,400f,8f,2f,4.99f,0.28f,73f,0.45f,7f)},
                    {20,new VehicleStats(0.43f,297f,120f,1.95f,475f,30f,80f,200f,16f,12f,4.99f,0.63f,40f,0.8f,7f)},
                    {21,new VehicleStats(0.5f,270f,86f,1.75f,485f,25f,70f,240f,12.5f,10f,4.99f,0.5f,40f,0.65f,7f)},
                    {22,new VehicleStats(0.7f,322f,120f,1.8f,480f,25f,70f,300f,15f,11f,4.99f,0.55f,45f,0.77f,7f)}
                };
            }

            readonly public struct Track
            {
                public const byte TheBoontaTrainingCourse = 0x00,
                    MonGazzaSpeedway = 0x10,
                    BeedosWildRide = 0x02,
                    AquilarisClassic = 0x06,
                    Malastare100 = 0x16,
                    Vengeance = 0x13,
                    SpiceMineRun = 0x11,
                    SunkenCity = 0x07,
                    HowlerGorge = 0x03,
                    DugDerby = 0x17,
                    ScrappersRun = 0x09,
                    ZuggaChallenge = 0x12,
                    BarooCoast = 0x0C,
                    BumpysBreakers = 0x08,
                    Executioner = 0x14,
                    SebulbasLegacy = 0x18,
                    GrabvineGateway = 0x0D,
                    AndobiMountainRun = 0x04,
                    DethrosRevenge = 0x0A,
                    FireMountainRally = 0x0E,
                    TheBoontaClassic = 0x01,
                    AndoPrimeCentrum = 0x05,
                    Abyss = 0x0B,
                    TheGauntlet = 0x15,
                    Inferno = 0x0F;

                readonly public static Dictionary<byte, string> Name = new Dictionary<byte, string>()
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

                readonly public static Dictionary<byte, byte> Favorite = new Dictionary<byte, byte>()
                {
                    { 0,  Vehicle.Sebulba }, // Amateur
                    { 16, Vehicle.TeemtoPagalies },
                    { 2,  Vehicle.AldarBeedo },
                    { 6,  Vehicle.CleggHoldfast },
                    { 22, Vehicle.DudBolt },
                    { 19, Vehicle.FudSang },
                    { 17, Vehicle.MarsGuo },
                    { 7,  Vehicle.BullseyeNavior },                // Semi-Pro
                    { 3,  Vehicle.RattsTyerell },
                    { 23, Vehicle.ElanMak },
                    { 9,  Vehicle.WanSandage },
                    { 18, Vehicle.BolesRoor },
                    { 12, Vehicle.NevaKee },
                    { 8,  Vehicle.ArkBumpyRoose },
                    { 20, Vehicle.ToyDampner },                // Galactic
                    { 24, Vehicle.Sebulba },
                    { 13, Vehicle.AnakinSkywalker },
                    { 4,  Vehicle.Mawhonic },
                    { 10, Vehicle.OdyMandrell },
                    { 14, Vehicle.EbeEndocott },
                    { 1,  Vehicle.Sebulba },
                    { 5,  Vehicle.SlideParamita },         // Invitational
                    { 11, Vehicle.BozzieBaranta },
                    { 21, Vehicle.Gasgano },
                    { 15, Vehicle.BenQuadinaros }
                };
            }

            readonly public struct Upgrade
            {
                public const byte AntiSkid = 0x0,
                    TurnResponse = 0x1,
                    Acceleration = 0x2,
                    MaxSpeed = 0x3,
                    AirBrakeInv = 0x4,
                    CoolRate = 0x5,
                    RepairRate = 0x6;

                readonly public static Dictionary<byte, UpgradeStats> Stats = new Dictionary<byte, UpgradeStats>()
                {
                    {AntiSkid,     new UpgradeStats(0.05f,0.1f,0.15f,0.2f,0.25f) },
                    {TurnResponse, new UpgradeStats(116,242,348,464,578) },
                    {Acceleration, new UpgradeStats(0.14f,0.28f,0.42f,0.56f,0.70f) },
                    {MaxSpeed,     new UpgradeStats(40,80,120,160,200) },
                    {AirBrakeInv,  new UpgradeStats(0.08f,0.17f,0.26f,0.35f,0.44f) },
                    {CoolRate,     new UpgradeStats(1.6f,3.2f,4.8f,6.4f,8.0f) },
                    {RepairRate,   new UpgradeStats(0.1f,0.2f,0.3f,0.4f,0.45f) }
                };
            }

            readonly public struct World
            {
                public const byte Tatooine = 0,
                    AndoPrime = 1,
                    Aquilaris = 2,
                    OrdIbanna = 3,
                    Baroonda = 4,
                    MonGazza = 5,
                    OovoIV = 6,
                    Malastare = 7;

                readonly public static Dictionary<byte, string> Name = new Dictionary<byte, string>()
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

            public struct VehicleStats
            {
                readonly public float AntiSkid, TurnResponse, MaxTurnRate, Acceleration, MaxSpeed,
                    AirBrakeInv, DecelInv, BoostThrust, HeatRate, CoolRate,
                    HoverHeight, RepairRate, BumpMass, DmgImmunity, ISectRadius;

                public VehicleStats(float AS, float TR, float MTR, float A, float MS,
                    float ABI, float DI, float BT, float HR, float CR,
                    float HH, float RR, float BM, float DmI, float ISR)
                {
                    AntiSkid = AS > 0 ? AS : 0;
                    TurnResponse = TR > 0 ? TR : 0;
                    MaxTurnRate = MTR > 0 ? MTR : 0;
                    Acceleration = A > 0 ? A : 0;
                    MaxSpeed = MS > 0 ? MS : 0;
                    AirBrakeInv = ABI > 0 ? ABI : 0;
                    DecelInv = DI > 0 ? DI : 0;
                    BoostThrust = BT > 0 ? BT : 0;
                    HeatRate = HR > 0 ? HR : 0;
                    CoolRate = CR > 0 ? CR : 0;
                    HoverHeight = HH > 0 ? HH : 0;
                    RepairRate = RR > 0 ? RR : 0;
                    BumpMass = BM > 0 ? BM : 0;
                    DmgImmunity = DmI > 0 ? DmI : 0;
                    ISectRadius = ISR > 0 ? ISR : 0;
                }
            }
            public struct UpgradeStats
            {
                readonly public float level1, level2, level3, level4, level5;

                public UpgradeStats(float lv1, float lv2, float lv3, float lv4, float lv5)
                {
                    level1 = lv1 > 0 ? lv1 : 0;
                    level2 = lv2 > 0 ? lv2 : 0;
                    level3 = lv3 > 0 ? lv3 : 0;
                    level4 = lv4 > 0 ? lv4 : 0;
                    level5 = lv5 > 0 ? lv5 : 0;
                }

                public float GetLevel(byte l)
                {
                    switch (l)
                    {
                        case 0:
                            return level1;
                        case 1:
                            return level2;
                        case 2:
                            return level3;
                        case 3:
                            return level4;
                        case 4:
                            return level5;
                        default:
                            throw new Exception("Upgrade level out of range.");
                    }
                }
            }
        }
    }
}
