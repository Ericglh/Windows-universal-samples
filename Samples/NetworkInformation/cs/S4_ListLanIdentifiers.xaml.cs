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
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SDKTemplate;

namespace NetworkInformationSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class S4ListLanIdentifiers : Page
    {
        private readonly MainPage _rootPage = MainPage.Current;

        public S4ListLanIdentifiers()
        {
            this.InitializeComponent();
        }

        private void S4_ClearClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;
        }
        
        private void S4_RefreshClickEvent(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            try
            {
                var startTime = DateTimeOffset.Now;

                foreach (var identifier in NetworkInformation.GetLanIdentifiers())
                {
                    OutputText.Text += NetworkInformationPrinting.WriteLanIdentifiers(identifier);
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
