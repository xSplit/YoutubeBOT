using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YTDom
{
    public class Web
    {
        public static string CookieString(CookieContainer container)
        {
            string cookies = "";
            if (container != null)
            {
                foreach (Cookie cookie in container.GetCookies(new Uri("https://www.youtube.com")))
                    cookies += cookie.ToString() + ";";
                foreach (Cookie cookie in container.GetCookies(new Uri("https://accounts.google.com")))
                    cookies += cookie.ToString() + ";";
                foreach (Cookie cookie in container.GetCookies(new Uri("https://accounts.youtube.com")))
                    cookies += cookie.ToString() + ";";
            }
            return cookies;
        }

        public static CookieContainer StringCookie(string str)
        {
            CookieContainer cookies = new CookieContainer();
            if (str.Contains('='))
            {
                foreach (string cookie in str.Split(';'))
                {
                    if (cookie.Contains("="))
                    {
                        //System.Windows.Forms.MessageBox.Show(cookie.Split('=')[0] + " ~ " + String.Join("=", cookie.Split('=').Skip(1)));
                        cookies.Add(new Cookie(cookie.Split('=')[0], String.Join("=", cookie.Split('=').Skip(1)), "/", "youtube.com"));
                    }
                }
            }
            return cookies;
        }
    }
}
