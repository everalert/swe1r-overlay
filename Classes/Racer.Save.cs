using SWE1R.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SWE1R
{
    public partial class Racer
    {
        public class Save
        {
            private const string gamePath = @"Z:\GOG\STAR WARS Racer";
            private const byte timeLen = 0x64;
            private readonly static float timeDef = BitConverter.ToSingle(new byte[4] { 0xD7, 0xFF, 0x60, 0x45 }, 0);
            private const byte nameLen = 0x20;
            private const string nameDef = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            //SAVE.sav structure

            //4x byte[0x50], file blocks
            //-> 0x00 4byte, always 03 00 01 00?
            //-> remainder seems to follow same format as "file blocks" (0x18-0x67) in tgfd.dat

            public class PlayerSave
            {

            }


            //tgfd.dat structure

            //0x18 length data block?
            //-> structure = ???

            //4x byte[0x50], file blocks
            //-> 0x00 string[0x20], most recent loaded filename?
            //-> 0x20-0x23 = ??? (0x21 = 1?, 0x22 = file slot?)
            //-> 0x24 byte, selected vehicle
            //-> 0x25 4byte, track unlocks (bits), default 01 01 01 00
            //-> 0x2A 7byte, track placements (sets of 2 bits)
            //-> 0x31-0x33 = ???
            //-> 0x34 3byte, vehicle unlocks (bits), default 01 2E 02
            //-> 0x37 = ???
            //-> 0x38 long(signed), truguts
            //-> 0x3C-0x3F = ??? (0x3C has data)
            //-> 0x40 byte, pit droids
            //-> 0x41 7byte, upgrade levels
            //-> 0x48 7byte, upgrade healths
            //-> 0x4F byte, always 0x00?

            //0x158 float[0x64], race times
            //-- 3lap times for all tracks in track id order
            //-- for each track entry, regular then mirrored mode times
            //-- same pattern repeats for 1lap times
            //-- default 3599.999 (0xD7FF6045) for no saved time
            //0x2E8 string[0x64], time names
            //-- same pattern for tracks as race times
            //-- 0x20 length strings
            //-- default AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA for no saved time, otherwise name padded with 0x00
            //0xF68 byte[0x64], time vehicles
            //-- same pattern for tracks as race times
            //-- defaults to track favourite for no saved time

            //0xFCC byte[0xC], EOF padding?
            //-- 0x00 only

            //game ignores name and pod if time is default

            //total length 0x0FD8

            public class GameSave
            {
                private List<RaceTime> Times;
                private List<PlayerSave> Players;

                public void ReadFile(string filename = gamePath+@"\data\player\tgfd.dat")
                {
                    FileStream file = File.OpenRead(filename);
                    Times = new List<RaceTime>();
                    //Players = new List<PlayerSave>();

                    float[] fileTimes = new float[timeLen];
                    file.Seek(0x158, SeekOrigin.Begin);
                    for (var i = 0; i < fileTimes.Length; i++)
                        fileTimes[i] = BitConverter.ToSingle(ReadChunk(file,4), 0);
                    string[] fileTimeNames = new string[timeLen];
                    for (var i = 0; i < fileTimeNames.Length; i++)
                        fileTimeNames[i] = Encoding.UTF8.GetString(ReadChunk(file, nameLen), 0, nameLen);
                    byte[] fileTimePods = new byte[timeLen];
                    for (var i = 0; i < fileTimePods.Length; i++)
                        fileTimePods[i] = ReadChunk(file, 1)[0];

                    for (var i = 0; i < timeLen; i++)
                    {
                        Times.Add(new RaceTime
                        {
                            Track = (byte)(Math.Floor((double)i/2)%25),
                            Time = fileTimes[i] == timeDef ? -1 : fileTimes[i],
                            Name = fileTimeNames[i],
                            Vehicle = fileTimePods[i],
                            Mirror = (i % 2 == 1),
                            TimeType = (i >= (timeLen / 2f)) ? RaceTimeType.SingleLap : RaceTimeType.FullTrack
                        });
                        Console.WriteLine(Times.Last().GetString());
                    }

                    file.Dispose();
                    file.Close();
                }
            }

            public struct RaceTime
            {
                public byte Track;
                public float Time;
                public bool Mirror;
                public byte TimeType;
                public byte Vehicle;
                public string Name;

                public string GetString()
                {
                    return Track.ToString("00") + "   " + (Time < 0 ? "NoTime  " : TimeSpan.FromSeconds(Time).ToString("m\\:ss\\.fff")) + "   " + TimeType + "   " + Mirror;
                }
            }

            public struct RaceTimeType
            {
                public const byte FullTrack = 0,
                    SingleLap = 1,
                    FirstLap = 2;
            }

            private static byte[] ReadChunk(FileStream file, int length)
            {
                byte[] data = new byte[length];
                file.Read(data, 0, length);
                return data;
            }
        }
    }
}
