using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mckielce {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.Location = new Point(Screen.FromControl(this).Bounds.Width/2-100, 0);
            totaltime = 3200;
            timer1.Start();
        }

        int totaltime;
        int time;

        public void modifyProgress(int i, int count) {
            totaltime = count * 900 + 6000 + 3000 + 200 + 600;
            float progress = ((float)i / (float)count) * 100;
            progressBar1.Value = (int)progress;
            label1.Text = "Wykonano "+i.ToString() + " / " + count.ToString();
           
        }

        public void modifyCoords(float x, float z) {
            label3.Text = "Pozycja " + x.ToString().Replace(",",".") + " / " + z.ToString().Replace(",", ".");
        }

        private void timer1_Tick(object sender, EventArgs e) {
            time += 100;
            int lefttime = totaltime - time;
            int min = lefttime / 60000;
            int sec = (lefttime - (min * 60000))/1000;
            label2.Text = "Pozostało "+min+"m "+sec+"s";
        }
    }
}
