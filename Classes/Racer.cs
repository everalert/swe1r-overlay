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
        public Process game;

        public Racer(Process target = null)
        {
            if (target != null)
                UpdateGame(target);
        }


        // WHYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
        // need more thought into structure
        // - fewer, more generalised functions
        // - possibly implement dictionary inputs for mass data reading from a pointer
        // also need to error handle memory reading exceptions
        // - not really a problem unless user selects wrong process tho lol
        // - also, does steam hack detection block this? autosplitter can read steam version so probably no, or livesplit uses a workaround


        // RACE

        public dynamic GetData(Addr.Race datapoint, uint len = 0)
        {
            uint[] path = { (uint)Addr.BasePtr.Race, (uint)datapoint };
            Addr.DataTypes type = len > 0 ? Addr.DataTypes.None : (Addr.TypesForRace.ContainsKey(datapoint) ? Addr.TypesForRace[datapoint] : Addr.DataTypes.Single);
            return GetData(path, type, Math.Max(0, len));
        }
        public void WriteData(Addr.Race offset, dynamic data)
        {
            uint[] path = { (uint)Addr.BasePtr.Race, (uint)offset };
            WriteData(path, data);
        }

        // POD

        public dynamic GetData(Addr.Pod datapoint, uint len = 0)
        {
            uint[] path = { (uint)Addr.BasePtr.Pod, (uint)datapoint };
            Addr.DataTypes type = len > 0 ? Addr.DataTypes.None : (Addr.TypesForPod.ContainsKey(datapoint) ? Addr.TypesForPod[datapoint] : Addr.DataTypes.Single);
            return GetData(path, type, Math.Max(0, len));
        }
        public void WriteData(Addr.Pod offset, dynamic data)
        {
            uint[] path = { (uint)Addr.BasePtr.Pod, (uint)offset };
            WriteData(path, data);
        }

        // POD STATE

        public dynamic GetData(Addr.PodState datapoint, uint len = 0)
        {
            uint[] path = { (uint)Addr.BasePtr.Pod, (uint)Addr.Pod.PtrPodState, (uint)datapoint };
            Addr.DataTypes type = len > 0 ? Addr.DataTypes.None : (Addr.TypesForPodState.ContainsKey(datapoint) ? Addr.TypesForPodState[datapoint] : Addr.DataTypes.Single);
            return GetData(path, type, Math.Max(0, len));
        }
        public void WriteData(Addr.PodState offset, dynamic data)
        {
            uint[] path = { (uint)Addr.BasePtr.Pod, (uint)Addr.Pod.PtrPodState, (uint)offset };
            WriteData(path, data);
        }

        // RENDERING

        public dynamic GetData(Addr.Rendering datapoint, uint len = 0)
        {
            uint[] path = { (uint)Addr.BasePtr.Rendering, (uint)datapoint };
            Addr.DataTypes type = len > 0 ? Addr.DataTypes.None : (Addr.TypesForRendering.ContainsKey(datapoint) ? Addr.TypesForRendering[datapoint] : Addr.DataTypes.Single);
            return GetData(path, type, Math.Max(0, len));
        }
        public void WriteData(Addr.Rendering offset, dynamic data)
        {
            uint[] path = { (uint)Addr.BasePtr.Rendering, (uint)offset };
            WriteData(path, data);
        }

        // STATIC

        public dynamic GetData(Addr.Static datapoint, uint len = 0)
        {
            uint[] path = { (uint)datapoint };
            Addr.DataTypes type = len > 0 ? Addr.DataTypes.None : (Addr.TypesForStatic.ContainsKey(datapoint) ? Addr.TypesForStatic[datapoint] : Addr.DataTypes.Single);
            return GetData(path, type, Math.Max(0, len));
        }
        public void WriteData(Addr.Static offset, dynamic data)
        {
            uint[] path = { (uint)offset };
            WriteData(path, data);
        }

        // GENERIC FUNCTIONS

        private dynamic GetData(uint[] path, Addr.DataTypes type, uint len = 4)
        {
            IntPtr addr;
            if (CheckGame())
            {
                try { addr = GetMemoryAddr(game, path); } catch (Exception) { return false; }
                uint defLen = 4;
                switch (type)
                {
                    case Addr.DataTypes.Byte:
                        return mem.ReadMemory(addr, 1, out bytesOut)[0];
                    case Addr.DataTypes.UInt16:
                        return BitConverter.ToUInt16(mem.ReadMemory(addr, 2, out bytesOut), 0);
                    case Addr.DataTypes.UInt32:
                        return BitConverter.ToUInt32(mem.ReadMemory(addr, 4, out bytesOut), 0);
                    case Addr.DataTypes.UInt64:
                        return BitConverter.ToUInt64(mem.ReadMemory(addr, 8, out bytesOut), 0);
                    case Addr.DataTypes.Single:
                        return BitConverter.ToSingle(mem.ReadMemory(addr, 4, out bytesOut), 0);
                    case Addr.DataTypes.Double:
                        return BitConverter.ToDouble(mem.ReadMemory(addr, 8, out bytesOut), 0);
                    case Addr.DataTypes.String:
                        return BitConverter.ToString(mem.ReadMemory(addr, len > 0 ? len : defLen, out bytesOut), 0);
                    default:
                        return mem.ReadMemory(addr, len > 0 ? len : defLen, out bytesOut);
                }
            }
            else
                throw new Exception("Game process not assigned.");
        }

        private void WriteData(uint[] path, dynamic data)
        {
            //InitProcess();
            if (CheckGame())
            {
                IntPtr addr = GetMemoryAddr(game, path);
                mem.WriteMemory(addr, ((data.GetType()==typeof(byte[]))?data:BitConverter.GetBytes(data)), out bytesIn);
            }
            else
                throw new Exception("Game process not assigned.");
        }

        private IntPtr GetMemoryAddr(Process game, uint[] pointerPath)
        {
            if (!CheckGame())
                throw new Exception("Game process not assigned.");
            if (!CheckPointerPath(pointerPath))
                throw new Exception("Pointer path invalid.");
            uint addr;
            uint next;
            addr = (uint)game.Modules[0].BaseAddress;
            for (int i=0; i<pointerPath.Length; i++)
            {
                if (i == pointerPath.Length - 1)
                    next = addr + pointerPath[i];
                else
                    next = BitConverter.ToUInt32(mem.ReadMemory((IntPtr)(addr + pointerPath[i]), 4, out bytesOut), 0);
                addr = next;
            }
            return (IntPtr)(addr);
        }

        public bool UpdateGame(Process target)
        {
            try { mem.CloseHandle(); } catch { }
            if (mem.ReadProcess != null)
                mem.ReadProcess = null;
            if (game != null)
                game = null;
            if (!target.HasExited)
            {
                game = target;
                mem.ReadProcess = game;
                mem.OpenProcess();
            }
            return CheckGame();
        }
        public bool CheckGame()
        {
            bool output = true;
            if (game == null || mem.ReadProcess == null)
                output = false;
            if (game != null && game.HasExited || mem.ReadProcess != null && mem.ReadProcess.HasExited) // crashes when game closed for a while?
            {
                try { mem.CloseHandle(); } catch { }
                mem.ReadProcess = null;
                game = null;
                output = false;
            }
            return output;
        }

        private bool CheckPointerPath(uint[] path)
        {
            //implement checking later
            return true;
        }
    }
}
