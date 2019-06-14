using SWE1R_Overlay.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1R_Overlay.Utilities
{
    public class RacerData
    {
        static int bytesOut;
        static int bytesIn;
        const string PROCESS_NAME = "SWEP1RCR";
        readonly ProcessMemoryReader mem = new ProcessMemoryReader();
        Process game;

        public RacerData(Process target = null)
        {
            if (target != null)
                game = target;
        }


        // WHYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
        // need more thought into structure
        // - fewer, more generalised functions
        // - possibly implement dictionary inputs for mass data reading from a pointer
        // also need to error handle memory reading exceptions
        // - not really a problem unless user selects wrong process tho lol
        // - also, does steam hack detection block this? autosplitter can read steam version so probably no, or livesplit uses a workaround

        public float GetPodTimeLap1()
        {
            return GetPodFloat("time_lap1");
        }
        public float GetPodTimeLap2()
        {
            return GetPodFloat("time_lap2");
        }
        public float GetPodTimeLap3()
        {
            return GetPodFloat("time_lap3");
        }
        public float GetPodTimeLap4()
        {
            return GetPodFloat("time_lap4");
        }
        public float GetPodTimeLap5()
        {
            return GetPodFloat("time_lap5");
        }
        public float GetPodTimeTotal()
        {
            return GetPodFloat("time_total");
        }
        public float[] GetPodTimeALL()
        {
            float[] times = {
                GetPodTimeLap1(),
                GetPodTimeLap2(),
                GetPodTimeLap3(),
                GetPodTimeLap4(),
                GetPodTimeLap5(),
                GetPodTimeTotal()
            };
            return times;
        }



        public float GetPodDataTerrainSlow()
        {
            return GetPodDataFloat("slow_terrain_mult");
        }
        public float GetPodDataTerrainIce()
        {
            return GetPodDataFloat("ice_terrain_mult");
        }
        public float GetPodDataSlide()
        {
            return GetPodDataFloat("slide");
        }
        public float GetPodDataHeat()
        {
            return GetPodDataFloat("heat");
        }

        public float GetPodDataHeatRate()
        {
            return GetPodDataFloat("heat_rate");
        }
        public float GetPodDataCoolRate()
        {
            return GetPodDataFloat("cool_rate");
        }

        public UInt64 GetPodDataFlags()
        {
            return GetPodDataULong("flags");
        }
        public UInt32 GetPodDataFlags1()
        {
            return GetPodDataUInt("flags1");
        }
        public UInt32 GetPodDataFlags2()
        {
            return GetPodDataUInt("flags2");
        }

        public byte[] GetPodDataALL()
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], 0x0 };
            return GetData(path, "", RacerAddresses.podDataLen);
        }

        public void WritePodDataALL(dynamic data)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], 0x0 };
            WriteData(path, data);
        }



        public byte[] GetCam1xyz()
        {
            uint[] path = { RacerAddresses.pCam1, RacerAddresses.oCam1["x"] };
            return GetData(path, "", 0x0C);
        }
        public void WriteCam1xyz(dynamic data)
        {
            uint[] path = { RacerAddresses.pCam1, RacerAddresses.oCam1["x"] };
            WriteData(path, data);
        }
        public byte[] GetCam2xyz()
        {
            uint[] path = { RacerAddresses.pCam2, RacerAddresses.oCam2["foc_x"] };
            return GetData(path, "", 0x0C);
        }
        public void WriteCam2xyz(dynamic data)
        {
            uint[] path = { RacerAddresses.pCam2, RacerAddresses.oCam2["foc_x"] };
            WriteData(path, data);
        }
        public byte[] GetRacerState()
        {
            var mem_size = (uint)game.PrivateMemorySize64;
            uint[] path = { 0x0 };
            return GetData(path, "", mem_size);
        }
        public void WriteRacerState(dynamic data)
        {
            uint[] path = { 0x0 };
            WriteData(path, data);
        }



        private float GetPodFloat(string datapoint)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod[datapoint] };
            return GetData(path, "float");
        }

        private float GetPodDataFloat(string datapoint)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], RacerAddresses.oPodData[datapoint] };
            return GetData(path, "float");
        }
        private UInt32 GetPodDataUInt(string datapoint)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], RacerAddresses.oPodData[datapoint] };
            return GetData(path, "uint");
        }
        private UInt64 GetPodDataULong(string datapoint)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], RacerAddresses.oPodData[datapoint] };
            return GetData(path, "ulong");
        }
        private Byte GetPodDataByte(string datapoint)
        {
            uint[] path = { RacerAddresses.pPod, RacerAddresses.oPod["pPodData"], RacerAddresses.oPodData[datapoint] };
            return GetData(path, "byte");
        }



        public Byte GetStaticInRace()
        {
            return GetStaticByte("in_race");
        }
        public Byte GetStaticInTournamentMode()
        {
            return GetStaticByte("in_tournament_mode");
        }
        public UInt16 GetStaticScene()
        {
            return GetStaticUShort("scene");
        }
        public Double GetStaticFrameLen()
        {
            return GetStaticDouble("frame_length");
        }
        public UInt32 GetStaticFrameCnt()
        {
            return GetStaticUInt("frame_count");
        }
        public Single[] GetStaticPodSelStats()
        {
            List<Single> output = new List<Single>();
            foreach(KeyValuePair<string,uint> entry in RacerAddresses.oPlayerStats)
            {
                output.Add(GetStaticPodSelFloat(entry.Key));
            }
            return output.ToArray();
        }


        public void EnableDebugMenu()
        {
            uint[] path1 = { RacerAddresses.oDebug["debug_menu"] };
            WriteData(path1, (uint)0x1);
            uint[] path2 = { RacerAddresses.oDebug["debug_menu_text"] };
            WriteData(path2, (uint)0x3F);
        }
        public void DisableDebugMenu()
        {
            uint[] path1 = { RacerAddresses.oDebug["debug_menu"] };
            WriteData(path1, (uint)0x0);
            uint[] path2 = { RacerAddresses.oDebug["debug_menu_text"] };
            WriteData(path2, (uint)0x0);
        }
        public void EnableDebugTerrainLabels()
        {
            uint[] path = { RacerAddresses.oDebug["terrain_labels"] };
            WriteData(path, (uint)0x1);
        }
        public void DisableDebugTerrainLabels()
        {
            uint[] path = { RacerAddresses.oDebug["terrain_labels"] };
            WriteData(path, (uint)0x0);
        }
        public void EnableDebugInvincibility()
        {
            uint[] path = { RacerAddresses.oDebug["invincibility"] };
            WriteData(path, (uint)0x1);
        }
        public void DisableDebugInvincibility()
        {
            uint[] path = { RacerAddresses.oDebug["invincibility"] };
            WriteData(path, (uint)0x0);
        }



        private byte GetStaticByte(string datapoint)
        {
            uint[] path = { RacerAddresses.oStaticAddresses[datapoint] };
            return GetData(path, "byte");
        }
        private UInt16 GetStaticUShort(string datapoint)
        {
            uint[] path = { RacerAddresses.oStaticAddresses[datapoint] };
            return GetData(path, "ushort");
        }
        private UInt32 GetStaticUInt(string datapoint)
        {
            uint[] path = { RacerAddresses.oStaticAddresses[datapoint] };
            return GetData(path, "uint");
        }
        private Single GetStaticFloat(string datapoint)
        {
            uint[] path = { RacerAddresses.oStaticAddresses[datapoint] };
            return GetData(path, "float");
        }
        private Double GetStaticDouble(string datapoint)
        {
            uint[] path = { RacerAddresses.oStaticAddresses[datapoint] };
            return GetData(path, "double");
        }
        private Single GetStaticPodSelFloat(string datapoint)
        {
            uint[] path = { RacerAddresses.oPlayerStats[datapoint] };
            return GetData(path, "float");
        }



        private dynamic GetData(uint[] path, string type = "", uint len = 4)
        {
            InitProcess();
            if (game != null)
            {
                IntPtr addr = GetMemoryAddr(game, path);
                switch (type)
                {
                    case "byte":
                        return mem.ReadMemory(addr, 1, out bytesOut)[0];
                    case "ushort":
                        return BitConverter.ToUInt16(mem.ReadMemory(addr, 2, out bytesOut), 0);
                    case "uint":
                        return BitConverter.ToUInt32(mem.ReadMemory(addr, 4, out bytesOut), 0);
                    case "ulong":
                        return BitConverter.ToUInt64(mem.ReadMemory(addr, 8, out bytesOut), 0);
                    case "float":
                        return BitConverter.ToSingle(mem.ReadMemory(addr, 4, out bytesOut), 0);
                    case "double":
                        return BitConverter.ToDouble(mem.ReadMemory(addr, 8, out bytesOut), 0);
                    case "string":
                        return BitConverter.ToString(mem.ReadMemory(addr, len, out bytesOut), 0);
                    default:
                        return mem.ReadMemory((IntPtr)(addr), len, out bytesOut);
                }
            }
            else
            {
                throw new Exception("Game process not found.");
            }
        }

        private void WriteData(uint[] path, dynamic data)
        {
            InitProcess();
            if (game != null)
            {
                IntPtr addr = GetMemoryAddr(game, path);
                mem.WriteMemory(addr, ((data.GetType()==typeof(byte[]))?data:BitConverter.GetBytes(data)), out bytesIn);
            }
            else
            {
                throw new Exception("Game process not found.");
            }
        }

        private IntPtr GetMemoryAddr(Process game, uint[] pointerPath)
        {
            InitProcess();
            uint addr;
            uint next;
            addr = (uint)game.Modules[0].BaseAddress;
            for (int i=0; i<pointerPath.Length; i++)
            {
                if (i == pointerPath.Length - 1)
                {
                    next = addr + pointerPath[i];
                } else
                {
                    next = BitConverter.ToUInt32(mem.ReadMemory((IntPtr)(addr + pointerPath[i]), 4, out bytesOut), 0);
                }
                addr = next;
            }
            return (IntPtr)(addr);
        }

        private bool InitProcess()
        {
            if (game == null)
                return false;
            if (mem.ReadProcess == null)
            {
                mem.ReadProcess = game;
                mem.OpenProcess();
            }
            return true;
        }
        public void SetGameTarget(Process tgt)
        {
            game = tgt;
            if (mem.ReadProcess != null)
                mem.CloseHandle();
            InitProcess();
        }
    }
}
