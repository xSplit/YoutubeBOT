using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YTDom
{
    public class Youtube
    {
        public bool running = false;
        public int videos_to_scrape, comments_likes;
        public int threads, time, done_search, done_comments;

        private List<string> comments, keywords, channels;
        private Dictionary<YTAccount, List<string>> comments_urls = new Dictionary<YTAccount, List<string>>();
        private List<YTAccount> accounts;

        private string panel;
        private CookieContainer cpanel = null;

        public Youtube(List<YTAccount> accounts, List<string> comments, List<string> keywords, List<string> channels, int videos_to_scrape, int comments_likes, string panel)
        {
            this.accounts = accounts;
            this.comments = comments;
            this.keywords = keywords;
            this.channels = channels;
            this.videos_to_scrape = videos_to_scrape;
            this.comments_likes = comments_likes;
            this.panel = panel;
        }

        //Delete Timer
        public void Start(bool update_form_status)
        {
            threads = time = done_search = done_comments = 0;
            running = true;
            var time_timer = new System.Timers.Timer();
            time_timer.Interval = 1000;
            time_timer.Elapsed += (o, e) => {
                time++;
                if (!running)
                {
                    time = threads = 0;
                    time_timer.Stop();
                }
                if (update_form_status)
                {
                    string timestr = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
                    Program.APP.UpdateRunStatus(threads, timestr, done_search, done_comments);
                }
            };
            time_timer.Start();
            new Thread(PollSearch).Start();
            new Thread(PollChannel).Start();
            new Thread(CommentsChecker).Start();
            threads = 4;
        }

        public void Stop()
        {
            running = false;
            comments_urls.Clear();
        }

        private bool poll_search, poll_channel;
        private object threads_update = new object(), comments_update = new object(), searches_update = new object();

        private void PollSearch()
        {
            poll_search = true;
            while (running)
            {
                //while (!poll_search) Thread.Sleep(60000);

            PollSearch_TryAgain:
                try
                {
                    if (!running || keywords.Count < 1) return;
                    var keyword = keywords[Program.RAND.Next(keywords.Count)];
                    var web = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/results?search_query=" + WebUtility.UrlEncode(keyword));
                    web.UserAgent = Program.DEFAULT_AGENT;
                    using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                    {
                        var html = read.ReadToEnd();
                        var hvideos = html.Split(new[] { "<h3 class=\"yt-lockup-title \"><a href=\"/watch?v=" }, StringSplitOptions.None).Skip(1).ToArray();
                        Parallel.For(0, videos_to_scrape, (i) =>
                            {
                                if(hvideos.Length > i)
                                    CommentLoop("https://www.youtube.com/watch?v=" + hvideos[i].Split('"')[0]);
                            });
                    }
                    lock (searches_update) done_search++;
                    keywords.Remove(keyword);
                    Program.APP.Log("Polled videos from keyword " + keyword, Form1.LogType.INFO);
                }
                catch { goto PollSearch_TryAgain; }

                poll_search = false;
            }
        }

        private void PollChannel()
        {
            poll_channel = true;
            while (running)
            {
                //while (!poll_channel) Thread.Sleep(60000);

            PollChannel_TryAgain:
                try
                {
                    if (!running || channels.Count < 1) return;
                    var channel = channels[Program.RAND.Next(channels.Count)];
                    var web = (HttpWebRequest)WebRequest.Create(channel+(channel.ToCharArray().Last()=='/'?"":"/")+"videos");
                    web.UserAgent = Program.DEFAULT_AGENT;
                    using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                    {
                        var html = read.ReadToEnd();
                        var hvideos = html.Split(new[] { "<span class=\" spf-link  ux-thumb-wrap contains-addto\"><a href=\"/watch?v=" }, StringSplitOptions.None).Skip(1).ToArray();
                        Parallel.For(0, videos_to_scrape, (i) =>
                        {
                            if (hvideos.Length > i)
                                CommentLoop("https://www.youtube.com/watch?v=" + hvideos[i].Split('"')[0]);
                        });
                    }
                    lock (searches_update) done_search++;
                    channels.Remove(channel);
                    Program.APP.Log("Polled videos from channel " + channel, Form1.LogType.INFO);
                }
                catch { goto PollChannel_TryAgain; }

                poll_channel = false;
            }
        }

        private void CommentsChecker()
        {
            while (running)
            {
                try
                {
                    foreach (var account_comments in comments_urls)
                    {
                        try
                        {
                            if (!running) return;
                            foreach (var comment in account_comments.Value)
                            {
                                if (!running) return;
                                account_comments.Key.DeleteReply(comment);
                            }
                        }
                        catch { }
                        Thread.Sleep(60000); //?
                    }
                }
                catch { }
            }
        }

        private void CommentLoop(string vid)
        {
            lock (threads_update) threads++;
            string video = vid;
            var runloop = true;
            while (runloop && running)
            {
                var account = accounts[Program.RAND.Next(accounts.Count)];
                if (account.status != "Invalid" && (account.status == "Validated" || account.Login() == YTAccount.LoginStatus.Logged))
                {
                    if (account.PostComment(video, comments[Program.RAND.Next(comments.Count)]))
                    {
                        lock (comments_update) done_comments++;
                        Program.APP.Log(account.email + " commented " + account.last_comment, Form1.LogType.INFO);
                        new Thread(LikesLoop).Start(account.last_comment);
                        PanelLikes(account.last_comment);
                        try
                        {
                            if (!comments_urls.ContainsKey(account))
                                comments_urls.Add(account, new List<string>());
                            comments_urls[account].Add(account.last_comment);
                        }
                        catch { }
                        runloop = false;
                    }
                    else
                    {
                        if(account.last_comment != null && account.last_comment.Length > 2)
                            Program.APP.Log("Comment " + account.last_comment + " not visible or invalid", Form1.LogType.ERROR);
                        accounts.Remove(account);
                    }
                }
                else
                {
                    Program.APP.Log("Account " + account.email + " has become invalid", Form1.LogType.ERROR);
                    accounts.Remove(account);
                }
                Program.APP.SetStatusFor(account);
            }
            lock (threads_update) threads--;
        }
        private void LikesLoop(object _comment_url)
        {
            lock (threads_update) threads++;
            string comment_url = (string)_comment_url;
            var runloop = true;
            int i = 0, v = 0;
            while (runloop && running)
            {
                if (v == comments_likes) break;
                if (accounts.Count > i)
                {
                    var account = accounts[i];
                    if (account.status != "Invalid" && (account.status == "Validated" || account.Login() == YTAccount.LoginStatus.Logged))
                    {
                        if (account.Like(comment_url))
                        {
                            Program.APP.Log(account.email + " liked " + comment_url, Form1.LogType.INFO);
                            v++;
                        }
                    }
                    else
                    {
                        Program.APP.Log("Account " + account.email + " has become invalid", Form1.LogType.ERROR);
                        accounts.Remove(account);
                    }
                    Program.APP.SetStatusFor(account);
                    i++;
                }
                else runloop = false;
            }
            lock (threads_update) threads--;
        }

        private void PanelLikes(string comment_url)
        {
            if (panel.Length > 2)
            {
                if (cpanel == null)
                {
                    if (!File.Exists("panel.txt"))
                    {
                        Program.APP.Log("Panel auth not set", Form1.LogType.ERROR);
                        return;
                    }
                    cpanel = new CookieContainer();
                    var webx = (HttpWebRequest)WebRequest.Create(panel + (panel.ToCharArray().Last() == '/' ? "" : "/") + "login.php");
                    webx.CookieContainer = cpanel;
                    webx.UserAgent = Program.DEFAULT_AGENT;
                    webx.GetResponse();
                    var data = File.ReadAllLines("panel.txt");
                    var web = (HttpWebRequest)WebRequest.Create(panel + (panel.ToCharArray().Last() == '/' ? "" : "/") + "login.php");
                    web.CookieContainer = cpanel;
                    web.Method = "POST";
                    web.ContentType = "application/x-www-form-urlencoded";
                    web.UserAgent = Program.DEFAULT_AGENT;
                    var req = Encoding.ASCII.GetBytes("username=" + WebUtility.UrlDecode(data[0]) + "&password=" + WebUtility.UrlDecode(data[1]) + "&submit=login");
                    web.ContentLength = req.Length;
                    web.GetRequestStream().Write(req, 0, req.Length);
                    web.GetResponse();
                }
                var web2 = (HttpWebRequest)WebRequest.Create(panel + (panel.ToCharArray().Last() == '/' ? "" : "/") + "index.php");
                web2.CookieContainer = cpanel;
                web2.Method = "POST";
                web2.ContentType = "application/x-www-form-urlencoded";
                web2.UserAgent = Program.DEFAULT_AGENT;
                var rd = Encoding.ASCII.GetBytes("url=" + WebUtility.UrlEncode(comment_url) + "&value=250");
                web2.ContentLength = rd.Length;
                web2.GetRequestStream().Write(rd, 0, rd.Length);
                web2.GetResponse();
            }
        }
    }
}
