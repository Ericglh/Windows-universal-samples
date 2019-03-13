//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SDKTemplate;
using Windows.Networking.NetworkOperators;

namespace NetworkInformationSample
{
    public sealed partial class S6WwanGetConnectionProfiles : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;
        private IReadOnlyList<string> deviceAccountId = null;

        public S6WwanGetConnectionProfiles()
        {
            this.InitializeComponent();
        }

        private void S6_ClearClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;
        }

        private void S6_RefreshSimpleClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                PrepareScenario();
                var startTime = DateTimeOffset.Now;
                var mobileBroadbandAccount = MobileBroadbandAccount.CreateFromNetworkAccountId(deviceAccountId[0]);

                var tasks = new List<Task<string>>();
                foreach (ConnectionProfile profile in mobileBroadbandAccount.GetConnectionProfiles())
                {
                    //tasks.Add(Task.Run(() =>
                    //{
                        string returnString = string.Empty;
                        returnString += "================================================================================\n\n";
                        returnString += "Profile Name: " + profile.ProfileName + "\n";
                        returnString += "Network Connectivity Level: " +
                                        NetworkInformationPrinting.WriteNetworkConnectivityLevel(profile) + "\n";
                        returnString += "Domain Connectivity Level: " +
                                        NetworkInformationPrinting.WriteDomainConnectivityLevel(profile) + "\n";
                        returnString += "Connection Cost: " +
                                        NetworkInformationPrinting.WriteConnectionCost(profile) + "\n";
                    OutputText.Text = returnString;
                        //return returnString;
                        
                    //}));
                }
                /*
                foreach (Task<string> task in tasks)
                {
                    task.Wait();
                    OutputText.Text += task.Result;
                }
                */
                var endTime = DateTimeOffset.Now;

                OutputText.Text += "Time taken: " + (endTime - startTime).TotalMilliseconds + " ms.\n";

                Debug.WriteLine(OutputText.Text);
                _rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void PrepareScenario()
        {
            _rootPage.NotifyUser("", NotifyType.StatusMessage);

            try
            {
                deviceAccountId = MobileBroadbandAccount.AvailableNetworkAccountIds;

            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser("Error:" + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void S6_RefreshDetailedClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;
                var mobileBroadbandAccount = MobileBroadbandAccount.CreateFromNetworkAccountId(deviceAccountId[0]);

                var tasks = new List<Task<string>>();
                foreach (ConnectionProfile profile in mobileBroadbandAccount.GetConnectionProfiles())
                {
                    tasks.Add(Task.Run(() => NetworkInformationPrinting.WriteConnectionProfiles(profile)));
                }
                foreach (Task<string> task in tasks)
                {
                    task.Wait();
                    OutputText.Text += task.Result;
                }

                var endTime = DateTimeOffset.Now;

                OutputText.Text += "Time taken: " + (endTime - startTime).TotalMilliseconds + " ms.\n";

                Debug.WriteLine(OutputText.Text);
                _rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}
