using System.Collections.Generic;

namespace SWE1R
{
    public partial class Racer
    {
        public struct Addr
        {
            /*
                future
                - addresses for CD (original) version?
                - restructuring?
            */

            public const DataType DefaultType = DataType.Single;
            public const uint DefaultLength = 4;
            
            public enum BasePtr
            {
                Pod = 0xD78A4,
                Race = 0xBFDB8,
                Rendering = 0xBFE80
            };

            public enum PtrLen
            {
                Pod = 0x88,
                PodState = 0x1F28
            };

            public enum Rendering
            {
                CameraMode = 0x07C,
                FogFlags = 0x2A8,
                FogColR = 0x2C8,
                FogColG = 0x2CC,
                FogColB = 0x2D0,
                FogDistance = 0x2D4
            };

            private static Dictionary<Rendering, DataType> TypesForRendering = new Dictionary<Rendering, DataType>
            {
                { Rendering.CameraMode , DataType.UInt32 },
                { Rendering.FogFlags , DataType.UInt32 },
                { Rendering.FogColR , DataType.UInt32 },
                { Rendering.FogColG , DataType.UInt32 },
                { Rendering.FogColB , DataType.UInt32 },
                { Rendering.FogDistance , DataType.Single },
            };

            private static Dictionary<Rendering, uint> LengthsForRendering = new Dictionary<Rendering, uint> { };

            public enum Race
            {
                SelectedTrack = 0x5D, //byte
                SelectedCircuit = 0x5E, //byte
                SetMirrored = 0x6E, //byte
                SelectedVehicle = 0x73, //byte
                SetAiDifficulty = 0x74, //byte, i think it's difficulty (i.e. not number of ai racers)
                SetWinnings = 0x91, //byte
            };

            private static Dictionary<Race, DataType> TypesForRace = new Dictionary<Race, DataType>
            {
                { Race.SelectedTrack , DataType.Byte },
                { Race.SelectedCircuit , DataType.Byte },
                { Race.SetMirrored , DataType.Byte },
                { Race.SelectedVehicle , DataType.Byte },
                { Race.SetAiDifficulty , DataType.Byte },
                { Race.SetWinnings , DataType.Byte },
            };

            private static Dictionary<Race, uint> LengthsForRace = new Dictionary<Race, uint> { };

            public enum Pod
            {
                Begin = 0x00,
                Flags =             0x08,
                PtrFile =           0x0C,
                PtrVehicle =        0x18,
                StatAntiSkid =      0x1C,
                StatTurnResponse =  0x20,
                StatMaxTurnRate =   0x24,
                StatAcceleration =  0x28,
                StatMaxSpeed =      0x2C,
                StatAirBrakeInv =   0x30,
                StatDecelInv =      0x34,
                StatBoostThrust =   0x38,
                StatHeatRate =      0x3C,
                StatCoolRate =      0x40,
                StatHoverHeight =   0x44,
                StatRepairRate =    0x48,
                StatBumpMass =      0x4C,
                StatDmgImmunity =   0x50,
                StatISectRadius =   0x54,
                Position =          0x5C,
                TimeLap1 =          0x60,
                TimeLap2 =          0x64,
                TimeLap3 =          0x68,
                TimeLap4 =          0x6C,
                TimeLap5 =          0x70,
                TimeTotal =         0x74,
                Lap =               0x78,
                PtrPodState =       0x84
            };

            private static Dictionary<Pod, DataType> TypesForPod = new Dictionary<Pod, DataType>
            {
                { Pod.Flags, DataType.UInt32 },
                { Pod.PtrFile, DataType.UInt32 },
                { Pod.PtrVehicle, DataType.UInt32 },
                { Pod.Position, DataType.UInt16 }, //pretty sure, code writes 2 bytes
                { Pod.Lap, DataType.Byte }, //i think?
                { Pod.PtrPodState, DataType.UInt32 },
            };

            private static Dictionary<Pod, uint> LengthsForPod = new Dictionary<Pod, uint> { };


