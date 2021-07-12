using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using mckielce;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace mcmacro {

    public partial class Form : System.Windows.Forms.Form {

        bool failSafe = false;

        private KeyHandler ghk;
        public Form() {
            InitializeComponent();
            ghk = new KeyHandler(Keys.F7, this);
            ghk.Register();
            comboBox2.SelectedIndex = 0;
        }

        private void HandleHotkey() {
            failSafe = true;
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
                HandleHotkey();
            base.WndProc(ref m);
        }

        public void wait(int milliseconds) {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            // Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled) {
                Application.DoEvents();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            int mode = comboBox2.SelectedIndex;
            Form1 progressW = new Form1();
            progressW.Show();
            this.Focus();
            failSafe = false;
            wait(3000);
            byte[] binary = Convert.FromBase64String(textBox1.Text);
            string json = ASCIIEncoding.ASCII.GetString(binary);
            McKielceData data = JsonConvert.DeserializeObject<McKielceData>(json);
            McKielceCoords[] coords = data.coords;
            var sim = new InputSimulator();
            if (mode == 0 || mode == 1) {
                sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
                wait(100);
                sim.Keyboard.TextEntry("//sel poly");
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                wait(100);
            } else {
                sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
                wait(100);
                sim.Keyboard.TextEntry("//sel cuboid");
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                wait(100);
            }
            int i = 1, i2 = 1 ;
            foreach (McKielceCoords coord in coords) {
                if (failSafe) break;
                float progress = ((float)i / (float)coords.Length) * 100;
                progressBar1.Value = (int)progress;
                label3.Text = i.ToString() + "/" + coords.Length.ToString();
                progressW.modifyProgress(i, coords.Length);
                progressW.modifyCoords(coord.x, coord.y);
                sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
                wait(100);
                if (failSafe) break;
                sim.Keyboard.TextEntry("/tp " + coord.x.ToString().Replace(",",".") + " 41 " + coord.y.ToString().Replace(",", "."));
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                if (i == 1) wait(6000);
                wait(500);
                if (failSafe) break;
                if (i2 == 1) {
                    sim.Mouse.LeftButtonClick();
                } else {
                    sim.Mouse.RightButtonClick();
                }
                wait(300);
                if (mode == 2 && i>1) {
                    sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
                    wait(100);
                    sim.Keyboard.TextEntry("//line " + comboBox1.Text);
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    wait(300);
                }
                i++;
                i2++;
                if (i2>2&&mode == 2) i2 = 1;
            }
            if (failSafe) {
                label3.Text = "Przerwanie awaryjne!";
                progressW.Close();
                return;
            }
            label3.Text = "Zastępowanie bloków...";
            wait(500);
            sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
            wait(100);
            switch(mode) {
                case 0:
                    sim.Keyboard.TextEntry("//replace grass_block " + comboBox1.Text);
                    break;
                case 1:
                    sim.Keyboard.TextEntry("//set " + comboBox1.Text);
                    break;
                default:
                    break;
            }
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            label3.Text = "Czekam na rozpoczęcie...";
            progressBar1.Value = 0;
            progressW.Close();

        }
    }

    public static class Constants {
        //windows message id for hotkey
        public const int WM_HOTKEY_MSG_ID = 0x0312;
    }

    public class KeyHandler {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private int key;
        private IntPtr hWnd;
        private int id;

        public KeyHandler(Keys key, Form form) {
            this.key = (int)key;
            this.hWnd = form.Handle;
            id = this.GetHashCode();
        }

        public override int GetHashCode() {
            return key ^ hWnd.ToInt32();
        }

        public bool Register() {
            return RegisterHotKey(hWnd, id, 0, key);
        }

        public bool Unregiser() {
            return UnregisterHotKey(hWnd, id);
        }
    }

    public class McKielceCoords {
        public float x;
        public float y;
    }

    public class McKielceData {
        public int version;
        public McKielceCoords[] coords;
    }
}
