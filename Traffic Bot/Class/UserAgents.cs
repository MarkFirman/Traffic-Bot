using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Traffic_Bot.Class
{
    /// <summary>
    /// The User Agents class
    /// </summary>
    public class UserAgents
    {

        /// <summary>
        /// Holds a collection of individual user agents
        /// </summary>
        public ObservableCollection<Agents> USER_AGENT_COLLECTION;

        /// <summary>
        /// Holds the total number of user agents selected
        /// </summary>
        public int TotalUserAgentsSelected = 0;

        /// <summary>
        /// Constructor
        /// Called when the user agents class is initialised
        /// </summary>
        public UserAgents()
        {
            // Initaise the user agent collection
            USER_AGENT_COLLECTION = new ObservableCollection<Agents>();
        }

        /// <summary>
        /// Class holds the individual user agents
        /// </summary>
        public class Agents
        {
            public string Agent { get; set; }
            public bool Enabled { get; set; }
        }

    }
}
