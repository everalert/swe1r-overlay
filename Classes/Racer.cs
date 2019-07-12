using SWE1R.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1R
{
    public partial class Racer
    {
        static int bytesOut;
        static int bytesIn;
        const string PROCESS_NAME = "SWEP1RCR";
        readonly ProcessMemoryReader mem = new ProcessMemoryReader();
        Process game;

        public Racer(Process target = null)
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



        // lmao
        public dynamic GetCustom(uint[] pointerpath, string datatype)
        {
            return GetData(pointerpath, datatype);
        }
        public void WriteCustom(uint[] pointerpath, dynamic data)
        {
            WriteData(pointerpath, data);
        }


        public dynamic GetRenderingCustom(uint offset, uint length)
        {
            uint[] path = { Addr.pRendering, offset };
            return GetData(path, "", length);
        }
        public void WriteRenderingCustom(uint offset, dynamic data)
        {
            uint[] path = { Addr.pRendering, offset };
            WriteData(path, data);
        }


        public dynamic GetRaceSetting(string datapoint, string datatype)
        {
            uint[] path = { Addr.pRaceSetting, Addr.oRaceSetting[datapoint] };
            return GetData(path, datatype);
        }


        public dynamic GetPod(string datapoint, string datatype)
        {
            uint[] path = { Addr.pPod, Addr.oPod[datapoint] };
            return GetData(path, datatype);
        }
        public float[] GetPodTimeALL()
        {
            byte[] data = GetData(new uint[]{ Addr.pPod, Addr.oPod["time_lap_1"] }, "", 0x18);
            List<float> times = new List<float>();
            for (var i = 0; i < data.Length; i += 4)
                times.Add(BitConverter.ToSingle(data, i));
            return times.ToArray();
        }
        public byte[] GetPodALL()
        {
            uint[] path = { Addr.pPod, 0x0 };
            return GetData(path, "", Addr.lPod);
        }
        public byte[] GetPodCustom(uint offset, uint length)
        {
            uint[] path = { Addr.pPod, offset };
            return GetData(path, "", length);
        }
        public void WritePodALL(dynamic data)
        {
            uint[] path = { Addr.pPod, 0x0 };
            WriteData(path, data);
        }



        public dynamic GetPodData(string datapoint, string datatype)
        {
            uint[] path = { Addr.pPod, Addr.oPod["pPodData"], Addr.oPodData[datapoint] };
            return GetData(path, datatype);
        }
        public byte[] GetPodDataALL()
        {
            uint[] path = { Addr.pPod, Addr.oPod["pPodData"], 0x0 };
            return GetData(path, "", Addr.lPodData);
        }
        public void WritePodDataALL(dynamic data)
        {
            uint[] path = { Addr.pPod, Addr.oPod["pPodData"], 0x0 };
            WriteData(path, data);
        }



        public dynamic GetStatic(string datapoint, string datatype)
        {
            uint[] path = { Addr.oStatic[datapoint] };
            return GetData(path, datatype);
        }



        public Single[] GetStatsALL()
        {
            List<Single> output = new List<Single>();
            foreach (KeyValuePair<string, uint> entry in Addr.oStats)
            {
                uint[] path = { Addr.oStats[entry.Key] };
                output.Add(GetData(path, "float"));
            }
            return output.ToArray();
        }



        public void SetDebugMenu(bool enable)
        {
            uint[] path1 = { Addr.oDebug["debug_menu"] };
            uint[] path2 = { Addr.oDebug["debug_menu_text"] };
            uint[] path3 = { Addr.oDebug["debug_level"] };
            WriteData(path1, (uint)(enable ? 0x01 : 0x0));
            WriteData(path2, (uint)(enable ? 0x3F : 0x0));
            WriteData(path3, (uint)(enable ? 0x06 : 0x0));
        }
        public void SetDebugTerrainLabels(bool enable)
        {
            uint[] path = { Addr.oDebug["terrain_labels"] };
            WriteData(path, (uint)(enable ? 0x01 : 0x0));
        }
        public void SetDebugInvincibility(bool enable)
        {
            uint[] path = { Addr.oDebug["invincibility"] };
            WriteData(path, (uint)(enable ? 0x01 : 0x0));
        }



        private dynamic GetData(uint[] path, string type = "", uint len = 4)
        {
            IntPtr addr;
            if (game != null)
            {
                try
                {
                    addr = GetMemoryAddr(game, path);
                }
                catch (Exception)
                {
                    // cannot locate process
                    return false;
                }
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
                        return mem.ReadMemory(addr, len, out bytesOut);
                }
            }
            // if all options failed
            return false;
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
                throw new Exception("Game process not found.");
        }

        private IntPtr GetMemoryAddr(Process game, uint[] pointerPath)
        {
            if (!CheckProcess() || !InitProcess())
                throw new Exception("Game target not initialized.");
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
            if (!CheckProcess())
                return false;
            if (mem.ReadProcess == null)
            {
                try { mem.CloseHandle(); } catch { }
                mem.ReadProcess = game;
                mem.OpenProcess();
            }
            // in case it failed
            if (mem.ReadProcess == null)
                return false;
            else
                return true;
        }
        public void SetGameTarget(Process tgt)
        {
            game = tgt;
            if (mem.ReadProcess != null) {
                try { mem.CloseHandle(); } catch {  }
            }
            InitProcess();
        }
        private bool CheckProcess()
        {
            if (game != null && game.HasExited)
            {
                game.Dispose();
                game = null;
                return false;
            }
            if (game == null)
                return false;

            return true;
        }
    }
}
