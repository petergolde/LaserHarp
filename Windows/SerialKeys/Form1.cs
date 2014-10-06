using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialKeys
{
    public partial class Form1 : Form
    {
        bool started = false;
        bool activeWindow = false;
        Task serialTask;
        CancellationTokenSource cancelTokenSource;
        CancellationToken cancelToken;
        SerialReader serialReader;

        TextBox[] vkControls;

        public Form1()
        {
            InitializeComponent();

            vkControls = new TextBox[] { vk0, vk1, vk2, vk3, vk4, vk5, vk6, vk7, vk8, vk9, vk10 };

            LoadSettings();
        }

        private void LoadSettings()
        {
            baudList.SelectedItem = Properties.Settings.Default.Baud;
            portList.SelectedItem = Properties.Settings.Default.Port;
            for (int i = 0; i < vkControls.Length; ++i) {
                vkControls[i].Text = (string) (Properties.Settings.Default["VK" + i.ToString()]);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (started) {
                Stop();
            }
            else {
                Start();
            }
        }

        private void Start()
        {
            if (!started) {
                bool ok = StartSerial((string)(portList.SelectedItem), int.Parse((string)(baudList.SelectedItem)));
                if (!ok) {
                    MessageBox.Show("Could not open serial port");
                }
                else {
                    cancelTokenSource = new CancellationTokenSource();
                    cancelToken = cancelTokenSource.Token;
                    serialTask = Task.Run(() => {
                        serialReader.ReadLines(new Action<string>(LineInput), cancelToken);
                    }, cancelToken);
                    SetStarted(true);
                }
            }
        }

        private void Stop()
        {
            if (started) {
                cancelTokenSource.Cancel();
                try {
                    serialTask.Wait();
                }
                catch (AggregateException) { }
                serialReader.Dispose();
                serialReader = null;
                SetStarted(false);
            }
        }

        void LineInput(string line)
        {
            int inputNumberValue;

            line = line.Trim();
            string inputNumber = line.Substring(0, line.Length - 1);
            char onOff = line[line.Length - 1];
            if (int.TryParse(inputNumber, out inputNumberValue)) {
                SendKeysForValue(inputNumberValue, onOff == '+');
            }
        }

        private void SendKeysForValue(int value, bool down)
        {
            if (!activeWindow && vkControls[value].Text.Length > 0) {
                uint virtualKey = VkFromText(vkControls[value].Text);
                if (virtualKey == 0)
                    return;
                uint scanCode = WinApi.MapVirtualKey(virtualKey, WinApi.MAPVK_VK_TO_VSC);

                WinApi.INPUT[] inputs = new WinApi.INPUT[1];
                inputs[0] = new WinApi.INPUT();
                inputs[0].type = WinApi.INPUT_TYPE.INPUT_KEYBOARD;
                inputs[0].U.ki = new WinApi.KEYBDINPUT() {
                    wScan = (short)scanCode,
                    wVk = (short)virtualKey,
                    dwFlags = down ? 0 : WinApi.KEYEVENTF.KEYUP
                };
                WinApi.SendInput((uint)inputs.Length, inputs, WinApi.INPUT.Size);
            }
        }

        uint VkFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            int value;
            if (int.TryParse(text, out value))
                return (uint)value;
            if (text.Length == 1) 
                return (uint)(char.ToUpper(text[0]));
            return 0;
        }

        private void SetStarted(bool started)
        {
            startButton.Text = started ? "Stop" : "Start";
            this.started = started;
        }

        bool StartSerial(string portName, int baudRate)
        {
            try {
                serialReader = new SerialReader(this, portName, baudRate);
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        private void maxValue11_ValueChanged(object sender, EventArgs e)
        {
            Stop();
        }

        private void keys11_TextChanged(object sender, EventArgs e)
        {
            Stop();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            activeWindow = true;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            activeWindow = false;
        }

        private void portList_SelectedValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Port = (string) portList.SelectedItem;
            Properties.Settings.Default.Save();
        }

        private void baudList_SelectedValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Baud = (string)baudList.SelectedItem;
            Properties.Settings.Default.Save();
        }

        private void vk0_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < vkControls.Length; ++i) {
                if (vkControls[i] == sender)
                    Properties.Settings.Default["VK" + i.ToString()] = vkControls[i].Text;
            }
            Properties.Settings.Default.Save();
        }
    }

    class SerialReader : IDisposable
    {
        SerialPort port;
        Control control;

        public SerialReader(Control control, string portName, int baudRate)
        {
            this.control = control;
            port = new SerialPort(portName, baudRate);
            port.ReadTimeout = 100;
            port.Open();
        }

        public void ReadLines(Delegate invokeMethod, CancellationToken cancelToken)
        {
            for (; ; ) {
                cancelToken.ThrowIfCancellationRequested();

                string line = null;
                try {
                    line = port.ReadLine();
                }
                catch (TimeoutException) { }

                cancelToken.ThrowIfCancellationRequested();

                if (line != null) {
                    control.BeginInvoke(invokeMethod, line);
                }
            }
        }

        public void Dispose()
        {
            if (port != null) {
                port.Close();
                port.Dispose();
                port = null;
            }
        }
    }

    static class WinApi
    {
        
        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(
        uint nInputs,
        [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
        int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal INPUT_TYPE type;
            internal InputUnion U;
            internal static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        internal enum INPUT_TYPE : uint
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal uint dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [Flags]
        internal enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [DllImport("user32.dll")]
        static internal extern uint MapVirtualKey(uint uCode, uint uMapType);

        internal const uint MAPVK_VK_TO_VSC = 0x00;
        internal const uint MAPVK_VSC_TO_VK = 0x01;
        internal const uint MAPVK_VK_TO_CHAR = 0x02;
        internal const uint MAPVK_VSC_TO_VK_EX = 0x03;
        internal const uint MAPVK_VK_TO_VSC_EX = 0x04;
    }
}
