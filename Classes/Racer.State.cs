using SWE1R.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Force.Crc32;

namespace SWE1R
{
    public partial class Racer
    {
        public class State
        {
            private static readonly byte[] fileMagicWord = new byte[16] { 0x89, 0x53, 0x57, 0x45, 0x31, 0x52, 0x53, 0x41, 0x56, 0x53, 0x54, 0x41, 0x0D, 0x0A, 0x1A, 0x0A };
            private static readonly byte[] fileEOFWord = new byte[8] { 0x2E, 0x44, 0x4F, 0x54, 0x44, 0x4F, 0x4E, 0x45 };
            public static readonly string fileExt = "e1rs";
            private const byte fileVersion = 1;

            public byte[] dPodData;
            public byte[] dPod;
            public byte pod;
            public byte track;
            public StateBlock[] data;
            //public byte world;
            //camera?
            //racetime?

            public State(byte[] infoPod, byte[] infoPodData, byte racePod, byte raceTrack)
            {
                dPod = infoPod;
                dPodData = infoPodData;
                track = raceTrack;
                pod = racePod;
            }
            public State(StateBlock[] blocks, byte racePod, byte raceTrack)
            {
                pod = racePod;
                track = raceTrack;
                data = blocks;
                ValidateData();
                //if (DataHasId(0))
                //    DataAssignPod();
                //if (DataHasId(1))
                //    DataAssignPodData();
            }

            public void SaveStateToFile(string filename)
            {
                //string filename = @"K:\Projects\swe1r\overlay\SWE1R Overlay\Format\Racer.State.WriteTest.e1rs";
                if (File.Exists(filename))
                    File.Delete(filename);
                FileStream file = File.OpenWrite(filename);

                // HEADER

                uint headerCRC32 = 0;
                WriteFileChunk(file, fileMagicWord, ref headerCRC32);
                byte readerVer = 1;
                byte writerVer = 1;
                WriteFileChunk(file, writerVer, ref headerCRC32);
                WriteFileChunk(file, readerVer, ref headerCRC32);
                WriteFileChunk(file, !BitConverter.IsLittleEndian, ref headerCRC32);
                uint dataLen = 2 + 4; // pod/track bytes + final crc32
                foreach (StateBlock block in data)
                    dataLen += 13 + (uint)block.data.Length; // + length of each block
                WriteFileChunk(file, dataLen, ref headerCRC32);
                ushort dataOff = (ushort)(file.Position + sizeof(ushort) + sizeof(uint));
                WriteFileChunk(file, dataOff, ref headerCRC32);
                file.Write(BitConverter.GetBytes(headerCRC32), 0, 4);

                // DATA

                uint dataCRC32 = 0;
                WriteFileChunk(file, pod, ref dataCRC32);
                WriteFileChunk(file, track, ref dataCRC32);
                uint blockCRC32;
                foreach (StateBlock block in data)
                {
                    blockCRC32 = 0;
                    WriteFileChunk(file, block.type, ref blockCRC32, ref dataCRC32);
                    WriteFileChunk(file, block.offset, ref blockCRC32, ref dataCRC32);
                    WriteFileChunk(file, block.data.Length, ref blockCRC32, ref dataCRC32);
                    WriteFileChunk(file, block.data, ref blockCRC32, ref dataCRC32);
                    WriteFileChunk(file, blockCRC32, ref dataCRC32);
                }
                file.Write(BitConverter.GetBytes(dataCRC32), 0, 4);
                file.Write(fileEOFWord, 0, fileEOFWord.Length);

                // CLEANUP

                file.Dispose();
                file.Close();
            }