            public enum PodState
            {
                Begin = 0x00,
                Vector3D_1A = 0x20,
                Vector3D_1B = 0x24,
                Vector3D_1C = 0x28,
                Vector3D_2A = 0x30,
                Vector3D_2B = 0x34,
                Vector3D_2C = 0x38,
                X = 0x50,
                Y = 0x54,
                Z = 0x58,
                Flags1 = 0x60,
                Flags2 = 0x64,
                StatAntiSkid = 0x6C,
                StatTurnResponse = 0x70,
                StatMaxTurnRate = 0x74,
                StatAcceleration = 0x78,
                StatMaxSpeed = 0x7C,
                StatAirBrakeInv = 0x80,
                StatDecelInv = 0x84,
                StatBoostThrust = 0x88,
                StatHeatRate = 0x8C,
                StatCoolRate = 0x90,
                StatHoverHeight = 0x94,
                StatRepairRate = 0x98,
                StatBumpMass = 0x9C,
                StatDmgImmunity = 0xA0,
                //BaseHoverHeight = 0xA4,
                StatISectRadius = 0xA8,
                LapCompletion = 0xE0,
                LapCompletionPrev = 0xE4,
                LapCompletionMax = 0xE8,
                Speed = 0x1A0,
                SlideModifier = 0x1E8, //???
                Heat = 0x218,
                SlowTerrainModifier = 0x244,
                IceTerrainModifier = 0x248,
                Slide = 0x24C, //???
                EngineDamageMaxTL = 0x288,
                EngineDamageMaxML = 0x28C,
                EngineDamageMaxBL = 0x290,
                EngineDamageMaxTR = 0x294,
                EngineDamageMaxMR = 0x298,
                EngineDamageMaxBR = 0x29C,
                EngineDamageTL = 0x288,
                EngineDamageML = 0x28C,
                EngineDamageBL = 0x290,
                EngineDamageTR = 0x294,
                EngineDamageMR = 0x298,
                EngineDamageBR = 0x29C,
                FallTimer = 0x2C8
            };

            private static Dictionary<PodState, DataType> TypesForPodState = new Dictionary<PodState, DataType>
            {
                { PodState.Flags1, DataType.UInt32 },
                { PodState.Flags2, DataType.UInt32 },
            };

            private static Dictionary<PodState, uint> LengthsForPodState = new Dictionary<PodState, uint>{};


            public enum Static
            {
                DebugLevel = 0x10C040,
                DebugMenu = 0x10C044,
                DebugMenuText = 0x10C048,
                DebugTerrainLabels = 0x10C88C,
                DebugInvincibility = 0x10CA28,
                SkipMainLoop = 0x10CB64,
                FrameCount = 0xA22A30,
                FrameTime = 0xA22A40,
                StatAntiSkid = 0xA29BDC,
                StatTurnResponse = 0xA29BE0,
                StatMaxTurnRate = 0xA29BE4,
                StatAcceleration = 0xA29BE8,
                StatMaxSpeed = 0xA29BEC,
                StatAirBrakeInv = 0xA29BF0,
                StatDecelInv = 0xA29BF4,
                StatBoostThrust = 0xA29BF8,
                StatHeatRate = 0xA29BFC,
                StatCoolRate = 0xA29C00,
                StatHoverHeight = 0xA29C04,
                StatRepairRate = 0xA29C08,
                StatBumpMass = 0xA29C0C,
                StatDmgImmunity = 0xA29C10,
                StatISectRadius = 0xA29C14,
                SceneId = 0xA9BA62,
                InRace = 0xA9BB81,
                InTournamentMode = 0x10C450,
                PauseState = 0x10C5F0,
                Text01 = 0xA2C380,
                SaveFile01 = 0xA35A60
            };

