using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;

namespace Traffic_Bot.Class
{
    /// <summary>
    /// The proxies class
    /// </summary>
    public class Proxies
    {

        /// <summary>
        /// Holds a collection of all proxys
        /// </summary>
        public ObservableCollection<Proxy> PROXY_COLLECTION;

        /// <summary>
        /// Holds the total number of proxys selected
        /// </summary>
        public int TotalProxiesSelected = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public Proxies()
        {
            // Initilaise the proxy collection
            PROXY_COLLECTION = new ObservableCollection<Proxy>();
        }

        /// <summary>
        /// The proxy class holds individual proxy information
        /// </summary>
        public class Proxy
        {
            public bool Enabled { get; set; }
            public string Prox { get; set; }
            public string Port { get; set; }
            public string Country { get; set; }
        }

        /// <summary>
        /// Verifies if the supplied proxy and port can be connected to
        /// Returns true if the proxy was OK
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyProxy(Proxy _proxy, int timeout)
        {
            // Start the verification process in a new thread/task
            return await Task.Run(() =>
            {
                // Open a new ping request to see if we can reach the IP and port in question
                System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();

                try
                {
                    // Get the ping reply
                    System.Net.NetworkInformation.PingReply reply = pinger.Send(_proxy.Prox, timeout);

                   

                    // Check if the reply was valid
                    if(reply == null) { return false; }

                    // Return status as bool
                    return (reply.Status == System.Net.NetworkInformation.IPStatus.Success);

                } catch(Exception ex)
                {
                    return false;
                }

            });
        }

    }
}