            public static State LoadStateFromFile(string filename)
            {
                //string filename = @"K:\Projects\swe1r\overlay\SWE1R Overlay\Format\Racer.State.WriteTest.e1rs";
                FileStream file = File.OpenRead(filename);
                uint headerCRC32 = 0;
                uint dataCRC32 = 0;

                // READ HEADER

                // read data
                byte[] inMagicWord = ReadFileChunk(file, fileMagicWord.Length, ref headerCRC32);
                if (!Win32.ByteArrayCompare(inMagicWord, fileMagicWord))
                    throw new Exception("Read Savestate: Invalid filetype.");
                byte inVerSrc = ReadFileChunk(file, 0x1, ref headerCRC32)[0]; //ideal/generated-from version
                byte inVerRead = ReadFileChunk(file, 0x1, ref headerCRC32)[0]; //readable version
                bool inBigEndian = Convert.ToBoolean(ReadFileChunk(file, 0x1, ref headerCRC32)[0]);
                byte[] inDataLen = ReadFileChunk(file, 0x4, ref headerCRC32);
                byte[] inDataOff = ReadFileChunk(file, 0x2, ref headerCRC32);
                byte[] inHeaderCRC32 = ReadFileChunk(file, 0x4);
                // convert to big endian if needed
                if (inBigEndian)
                {
                    inDataLen = inDataLen.Reverse().ToArray();
                    inDataOff = inDataOff.Reverse().ToArray();
                    inHeaderCRC32 = inHeaderCRC32.Reverse().ToArray();
                }
                // check crc32
                if (Crc32Algorithm.Append(headerCRC32,inHeaderCRC32,0,0x4) != 0x2144DF1C)
                    throw new Exception("Read Savestate: Header invalid.");
                // check eof
                file.Seek(BitConverter.ToUInt16(inDataOff, 0) + BitConverter.ToUInt32(inDataLen, 0), SeekOrigin.Begin);
                byte[] inEOFCheck = ReadFileChunk(file, fileEOFWord.Length);
                if (!Win32.ByteArrayCompare(inEOFCheck, fileEOFWord))
                    throw new Exception("Read Savestate: File length invalid.");

                // READ DATA

                file.Seek(BitConverter.ToUInt16(inDataOff, 0), SeekOrigin.Begin);
                // read pod and track
                //if (inVerRead <= 0xFF) //add this when moving to race struct storing
                byte inDataPod = ReadFileChunk(file, 0x1, ref dataCRC32)[0]; //ideal/generated-from version
                byte inDataTrack = ReadFileChunk(file, 0x1, ref dataCRC32)[0]; //readable version
                List<StateBlock> inDataBlocks = new List<StateBlock>();
                while (file.Position < BitConverter.ToUInt16(inDataOff, 0) + BitConverter.ToUInt32(inDataLen, 0) - 4)
                {
                    uint blockCRC32 = 0;
                    inDataBlocks.Add(new StateBlock());
                    inDataBlocks.Last().type = ReadFileChunk(file, 0x1, ref blockCRC32, ref dataCRC32)[0];
                    inDataBlocks.Last().offset = ReadFileChunk(file, 0x4, ref blockCRC32, ref dataCRC32);
                    inDataBlocks.Last().length = ReadFileChunk(file, 0x4, ref blockCRC32, ref dataCRC32);
                    inDataBlocks.Last().data = ReadFileChunk(file, BitConverter.ToInt32((inBigEndian?inDataBlocks.Last().length.Reverse().ToArray():inDataBlocks.Last().length),0), ref blockCRC32, ref dataCRC32);
                    inDataBlocks.Last().crc32 = ReadFileChunk(file, 0x4, ref dataCRC32);
                    if (inBigEndian)
                        inDataBlocks.Last().ReverseArrays();
                    if (Crc32Algorithm.Append(blockCRC32, inDataBlocks.Last().crc32) != 0x2144DF1C)
                        throw new Exception("Read Savestate: Data block "+inDataBlocks.Count+" invalid.");
                }
                // check entire data set is valid
                byte[] inDataCRC32 = ReadFileChunk(file, 0x4);
                if (inBigEndian)
                    inDataCRC32 = inDataCRC32.Reverse().ToArray();
                if (Crc32Algorithm.Append(dataCRC32, inDataCRC32, 0, 0x4) != 0x2144DF1C)
                    throw new Exception("Read Savestate: Data invalid.");
                // check end of data is actually end of file
                inEOFCheck = ReadFileChunk(file, fileEOFWord.Length);
                if (!Win32.ByteArrayCompare(inEOFCheck, fileEOFWord))
                    throw new Exception("Read Savestate: File length invalid.");

                State output = new State(inDataBlocks.ToArray(), inDataPod, inDataTrack);

                file.Dispose();
                file.Close();

                return output;
            }

            public bool ValidateData(byte ver = fileVersion)
            {
                bool valid = true;
                if (!DataHasValue(BlockType.Pod, 0x60, 0x19))
                    valid = false;
                if (!DataHasValue(BlockType.PodData, 0x0, Addr.lPodData))
                    valid = false;
                return valid;
            }

