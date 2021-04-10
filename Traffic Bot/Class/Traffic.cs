using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic_Bot.Class
{
    public class Traffic
    {

        /// <summary>
        /// The URL of the site to send traffic to
        /// </summary>
        public string URL;

        /// <summary>
        /// The total number of hits to send to the website
        /// </summary>
        public int HITS;

        /// <summary>
        /// The number of threads to use in the process
        /// </summary>
        public int THREADS;

        /// <summary>
        /// The total number of successfully hits that have been sent
        /// </summary>
        public int HITS_SENT;

        /// <summary>
        /// The total number of current threads in operation by TBOT
        /// </summary>
        public int THREADS_OPEN;

        /// <summary>
        /// The speed in Kbps/Mbps of the current hit
        /// </summary>
        public int SPEED_OF_CURRENT_HIT;

        /// <summary>
        /// The average speed across all successful hits
        /// </summary>
        public int AVERAGE_SPEED_PER_HIT;

        /// <summary>
        /// The response time for the current hit
        /// </summary>
        public int RESPONSE_TIME;

        /// <summary>
        /// The average response time across all successful hits
        /// </summary>
        public int AVERAGE_REPSONSE_TIME;

        /// <summary>
        /// Performs a new traffic request
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendTraffic()
        {
            return await Task.Run(() =>
            {
                return true;
            });
        }


    }
}
