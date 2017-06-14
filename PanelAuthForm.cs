using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTDom
{
    public partial class PanelAuthForm : Form
    {
        public PanelAuthForm()
        {
            InitializeComponent();
            if (File.Exists("panel.txt"))
            {
                var data = File.ReadAllLines("panel.txt");
                textBox1.Text = data[0];
                textBox2.Text = data[1];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("panel.txt",new[]{textBox1.Text,textBox2.Text});
        }
    }
}
