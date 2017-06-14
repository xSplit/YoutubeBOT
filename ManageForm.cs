using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTDom
{
    public partial class ManageForm : Form
    {
        public ManageForm()
        {
            InitializeComponent();
        }

        public YTAccount account;
        public DataGridViewRow row;
        public ManageForm(YTAccount account, DataGridViewRow row) : this()
        {
            this.account = account;
            this.row = row;
            this.textBox1.Text = account.username;
            this.textBox2.Text = account.password;
            this.textBox3.Text = account.email;
            this.textBox4.Text = String.Join(":",account.proxy);
            this.textBox5.Text = account.phone;
            this.textBox6.Text = account.surname;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            new Thread(() =>
            {
                try
                {
                    if (account.Login() == YTAccount.LoginStatus.Logged)
                    {
                        MessageBox.Show("Account validated!");
                        this.Invoke((MethodInvoker)delegate() { row.Cells[3].Value = "Validated"; });
                    }
                    else
                    {
                        MessageBox.Show("Account is invalid!");
                        this.Invoke((MethodInvoker)delegate() { row.Cells[3].Value = "Invalid"; });
                        account.SaveData();
                    }
                    this.Invoke((MethodInvoker)delegate()
                    {
                        row.Cells[4].Value = Program.APP.FilterAgent(account.agent);
                        button1.Enabled = true;
                    });
                }
                catch { }
            }).Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            row.Cells[0].Value = account.username = this.textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            account.password = this.textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            row.Cells[1].Value = account.email = this.textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            account.proxy = this.textBox4.Text.Split(':');
            row.Cells[2].Value = account.proxy.Length > 1 ? account.proxy[0] + ':' + account.proxy[1] : "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.Delete("ids/"+account.id);
            Program.APP.removeRow(row.Index);
            Program.APP.Log("Deleted account with username "+account.username, Form1.LogType.INFO);
            Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            account.surname = this.textBox6.Text;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            account.phone = this.textBox5.Text;
        }
    }
}
