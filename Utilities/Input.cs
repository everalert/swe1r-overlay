using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SlimDX.XInput;
using SlimDX.DirectInput;

namespace SWE1R_Overlay.Utilities
{
    public class Input
    {
        private bool hotkeys_on = true;
        public byte map = 0;
        private List<HotkeyMap> map_groups;
        private XInput control_xi;
        private Keyboard control_kb;
        private System.Windows.Forms.Form parent;

        public Input (System.Windows.Forms.Form host)
        {
            parent = host;
            control_xi = new XInput();
            control_kb = new Keyboard(parent);
            map_groups = new List<HotkeyMap>(byte.MaxValue);
            map_groups.Add(new HotkeyMap());
            map_groups.LastOrDefault().MAP["state_inrace_load"].xi = XInput.BUTTONS["DPAD_UP"];
            map_groups.LastOrDefault().MAP["state_inrace_load"].kb = Key.D1;
            map_groups.LastOrDefault().MAP["state_inrace_save"].xi = XInput.BUTTONS["DPAD_DOWN"];
            map_groups.LastOrDefault().MAP["state_inrace_save"].kb = Key.D2;
            map_groups.Add(new HotkeyMap());
            map_groups.LastOrDefault().MAP["frameadv_advance"].xi = XInput.BUTTONS["DPAD_UP"];
            map_groups.LastOrDefault().MAP["frameadv_toggle"].xi = XInput.BUTTONS["DPAD_DOWN"];
            map_groups.LastOrDefault().MAP["state_inrace_load"].kb = Key.D4;
            map_groups.LastOrDefault().MAP["state_inrace_save"].kb = Key.D3;
        }

        public void Update()
        {
            control_xi.Update();
            control_kb.Update();
        }

        public bool CheckHotkey(string hk)
        {
            var hkMap = map_groups[map % map_groups.Count].MAP;
            if (hkMap.ContainsKey(hk) && hotkeys_on)
                return (control_xi.GetButton(hkMap[hk].xi, 0) || control_kb.GetKey(hkMap[hk].kb, 0));
            return false;
        }

        public HotkeyMap GetCurrentMap()
        {
            return map_groups[map % map_groups.Count];
        }

        public void EnableHotkeys(bool enable)
        {
            hotkeys_on = enable;
        }

        private class XInput
        {
            private Controller controller;
            private Gamepad gamepad_new, gamepad_old;
            public bool connected = false;
            public int deadband = 2500;
            public Point leftThumb, rightThumb = new Point(0, 0);
            public float leftTrigger, rightTrigger;
            public static Dictionary<string, int> BUTTONS = new Dictionary<string, int>()
            {
                { "DPAD_UP",    0x0001 },
                { "DPAD_DOWN",  0x0002 },
                { "DPAD_LEFT",  0x0004 },
                { "DPAD_RIGHT", 0x0008 },
                { "START", 0x0010 },
                { "BACK",  0x0020 },
                { "LEFT_THUMB",     0x0040 },
                { "RIGHT_THUMB",    0x0080 },
                { "LEFT_SHOULDER",  0x0100 },
                { "RIGHT_SHOULDER", 0x0200 },
                { "A", 0x1000 },
                { "B", 0x2000 },
                { "X", 0x4000 },
                { "Y", 0x8000 }
            };
            public static Dictionary<int, string> BUTTON_LABELS = new Dictionary<int, string>()
            {
                { 0x0001, "D Up" },
                { 0x0002, "D Dn" },
                { 0x0004, "D Lf" },
                { 0x0008, "D Rt" },
                { 0x0010, "Start" },
                { 0x0020, "Back" },
                { 0x0040, "L3" },
                { 0x0080, "R3" },
                { 0x0100, "LB" },
                { 0x0200, "RB" },
                { 0x1000, "A" },
                { 0x2000, "B" },
                { 0x4000, "X" },
                { 0x8000, "Y" }
            };

            public XInput()
            {
                controller = new Controller(UserIndex.One);
                connected = controller.IsConnected;
            }

            public void Update()
            {
                connected = controller.IsConnected;
                if (!connected)
                    return;

                if (gamepad_new != null)
                    gamepad_old = gamepad_new;
                gamepad_new = controller.GetState().Gamepad;

                leftThumb.X = gamepad_new.LeftThumbX;
                leftThumb.Y = gamepad_new.LeftThumbY;
                rightThumb.X = gamepad_new.RightThumbX;
                rightThumb.Y = gamepad_new.RightThumbY;

                leftTrigger = gamepad_new.LeftTrigger;
                rightTrigger = gamepad_new.RightTrigger;
            }

            public bool GetButton(int btn, int mode = 2)
            {
                if (!connected || gamepad_new == null || gamepad_old == null)
                    return false;
                switch (mode)
                {
                    case 0: //down
                        return ((gamepad_old.Buttons.GetHashCode() & btn) == 0 && (gamepad_new.Buttons.GetHashCode() & btn) != 0);
                    case 1: //up
                        return ((gamepad_old.Buttons.GetHashCode() & btn) != 0 && (gamepad_new.Buttons.GetHashCode() & btn) == 0);
                    default: //raw
                        return ((gamepad_new.Buttons.GetHashCode() & btn) != 0);
                }
            }
        }

        private class Keyboard
        {
            private DirectInput directInput;
            private SlimDX.DirectInput.Keyboard keyboard;
            private KeyboardState keys_old, keys_new;

            public Keyboard(System.Windows.Forms.Form host)
            {
                directInput = new DirectInput();
                keyboard = new SlimDX.DirectInput.Keyboard(directInput);
                keyboard.SetCooperativeLevel(host, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
                keyboard.Acquire();
            }

            public void Update()
            {
                if (keys_new != null)
                    keys_old = keys_new;
                keys_new = keyboard.GetCurrentState();
            }

            public bool GetKey(Key key, int mode = 2)
            {
                keys_new.IsPressed(Key.D1);
                switch (mode)
                {
                    case 0: //down
                        return (keys_new.IsPressed(key) && keys_old.IsReleased(key));
                    case 1: //up
                        return (keys_new.IsReleased(key) && keys_old.IsPressed(key));
                    default: //raw
                        return keys_new.IsPressed(key);
                }
            }
        }

        public class HotkeyMap
        {
            public Dictionary<string, HotkeySet> MAP = new Dictionary<string, HotkeySet>()
            {
                { "state_inrace_save",     new HotkeySet() },
                { "state_inrace_load",     new HotkeySet() },
                { "state_inrace_cycle_up", new HotkeySet() },
                { "state_inrace_cycle_dn", new HotkeySet() },
                { "frameadv_toggle",  new HotkeySet() },
                { "frameadv_advance", new HotkeySet() }
            };
        }

        public class HotkeySet
        {
            public Key kb = Key.Unknown;
            public int xi = 0x0;

            public string[] GetLabels()
            {
                return new string[2] { (xi!=0?XInput.BUTTON_LABELS[xi]:""), (kb!=Key.Unknown?kb.ToString():"") };
            }
        }
    }
}
