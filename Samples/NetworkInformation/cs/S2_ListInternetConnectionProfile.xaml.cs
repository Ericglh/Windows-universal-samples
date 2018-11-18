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
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SDKTemplate;

namespace NetworkInformationSample
{
    public sealed partial class S2ListConnectionProfiles : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;

        public S2ListConnectionProfiles()
        {
            this.InitializeComponent();
        }

        private void S2_ClearClickEvent(object sender, RoutedEventArgs e)
        {
            NetworkInformation.NetworkStatusChanged -= S2_NetworkStatusChangedEventHandler;
            OutputText.Text = string.Empty;
        }

        private void S2_RefreshSimpleClickEvent(object sender, RoutedEventArgs e)
        {
            NetworkInformation.NetworkStatusChanged -= S2_NetworkStatusChangedEventHandler;
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;

                string returnString = string.Empty;
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    returnString += "GetInternetConnectionProfile ConnectionProfile is null";
                }
                else
                {
                    returnString += "================================================================================\n\n";
                    returnString += "Profile Name: " + profile.ProfileName + "\n";
                    returnString += "Network Connectivity Level: " +
                                    NetworkInformationPrinting.WriteNetworkConnectivityLevel(profile) + "\n";
                    returnString += "Domain Connectivity Level: " +
                                    NetworkInformationPrinting.WriteDomainConnectivityLevel(profile) + "\n";
                    returnString += "Connection Cost: " +
                                    NetworkInformationPrinting.WriteConnectionCost(profile) + "\n";
                }

                var endTime = DateTimeOffset.Now;

                OutputText.Text += returnString;
                OutputText.Text += "Time taken: " + (endTime - startTime).TotalMilliseconds + " ms.\n";

                Debug.WriteLine(OutputText.Text);
                _rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void S2_RefreshDetailedClickEvent(object sender, RoutedEventArgs e)
        {
            NetworkInformation.NetworkStatusChanged -= S2_NetworkStatusChangedEventHandler;
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;

                var task = Task.Run(() => NetworkInformationPrinting.WriteConnectionProfiles(NetworkInformation.GetInternetConnectionProfile()));
                task.Wait();
                var endTime = DateTimeOffset.Now;

                OutputText.Text += task.Result;
                OutputText.Text += "Time taken: " + (endTime - startTime).TotalMilliseconds + " ms.\n";

                Debug.WriteLine(OutputText.Text);
                _rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void S2_RegisterForChangesClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                NetworkInformation.NetworkStatusChanged += S2_NetworkStatusChangedEventHandler;
                OutputText.Text += "Successfully registered for Network status changes\n";
                OutputText.Text += "Will update this window with the current Internet profile as changes are indicated\n\n";
            }
            catch (Exception ex)
            {
                _rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void S2_NetworkStatusChangedEventHandler(object eventObject)
        {
            try
            {
                var startTime = DateTimeOffset.Now;

                string returnString = string.Empty;
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    returnString += "GetInternetConnectionProfile ConnectionProfile is null";
                }
                else
                {
                    returnString += "================================================================================\n\n";
                    returnString += "Profile Name: " + profile.ProfileName + "\n";
                    returnString += "Network Connectivity Level: " +
                                    NetworkInformationPrinting.WriteNetworkConnectivityLevel(profile) + "\n";
                    returnString += "Domain Connectivity Level: " +
                                    NetworkInformationPrinting.WriteDomainConnectivityLevel(profile) + "\n";
                    returnString += "Connection Cost: " +
                                    NetworkInformationPrinting.WriteConnectionCost(profile) + "\n";
                }

                var endTime = DateTimeOffset.Now;

                OutputText.Text += returnString;
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
