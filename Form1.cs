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
    public partial class Form1 : Form
    {
        public List<YTAccount> accounts = new List<YTAccount>();
        public List<string[]> aproxies = new List<string[]>();
        public List<string> agents = new List<string>();

        public List<string> comments = new List<string>();
        public List<string> keywords = new List<string>();
        public List<string> channels = new List<string>();

        public Form1()
        {
            InitializeComponent();
            #region GRID COLUMNS
            this.dataGridView1.AutoGenerateColumns = false;
            this.Username = new System.Windows.Forms.DataGridViewColumn() { CellTemplate = new System.Windows.Forms.DataGridViewTextBoxCell() };
            this.Email = new System.Windows.Forms.DataGridViewColumn() { CellTemplate = new System.Windows.Forms.DataGridViewTextBoxCell() };
            this.Proxy = new System.Windows.Forms.DataGridViewColumn() { CellTemplate = new System.Windows.Forms.DataGridViewTextBoxCell() };
            this.Status = new System.Windows.Forms.DataGridViewColumn() { CellTemplate = new System.Windows.Forms.DataGridViewTextBoxCell() };
            this.UserAgent = new System.Windows.Forms.DataGridViewColumn() { CellTemplate = new System.Windows.Forms.DataGridViewTextBoxCell() };
            this.Action = new System.Windows.Forms.DataGridViewButtonColumn() { CellTemplate = new System.Windows.Forms.DataGridViewButtonCell() };
            // 
            // Username
            // 
            this.Username.DataPropertyName = "Channel Name";
            this.Username.HeaderText = "Channel Name";
            this.Username.Name = "ChannelName";
            this.Username.Width = 138;
            this.Username.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.Username);
            // 
            // Email
            // 
            this.Email.DataPropertyName = "Email";
            this.Email.HeaderText = "Email";
            this.Email.Name = "Email";
            this.Email.Width = 164;
            this.Email.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.Email);
            // 
            // Proxy
            // 
            this.Proxy.DataPropertyName = "Proxy";
            this.Proxy.HeaderText = "Proxy";
            this.Proxy.Name = "Proxy";
            this.Proxy.Width = 125;
            this.Proxy.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.Proxy);
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.Width = 117;
            this.Status.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.Status);
            // 
            // UserAgent
            // 
            this.UserAgent.DataPropertyName = "UserAgent";
            this.UserAgent.HeaderText = "User-Agent";
            this.UserAgent.Name = "UserAgent";
            this.UserAgent.Width = 123;
            this.UserAgent.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.UserAgent);
            // 
            // Action
            // 
            this.Action.DataPropertyName = "Action";
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.Width = 101;
            this.Action.ReadOnly = true;
            this.dataGridView1.Columns.Add(this.Action);
#endregion
            dataGridView1.CellClick += CellClick;
            dataGridView1.SelectionChanged += (o, e) => dataGridView1.ClearSelection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log("Starting Application", LogType.INFO);
            FormClosed += (so, se) => Environment.Exit(0);
            #region WARNINGS
            try
            {
                agents.AddRange(File.ReadAllLines("agents.txt"));
            }
            catch
            {
                Log("Agents file not found", LogType.WARNING);
            }
            try
            {
                comments.AddRange(File.ReadAllText("comments.txt").Split(new[] { "\r\n~\r\n" }, StringSplitOptions.None).Where(x => x.Any(y => Char.IsLetter(y))));
            }
            catch
            {
                Log("Comments file not found", LogType.WARNING);
            }
            try
            {
                richTextBox3.Text = File.ReadAllText("keywords.txt");
                keywords.AddRange(richTextBox3.Text.Split(',').Where(x => x.Any(y => Char.IsLetter(y))));
            }
            catch
            {
                Log("Keywords file not found", LogType.WARNING);
            }
            try
            {
                richTextBox4.Text = File.ReadAllText("channels.txt");
                channels.AddRange(richTextBox4.Text.Split(new[] { "\r\n" }, StringSplitOptions.None).Where(x => x.Any(y => Char.IsLetter(y))));
            }
            catch
            {
                Log("Channels file not found", LogType.WARNING);
            }
            try
            {
                foreach (string file in Directory.GetFiles("ids"))
                {
                    try
                    {
                        var lines = File.ReadAllLines(file);
                        AddAccount(new YTAccount(lines[0].Split(','), file.Split('\\')[1], lines[1], lines[2]));
                    }
                    catch
                    {
                        Log("Invalid account in " + file, LogType.ERROR);
                    }
                }
            }
            catch
            {
                if(!Directory.Exists("ids"))
                    Directory.CreateDirectory("ids");
            }
            #endregion
            Log("Ready", LogType.INFO);
        }

        #region IMPORTS
        private void button1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                foreach (string line in File.ReadAllLines(file.FileName))
                {
                    var data = line.Split(','); //username,password,email,proxy
                    if (data.Length > 1)
                        AddAccount(new YTAccount(data));
                    else
                        Log("Invalid account for " + line, LogType.WARNING);
                }
                Log("Loaded accounts", LogType.INFO);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                foreach (string line in File.ReadAllLines(file.FileName))
                {
                    var data = line.Split(':'); //ip:port:username:password
                    if (data.Length > 1)
                        aproxies.Add(data);
                    else
                        Log("Invalid proxy for " + line, LogType.WARNING);
                }
                Log("Loaded proxies", LogType.INFO);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                foreach (string line in File.ReadAllLines(file.FileName))
                    agents.Add(line);
                Log("Loaded user agents", LogType.INFO);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            var file = new SaveFileDialog();
            file.FileName = "accounts.txt";
            if (file.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(file.FileName, accounts.Select(x => x.ToString()));
                Log("Exported " + accounts.Count + " accounts", LogType.INFO);
            }
        }
