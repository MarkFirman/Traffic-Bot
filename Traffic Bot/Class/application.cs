using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace Traffic_Bot.Class
{
    /// <summary>
    /// 
    /// </summary>
    public class application
    {

        /// <summary>
        /// The host form handle
        /// </summary>
        public UI.Host HOST;

        /// <summary>
        /// The user agents class handle
        /// </summary>
        public Class.UserAgents USERAGENTS;

        /// <summary>
        /// The referrals class handle
        /// </summary>
        public Class.Referrals REFERRALS;

        /// <summary>
        /// The proxy class handle
        /// </summary>
        public Class.Proxies PROXIES;

        /// <summary>
        /// The scraper class handle
        /// </summary>
        public Class.Scraper SCRAPER;

        /// <summary>
        /// The traffic class handle
        /// </summary>
        public Class.Traffic TRAFFIC;

        /// <summary>
        /// The snackbar message queue
        /// </summary>
        public MaterialDesignThemes.Wpf.SnackbarMessageQueue MESSAGE_QUEUE;

        /// <summary>
        /// Constructor
        /// Called when the application class is initialsied
        /// </summary>
        public application(UI.Host _host)
        {
            // Initialise the host form
            HOST = _host;

            // Initialise the user agents
            USERAGENTS = new UserAgents();

            // Initialise the referrals
            REFERRALS = new Referrals();

            // Initialse the proxies
            PROXIES = new Proxies();

            // Initialise the scraper
            SCRAPER = new Scraper();

            // Initialise the traffic class
            TRAFFIC = new Traffic();

            // Intialise the message queue
            MESSAGE_QUEUE = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();
        }


        /// ==============================================================================================================================


        /// <summary>
        /// Checks if the supplied URL is valid
        /// Returns true if the URL provided is of correct format
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Task<bool> IsValidURL(string URL)
        {
            // Run this method in a new task, so that the GUI does not freeze
            return Task.Run(() =>
            {
                string Pattern = @"(http|https|ftp|)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?([a-zA-Z0-9\-\?\,\'\/\+&%\$#_]+)";
                Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return Rgx.IsMatch(URL);
            });
        }

        /// <summary>
        /// Checks if the internet is available on the local machine
        /// Attempts a new connection to Google.com (Lets face it, google is never down)
        /// Returns true if an internet connection is available
        /// NOTE: This can be spoofed in the users HOSTS file (But dont tell them)
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public bool IsInternetAvailable()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://google.com/generate_204"))
                    {
                        return true;
                    }
                }

            } catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given URL is reachable (is it online)
        /// Returns true if a connection to the URL could be established
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsURLReachable(string URL)
        {
            // Run this method in a new task, so that the GUI does not freeze
            return Task.Run(() =>
            {

                try
                {
                    // Create the web request to the URL
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

                    // Set the timeout to 10 seconds
                    request.Timeout = 10000;

                    // Set the request method to Head so that we do not actually download content
                    request.Method = "HEAD";

                    // Attempt to connect to the given URL
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        return true;
                    }
                }
                catch (WebException ex)
                {
                    return false;
                }

            });
        }

    }
}
