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
using Windows.UI.Xaml.Controls;
using NetworkInformationSample;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "NetworkInformation C# Sample";

        readonly List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Show All Profiles", ClassType=typeof(S5GetHostNames)},
            new Scenario() { Title="Internet Connected Profiles", ClassType=typeof(S2ListConnectionProfiles)},
            new Scenario() { Title="Filter Network Profiles", ClassType=typeof(S3ListConnectionProfiles)},
            new Scenario() { Title="Show Lan Identifiers", ClassType=typeof(S4ListLanIdentifiers)},
            new Scenario() { Title="Show Host Names", ClassType=typeof(S5GetHostNames)},
        };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