#endregion

        void AddAccount(YTAccount account)
        {
            var actButton = new System.Windows.Forms.DataGridViewButtonCell();
            actButton.FlatStyle = FlatStyle.Flat;
            actButton.Value = "Manage";
            actButton.Tag = account.id;
            dataGridView1.Rows.Add(account.username +' '+account.surname, account.email, account.proxy!=null?account.proxy[0] + ':' + account.proxy[1]:"", account.status, FilterAgent(account.agent));
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5] = actButton;
            accounts.Add(account);
        }
        public string FilterAgent(string agent)
        {
            if (agent.Contains("Chrome"))
                return "Chrome Browser";
            if (agent.Contains("Firefox"))
                return "Firefox Browser";
            if (agent.Contains("Windows NT"))
                return "Internet Browser";
            return "Not Identified";
        }
        private void CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewCell cell = dataGridView1[e.ColumnIndex, e.RowIndex];
                if (cell is DataGridViewButtonCell)
                    new ManageForm(GetFromID((string)cell.Tag), cell.OwningRow).Show();
            }catch { }
        }
        public YTAccount GetFromID(string id)
        {
            foreach (YTAccount account in accounts)
                if (account.id == id)
                    return account;
            return null;
        }
        public void removeRow(int index)
        {
            dataGridView1.Rows.RemoveAt(index);
        }

        public enum LogType
        {
            ERROR,WARNING,INFO
        }
        public void Log(string text, LogType type) //CLEAN?
        {
            this.Invoke((MethodInvoker)delegate()
            {
                richTextBox1.AppendText("[" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "] [" + type.ToString() + "] " + text + "\n");
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (YTAccount account in accounts)
                File.Delete("ids/" + account.id);
            accounts.Clear();
            dataGridView1.Rows.Clear();
            MessageBox.Show("Accounts removed!");
            Log("Accounts removed", LogType.INFO);
        }

        #region SETTINGS
        private void button6_Click(object sender, EventArgs e)
        {
            comments.Add(richTextBox2.Text.Replace("\n","\r\n"));
            File.WriteAllText("comments.txt", String.Join("\r\n~\r\n", comments.Where(x => x.Any(y => Char.IsLetter(y))).ToArray()));
            richTextBox2.Text = "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            keywords.Clear();
            keywords.AddRange(richTextBox3.Text.Split(','));
            File.WriteAllText("keywords.txt", String.Join(",", keywords.Where(x => x.Any(y => Char.IsLetter(y))).ToArray()));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = "";
            keywords.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            channels.Clear();
            channels.AddRange(richTextBox4.Text.Split('\n'));
            File.WriteAllText("channels.txt", String.Join("\r\n", channels.Where(x => x.Any(y => Char.IsLetter(y))).ToArray()));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox4.Text = "";
            channels.Clear();
        }

        private CommentsForm commentsForm;
        private void button7_Click(object sender, EventArgs e)
        {
            commentsForm = new CommentsForm(String.Join("\n~\n", comments.Where(x => x.Any(y => Char.IsLetter(y))).ToArray()));
            commentsForm.Show();
        }
#endregion

        Youtube yt;
        private void button12_Click(object sender, EventArgs e)
        {
            button9_Click(null, null); button11_Click(null, null);
            Log("Started bot", LogType.INFO);
            if (commentsForm != null && commentsForm.Visible) commentsForm.Close();
            button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled 
                = button12.Enabled = button6.Enabled = button7.Enabled = button8.Enabled = button9.Enabled = button10.Enabled = button11.Enabled = false; 
            button13.Enabled = true;
            yt = new Youtube(accounts, comments, keywords, channels, (int)numericUpDown1.Value, (int)numericUpDown2.Value, textBox1.Text);
            yt.Start(true);
        }

        public void button13_Click(object sender, EventArgs e)
        {
            yt.Stop();
            button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled
                = button12.Enabled = button6.Enabled = button7.Enabled = button8.Enabled = button9.Enabled = button10.Enabled = button11.Enabled = true; 
            button13.Enabled = false;
            Log("Stopped bot", LogType.INFO);
        }

        public void UpdateRunStatus(long threads, string timestr, long done_search, long done_comments)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                label7.Text = "Searches: " + done_search;
                label6.Text = "Comments: " + done_comments;
                label9.Text = "Run Time: " + timestr;
                label8.Text = "Run Threads: " + threads;
            });
        }
        public void SetStatusFor(YTAccount acc)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                for (var i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (((string)dataGridView1.Rows[i].Cells[0].Value) ==  (string)(acc.username + ' ' + acc.surname))
                    {
                        dataGridView1.Rows[i].Cells[3].Value = acc.status;
                        break;
                    }
                }
            });
        }

        bool runlikes;
        int done_likes;
        object update_likes = new object();
        private void button14_Click(object sender, EventArgs e)
        {
            done_likes = 0;
            button14.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = false; button15.Enabled = true;
            runlikes = true;
            var comment_url = textBox2.Text;
            new Thread(() =>
            {
                Parallel.ForEach(accounts, (account) =>
                {
                    if (!runlikes) return;
                    if (account.status != "Invalid" && (account.status == "Validated" || account.Login() == YTAccount.LoginStatus.Logged))
                    {
                        if (account.Like(comment_url))
                        {
                            lock (update_likes) done_likes++;
                            this.Invoke((MethodInvoker)delegate()
                            {
                                label12.Text = "Likes: " + done_likes;
                            });
                            Program.APP.Log(account.email + " liked " + comment_url, Form1.LogType.INFO);
                        }
                    }
                    else
                    {
                        Program.APP.Log("Account " + account.email + " has become invalid", Form1.LogType.ERROR);
                    }
                    SetStatusFor(account);
                });
             }).Start();
        }
        private void button15_Click(object sender, EventArgs e)
        {
            runlikes = false;
            button14.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = true; button15.Enabled = false;
        }

        bool runtarget;
        int done_target;
        object update_target = new object();
        private void button17_Click(object sender, EventArgs e)
        {
            done_target = 0;
            button17.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = button7.Enabled = false; button16.Enabled = true;
            runtarget = true;
            var video_target = textBox3.Text;
            new Thread(() =>
            {
                Parallel.ForEach(accounts, (account) =>
                {
                    if (!runtarget || comments.Count < 1) return;
                    if (account.status != "Invalid" && (account.status == "Validated" || account.Login() == YTAccount.LoginStatus.Logged))
                    {
                        if (account.PostComment(video_target, comments[Program.RAND.Next(comments.Count)]))
                        {
                            lock (update_target) done_target++;
                            this.Invoke((MethodInvoker)delegate()
                            {
                                label13.Text = "Comments: " + done_target;
                            });
                            Program.APP.Log(account.email + " commented " + account.last_comment, Form1.LogType.INFO);
                        }
                        else
                        {
                            Program.APP.Log("Comment " + account.last_comment + " not visible or invalid", Form1.LogType.ERROR);
                        }
                    }
                    else
                    {
                        Program.APP.Log("Account " + account.email + " has become invalid", Form1.LogType.ERROR);
                    }
                    SetStatusFor(account);
                });
            }).Start();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            runtarget = false;
            button17.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = button7.Enabled = true; button16.Enabled = false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            new PanelAuthForm().Show();
        }

        bool upload = false;
        int done_avatars;
        object update_avatar = new object();
        private void button20_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("avatars");
            upload = true;
            button20.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = false; button19.Enabled = true;
            new Thread(() => {
                Parallel.ForEach(accounts, (account) =>
                {
                    if (!upload) return;
                    var avatar = files[Program.RAND.Next(files.Length)];
                    if (account.status != "Invalid" && (account.status == "Validated" || account.Login() == YTAccount.LoginStatus.Logged))
                    {
                        account.UploadAvatar(avatar);
                        lock (update_avatar) done_avatars++;
                        this.Invoke((MethodInvoker)delegate()
                        {
                            label16.Text = "Uploaded: " + done_avatars;
                        });
                        Program.APP.Log(account.email + " has new avatar", Form1.LogType.INFO);

                    }
                    else
                    {
                        Program.APP.Log("Account " + account.email + " has become invalid", Form1.LogType.ERROR);
                    }
                    SetStatusFor(account);
                });
            }).Start();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            upload = false;
            button20.Enabled = button1.Enabled = button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = true; button19.Enabled = false;
        }
    }
}
