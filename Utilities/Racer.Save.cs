using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWE1R
{
    public partial class Racer
    {
        public class Save
        {
            //0x170 HG 3lap
            //0x238 HG 1lap

            //0x18 length data block?
            //-> structure = ???
            //4x 0x50 length file blocks?
            //-> 0x20 length most recent loaded filename?
            //-> 0x23 selected pod(maybe 0x22 len 2?)
            //-> rest of structure?
            //times starting at 0x158
            //loop following for tracks in track ID order
            //-> 3lap time, total seconds(float)
            //-> 3lap mirrored time, total seconds(float)
            //followed by the same pattern for 1lap times
            //followed by same pattern for names, 0x20 length strings
            //followed by same pattern for pods used
            //0x70 length block of some data?
            //0xC length block(12x 0x00)

            //0xD7FF6045 - (float)3599.999 - default time for no time
            //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA - default name for no time
            //default pod used is track favourite
            //game ignores name and pod if time is default

            //total length 0x0FD8

            public struct RaceTime
            {
                float Time;
                bool Mirrored;
                byte TimeType;
                byte Vehicle;
                string Name;
                private const byte NameLimit = 0x20;
            }

            public struct RaceTimeType
            {
                public const byte FullTrack = 0,
                    SingleLap = 1,
                    FirstLap = 2;
            }
        }
    }
}
