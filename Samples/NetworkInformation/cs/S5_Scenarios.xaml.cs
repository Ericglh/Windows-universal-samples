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
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

namespace NetworkInformationSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S5Test : Page
    {
        MainPage rootPage = MainPage.Current;
        private CoreDispatcher _cd = Window.Current.CoreWindow.Dispatcher;
        private string gTmpRes = string.Empty;
        private NetworkStatusChangedEventHandler networkStatusChangedEventHandler = null;
        private string internetConnectionProfile = string.Empty;
        private bool registerNetworkStatusChange = false;

        //Save current internet profile and network adapter ID globally
        //string internetProfile = "Not connected to Internet";
        //string networkAdapter = "Not connected to Internet";

        public S5Test()
        {
            this.InitializeComponent();
        }
        /*
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == BackgroundTaskSample.SampleBackgroundTaskWithConditionName)
                {
                    //Associate background task completed event handler with background task
                    var isTaskRegistered = RegisterCompletedHandlerforBackgroundTask(task.Value);
                    //UpdateButton(isTaskRegistered);
                }
            }

        }
        */
        private void S8_ClickGetHostNames(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<Windows.Networking.HostName> hostNames = NetworkInformation.GetHostNames();
            OutputText.Text = "S8_Scenarios Result:\n";
            string str = string.Empty;
            str += "S8_GetHostName Result\n";
            if(hostNames != null)
            {
                foreach(Windows.Networking.HostName hostname in hostNames)
                {
                    OutputText.Text += "DisplayName: " + hostname.DisplayName + "\t";
                    OutputText.Text += "CanonicalName: " + hostname.CanonicalName + "\t";
                    OutputText.Text += "RawName: " + hostname.RawName + "\t";
                    //hostname.IPInformation.NetworkAdapter already exist in Scenario 4. Connection Profile.
                    OutputText.Text += "IPInformation Prefix Length: " + (hostname.IPInformation == null ? "null" : hostname.IPInformation.PrefixLength.ToString()) + "\t";
                    OutputText.Text += "\n----------------------------------------------------------\n";
                    
                }
                str += OutputText.Text;
                Debug.WriteLine(str);
                rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            }
            else
            {
                OutputText.Text += "Get HostName returns null";
                str = OutputText.Text;
            }
            writeToFile(str);
        }

        private void S8_ClickGetSortedEP(object sender, RoutedEventArgs e)
        {
            OutputText.Text = "S8_Scenarios Result:\n";
            string tmpRes = string.Empty;
            tmpRes += "S9_GetSortedPairs Result\n";
            Windows.Networking.HostName localHostName = new Windows.Networking.HostName("www.contoso.com");
            Windows.Networking.HostName remoteHostName = new Windows.Networking.HostName("www.example.com");
            
            
            Task<IReadOnlyList<Windows.Networking.EndpointPair>> task = Windows.Networking.Sockets.DatagramSocket.GetEndpointPairsAsync(localHostName, "12345", Windows.Networking.HostNameSortOptions.OptimizeForLongConnections).AsTask();
            var endpointPairs = task.Result;
            var originalEndpointPairs = new List<Windows.Networking.EndpointPair>();

            for (int i = 0; i < endpointPairs.Count; i++)
            {
                originalEndpointPairs.Add(endpointPairs.ElementAt(i));
            }

            IReadOnlyList<Windows.Networking.EndpointPair> sortedEndPointPairs = NetworkInformation.GetSortedEndpointPairs(endpointPairs, Windows.Networking.HostNameSortOptions.OptimizeForLongConnections);
            if(sortedEndPointPairs != null)
            {
                //tmpRes += sortedEndPointPairs.Count + "   " + originalEndpointPairs.Count + "\n";
                for(int i = 0; i < endpointPairs.Count; i++)
                {
                    tmpRes += "Sorted local host name: " + endpointPairs[i].LocalHostName + "\t";
                    tmpRes += "Sorted remote host name: " + endpointPairs[i].RemoteHostName + "\n";
                    if(originalEndpointPairs.ElementAt(i) == endpointPairs.ElementAt(i))
                    {
                        tmpRes += "Correctly sorted? Yes\n";
                    }
                    else
                    {
                        tmpRes += "Correctly sorted? No\n";
                    }
                    tmpRes += "----------------------------------------------------------------------\n";
                }
            }
            else
            {
                tmpRes = "GetSortedEndpointPairs() returns null.";
            }
            rootPage.NotifyUser("Success", NotifyType.StatusMessage);
            writeToFile(tmpRes);
            Debug.WriteLine(tmpRes);
            OutputText.Text += tmpRes;
            
        }

        private void S8_ClickGetProxyConf(object sender, RoutedEventArgs e)
        {
            OutputText.Text = "S8_Scenarios Result:\n";
            gTmpRes = string.Empty;
            gTmpRes += "S10_GetProxyConfiguration Result\n";
            Uri uri = new Uri("https://www.example.com");


            NetworkInformation.GetProxyConfigurationAsync(uri).Completed = GetProxyConfigurationHandler;
            rootPage.NotifyUser("Success", NotifyType.StatusMessage);
        }

        async private void GetProxyConfigurationHandler(IAsyncOperation<ProxyConfiguration> asyncInfo, AsyncStatus asyncStatus)
        {

            if(asyncStatus == AsyncStatus.Completed)
            {
                ProxyConfiguration proxyConfiguration = asyncInfo.GetResults();
                asyncInfo.Close();
                if(proxyConfiguration != null)
                {
                    gTmpRes += "Can Connect Directly: " + proxyConfiguration.CanConnectDirectly + "\t";
                    gTmpRes += "Proxy Uris: " + proxyConfiguration.ProxyUris + "\t";
                }
                else
                {
                    gTmpRes += "GetProxyConfigutationAsync() returns null.";
                }
            }
            else
            {
                gTmpRes += "GetProxyConfigutationAsync() not completed.";
            }
            //OutputText.Text = gTmpRes;
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text += gTmpRes;
                writeToFile(gTmpRes);
                Debug.WriteLine(gTmpRes);
                gTmpRes = string.Empty;
            });

        }


        private void S8_ClickNetworkStatusChanged(object sender, RoutedEventArgs e) {
            OutputText.Text = "S8_Scenarios Listening to Network Status Change:\n";
            gTmpRes += "S11_NetworkStatusChanged Result\n";
            networkStatusChangedEventHandler = new NetworkStatusChangedEventHandler(OnNetworkStatusChanged);
            if(!registerNetworkStatusChange)
            {
                NetworkInformation.NetworkStatusChanged += networkStatusChangedEventHandler;
                registerNetworkStatusChange = true;
                rootPage.NotifyUser("Register for network status change successfully.", NotifyType.StatusMessage);
            }
            else
            {
                gTmpRes += "Already registered for network status change.\n";
            }


        }

        async void OnNetworkStatusChanged(object sender)
        {

            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("Network status changed\n", NotifyType.StatusMessage);
            });

            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();

            if (connectionProfile != null)
            {
                await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    gTmpRes += "Profile Name: " + connectionProfile.ProfileName.ToString() + "\n";
                });
            }
            else
            {
                await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    gTmpRes += "GetInternetConnectionProfile() returns null.\n";
                });
            }
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = gTmpRes;
                writeToFile(gTmpRes);
                Debug.WriteLine(gTmpRes);
                gTmpRes = string.Empty;
            });
        }

        void UnRegisterForNetworkStatusChangeNotify()
        {
            NetworkInformation.NetworkStatusChanged -= networkStatusChangedEventHandler;
            gTmpRes = string.Empty;
            registerNetworkStatusChange = false;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (registerNetworkStatusChange)
            {
                UnRegisterForNetworkStatusChangeNotify();
                registerNetworkStatusChange = false;
            }
        }

        private async void writeToFile(string input)
        {

            try
            {
                input += "\n**********************************************\n";
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("sample.txt");
                await Windows.Storage.FileIO.AppendTextAsync(sampleFile, input);
                Debug.WriteLine("Open/create file succeeded." + sampleFile.Path);
            }
            catch
            {
                Debug.WriteLine("Open/create file failed.");
            }
        }

        /*
        private void S8_ClickNetworkStateChanged(object sender, RoutedEventArgs e) {
            try
            {
                //Save current internet profile and network adapter ID globally
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (connectionProfile != null)
                {
                    internetProfile = connectionProfile.ProfileName;
                    var networkAdapterInfo = connectionProfile.NetworkAdapter;
                    if (networkAdapterInfo != null)
                    {
                        networkAdapter = networkAdapterInfo.NetworkAdapterId.ToString();
                    }
                    else
                    {
                        networkAdapter = "Not connected to Internet";
                    }
                }
                else
                {
                    internetProfile = "Not connected to Internet";
                    networkAdapter = "Not connected to Internet";
                }
                
                var task = BackgroundTaskSample.RegisterBackgroundTask(BackgroundTaskSample.SampleBackgroundTaskEntryPoint,
                                                                       BackgroundTaskSample.SampleBackgroundTaskWithConditionName,
                                                                       new SystemTrigger(SystemTriggerType.NetworkStateChange, false),
                                                                       new SystemCondition(SystemConditionType.InternetAvailable));
                
                //Associate background task completed event handler with background task.
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                rootPage.NotifyUser("Registered for NetworkStatusChange background task with Internet present condition\n", NotifyType.StatusMessage);
                UpdateButton(true);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
        }

        private void UnregisterButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskWithConditionName);
            rootPage.NotifyUser("Unregistered for NetworkStatusChange background task with Internet present condition\n", NotifyType.StatusMessage);
            UpdateButton(false);
        }

        private bool RegisterCompletedHandlerforBackgroundTask(IBackgroundTaskRegistration task)
        {
            bool taskRegistered = false;
            try
            {
                //Associate background task completed event handler with background task.
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
                taskRegistered = true;
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
            }
            return taskRegistered;
        }

        private async void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            Object profile = localSettings.Values["InternetProfile"];
            Object adapter = localSettings.Values["NetworkAdapterId"];

            Object hasNewConnectionCost = localSettings.Values["HasNewConnectionCost"];
            Object hasNewDomainConnectivityLevel = localSettings.Values["HasNewDomainConnectivityLevel"];
            Object hasNewHostNameList = localSettings.Values["HasNewHostNameList"];
            Object hasNewInternetConnectionProfile = localSettings.Values["HasNewInternetConnectionProfile"];
            Object hasNewNetworkConnectivityLevel = localSettings.Values["HasNewNetworkConnectivityLevel"];
            OutputText.Text = "In OnComplete.";
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string outputText = string.Empty;

                if ((profile != null) && (adapter != null))
                {
                    //If internet profile has changed, display the new internet profile
                    if ((string.Equals(profile.ToString(), internetProfile, StringComparison.Ordinal) == false) ||
                            (string.Equals(adapter.ToString(), networkAdapter, StringComparison.Ordinal) == false))
                    {
                        internetProfile = profile.ToString();
                        networkAdapter = adapter.ToString();
                        outputText += "Internet Profile changed\n" + "=================\n" + "Current Internet Profile : " + internetProfile + "\n\n";

                        if (hasNewConnectionCost != null)
                        {
                            outputText += hasNewConnectionCost.ToString() + "\n";
                        }
                        if (hasNewDomainConnectivityLevel != null)
                        {
                            outputText += hasNewDomainConnectivityLevel.ToString() + "\n";
                        }
                        if (hasNewHostNameList != null)
                        {
                            outputText += hasNewHostNameList.ToString() + "\n";
                        }
                        if (hasNewInternetConnectionProfile != null)
                        {
                            outputText += hasNewInternetConnectionProfile.ToString() + "\n";
                        }
                        if (hasNewNetworkConnectivityLevel != null)
                        {
                            outputText += hasNewNetworkConnectivityLevel.ToString() + "\n";
                        }

                        rootPage.NotifyUser(outputText, NotifyType.StatusMessage);
                    }
                }
            });
        }

        private void UpdateButton(bool registered)
        {
            S8_networkStateChanged.IsEnabled = !registered;
            S8_unnetworkStateChanged.IsEnabled = registered;
        }
        */

    }
}