            private bool DataHasId(byte id)
            {
                if (data == null || data.Count() <= 0)
                    return false;
                bool found = false;
                for (var i = 0; found == false && i<data.Count(); i++)
                    if (data[i].type == id)
                        found = true;
                return found;
            }
            private bool DataHasValue(byte id, uint offset, uint length)
            {
                if (data == null || data.Count() <= 0)
                    return false;
                bool found = false;
                uint off;
                for (var i = 0; found == false && i < data.Count(); i++)
                {
                    off = BitConverter.ToUInt32(data[i].offset, 0);
                    if (data[i].type == id && off <= offset && off + data[i].data.Length >= offset + length)
                        found = true;
                }
                return found;
            }

            public uint DataFirstIndexOfId(byte id)
            {
                int index = -1;
                for (var i = 0; i<data.Count(); i++)
                    if (data[i].type == id)
                        index = i;
                if (index < 0)
                    throw new Exception("Savestate validation: Data of type "+id+" not found in data set.");
                return (uint)index;
            }

            private void DataAssignPod()
            {
                if (!DataHasId(0))
                {
                    dPod = null;
                    return;
                }
                dPod = data[DataFirstIndexOfId(0)].data;
            }

            private void DataAssignPodData()
            {
                if (!DataHasId(1))
                {
                    dPodData = null;
                    return;
                }
                dPodData = data[DataFirstIndexOfId(1)].data;
            }


            // STATE DATA HOLDER

            public class StateBlock
            {
                public byte type;
                public byte[] offset;
                public byte[] length;
                public byte[] data;
                public byte[] crc32;

                public StateBlock()
                {
                }
                public StateBlock(byte t, uint o, byte[] d)
                {
                    type = t;
                    offset = BitConverter.GetBytes(o);
                    data = d;
                    length = BitConverter.GetBytes((uint)data.Length);
                }

                //this doesn't need to happen here
                public void ReverseArrays()
                {
                    offset = offset.Reverse().ToArray();
                    length = length.Reverse().ToArray();
                    data = data.Reverse().ToArray();
                    crc32 = crc32.Reverse().ToArray();
                }
            }

            public struct BlockType
            {
                public const byte Pod = 0,
                    PodData = 1,
                    Race = 2,
                    Rendering = 3;
            }



            // FILE I/O (MOVE TO UTIL CLASS???)

            //writing

            private static void WriteFileChunk(FileStream file, byte[] data, ref uint crc32)
            {
                file.Write(data, 0, data.Length);
                crc32 = Crc32Algorithm.Append(crc32, data);
            }
            private static void WriteFileChunk(FileStream file, byte data, ref uint crc32)
            {
                byte[] output = new byte[1] { data };
                file.Write(output, 0, 1);
                crc32 = Crc32Algorithm.Append(crc32, output);
            }
            private static void WriteFileChunk(FileStream file, dynamic data, ref uint crc32)
            {
                byte[] output = BitConverter.GetBytes(data);
                file.Write(output, 0, output.Length);
                crc32 = Crc32Algorithm.Append(crc32, output);
            }
            private static void WriteFileChunk(FileStream file, byte[] data, ref uint crc32a, ref uint crc32b)
            {
                file.Write(data, 0, data.Length);
                crc32a = Crc32Algorithm.Append(crc32a, data);
                crc32b = Crc32Algorithm.Append(crc32b, data);
            }
            private static void WriteFileChunk(FileStream file, byte data, ref uint crc32a, ref uint crc32b)
            {
                byte[] output = new byte[1] { data };
                file.Write(output, 0, 1);
                crc32a = Crc32Algorithm.Append(crc32a, output);
                crc32b = Crc32Algorithm.Append(crc32b, output);
            }
            private static void WriteFileChunk(FileStream file, dynamic data, ref uint crc32a, ref uint crc32b)
            {
                byte[] output = BitConverter.GetBytes(data);
                file.Write(output, 0, output.Length);
                crc32a = Crc32Algorithm.Append(crc32a, output);
                crc32b = Crc32Algorithm.Append(crc32b, output);
            }

            //reading

            private static byte[] ReadFileChunk(FileStream file, int length)
            {
                byte[] data = new byte[length];
                file.Read(data, 0, length);
                return data;
            }
            private static byte[] ReadFileChunk(FileStream file, int length, ref uint crc32)
            {
                byte[] data = new byte[length];
                file.Read(data, 0, length);
                crc32 = Crc32Algorithm.Append(crc32, data);
                return data;
            }
            private static byte[] ReadFileChunk(FileStream file, int length, ref uint crc32a, ref uint crc32b)
            {
                byte[] data = new byte[length];
                file.Read(data, 0, length);
                crc32a = Crc32Algorithm.Append(crc32a, data);
                crc32b = Crc32Algorithm.Append(crc32b, data);
                return data;
            }
        }
    }
}
