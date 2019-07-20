using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            /*
                pX = pointer
                oX = offset
                lX = length in bytes
             */
            
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

            public static Dictionary<Rendering, DataTypes> TypesForRendering = new Dictionary<Rendering, DataTypes>
            {
                { Rendering.CameraMode , DataTypes.UInt32 },
                { Rendering.FogFlags , DataTypes.UInt32 },
                { Rendering.FogColR , DataTypes.UInt32 },
                { Rendering.FogColG , DataTypes.UInt32 },
                { Rendering.FogColB , DataTypes.UInt32 },
                { Rendering.FogDistance , DataTypes.Single },
            };

            public enum Race
            {
                SelectedTrack = 0x5D, //byte
                SelectedCircuit = 0x5E, //byte
                SetMirrored = 0x6E, //byte
                SelectedVehicle = 0x73, //byte
                SetAiDifficulty = 0x74, //byte, i think it's difficulty (i.e. not number of ai racers)
                SetWinnings = 0x91, //byte
            };

            public static Dictionary<Race, DataTypes> TypesForRace = new Dictionary<Race, DataTypes>
            {
                { Race.SelectedTrack , DataTypes.Byte },
                { Race.SelectedCircuit , DataTypes.Byte },
                { Race.SetMirrored , DataTypes.Byte },
                { Race.SelectedVehicle , DataTypes.Byte },
                { Race.SetAiDifficulty , DataTypes.Byte },
                { Race.SetWinnings , DataTypes.Byte },
            };

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

            public static Dictionary<Pod, DataTypes> TypesForPod = new Dictionary<Pod, DataTypes>
            {
                { Pod.Flags, DataTypes.UInt32 },
                { Pod.PtrFile, DataTypes.UInt32 },
                { Pod.PtrVehicle, DataTypes.UInt32 },
                { Pod.Position, DataTypes.UInt16 }, //pretty sure, code writes 2 bytes
                { Pod.Lap, DataTypes.Byte }, //i think?
                { Pod.PtrPodState, DataTypes.UInt32 },
            };

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
                FallTimer = 0x2C8
            };

            public static Dictionary<PodState, DataTypes> TypesForPodState = new Dictionary<PodState, DataTypes>
            {
                { PodState.Flags1, DataTypes.UInt32 },
                { PodState.Flags2, DataTypes.UInt32 },
            };

            public enum Static
            {
                DebugLevel = 0x10C040,
                DebugMenu = 0x10C044,
                DebugMenuText = 0x10C048,
                DebugTerrainLabels = 0x10C88C,
                DebugInvincibility = 0x10CA28,
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
                InTournamentMode = 0x10C450
            };

            public static Dictionary<Static, DataTypes> TypesForStatic = new Dictionary<Static, DataTypes>
            {
                { Static.DebugLevel, DataTypes.UInt32 },
                { Static.DebugMenu, DataTypes.UInt32 },
                { Static.DebugMenuText, DataTypes.UInt32 },
                { Static.DebugTerrainLabels, DataTypes.UInt32 },
                { Static.DebugInvincibility, DataTypes.UInt32 },
                { Static.FrameCount, DataTypes.UInt32 },
                { Static.FrameTime, DataTypes.Double},
                { Static.SceneId, DataTypes.UInt16},
                { Static.InRace, DataTypes.Byte},
                { Static.InTournamentMode, DataTypes.Byte},
            };

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

            public enum DataTypes
            {
                //unsigned = no. of bits, signed = bits-1, fractional = bits+1
                None = -1,
                String = 0,
                SByte = 7,
                Byte = 8,
                Int16 = 15,
                UInt16 = 16,
                Int32 = 31,
                UInt32 = 32,
                Single = 33,
                Int64 = 63,
                UInt64 = 64,
                Double = 65,
                Decimal = 129
            };
        }
    }
}