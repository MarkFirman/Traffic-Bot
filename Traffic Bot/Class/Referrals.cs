using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Traffic_Bot.Class
{
    /// <summary>
    /// The referrals class
    /// </summary>
    public class Referrals
    {

        /// <summary>
        /// Holds a collection of individual referrers
        /// </summary>
        public ObservableCollection<Referrer> REFERRALS_COLLECTION;

        /// <summary>
        /// Holds the total number of user agents selected
        /// </summary>
        public int TotalReferersSelected = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public Referrals()
        {
            // Initialise the referrals collection
            REFERRALS_COLLECTION = new ObservableCollection<Referrer>();
        }

        /// <summary>
        /// The refferer class
        /// </summary>
        public class Referrer
        {
            public bool Enabled { get; set; }
            public string Refferer { get; set; }
        }

    }
}
