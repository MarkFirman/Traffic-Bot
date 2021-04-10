using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Windows;

namespace Traffic_Bot.Class
{
    public class Scraper
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public Scraper()
        {
            // Initialise the scraper collection
            SCRAPED_COLLECTION = new ObservableCollection<ScraperResults>();
        }

        /// <summary>
        /// The URL to scrape
        /// </summary>
        public string ScrapeUrl = "https://free-proxy-list.net/";

        /// <summary>
        /// Holds a collection of scraped proxies
        /// </summary>
        public ObservableCollection<ScraperResults> SCRAPED_COLLECTION;
        
        /// <summary>
        /// Scrapes the URL to get all availble proxies and their ports
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Scrape()
        {
            // Run the scraper in a new task, as to not freeze the GUI. Scraping should be very quick if the website to scrap is up
            return await Task.Run(() =>
            {
                // Create a new HTTP WEB REQUEST
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.ScrapeUrl);

                // Create a HTTP RESPONSE holder
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                // Check that the status recieved was OK
                if (res.StatusCode == HttpStatusCode.OK)
                {

                    // Get the response stream
                    Stream recStream = res.GetResponseStream();

                    // Initialise a stream reader
                    StreamReader readStream = null;

                    // Handle encoding
                    if (String.IsNullOrWhiteSpace(res.CharacterSet))
                    {
                        readStream = new StreamReader(recStream);
                    }
                    else
                    {
                        readStream = new StreamReader(recStream, Encoding.GetEncoding(res.CharacterSet));
                    }

                    // Get the raw data from the stream as text
                    string data = readStream.ReadToEnd();

                    // Close the http response
                    res.Close();

                    // Close the stream
                    readStream.Close();

                    // Split away from the HTML table
                    string dataspliceone = data.Split(new string[] { "<tbody>" }, StringSplitOptions.None)[1]
                                               .Split(new string[] { "</tbody>" }, StringSplitOptions.None)[0];

                    // Get each table row
                    string[] datasplicetwo = dataspliceone.Split(new string[] { "<tr>" }, StringSplitOptions.None);

                    // Iterate over each row
                    foreach (string row in datasplicetwo)
                    {
                        // Do not process empty rows
                        if (row.Length <= 4) { continue; }

                        // Remove HTML tags
                        string linedata = row.Replace("<td>", "").Replace("</tr>", "").Replace("<td class='hm'>", "").Replace("<td class='hx'>", "").Replace("</td>", "|");

                        // Split into data items
                        string[] splitlinedata = linedata.Split(new string[] { "|" }, StringSplitOptions.None);

                        // Get the proxy, port and country
                        string proxy = splitlinedata[0];
                        string port = splitlinedata[1];
                        string country = splitlinedata[2];

                        // Check that the scraper does not already contain the same entry
                        if (!SCRAPED_COLLECTION.Any(i => i.IP == proxy))
                        {
                            // Add to the scraped collection
                            SCRAPED_COLLECTION.Add(new ScraperResults() { IP = proxy, Port = port, Country = country });
                        }

                    }

                    return true;

                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Validates the given @url
        /// Checks if the URL is reachable and is atleast a certain speed
        /// </summary>
        public void ValidateProxy(string url)
        {

        }

        /// <summary>
        /// The scraper results class
        /// Used to hold scraper results
        /// </summary>
        public class ScraperResults
        {
            public string IP { get; set; }
            public string Port { get; set; }
            public string Country { get; set; }
        }


    }
}
