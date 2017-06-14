using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YTDom
{
    public class YTAccount
    {
        public string username, surname, password, email = "", phone ="", agent = "", status = "Not Tested";
        public string[] proxy;
        public string id;

        public string cookies;
        public bool free = true;

        public YTAccount(string[] data)
        {
            InitAccount(data);
        }
        private void InitAccount(string[] data)
        {
            this.username = data[0];
            this.surname = data[1];
            this.password = data[2];
            if (data.Length > 3) this.email = data[3];
            if (data.Length > 4) this.phone = data[4];
            if (data.Length > 5) this.proxy = data[5].Split(':');
            if (id == null)
            {
                id = Guid.NewGuid().ToString() + ".txt";
                SaveData();
            }
            else if (cookies.Contains("HSID"))
            {
                status = "Validated";
            }
        }

        public YTAccount(string[] data, string id, string agent, string cookies)
        {
            this.id = id;
            this.agent = agent;
            this.cookies = cookies;
            InitAccount(data);
        }

        private IWebProxy GetProxy()
        {
            if (proxy != null && proxy.Length > 1)
            {
                var w = new WebProxy(proxy[0], int.Parse(proxy[1]));
                if (proxy.Length > 3)
                    w.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                return w;
            }
            return null;
        }

        public void DeleteReply(string comment_url)
        {
            try
            {
                //Program.APP.accounts
                var v = comment_url.Split(new[] { "watch?v=" }, StringSplitOptions.None)[1].Split('&')[0];
                var lc = comment_url.Split(new[] { "&lc=" }, StringSplitOptions.None)[1];
                var web = (HttpWebRequest)WebRequest.Create(comment_url);
                web.CookieContainer = Web.StringCookie(cookies);
                web.UserAgent = agent;
                web.Proxy = GetProxy();
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    File.WriteAllText("test.html", data);
                    var rpcheck = data.Split(new[] { "<div class=\"comment-replies-renderer\" data-visibility-tracking=\"" }, StringSplitOptions.None)[2].Split('"')[0];
                    if (rpcheck.Length > 2)
                    {
                        System.Windows.Forms.MessageBox.Show("LOL");
                        var page_cl = data.Split(new[] { "'PAGE_CL': " }, StringSplitOptions.None)[1].Split(',')[0];
                        var build_label = data.Split(new[] { "'PAGE_BUILD_LABEL':" }, StringSplitOptions.None)[1].Split('"')[1];
                        var variants_checksum = data.Split(new[] { "'VARIANTS_CHECKSUM':" }, StringSplitOptions.None)[1].Split('"')[1];
                        var version = data.Split(new[] { "INNERTUBE_CONTEXT_CLIENT_VERSION:" }, StringSplitOptions.None)[1].Split('"')[1];
                        var session_token = data.Split(new[] { "'XSRF_TOKEN':" }, StringSplitOptions.None)[1].Split('"')[1];
                        var identity_token = data.Split(new[] { "'X-YouTube-Identity-Token':" }, StringSplitOptions.None)[1].Split('"')[1];
                        var ytparams = data.Split(new[] { "data-params=\"" }, StringSplitOptions.None)[2].Split('"')[0];
                        var web2 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/flag_service_ajax?action_get_report_form=1");
                        web2.CookieContainer = web.CookieContainer;
                        web2.UserAgent = agent;
                        web2.Proxy = GetProxy();
                        web2.Method = "POST";
                        web2.Headers["X-YouTube-Page-Label"] = build_label;
                        web2.Headers["X-YouTube-Page-CL"] = page_cl;
                        web2.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                        web2.Headers["X-YouTube-Identity-Token"] = identity_token;
                        web2.Headers["X-YouTube-Client-Version"] = version;
                        web2.Headers["X-YouTube-Client-Name"] = "1";
                        web2.Headers["X-Client-Data"] = "x";
                        web2.ContentType = "application/x-www-form-urlencoded";
                        var req = Encoding.ASCII.GetBytes("session_token=" + WebUtility.UrlEncode(session_token) + "&params=" + ytparams);
                        web2.ContentLength = req.Length;
                        web2.GetRequestStream().Write(req, 0, req.Length);
                        using (var read2 = new StreamReader(web2.GetResponse().GetResponseStream()))
                        {
                            data = read2.ReadToEnd();
                            var flag_action = data.Split(new[] { "data-flag-action=\\\"" }, StringSplitOptions.None)[1].Split('\\')[0];
                            System.Windows.Forms.MessageBox.Show(flag_action);
                            var web3 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/flag_service_ajax?action_perform_flag_action=1");
                            web3.CookieContainer = web2.CookieContainer;
                            web3.UserAgent = agent;
                            web3.Proxy = GetProxy();
                            web3.Method = "POST";
                            web3.Headers["X-YouTube-Page-Label"] = build_label;
                            web3.Headers["X-YouTube-Page-CL"] = page_cl;
                            web3.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                            web3.Headers["X-YouTube-Identity-Token"] = identity_token;
                            web3.Headers["X-YouTube-Client-Version"] = version;
                            web3.Headers["X-YouTube-Client-Name"] = "1";
                            web3.Headers["X-Client-Data"] = "x";
                            web3.Referer = comment_url;
                            web3.ContentType = "application/x-www-form-urlencoded";
                            req = Encoding.ASCII.GetBytes("session_token=" + WebUtility.UrlEncode(session_token) + "&flagging_action=" + flag_action);
                            web3.ContentLength = req.Length;
                            web3.GetRequestStream().Write(req, 0, req.Length);
                            web3.GetResponse();
                        }
                    }
                }
            }
            catch { }
        }

        public bool Like(string comment_url)
        {
            CreateChannel();
            try
            {
                var v = comment_url.Split(new[] { "watch?v=" }, StringSplitOptions.None)[1].Split('&')[0];
                var lc = comment_url.Split(new[] { "&lc=" }, StringSplitOptions.None)[1];
                var web = (HttpWebRequest)WebRequest.Create(comment_url);
                web.CookieContainer = Web.StringCookie(cookies);
                web.UserAgent = agent;
                web.Proxy = GetProxy();
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    var page_cl = data.Split(new[] { "'PAGE_CL': " }, StringSplitOptions.None)[1].Split(',')[0];
                    var build_label = data.Split(new[] { "'PAGE_BUILD_LABEL':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var variants_checksum = data.Split(new[] { "'VARIANTS_CHECKSUM':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var version = data.Split(new[] { "INNERTUBE_CONTEXT_CLIENT_VERSION:" }, StringSplitOptions.None)[1].Split('"')[1];
                    var session_token = data.Split(new[] { "'XSRF_TOKEN':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var identity_token = data.Split(new[] { "'X-YouTube-Identity-Token':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var action = data.Split(new[] { "data-action-type=\"like\"" }, StringSplitOptions.None)[1]
                        .Split(new[] { "data-action=\"" }, StringSplitOptions.None)[1].Split('"')[0];
                    var web2 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/comment_service_ajax?action_perform_comment_action=1");
                    web2.CookieContainer = web.CookieContainer;
                    web2.UserAgent = agent;
                    web2.Proxy = GetProxy();
                    web2.Method = "POST";
                    web2.Headers["X-YouTube-Page-Label"] = build_label;
                    web2.Headers["X-YouTube-Page-CL"] = page_cl;
                    web2.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                    web2.Headers["X-YouTube-Identity-Token"] = identity_token;
                    web2.Headers["X-YouTube-Client-Version"] = version;
                    web2.Headers["X-YouTube-Client-Name"] = "1";
                    web2.Referer = comment_url;
                    web2.ContentType = "application/x-www-form-urlencoded";
                    var req = Encoding.ASCII.GetBytes("action=" + action + "&session_token=" + WebUtility.UrlEncode(session_token) + "&vote_status=INDIFFERENT");
                    web2.ContentLength = req.Length;
                    web2.GetRequestStream().Write(req, 0, req.Length);
                    web2.GetResponse();
                    return true;
                }
            }
            catch { }
            return false;
        }
        
        public bool PostComment(string video, string comment)
        {
            CreateChannel();
            try
            {
                var v = video.Split(new[] { "watch?v=" }, StringSplitOptions.None)[1];
                var web = (HttpWebRequest)WebRequest.Create(video);
                web.CookieContainer = Web.StringCookie(cookies);
                web.UserAgent = agent;
                web.Proxy = GetProxy();
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    var page_cl = data.Split(new[] { "'PAGE_CL': " }, StringSplitOptions.None)[1].Split(',')[0];
                    var build_label = data.Split(new[] { "'PAGE_BUILD_LABEL':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var variants_checksum = data.Split(new[] { "'VARIANTS_CHECKSUM':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var version = data.Split(new[] { "INNERTUBE_CONTEXT_CLIENT_VERSION:" }, StringSplitOptions.None)[1].Split('"')[1];
                    var comment_token = data.Split(new[] { "'COMMENTS_TOKEN':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var session_token = data.Split(new[] { "'XSRF_TOKEN':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var identity_token = data.Split(new[] { "'X-YouTube-Identity-Token':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var web2 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/watch_fragments_ajax?v="+v+"&tr=time&distiller=1&ctoken=" + comment_token + "&frags=comments&spf=load");
                    web2.CookieContainer = web.CookieContainer;
                    web2.UserAgent = agent;
                    web2.Proxy = GetProxy();
                    web2.Method = "POST";
                    web2.Headers["X-YouTube-Identity-Token"] = identity_token;
                    web2.Headers["X-Client-Data"] = "x";
                    web2.ContentType = "application/x-www-form-urlencoded";
                    var req = Encoding.ASCII.GetBytes("session_token=" + WebUtility.UrlEncode(session_token) + "&client_url=" + WebUtility.UrlEncode(video));
                    web2.ContentLength = req.Length;
                    web2.GetRequestStream().Write(req, 0, req.Length);
                    using(var read2 = new StreamReader(web2.GetResponse().GetResponseStream()))
                    {
                        data = read2.ReadToEnd();
                        var page_token = data.Split(new[] { "data-token=\\\"" }, StringSplitOptions.None)[2].Split('\\')[0];
                        var ytparams = data.Split(new[] { "data-simplebox-params=\\\"" }, StringSplitOptions.None)[1].Split('\\')[0];
                        var web3 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/comment_service_ajax?action_create_comment=1");
                        web3.CookieContainer = web2.CookieContainer;
                        web3.UserAgent = agent;
                        web3.Proxy = GetProxy();
                        web3.Method = "POST";
                        web3.Headers["X-YouTube-Page-Label"] = build_label;
                        web3.Headers["X-YouTube-Page-CL"] = page_cl;
                        web3.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                        web3.Headers["X-YouTube-Identity-Token"] = identity_token;
                        web3.Headers["X-YouTube-Client-Version"] = version;
                        web3.Headers["X-YouTube-Client-Name"] = "1";
                        web3.Headers["X-Client-Data"] = "x";
                        web3.Referer = video;
                        web3.ContentType = "application/x-www-form-urlencoded";
                        req = Encoding.ASCII.GetBytes("bgr=js_disabled&session_token=" + WebUtility.UrlEncode(session_token) + "&content=" + WebUtility.UrlEncode(comment) + "&params=" + ytparams);
                        web3.ContentLength = req.Length;
                        web3.GetRequestStream().Write(req, 0, req.Length);
                        web3.GetResponse();
                        var web4 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/comment_service_ajax?action_get_comments=1");
                        web4.UserAgent = agent;
                        web4.Proxy = GetProxy();
                        web4.CookieContainer = web3.CookieContainer;
                        web4.Method = "POST";
                        web4.Headers["X-YouTube-Page-Label"] = build_label;
                        web4.Headers["X-YouTube-Page-CL"] = page_cl;
                        web4.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                        web4.Headers["X-YouTube-Client-Version"] = version;
                        web4.Headers["X-YouTube-Client-Name"] = "1";
                        web4.Referer = video;
                        web4.ContentType = "application/x-www-form-urlencoded";
                        req = Encoding.ASCII.GetBytes("session_token=" + WebUtility.UrlEncode(session_token) + "&page_token=" + page_token);
                        web4.ContentLength = req.Length;
                        web4.GetRequestStream().Write(req, 0, req.Length);
                        using (var readf = new StreamReader(web4.GetResponse().GetResponseStream()))
                        {
                            data = readf.ReadToEnd();
                            if (data.Contains(username) && data.Contains(surname))
                            {
                                var cid = data.Split(new[] { "data-cid=\\\"" }, StringSplitOptions.None);
                                for (var i = 1; i < cid.Length; i++)
                                {
                                    var lc = cid[i].Split('\\')[0];
                                    last_comment = video + "&lc=" + lc;
                                    var ret = IsCommentVisible(last_comment);
                                    if(ret) return true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }
        public string last_comment;
        private bool IsCommentVisible(string comment_url)
        {
            try
            {
                var v = comment_url.Split(new[] { "watch?v=" }, StringSplitOptions.None)[1].Split('&')[0];
                var lc = comment_url.Split(new[] { "&lc=" }, StringSplitOptions.None)[1];
                var web = (HttpWebRequest)WebRequest.Create(comment_url);
                var cc = new CookieContainer();
                cc.Add(new Cookie("test", "x", "/", "youtube.com"));
                web.CookieContainer = cc;
                web.UserAgent = Program.DEFAULT_AGENT;
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    return data.Contains(username) && data.Contains(surname);
                }
            }
            catch { }
            return false;
        }

        public LoginStatus Login()
        {
            try
            {
                if (agent.Length < 1 && Program.APP.agents.Count > 0)
                    agent = Program.APP.agents[Program.RAND.Next(Program.APP.agents.Count)];
                else if (agent.Length < 1)
                    agent = Program.DEFAULT_AGENT;
                var webx = (HttpWebRequest)WebRequest.Create("https://www.youtube.com");
                var cc = new CookieContainer();
                cc.Add(new Cookie("test", "x", "/", "youtube.com"));
                webx.CookieContainer = cc;
                webx.Proxy = GetProxy();
                webx.UserAgent = agent;
                webx.AllowAutoRedirect = true;
                webx.GetResponse();
                var web = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/ServiceLogin");
                web.Proxy = GetProxy();
                web.CookieContainer = webx.CookieContainer;
                web.UserAgent = agent;
                web.Headers["DNT"] = "1";
                web.AllowAutoRedirect = true;
                var resp = web.GetResponse();
                using (var read = new StreamReader(resp.GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    var req = Encoding.ASCII.GetBytes("Page=PasswordSeparationSignIn&ProfileInformation=&continue=" + WebUtility.UrlEncode("https://www.youtube.com/signin?next=%2Ffeed%2Fsubscriptions&feature=redirect_login&app=desktop&action_handle_signin=true&hl=en")
                        + "&service=youtube&hl=en-GB&_utf8=" + WebUtility.UrlEncode("&#9731;") + "&bgresponse=js_disabled&pstMsg=0&checkConnection=&checkedDomains=youtube"
                    + "&identifiertoken=&identifiertoken_audio=&identifier-captcha-input=&Email=" + WebUtility.UrlEncode(email) + "&Passwd=" + WebUtility.UrlEncode(password)
                    + "&PersistentCookie=yes&rmShown=1&GALX=" + WebUtility.UrlEncode(data.Split(new[] { "name=\"GALX\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0])
                    + "&gxf=" + WebUtility.UrlEncode(data.Split(new[] { "name=\"gxf\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0]));
                    var web2 = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/signin/challenge/sl/password");
                    web2.Proxy = GetProxy();
                    web2.UserAgent = agent;
                    web2.Headers["DNT"] = "1";
                    web2.CookieContainer = web.CookieContainer;
                    web2.Method = "POST";
                    web2.ContentType = "application/x-www-form-urlencoded";
                    web2.ContentLength = req.Length;
                    web2.AllowAutoRedirect = true;
                    web2.Referer = "https://accounts.google.com/ServiceLogin?service=youtube&continue=" + WebUtility.UrlEncode("https://www.youtube.com/signin?next=%2Ffeed%2Fsubscriptions&feature=redirect_login&app=desktop&action_handle_signin=true&hl=en");
                    web2.GetRequestStream().Write(req, 0, req.Length);
                    resp = web2.GetResponse();
                    using (var read2 = new StreamReader(web2.GetResponse().GetResponseStream()))
                    {
                        data = read2.ReadToEnd();
                        if (data.Contains("errormsg"))
                        {
                            status = "Invalid";
                            cookies = "";
                            return LoginStatus.Error;
                        }
                        else
                        {
                            if (data.Contains("signin/challenge/kp"))
                            {
                                if (VerifyProcess(web2.CookieContainer, data))
                                {
                                    status = "Validated";
                                    SaveData();
                                    return LoginStatus.Logged;
                                }
                                else
                                {
                                    status = "Invalid";
                                    cookies = "";
                                    return LoginStatus.Error;
                                }
                            }
                            else
                                cookies = Web.CookieString(web2.CookieContainer);
                            if (cookies.Contains("HSID"))
                            {
                                status = "Validated";
                                SaveData();
                                return LoginStatus.Logged;
                            }
                        }
                    }
                }
            }
            catch { }
            status = "Invalid";
            cookies = "";
            return LoginStatus.Error;
        }

        #region VERIFY RECOVER PHONE OR EMAIL OR CREATE CHANNEL
        private bool VerifyProcess(CookieContainer cc, string data)
        {
            try
            {
                var challenge = data.Contains("/signin/challenge/kpp/3") ? 3 : 2;
                var challengeType = challenge == 3 ? 13 : 12;
                if (data.Contains("name=\"phoneNumber\"") || data.Contains("name=\"email\""))
                    StepLogin(cc, data, challenge, challengeType);
                else
                {
                    var web = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/signin/challenge/kp"+(challenge==3?"p":"e")+"/"+challenge);
                    web.Proxy = GetProxy();
                    web.Method = "POST";
                    web.CookieContainer = cc;
                    web.UserAgent = agent;
                    web.Headers["DNT"] = "1";
                    web.ContentType = "application/x-www-form-urlencoded";
                    var req = Encoding.ASCII.GetBytes("challengeId="+challenge+"&challengeType="+challengeType+"&service=youtube&continue=" + WebUtility.UrlEncode("https://www.youtube.com/signin?next=%2Ffeed%2Fsubscriptions&feature=redirect_login&app=desktop&action_handle_signin=true&hl=en-GB")
                        + "&hl=en-GB&checkedDomains=youtube&pstMsg=0&TL="
                        + WebUtility.UrlEncode(data.Split(new[] { "name=\"TL\" type=\"hidden\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0])
                        + "&subAction=selectChallenge&gxf="
                        + WebUtility.UrlEncode(data.Split(new[] { "name=\"gxf\" id=\"gxf\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0]));
                    web.ContentLength = req.Length;
                    web.AllowAutoRedirect = true;
                    web.GetRequestStream().Write(req, 0, req.Length);
                    using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                        return StepLogin(web.CookieContainer, read.ReadToEnd(), challenge, challengeType);
                }
            }
            catch { }
            return false;
        }

        private bool StepLogin(CookieContainer cc, string data, int challenge, int challengeType)
        {
            try
            {
                var paramChallenge = challenge == 3 ? "&phoneNumber=" + WebUtility.UrlEncode(phone) : "&email=" + WebUtility.UrlEncode(email);
                var web2 = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/signin/challenge/kp"+(challenge==3?"p":"e")+"/"+challenge);
                web2.Proxy = GetProxy();
                web2.Method = "POST";
                web2.CookieContainer = cc;
                web2.UserAgent = agent;
                web2.Headers["DNT"] = "1";
                web2.ContentType = "application/x-www-form-urlencoded";
                var req = Encoding.ASCII.GetBytes("challengeId="+challenge+"&challengeType="+challengeType+"&service=youtube&continue=" + WebUtility.UrlEncode("https://www.youtube.com/signin?next=%2Ffeed%2Fsubscriptions&feature=redirect_login&app=desktop&action_handle_signin=true&hl=en-GB")
                    + "&hl=en-GB&checkedDomains=youtube&pstMsg=0&TL="
                    + WebUtility.UrlEncode(data.Split(new[] { "name=\"TL\" type=\"hidden\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0]) + paramChallenge 
                    + "&gxf=" + WebUtility.UrlEncode(data.Split(new[] { "name=\"gxf\" id=\"gxf\" value=\"" }, StringSplitOptions.None)[1].Split('"')[0]));
                web2.ContentLength = req.Length;
                web2.AllowAutoRedirect = true;
                web2.GetRequestStream().Write(req, 0, req.Length);
                using (var read2 = new StreamReader(web2.GetResponse().GetResponseStream()))
                {
                    data = read2.ReadToEnd();
                    if (data.Contains("/signin/privacyreminder/save"))
                        return AgreePrivacy(web2.CookieContainer, data);
                    else
                    {
                        cookies = Web.CookieString(web2.CookieContainer);
                        if (cookies.Contains("HSID"))
                            return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private bool AgreePrivacy(CookieContainer cc, string data) // DX
        {
            try
            {
                var cp_psd = WebUtility.UrlEncode(data.Split(new[] { "name=\"cb-psd\"" }, StringSplitOptions.None)[1].Split('"')[1]);
                var tl = WebUtility.UrlEncode(data.Split(new[] { "name=\"TL\"" }, StringSplitOptions.None)[1].Split('"')[1]);
                var lcb_pkl = data.Split(new[] { "name=\"cb-pkl\""},StringSplitOptions.None);
                var cb_pkl = lcb_pkl[lcb_pkl.Length - 2].Split('"')[1];
                var atu = data.Split(new[]{"name=\"atu\""},StringSplitOptions.None)[1].Split('"')[1];
                var web2 = (HttpWebRequest)WebRequest.Create("/signin/privacyreminder/save");
                web2.Proxy = GetProxy();
                web2.Method = "POST";
                web2.CookieContainer = cc;
                web2.UserAgent = agent;
                web2.ContentType = "application/x-www-form-urlencoded";
                var req = Encoding.ASCII.GetBytes("cp_psd=" + cp_psd + "&tl=" + tl + "&cb_pkl=" + cb_pkl + "&atu=" + atu + "&hl=en-GB&checkedDomains=youtube&pstMsg=1&service=youtube&continue=" + WebUtility.UrlEncode("https://www.youtube.com/signin?next=%2Ffeed%2Fsubscriptions&feature=redirect_login&app=desktop&action_handle_signin=true&hl=en-GB"));
                web2.ContentLength = req.Length;
                web2.AllowAutoRedirect = true;
                web2.GetRequestStream().Write(req, 0, req.Length);
                web2.GetResponse();
                cookies = Web.CookieString(web2.CookieContainer);
                if (cookies.Contains("HSID"))
                    return true;
            }
            catch { }
            return false;
        }

        private void CreateChannel()
        {
            try
            {
                var web = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/create_channel");
                web.CookieContainer = Web.StringCookie(cookies);
                web.UserAgent = agent;
                web.Proxy = GetProxy();
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                    if (data.Contains("This account already has a channel")) return;
                    var page_cl = data.Split(new[] { "'PAGE_CL': " }, StringSplitOptions.None)[1].Split(',')[0];
                    var session_token = data.Split(new[] { "'XSRF_TOKEN':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var variants_checksum = data.Split(new[] { "'VARIANTS_CHECKSUM':" }, StringSplitOptions.None)[1].Split('"')[1];
                    var web2 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/create_channel_ajax?feature=default&next=%2Fprofile");
                    web2.CookieContainer = web.CookieContainer;
                    web2.UserAgent = agent;
                    web2.Proxy = GetProxy();
                    web2.Method = "POST";
                    web2.ContentType = "application/x-www-form-urlencoded";
                    web2.Headers["X-YouTube-Page-CL"] = page_cl;
                    web2.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                    var req = Encoding.ASCII.GetBytes("session_token=" + session_token);
                    web2.ContentLength = req.Length;
                    web2.GetRequestStream().Write(req, 0, req.Length);
                    using (var read2 = new StreamReader(web2.GetResponse().GetResponseStream()))
                    {
                        data = read2.ReadToEnd();
                        var creation_token = data.Split(new[] { "data-channel-creation-token=\"" }, StringSplitOptions.None)[1].Split('"')[0];
                        var web3 = (HttpWebRequest)WebRequest.Create("https://www.youtube.com/create_channel_ajax?action_create_channel_google_name=1");
                        web3.CookieContainer = web.CookieContainer;
                        web3.UserAgent = agent;
                        web3.Proxy = GetProxy();
                        web3.Method = "POST";
                        web3.ContentType = "application/x-www-form-urlencoded";
                        web3.Headers["X-YouTube-Page-CL"] = page_cl;
                        web3.Headers["X-YouTube-Variants-Checksum"] = variants_checksum;
                        web3.Referer = "https://www.youtube.com/create_channel";
                        req = Encoding.ASCII.GetBytes("session_token=" + session_token + "&channel_creation_token=" + creation_token + "&screen=" 
                            + WebUtility.UrlEncode("h=1080&w=1920")+"&given_name="+username+"&family_name="+surname);
                        web3.ContentLength = req.Length;
                        web3.GetRequestStream().Write(req, 0, req.Length);
                        web3.GetResponse();
                    }
                }
            }
            catch { }
        }
#endregion

        public void UploadAvatar(string file)
        {
            try
            {
                return;
                var web = (HttpWebRequest)WebRequest.Create("https://aboutme.google.com/u/0/");
                web.CookieContainer = Web.StringCookie(cookies);
                web.UserAgent = agent;
                web.Proxy = GetProxy();
                using (var read = new StreamReader(web.GetResponse().GetResponseStream()))
                {
                    var data = read.ReadToEnd();
                }
            }
            catch { }
        }

        public void SaveData()
        {
            File.WriteAllLines("ids/"+id, new[] {
                username+","+surname+","+password+","+email+","+phone+","+(proxy!=null?String.Join(":",proxy):""),
                agent,
                cookies
            });
        }

        public enum LoginStatus //banned?
        {
            Logged, Banned, Error
        }

        public string ToString()
        {
            return username + "," + password + "," + email + "," + String.Join(":", proxy);
        }
    }
}
