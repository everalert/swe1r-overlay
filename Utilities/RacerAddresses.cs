using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1R_Overlay.Utilities
{
    class RacerAddresses
    {
        static public uint pPod = 0xD78A4;
        static public uint podLen = 0x88;
        static public Dictionary<string, uint> oPod = new Dictionary<string, uint> {
            {"position",  0x5C},
            {"time_lap1",  0x60},
            {"time_lap2",  0x64},
            {"time_lap3",  0x68},
            {"time_lap4",  0x6C},
            {"time_lap5",  0x70},
            {"time_total", 0x74},
            {"pPodData",  0x84}
        };
        static public uint podDataLen = 0x1F28;
        static public Dictionary<string, uint> oPodData = new Dictionary<string, uint>
        {
            {"flags",   0x60}, //uint64
            {"flags1",  0x60}, //uint32
            {"flags1b", 0x61}, //byte
            {"flags1c", 0x62},
            {"flags2",  0x64}, //uint32
            {"flags2d", 0x67},
            {"anti_skid",     0x6C},
            {"turn_response", 0x70},
            {"max_turn_rate", 0x74},
            {"acceleration",  0x78},
            {"max_speed",     0x7C},
            {"air_brake_inv", 0x80},
            {"decel_inv",     0x84},
            {"boost_thrust",  0x88},
            {"heat_rate",     0x8C},
            {"cool_rate",     0x90},
            {"hover_height",  0x94},
            {"repair_rate",   0x98},
            {"bump_mass",     0x9C},
            {"dmg_immunity",  0xA0},
            {"isect_radius",  0xA8},
            {"speed",             0x1A0},
            {"heat",              0x218},
            {"slow_terrain_mult", 0x244},
            {"ice_terrain_mult",  0x248},
            {"slide",             0x24C}
        };

        static public uint pCam1 = 0xA9ADAC;
        static public Dictionary<string, uint> oCam1 = new Dictionary<string, uint>
        {
            {"x", 0x30},
            {"y", 0x34},
            {"z", 0x38}
        };
        static public uint pCam2 = 0xA9ADB4;
        static public Dictionary<string, uint> oCam2 = new Dictionary<string, uint>
        {
            {"foc_x", 0x30},
            {"foc_y", 0x34},
            {"foc_z", 0x38}
        };

        static public Dictionary<string, uint> oPlayerStats = new Dictionary<string, uint>
        {
            {"anti_skid",     0xA29BDC},
            {"turn_response", 0xA29BE0},
            {"max_turn_rate", 0xA29BE4},
            {"acceleration",  0xA29BE8},
            {"max_speed",     0xA29BEC},
            {"air_brake_inv", 0xA29BF0},
            {"decel_inv",     0xA29BF4},
            {"boost_thrust",  0xA29BF8},
            {"heat_rate",     0xA29BFC},
            {"cool_rate",     0xA29C00},
            {"hover_height",  0xA29C04},
            {"repair_rate",   0xA29C08},
            {"bump_mass",     0xA29C0C},
            {"dmg_immunity",  0xA29C10},
            {"isect_radius",  0xA29C14}
        };

        static public Dictionary<string, uint> oStaticAddresses = new Dictionary<string, uint>
        {
            {"frame_count",        0xA22A30 }, //uint
            {"frame_length",       0xA22A40 }, //double
            {"scene",              0xA9BA62 }, //ushort
            {"in_race",            0xA9BB81 }, //byte
            {"in_tournament_mode", 0x10C450 }  //byte
        };

        static public Dictionary<string, uint> oDebug = new Dictionary<string, uint>
        {
            {"debug_menu",      0x10C044 }, //ulong
            {"debug_menu_text", 0x10C048 }, //ulong
            {"terrain_labels",  0x10C88C }, //ulong
            {"invincibility",   0x10CA28 } //ulong
        };
    }
}
