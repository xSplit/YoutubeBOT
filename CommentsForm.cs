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
    public partial class CommentsForm : Form
    {
        public CommentsForm()
        {
            InitializeComponent();
        }

        public CommentsForm(string comments) : this()
        {
            this.richTextBox1.Text = comments;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.APP.comments.Clear();
            Program.APP.comments.AddRange(richTextBox1.Text.Replace("\n", "\r\n").Split(new[]{"\r\n~\r\n"}, StringSplitOptions.None)); //r
            File.WriteAllText("comments.txt", String.Join("\r\n~\r\n", Program.APP.comments.Where(x => x.Any(y => Char.IsLetter(y))).ToArray()));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
}
