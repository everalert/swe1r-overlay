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
            */

            /*
                pX = pointer
                oX = offset
                lX = length in bytes
             */

            static public uint pPod = 0xD78A4;
            static public uint lPod = 0x88; // length in bytes
            static public Dictionary<string, uint> oPod = new Dictionary<string, uint>
            {
                {"position",   0x5C},
                {"time_lap_1", 0x60},
                {"time_lap_2", 0x64},
                {"time_lap_3", 0x68},
                {"time_lap_4", 0x6C},
                {"time_lap_5", 0x70},
                {"time_total", 0x74},
                {"lap",        0x78},
                {"pPodData",   0x84}
            };
            static public uint lPodData = 0x1F28;
            static public Dictionary<string, uint> oPodData = new Dictionary<string, uint>
            {
                {"3d_vector_1a",  0x20},
                {"3d_vector_1b",  0x24},
                {"3d_vector_1c",  0x28},
                {"3d_vector_2a",  0x30},
                {"3d_vector_2b",  0x34},
                {"3d_vector_2c",  0x38},
                {"xpos",          0x50},
                {"ypos",          0x54},
                {"zpos",          0x58},
                {"flags1",        0x60},
                {"flags2",        0x64},
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
                {"lap_completion_1",  0xE0},
                {"lap_completion_2",  0xE4},
                {"lap_completion_3",  0xE8},
                {"speed",             0x1A0},
                {"slide_mult",        0x1E8},
                {"heat",              0x218},
                {"slow_terrain_mult", 0x244},
                {"ice_terrain_mult",  0x248},
                {"slide",             0x24C}
            };

            static public Dictionary<string, uint> oStats = new Dictionary<string, uint>
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

            static public Dictionary<string, uint> oStatic = new Dictionary<string, uint>
            {
                {"selected_pod_stats", 0xA29BDC }, //len=0x78
                {"frame_count",        0xA22A30 }, //uint
                {"frame_time",       0xA22A40 }, //double
                {"scene",              0xA9BA62 }, //ushort
                {"in_race",            0xA9BB81 }, //byte
                {"in_tournament_mode", 0x10C450 }  //byte
            };

            static public Dictionary<string, uint> oDebug = new Dictionary<string, uint>
            {
                {"debug_level",     0x10C040 }, //long, 0-6 normally
                {"debug_menu",      0x10C044 }, //ulong
                {"debug_menu_text", 0x10C048 }, //ulong
                {"terrain_labels",  0x10C88C }, //ulong
                {"invincibility",   0x10CA28 } //ulong
            };

            static public uint pRaceSetting = 0xBFDB8;
            static public Dictionary<string, uint> oRaceSetting = new Dictionary<string, uint>
            {
                {"selected_track",    0x5D }, //byte
                {"selected_circuit",  0x5E }, //byte
                {"set_mirrored",      0x6E }, //byte
                {"selected_pod",      0x73 }, //byte
                {"set_ai_difficulty", 0x74 }, //byte, i think it's difficulty (i.e. not number of ai racers)
                {"set_winnings",      0x91 }, //byte
            };
        }
    }
}