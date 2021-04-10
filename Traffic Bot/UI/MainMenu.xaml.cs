using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Traffic_Bot.UI
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {

        /// <summary>
        /// The application class handle
        /// </summary>
        public Class.application APPLICATION;

        /// <summary>
        /// Constructor
        /// Called when the MainMenu page is intialised
        /// </summary>
        /// <param name="_application"></param>
        public MainMenu(Class.application _application)
        {
            InitializeComponent();

            // Initialise the application class
            APPLICATION = _application;

            // Set the snackbar queue
            MessageQueue.MessageQueue = APPLICATION.MESSAGE_QUEUE;

            // Loads the user agents
            LoadUserAgents();

            // Load the referrals
            LoadReferrals();

            // Load the proxies
            LoadProxies();

        }

        #region EVENTS

        /// ************************************************************************
        /// HOST
        /// ************************************************************************

        /// <summary>
        /// Event is fired when the exit application toolbar menu item is clicked
        /// Cleanly closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication_Click(object sender, EventArgs e)
        {
            // Shutsdown the current application
            Application.Current.Shutdown(0);
        }

        /// <summary>
        /// Event is fired when the Open Credits application toolbar menu item is clicked
        /// Opens the credits pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCredits_Click(object sender, EventArgs e)
        {
            // Open the credit host popup window
            CreditHost.IsOpen = true;
        }

        /// <summary>
        /// Event is fired when the Open Settings application toolbar menu item is clicked
        /// Opens the settings pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettings_Click(object sender, EventArgs e)
        {
            SettingsHost.IsOpen = true;
        }

        /// ************************************************************************
        /// SETUP
        /// ************************************************************************

        /// <summary>
        /// Event occurs when the WebsiteURL textbox text is changed
        /// Performs validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebsiteUrl_TextChanged(object sender, EventArgs e)
        {
            if (WebsiteUrlTextbox.Text != "")
            {

                URLValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Loading;
                URLValidator.Foreground = new BrushConverter().ConvertFromString("Yellow") as SolidColorBrush;

                // Check if the provided URL is of valid format
                if (await APPLICATION.IsValidURL(WebsiteUrlTextbox.Text))
                {
                    if (await APPLICATION.IsURLReachable(WebsiteUrlTextbox.Text))
                    {

                        // URL is validated - save URL to traffic class
                        APPLICATION.TRAFFIC.URL = WebsiteUrlTextbox.Text;


                        URLValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.TickCircle;
                        URLValidator.Foreground = new BrushConverter().ConvertFromString("Green") as SolidColorBrush;

                        // Check if the transitioner page 1 has been succesfully validated
                        ValidatePageOne();

                    } else
                    {
                        URLValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                        URLValidator.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
                    }
                }
                else
                {
                    URLValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                    URLValidator.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
                }
            } else
            {
                URLValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                URLValidator.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
            }
        }

        /// <summary>
        /// Event occurs when the Hits textbox text is changed
        /// Performs validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HitsTextbox_TextChanged(object sender, EventArgs e)
        {
            // Holds the number of attempts
            int result = 1000;

            if (HitsTextbox.Text.Length > 0)
            {
                // Check if the text can be converted to a valid integer
                if (int.TryParse(HitsTextbox.Text, out result) && result > 0)
                {
                    HitsValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.TickCircle;
                    HitsValidator.Foreground = new BrushConverter().ConvertFromString("Green") as SolidColorBrush;

                    // Check if the transitioner page 1 has been succesfully validated
                    ValidatePageOne();
                }
                else
                {
                    HitsValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                    HitsValidator.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
                }
            } else
            {
                HitsValidator.Kind = MaterialDesignThemes.Wpf.PackIconKind.Error;
                HitsValidator.Foreground = new BrushConverter().ConvertFromString("Red") as SolidColorBrush;
            }
        }

        /// ************************************************************************
        /// USER AGENTS
        /// ************************************************************************

        /// <summary>
        /// Event is fired when the user agent checkbox is clicked
        /// Handles enabling and disabling of user agents and updating the source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserAgentCheck(object sender, EventArgs e)
        {

            // Reset the total user agents
            APPLICATION.USERAGENTS.TotalUserAgentsSelected = 0;

            // Get the useragent value of the selected column
            string value = ((Class.UserAgents.Agents)UserAgentsDataGrid.Items[UserAgentsDataGrid.SelectedIndex]).Agent;

            // Get the user agents where the value matches the selected value
            foreach (Class.UserAgents.Agents AGENT in APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Where<Class.UserAgents.Agents>(i => i.Agent == value))
            {
                // Toggle the enabled value
                if (AGENT.Enabled)
                {
                    AGENT.Enabled = false;
                }
                else
                {
                    AGENT.Enabled = true;
                }
            }

            // Get each user agent that is enabled
            foreach (Class.UserAgents.Agents AGENT in APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Where<Class.UserAgents.Agents>(i => i.Enabled == true))
            {

                // Increment the total active user agents
                APPLICATION.USERAGENTS.TotalUserAgentsSelected++;
            }

            // Update the user agents label
            UserAgentCountLabel.Text = (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 1 ? APPLICATION.USERAGENTS.TotalUserAgentsSelected.ToString() + " User Agents Selected" : (APPLICATION.USERAGENTS.TotalUserAgentsSelected == 0 ? "No User Agents Selected" : "1 User Agent Selected"));

        }

        /// <summary>
        /// Event is fired when the reload user agents toolbar button is clicked
        /// Reloads the user agents from the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadUserAgents_Click(object sender, EventArgs e)
        {
            // Clear the current list of user agents
            APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Clear();

            // Reset the total selected user agents
            APPLICATION.USERAGENTS.TotalUserAgentsSelected = 0;

            // Reload the user agents
            LoadUserAgents();
        }

        /// <summary>
        /// Event is fired when the remove user agent toolbar button is clicked
        /// Removes the selected user agent from the datagrid, but also removes it from then @ Resources/useragents file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserAgent_Click(object sender, EventArgs e)
        {
            // Check that a valid item IS selected
            if (UserAgentsDataGrid.SelectedItem != null)
            {
                // Check that the useragents file can be found
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents"))
                {

                    // Get the selected datagrid item
                    Class.UserAgents.Agents value = ((Class.UserAgents.Agents)UserAgentsDataGrid.Items[UserAgentsDataGrid.SelectedIndex]);

                    // Remove the user agent from the user agent collection
                    APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Remove(value);

                    // Remove the user agent from the useragent file
                    // Create a temp file
                    if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp"))
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp").Close();
                    }

                    // Get all the lines we want to keep
                    var linesToKeep = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents").Where(i => i != value.Agent);

                    // Write the lines to keep to the temp file
                    File.WriteAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", linesToKeep);

                    // Delete the useragents file
                    File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents");

                    // Change the temp file to the useragent file
                    File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents");

                    // Update the number of useragents
                    APPLICATION.USERAGENTS.TotalUserAgentsSelected--;

                    // Update the user agents label
                    UserAgentCountLabel.Text = (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 1 ? APPLICATION.USERAGENTS.TotalUserAgentsSelected.ToString() + " User Agents Selected" : (APPLICATION.USERAGENTS.TotalUserAgentsSelected == 0 ? "No User Agents Selected" : "1 User Agent Selected"));

                    // Enable the user agent next button if atleast 1 user agent is selected
                    if (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 0)
                    {
                        UserAgentNextButton.IsEnabled = true;
                    }

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The user agent was removed");

                }
                else
                {
                    // Send the no user agent file could be found
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The user agent file could not be found @ /Resources/useragents");
                }
            }
            else
            {
                // Send the no user agent selected for deletion messsage to the snackbar queue
                APPLICATION.MESSAGE_QUEUE.Enqueue("No user agent was selected to be deleted");
            }
        }

        /// <summary>
        /// Event is fired when the add user agent toolbar button is clicked
        /// Opens the new user agent dialog window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUserAgent_Click(object sender, EventArgs e)
        {
            UserAgentHost.IsOpen = true;
        }

        /// <summary>
        /// Event is fired when the cancel user agent form button is pressed
        /// Closes the add new user agent form and resets values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelNewUserAgent_Click(object sender, EventArgs e)
        {
            // Close the user agent host
            UserAgentHost.IsOpen = false;

            // Reset the user agent text
            NewUserAgentTextbox.Text = "";
        }

        /// <summary>
        /// Event is fired when the Add new user agent button is pressed
        /// Validates the user agent and adds it into the user agent file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveNewUserAgent_Click(object sender, EventArgs e)
        {
            // Perform some basic agent checking validation
            if (NewUserAgentTextbox.Text != "")
            {
                // Create the agent
                Class.UserAgents.Agents agent = new Class.UserAgents.Agents() { Agent = NewUserAgentTextbox.Text, Enabled = true };

                // Add the user agent to the collection
                APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Add(agent);

                // Add the agent to the user agent file
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents"))
                {
                    // Open the user agent file
                    using (var sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents", append: true))
                    {
                        // Write the new agent to the file
                        sw.WriteLine(agent.Agent);
                    }

                    // Update the number of in-use user agents
                    APPLICATION.USERAGENTS.TotalUserAgentsSelected++;

                    // Reload the user agent label
                    UserAgentCountLabel.Text = (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 1 ? APPLICATION.USERAGENTS.TotalUserAgentsSelected.ToString() + " User Agents Selected" : (APPLICATION.USERAGENTS.TotalUserAgentsSelected == 0 ? "No User Agents Selected" : "1 User Agent Selected"));

                    // Close the user agent host
                    UserAgentHost.IsOpen = false;

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The user agent was added successfully!");

                    // Enable the user agent next button if atleast 1 user agent is selected
                    if (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 0)
                    {
                        UserAgentNextButton.IsEnabled = true;
                    }
                }
                else
                {
                    // Show the error message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The user agent file could not be found, no user agent was added!");
                }
            }
        }

        /// <summary>
        /// Event is fired when the selected item within the UserAgentDataGrid is changed
        /// Check if a valid selection has been made
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAgentsDataGrid_SelectedCellsChanged(object sender, EventArgs e)
        {
            // Check if a valid item is selected 
            if (UserAgentsDataGrid.SelectedItem != null)
            {
                // Enable the remove button
                RemoveUserAgentButton.IsEnabled = true;

            }
            else
            {
                // Disable the remove button
                RemoveUserAgentButton.IsEnabled = false;
            }
        }

        /// ************************************************************************
        /// REFERRALS
        /// ************************************************************************

        /// <summary>
        /// Event is fired when the referal checkbox item is clicked
        /// Enables/Disables Referrals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReferralCheck(object sender, EventArgs e)
        {
            // Reset the total user agents
            APPLICATION.REFERRALS.TotalReferersSelected = 0;

            // Get the useragent value of the selected column
            string value = ((Class.Referrals.Referrer)ReferrerDataGrid.Items[ReferrerDataGrid.SelectedIndex]).Refferer;

            // Get the user agents where the value matches the selected value
            foreach (Class.Referrals.Referrer REF in APPLICATION.REFERRALS.REFERRALS_COLLECTION.Where<Class.Referrals.Referrer>(i => i.Refferer == value))
            {
                // Toggle the enabled value
                if (REF.Enabled)
                {
                    REF.Enabled = false;
                }
                else
                {
                    REF.Enabled = true;
                }
            }

            // Get each refferal that is enabled
            foreach (Class.Referrals.Referrer REF in APPLICATION.REFERRALS.REFERRALS_COLLECTION.Where<Class.Referrals.Referrer>(i => i.Enabled == true))
            {
                // Increment the total active referrals
                APPLICATION.REFERRALS.TotalReferersSelected++;
            }

            // Update the user agents label
            ReferrerCountLabel.Text = (APPLICATION.REFERRALS.TotalReferersSelected > 1 ? APPLICATION.REFERRALS.TotalReferersSelected.ToString() + " Referrals Selected" : (APPLICATION.REFERRALS.TotalReferersSelected == 0 ? "No Referrals Selected" : "1 Referral Selected"));

        }

        /// <summary>
        /// Event is fired when the new refferal toolbar button is clicked
        /// Opens the new refferral host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewReffererHost_Click(object sender, EventArgs e)
        {
            // Open the new referral host
            NewReferralHost.IsOpen = true;
        }

        /// <summary>
        /// Event is fired when the remove referral toolbar button is pressed
        /// Removes the selected referral from the datagrid, collection and referrals file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveReferral_Click(object sender, EventArgs e)
        {
            if (ReferrerDataGrid.SelectedItem != null)
            {
                // Check that the referral file can be found
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals"))
                {

                    // Get the selected datagrid item
                    Class.Referrals.Referrer value = ((Class.Referrals.Referrer)ReferrerDataGrid.Items[ReferrerDataGrid.SelectedIndex]);

                    // Remove the referral from the user referral collection
                    APPLICATION.REFERRALS.REFERRALS_COLLECTION.Remove(value);

                    // Remove the user agent from the useragent file
                    // Create a temp file
                    if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp"))
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp").Close();
                    }

                    // Get all the lines we want to keep
                    var linesToKeep = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals").Where(i => i != value.Refferer);

                    // Write the lines to keep to the temp file
                    File.WriteAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", linesToKeep);

                    // Delete the referral file
                    File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals");

                    // Change the temp file to the referral file
                    File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals");

                    // Update the number of referrals
                    APPLICATION.REFERRALS.TotalReferersSelected--;

                    // Update the referrals label
                    ReferrerCountLabel.Text = (APPLICATION.REFERRALS.TotalReferersSelected > 1 ? APPLICATION.REFERRALS.TotalReferersSelected.ToString() + " Referrals Selected" : (APPLICATION.REFERRALS.TotalReferersSelected == 0 ? "No Referrals Selected" : "1 Referral Selected"));

                    // Enable the referral next button if atleast 1 referral is selected
                    if (APPLICATION.REFERRALS.TotalReferersSelected > 0)
                    {
                        ReferralNextButton.IsEnabled = true;
                    }

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The referral '" + value.Refferer + "' was successfully removed");

                }
                else
                {
                    // Send the no user agent file could be found
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The referral file could not be found @ /Resources/referral");
                }
            }
            else
            {
                // Send the no user agent selected for deletion messsage to the snackbar queue
                APPLICATION.MESSAGE_QUEUE.Enqueue("No referral was selected to be deleted");
            }

        }

        /// <summary>
        /// Event is fired when the selected cells inside the ReferralsDataGrid is changed
        /// Checks if a valid selection is made and actions accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferralsDataGrid_SelectedCellsChanged(object sender, EventArgs e)
        {
            // Check if a valid item is selected 
            if (ReferrerDataGrid.SelectedItem != null)
            {
                // Enable the remove button
                RemoveReferrerButton.IsEnabled = true;

            }
            else
            {
                // Disable the remove button
                RemoveReferrerButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Event is fired when the reload referrals button is clicked
        /// Reloads the referrals from file into the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadReferrals_Click(Object sender, EventArgs e)
        {
            // Clear the current list of referrals
            APPLICATION.REFERRALS.REFERRALS_COLLECTION.Clear();

            // Reset the total selected referrals
            APPLICATION.REFERRALS.TotalReferersSelected = 0;

            // Reload the referrals
            LoadReferrals();
        }

        /// <summary>
        /// Event is fired when the New Referral host cancel button is clicked
        /// Closes the add new referral host form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelReferralHost_Click(object sender, EventArgs e)
        {
            // Clear out the form
            NewReferralTextbox.Text = "";

            // Close the host
            NewReferralHost.IsOpen = false;
        }

        /// <summary>
        /// Event is fired when the New Referral host save button is clicked
        /// Saves the new referral to file and loads it into the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveReferralHost_Click(object sender, EventArgs e)
        {
            // Perform some basic agent checking validation
            if (NewReferralTextbox.Text != "")
            {
                // Create the referral
                Class.Referrals.Referrer REF = new Class.Referrals.Referrer() { Refferer = NewReferralTextbox.Text, Enabled = true };

                // Add the referral to the collection
                APPLICATION.REFERRALS.REFERRALS_COLLECTION.Add(REF);

                // Add the referral to the referral file
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals"))
                {
                    // Open the referral file
                    using (var sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals", append: true))
                    {
                        // Write the new referral to the file
                        sw.WriteLine(REF.Refferer);
                    }

                    // Update the number of in-use referral
                    APPLICATION.REFERRALS.TotalReferersSelected++;

                    // Reload the referral label
                    ReferrerCountLabel.Text = (APPLICATION.REFERRALS.TotalReferersSelected > 1 ? APPLICATION.REFERRALS.TotalReferersSelected.ToString() + " Referrals Selected" : (APPLICATION.REFERRALS.TotalReferersSelected == 0 ? "No Referrals Selected" : "1 Referral Selected"));

                    // Close the referral host
                    NewReferralHost.IsOpen = false;

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The referral '" + REF.Refferer + "' was added successfully!");

                    // Enable the referral next button if atleast 1 referral is selected
                    if (APPLICATION.REFERRALS.TotalReferersSelected > 0)
                    {
                        ReferralNextButton.IsEnabled = true;
                    }
                }
                else
                {
                    // Show the error message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The referral file could not be found, no ureferral was added!");
                }
            }
        }

        /// ************************************************************************
        /// PROXIES
        /// ************************************************************************

        /// <summary>
        /// Event is fired when the proxy checkbox is clicked
        /// Toggles whether the selected proxy is enable or disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProxyCheck(object sender, EventArgs e)
        {
            // Reset the total user agents
            APPLICATION.PROXIES.TotalProxiesSelected = 0;

            // Get the Proxy and Port values of the selected column
            string value = ((Class.Proxies.Proxy)ProxiesDataGrid.Items[ProxiesDataGrid.SelectedIndex]).Prox;
            string port = ((Class.Proxies.Proxy)ProxiesDataGrid.Items[ProxiesDataGrid.SelectedIndex]).Port;

            // Get the proxy where the value matches the selected value
            foreach (Class.Proxies.Proxy PROX in APPLICATION.PROXIES.PROXY_COLLECTION.Where<Class.Proxies.Proxy>(i => i.Prox == value))
            {
                // Toggle the enabled value
                if (PROX.Enabled)
                {
                    PROX.Enabled = false;
                }
                else
                {
                    PROX.Enabled = true;
                }
            }

            // Get each proxy that is enabled
            foreach (Class.Proxies.Proxy PROX in APPLICATION.PROXIES.PROXY_COLLECTION.Where<Class.Proxies.Proxy>(i => i.Enabled == true))
            {
                // Increment the total active referrals
                APPLICATION.PROXIES.TotalProxiesSelected++;
            }

            // Update the proxy count label
            ProxyCountLabel.Text = (APPLICATION.PROXIES.TotalProxiesSelected > 1 ? APPLICATION.PROXIES.TotalProxiesSelected.ToString() + " Proxies Selected" : (APPLICATION.PROXIES.TotalProxiesSelected == 0 ? "No Proxies Selected" : "1 Proxy Selected"));

        }

        /// <summary>
        /// Event is fired when the new proxy toolbar button is clicked
        /// Opens the new proxy host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProxyHost_Click(object sender, EventArgs e)
        {
            // Open the new proxy host
            NewProxyHost.IsOpen = true;
        }

        /// <summary>
        /// Event is fired when the remove proxy toolbar button is clicked
        /// Removes the selected proxy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveProxyButton_Click(object sender, EventArgs e)
        {
            if (ProxiesDataGrid.SelectedItem != null)
            {
                // Check that the useragents file can be found
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies"))
                {

                    // Get the selected datagrid item
                    Class.Proxies.Proxy value = ((Class.Proxies.Proxy)ProxiesDataGrid.Items[ProxiesDataGrid.SelectedIndex]);

                    // Remove the user agent from the user agent collection
                    APPLICATION.PROXIES.PROXY_COLLECTION.Remove(value);

                    // Remove the proxy from the proxies file
                    // Create a temp file
                    if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp"))
                    {
                        File.Create(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp").Close();
                    }

                    // Get all the lines we want to keep
                    var linesToKeep = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies").Where(i => i != value.Prox);

                    // Write the lines to keep to the temp file
                    File.WriteAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", linesToKeep);

                    // Delete the proxies file
                    File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies");

                    // Change the temp file to the proxies file
                    File.Move(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\temp", System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies");

                    // Update the number of proxies
                    APPLICATION.PROXIES.TotalProxiesSelected--;

                    // Update the proxies label
                    ProxyCountLabel.Text = (APPLICATION.PROXIES.TotalProxiesSelected > 1 ? APPLICATION.PROXIES.TotalProxiesSelected.ToString() + " Proxies Selected" : (APPLICATION.PROXIES.TotalProxiesSelected == 0 ? "No Proxies Selected" : "1 Proxy Selected"));

                    // Enable the user agent next button if atleast 1 proxy is selected
                    if (APPLICATION.PROXIES.TotalProxiesSelected > 0)
                    {
                        ProxyNextButton.IsEnabled = true;
                    }

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The proxy '" + value.Prox + "' was successfully removed");
                }
            }
        }

        /// <summary>
        /// Event is fired when the reload proxies toolbar button is clicked
        /// Reloads the proxies from file and repopulates the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadProxiesButton_Click(object sender, EventArgs e)
        {
            // Clear the current list of proxies
            APPLICATION.PROXIES.PROXY_COLLECTION.Clear();

            // Reset the total selected proxies
            APPLICATION.PROXIES.TotalProxiesSelected = 0;

            // Reload the proxies
            LoadProxies();
        }

        /// <summary>
        /// Event is fired when the scan for proxies toolbar button is clicked
        /// Scans the internet to find available and open proxies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScanProxiesButton_Click(object sender, EventArgs e)
        {
            // Scrape the proxies
            if (await APPLICATION.SCRAPER.Scrape())
            {
                // Show the scraper success message with the number of proxies found
                APPLICATION.MESSAGE_QUEUE.Enqueue("The proxy scraper found " + APPLICATION.SCRAPER.SCRAPED_COLLECTION.Count.ToString() + " proxies from " + APPLICATION.SCRAPER.ScrapeUrl);

            } else
            {
                // Show the scraper error message with the number of proxies found
                APPLICATION.MESSAGE_QUEUE.Enqueue("An error occured whilst scraping for proxies, please try again later...");
            }

            // Add the scraped proxies to the datagrid
            foreach (Class.Scraper.ScraperResults SCRES in APPLICATION.SCRAPER.SCRAPED_COLLECTION)
            {
                // Prevent duplicate proxies from being added
                if (!APPLICATION.PROXIES.PROXY_COLLECTION.Any(i => i.Prox == SCRES.IP))
                {
                    // Add the scraped proxy to the proxy collection
                    APPLICATION.PROXIES.PROXY_COLLECTION.Add(new Class.Proxies.Proxy() { Enabled = true, Prox = SCRES.IP, Port = SCRES.Port, Country = SCRES.Country });

                    // Increment the number of entries within the proxy collection
                    APPLICATION.PROXIES.TotalProxiesSelected++;
                }

            }

            // Update the proxy count label
            ProxyCountLabel.Text = (APPLICATION.PROXIES.TotalProxiesSelected > 1 ? APPLICATION.PROXIES.TotalProxiesSelected.ToString() + " Proxies Selected" : (APPLICATION.PROXIES.TotalProxiesSelected == 0 ? "No Proxies Selected" : "1 Proxy Selected"));
        }

        /// <summary>
        /// Event is fired when the verify proxies toolbar button is clicked
        /// Verifys the list of proxies to check if they are reachable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VerifyProxiesButton_Click(object sender, EventArgs e)
        {
            // Open the verify proxy host
            VerifyProxyHost.IsOpen = true;

            // Set the correct host label and progress information
            VerifyProxyLabel.Text = "Verifying Proxy 1 of " + APPLICATION.PROXIES.PROXY_COLLECTION.Count.ToString();

            // Holds the proxy good/bad count information
            int NumberOfBadProxies = 0;
            int NumberOfGoodProxies = 0;

            // Sets the current proxy count
            int counter = 1;

            // The total number or proxies to test
            int TotalProxiesTesting = APPLICATION.PROXIES.PROXY_COLLECTION.Count;

            // Verify each proxy
            foreach (Class.Proxies.Proxy PROX in APPLICATION.PROXIES.PROXY_COLLECTION.ToList())
            {
                // Update the label
                VerifyProxyLabel.Text = "Verifying Proxy " + counter.ToString() + " of " + TotalProxiesTesting.ToString();

                if (await APPLICATION.PROXIES.VerifyProxy(PROX, 1000))
                {
                    // Increment the number of good proxies
                    NumberOfGoodProxies++;

                } else
                {
                    // Remove the proxy from the collection as it could not be verified
                    APPLICATION.PROXIES.PROXY_COLLECTION.Remove(PROX);

                    // Increment the number of bad proxies
                    NumberOfBadProxies++;
                }

                // Increment the counter
                counter++;
            }

            // Close the Verify Proxy Host
            VerifyProxyHost.IsOpen = false;

            // Show report
            APPLICATION.MESSAGE_QUEUE.Enqueue(NumberOfBadProxies.ToString() + " Proxies were removed as a connection could not be established!");
            APPLICATION.MESSAGE_QUEUE.Enqueue(NumberOfGoodProxies.ToString() + " Proxies verified to be working correctly!");
        }

        /// <summary>
        /// Event is fired when the proxy datagrids selected cell is changed
        /// Handles whether a valid item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProxySelectedCell_Changed(object sender, EventArgs e)
        {
            if (ProxiesDataGrid.SelectedItem != null)
            {
                RemoveProxyButton.IsEnabled = true;
            } else
            {
                RemoveProxyButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Event is fired when the new proxy host cancel button is clicked
        /// Cancels and closes the new proxy host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProxyHostCancel_Click(object sender, EventArgs e)
        {
            NewProxyHost.IsOpen = false;
        }

        /// <summary>
        /// Event is fired when the new proxy host save button is clicked
        /// Validates and saves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProxyHostSave_Click(object sender, EventArgs e)
        {
            // Perform some basic proxy checking validation
            if (NewProxyTextbox.Text != "" && NewProxyPortTextbox.Text != "")
            {
                // Create the proxy
                Class.Proxies.Proxy REF = new Class.Proxies.Proxy() { Prox = NewProxyTextbox.Text, Port = NewProxyPortTextbox.Text, Enabled = true };

                // Add the proxy to the collection
                APPLICATION.PROXIES.PROXY_COLLECTION.Add(REF);

                // Add the proxy to the proxy file
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies"))
                {
                    // Open the proxy file
                    using (var sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies", append: true))
                    {
                        // Write the new referral to the file
                        sw.WriteLine(REF.Prox);
                    }

                    // Update the number of in-use proxies
                    APPLICATION.PROXIES.TotalProxiesSelected++;

                    // Reload the proxy label
                    ProxyCountLabel.Text = (APPLICATION.PROXIES.TotalProxiesSelected > 1 ? APPLICATION.PROXIES.TotalProxiesSelected.ToString() + " Proxies Selected" : (APPLICATION.PROXIES.TotalProxiesSelected == 0 ? "No Proxies Selected" : "1 Proxy Selected"));

                    // Close the proxy host
                    NewProxyHost.IsOpen = false;

                    // Show the success message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The proxy '" + REF.Prox + "' was added successfully!");

                    // Enable the proxy next button if atleast 1 proxy is selected
                    if (APPLICATION.PROXIES.TotalProxiesSelected > 0)
                    {
                        ProxyNextButton.IsEnabled = true;
                    }
                }
                else
                {
                    // Show the error message
                    APPLICATION.MESSAGE_QUEUE.Enqueue("The proxy file could not be found, no proxy was added!");
                }
            }
        }

        /// ************************************************************************
        /// TBOT - THE ACTUAL TRAFFIC MAKER
        /// ************************************************************************
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrafficBotStartButton_Click(object sender, EventArgs e)
        {
            /// Start the traffic bot session
            // Write message to the message queue
            WriteToConsoleBlock("Initialising new traffic session to {URL}");

            /// Setup the GUI
            // Set the maximum traffic progress bar value (this should be the total number of hits requested)
            TrafficProgress.Maximum = 100;



            WriteToConsoleBlock("{NumberOfProxies} proxies selected");
            WriteToConsoleBlock("{NumberOfReferrals} referrals selected");
            WriteToConsoleBlock("{NumberOfUserAgents} user agents selected");
            WriteToConsoleBlock("{NumberOfThreads} proxies selected");


            PreviewBrowser.Navigate(APPLICATION.TRAFFIC.URL);
        


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrafficBotStopButton_Click(object sender, EventArgs e)
        {

        }




            


        #endregion

        #region METHODS

        /// <summary>
        /// Validates the transitioner page 1 to check if the form was completed OK
        /// If the form was completed OK, then we enable the next button
        /// </summary>
        /// <returns></returns>
        private bool ValidatePageOne()
        {
            if (HitsValidator.Kind == MaterialDesignThemes.Wpf.PackIconKind.TickCircle && URLValidator.Kind == MaterialDesignThemes.Wpf.PackIconKind.TickCircle)
            {
                PageOneNextButton.IsEnabled = true;
                return true;
            }
            PageOneNextButton.IsEnabled = false;
            return false;
        }

        /// <summary>
        /// Loads the default user agents from the useragents file located within the resources folder into the user agent data grid
        /// </summary>
        private void LoadUserAgents()
        {
            // Check if the useragents file could be found
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents"))
            {
                // Open a new stream to the user agents file
                using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\useragents"))
                {
                    // Holds the line data
                    string line;

                    // Read through each line within the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Dont read empty lines
                        if (line != "")
                        {
                            // Create the user agent
                            Class.UserAgents.Agents agent = new Class.UserAgents.Agents() { Enabled = true, Agent = line };

                            // Add the user agent to the collection
                            APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Add(agent);

                            // Set the User Agent data grid souce to pull from the user agent collecction
                            UserAgentsDataGrid.ItemsSource = APPLICATION.USERAGENTS.USER_AGENT_COLLECTION;
                        }
                    }
                }
            }

            // Get each user agent that is enabled
            foreach (Class.UserAgents.Agents AGENT in APPLICATION.USERAGENTS.USER_AGENT_COLLECTION.Where<Class.UserAgents.Agents>(i => i.Enabled == true))
            {
                // Increment the total active user agents
                APPLICATION.USERAGENTS.TotalUserAgentsSelected++;
            }

            // Update the user agents label
            UserAgentCountLabel.Text = (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 1 ? APPLICATION.USERAGENTS.TotalUserAgentsSelected.ToString() + " User Agents Selected" : (APPLICATION.USERAGENTS.TotalUserAgentsSelected == 0 ? "No User Agents Selected" : "1 User Agent Selected"));

            // Enable the user agent next button if atleast 1 user agent is selected
            if (APPLICATION.USERAGENTS.TotalUserAgentsSelected > 0)
            {
                // Enable the user agent next button
                UserAgentNextButton.IsEnabled = true;
            } else
            {
                // Disable the user agent next button
                UserAgentNextButton.IsEnabled = false;
            }

        }

        /// <summary>
        /// Loads the default referrals from the referrals file located within the resources folder into the referrals data grid
        /// </summary>
        private void LoadReferrals()
        {
            // Check if the referrer file could be found
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals"))
            {
                // Open a new stream to the referrer file
                using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\referrals"))
                {
                    // Holds the line data
                    string line;

                    // Read through each line within the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Dont read empty lines
                        if (line != "")
                        {
                            // Create the user agent
                            Class.Referrals.Referrer referrer = new Class.Referrals.Referrer() { Enabled = true, Refferer = line };

                            // Add the referrer to the collection
                            APPLICATION.REFERRALS.REFERRALS_COLLECTION.Add(referrer);

                            // Set the referrer data grid souce to pull from the referrer collecction
                            ReferrerDataGrid.ItemsSource = APPLICATION.REFERRALS.REFERRALS_COLLECTION;
                        }
                    }
                }
            }

            // Get each referrer  that is enabled
            foreach (Class.Referrals.Referrer REFERRER in APPLICATION.REFERRALS.REFERRALS_COLLECTION.Where<Class.Referrals.Referrer>(i => i.Enabled == true))
            {
                // Increment the total active user agents
                APPLICATION.REFERRALS.TotalReferersSelected++;
            }

            // Update the user agents label
            ReferrerCountLabel.Text = (APPLICATION.REFERRALS.TotalReferersSelected > 1 ? APPLICATION.REFERRALS.TotalReferersSelected.ToString() + " Referrals Selected" : (APPLICATION.USERAGENTS.TotalUserAgentsSelected == 0 ? "No Referrals Selected" : "1 Referral Selected"));

            // Enable the user agent next button if atleast 1 user agent is selected
            if (APPLICATION.REFERRALS.TotalReferersSelected > 0)
            {
                // Enable the referral next button
                ReferralNextButton.IsEnabled = true;
            } else
            {
                // Disable the referral next button
                ReferralNextButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Loads the default proxies from the proxies file located within the resources folder into the proxies data grid
        /// </summary>
        private void LoadProxies()
        {
            // Check if the proxy file could be found
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies"))
            {
                // Open a new stream to the proxies file
                using (StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"\Resources\proxies"))
                {
                    // Holds the line data
                    string line;

                    // Set the referrer data grid souce to pull from the referrer collecction
                    ProxiesDataGrid.ItemsSource = APPLICATION.PROXIES.PROXY_COLLECTION;

                    // Read through each line within the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Dont read empty lines
                        if (line != "")
                        {

                            string proxy = "";
                            string port = "";

                            // Extract the data
                            if (line.Contains(":")) { proxy = line.Split(':')[0]; port = line.Split(':')[1]; }
                            else
                            {
                                proxy = line; port = "0";
                            }

                            // Create the proxy
                            Class.Proxies.Proxy prox = new Class.Proxies.Proxy() { Enabled = true, Prox = proxy, Port = port };

                            // Add the proxy to the collection
                            APPLICATION.PROXIES.PROXY_COLLECTION.Add(prox);

                        }
                    }
                }
            }

            // Get each proxy that is enabled
            foreach (Class.Proxies.Proxy PROX in APPLICATION.PROXIES.PROXY_COLLECTION.Where<Class.Proxies.Proxy>(i => i.Enabled == true))
            {
                // Increment the total active proxies
                APPLICATION.PROXIES.TotalProxiesSelected++;
            }

            // Update the proxies label
            ProxyCountLabel.Text = (APPLICATION.PROXIES.TotalProxiesSelected > 1 ? APPLICATION.PROXIES.TotalProxiesSelected.ToString() + " Proxies Selected" : (APPLICATION.PROXIES.TotalProxiesSelected == 0 ? "No Proxies Selected" : "1 Proxy Selected"));

            // Enable the proxy next button if atleast 1 proxy is selected
            if (APPLICATION.PROXIES.TotalProxiesSelected > 0)
            {
                // Enable the proxy next button
                ProxyNextButton.IsEnabled = true;
            } else
            {
                // Disable the proxy next button
                ProxyNextButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Writes text to the console block
        /// </summary>
        /// <param name="text"></param>
        private void WriteToConsoleBlock(string text)
        {
            // Write the text to the console block
            ConsoleBlock.Text += DateTime.Now.TimeOfDay.ToString() + " => " + text + Environment.NewLine;

            // Ensure the console block always scrolls to the most recent text
           
        }

        #endregion


    }
}
