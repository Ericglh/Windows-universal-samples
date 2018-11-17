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
    public sealed partial class S3ListConnectionProfiles : Page
    {
        readonly MainPage _rootPage = MainPage.Current;

        public S3ListConnectionProfiles()
        {
            this.InitializeComponent();
        }

        private void S3_ButtonClick(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                string backgroundRestricted = ((ComboBoxItem)ComboBackRestricted.SelectedItem)?.Content?.ToString();
                string roaming = ((ComboBoxItem)ComboIsRoaming.SelectedItem)?.Content?.ToString();
                string connected = ((ComboBoxItem)ComboIsConnected.SelectedItem)?.Content?.ToString();
                string isWlanProfile = ((ComboBoxItem)ComboIsWlanProfile.SelectedItem)?.Content?.ToString();
                string isWwanProfile = ((ComboBoxItem)ComboIsWwanProfile.SelectedItem)?.Content?.ToString();
                string networkCostType = ((ComboBoxItem)ComboNetworkCostType.SelectedItem)?.Content?.ToString();

                ConnectionProfileFilter connectionProfileFilter = new ConnectionProfileFilter();

                Guid.TryParse(ComboPurposeGuid.Text, out var purposeGuid);
                connectionProfileFilter.PurposeGuid = purposeGuid;

                connectionProfileFilter.IsBackgroundDataUsageRestricted = false;
                if (backgroundRestricted != null && backgroundRestricted.ToLower() == "yes")
                {
                    connectionProfileFilter.IsBackgroundDataUsageRestricted = true;
                }

                connectionProfileFilter.IsRoaming = false;
                if (roaming != null && roaming.ToLower() == "yes")
                {
                    connectionProfileFilter.IsRoaming = true;
                }

                connectionProfileFilter.IsConnected = false;
                if (connected != null && connected.ToLower() == "yes")
                {
                    connectionProfileFilter.IsConnected = true;
                }

                connectionProfileFilter.IsWwanConnectionProfile = false;
                if (isWlanProfile != null && isWlanProfile.ToLower() == "yes")
                {
                    connectionProfileFilter.IsWlanConnectionProfile = true;
                }

                connectionProfileFilter.IsWwanConnectionProfile = false;
                if (isWwanProfile != null && isWwanProfile.ToLower() == "yes")
                {
                    connectionProfileFilter.IsWwanConnectionProfile = true;
                }

                if (networkCostType != null)
                {
                    switch (networkCostType.ToLower())
                    {
                        case "unrestricted":
                            connectionProfileFilter.NetworkCostType = NetworkCostType.Unrestricted;
                            break;
                        case "fixed":
                            connectionProfileFilter.NetworkCostType = NetworkCostType.Fixed;
                            break;
                        case "variable":
                            connectionProfileFilter.NetworkCostType = NetworkCostType.Variable;
                            break;
                        case "unknown":
                            connectionProfileFilter.NetworkCostType = NetworkCostType.Unknown;
                            break;
                    }
                }

                var startTime = DateTimeOffset.Now;

                var findConnectionTask = Task.Run(async () =>
                    await NetworkInformation.FindConnectionProfilesAsync(connectionProfileFilter));
                findConnectionTask.Wait();

                // write each in parallel as they are async
                var tasks = new List<Task<string>>();
                foreach (ConnectionProfile profile in findConnectionTask.Result)
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