            private static Dictionary<Static, DataType> TypesForStatic = new Dictionary<Static, DataType>
            {
                { Static.DebugLevel, DataType.UInt32 },
                { Static.DebugMenu, DataType.UInt32 },
                { Static.DebugMenuText, DataType.UInt32 },
                { Static.DebugTerrainLabels, DataType.UInt32 },
                { Static.DebugInvincibility, DataType.UInt32 },
                { Static.SkipMainLoop, DataType.Byte},
                { Static.FrameCount, DataType.UInt32 },
                { Static.FrameTime, DataType.Double},
                { Static.SceneId, DataType.UInt16},
                { Static.InRace, DataType.Byte},
                { Static.InTournamentMode, DataType.Byte},
                { Static.PauseState, DataType.Byte},
                { Static.Text01, DataType.String }, // known to repeat every 0x80 at least 52 times
                { Static.SaveFile01, DataType.None }, // seems to repeat for 20 times
            };

            private static Dictionary<Static, uint> LengthsForStatic = new Dictionary<Static, uint>
            {
                { Static.Text01, 0x80 },
                { Static.SaveFile01, 0x50 }
            };




            public static DataType GetType(Pod k)
            {
                return TypesForPod.ContainsKey(k) ? TypesForPod[k] : DefaultType;
            }
            public static DataType GetType(PodState k)
            {
                return TypesForPodState.ContainsKey(k) ? TypesForPodState[k] : DefaultType;
            }
            public static DataType GetType(Race k)
            {
                return TypesForRace.ContainsKey(k) ? TypesForRace[k] : DefaultType;
            }
            public static DataType GetType(Rendering k)
            {
                return TypesForRendering.ContainsKey(k) ? TypesForRendering[k] : DefaultType;
            }
            public static DataType GetType(Static k)
            {
                return TypesForStatic.ContainsKey(k) ? TypesForStatic[k] : DefaultType;
            }


            public static uint GetLength(Pod k)
            {
                return LengthsForPod.ContainsKey(k) ? LengthsForPod[k] : TypesForPod.ContainsKey(k) ? (DataTypeLength(TypesForPod[k]) > 0 ? DataTypeLength(TypesForPod[k]) : DefaultLength) : DataTypeLength(DefaultType);
            }
            public static uint GetLength(PodState k)
            {
                return LengthsForPodState.ContainsKey(k) ? LengthsForPodState[k] : TypesForPodState.ContainsKey(k) ? (DataTypeLength(TypesForPodState[k]) > 0 ? DataTypeLength(TypesForPodState[k]) : DefaultLength) : DataTypeLength(DefaultType);
            }
            public static uint GetLength(Race k)
            {
                return LengthsForRace.ContainsKey(k) ? LengthsForRace[k] : TypesForRace.ContainsKey(k) ? (DataTypeLength(TypesForRace[k]) > 0 ? DataTypeLength(TypesForRace[k]) : DefaultLength) : DataTypeLength(DefaultType);
            }
            public static uint GetLength(Rendering k)
            {
                return LengthsForRendering.ContainsKey(k) ? LengthsForRendering[k] : TypesForRendering.ContainsKey(k) ? (DataTypeLength(TypesForRendering[k]) > 0 ? DataTypeLength(TypesForRendering[k]) : DefaultLength) : DataTypeLength(DefaultType);
            }
            public static uint GetLength(Static k)
            {
                return LengthsForStatic.ContainsKey(k) ? LengthsForStatic[k] : TypesForStatic.ContainsKey(k) ? (DataTypeLength(TypesForStatic[k]) > 0 ? DataTypeLength(TypesForStatic[k]) : DefaultLength) : DataTypeLength(DefaultType);
            }




            static public Dictionary<string, uint> oInput = new Dictionary<string, uint>()
            {
                { "steering",     0xD5E38 },
                { "nose",         0xD5E3C },
                { "menu_leftright",     0xD5E58 },
                { "cycle_camera", 0xD5E80 },
                { "look_back",    0xD5E84 },
                { "brake",        0xD5E88 },
                { "throttle",     0xD5E8C },
                { "boost",        0xD5E90 },
                { "slide",        0xD5E94 },
                { "roll_left",    0xD5E98 },
                { "roll_right",   0xD5E9C },
                { "taunt",        0xD5EA0 },
                { "repair",       0xD5EA4 },
                { "menu_down",        0xD5F00 },
                { "menu_up",        0xD5F04 },
                { "pause",        0xD5F20 },
            };
        }
    }
}