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

namespace NetworkInformationSample
{
    public sealed partial class S1ListConnectionProfiles : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;

        public S1ListConnectionProfiles()
        {
            this.InitializeComponent();
        }

        private void S1_ClearClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;
        }

        private void S1_RefreshSimpleClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;

                var tasks = new List<Task<string>>();
                foreach (ConnectionProfile profile in NetworkInformation.GetConnectionProfiles())
                {
                    tasks.Add(Task.Run(() =>
                    {
                        string returnString = string.Empty;
                        returnString += "================================================================================\n\n";
                        returnString += "Profile Name: " + profile.ProfileName + "\n";
                        returnString += "Network Connectivity Level: " + 
                                        NetworkInformationPrinting.WriteNetworkConnectivityLevel(profile) + "\n";
                        returnString += "Domain Connectivity Level: " + 
                                        NetworkInformationPrinting.WriteDomainConnectivityLevel(profile) + "\n";
                        returnString += "Connection Cost: " + 
                                        NetworkInformationPrinting.WriteConnectionCost(profile) + "\n";
                        return returnString;
                    }));
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

        private void S1_RefreshDetailedClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;

                var tasks = new List<Task<string>>();
                foreach (ConnectionProfile profile in NetworkInformation.GetConnectionProfiles())
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
