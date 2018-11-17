using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace NetworkInformationSample
{
    public class NetworkInformationPrinting
    {
        public static string WriteHostNames(HostName hostname)
        {
            if (hostname == null)
            {
                return "HostName is null";
            }

            string returnString = string.Empty;

            returnString += "Canonical Name: " + hostname.CanonicalName + "\n";
            returnString += "Display Name: " + hostname.DisplayName + "\n";
            returnString += "Raw Name: " + hostname.RawName + "\n";
            switch (hostname.Type)
            {
                case HostNameType.DomainName:
                    returnString += "Type: DomainName\n";
                    break;
                case HostNameType.Ipv4:
                    returnString += "Type: Ipv4\n";
                    break;
                case HostNameType.Ipv6:
                    returnString += "Type: Ipv6\n";
                    break;
                case HostNameType.Bluetooth:
                    returnString += "Type: Bluetooth\n";
                    break;
            }

            returnString += "IP Information: ";
            if (hostname.IPInformation == null)
            {
                returnString += "null\n";
            }
            else
            {
                returnString += "\n";
                byte? prefixLength = hostname.IPInformation.PrefixLength;
                returnString += "Prefix Length: " + (prefixLength.HasValue ? prefixLength.Value.ToString() : "null") + "\n";
                returnString += "Network Adapter: ";
                returnString += WriteNetworkAdapters(hostname.IPInformation.NetworkAdapter) + "\n";
            }
            returnString += "\n";
            returnString += "================================================================================\n\n";
            return returnString;
        }

        public static string WriteLanIdentifiers(LanIdentifier lanIdentifier)
        {
            if (lanIdentifier == null)
            {
                return "LanIdentifier is null";

            }

            string returnString = string.Empty;

            if (lanIdentifier.InfrastructureId != null)
            {
                returnString += "\n";
                returnString += "Infrastructure Type: " + lanIdentifier.InfrastructureId.Type + "\n";
                returnString += "Infrastructure Value: ";
                foreach (var value in lanIdentifier.InfrastructureId.Value)
                {
                    returnString += value + " ";
                }
            }

            if (lanIdentifier.PortId != null)
            {
                returnString += "\n";
                returnString += "Port Type : " + lanIdentifier.PortId.Type + "\n";
                returnString += "Port Value: ";
                foreach (var value in lanIdentifier.PortId.Value)
                {
                    returnString += value + " ";
                }
            }

            returnString += "\n";
            returnString += "Network Adapter Id : " + lanIdentifier.NetworkAdapterId + "\n";
            returnString += "================================================================================\n\n";
            return returnString;
        }

        public static async Task<string> WriteConnectionProfiles(ConnectionProfile profile)
        {
            if (profile == null)
            {
                return "ConnectionProfile is null";
            }

            string attributedNetworkUsage = await WriteAttributedNetworkUsageAsync(profile);
            string connectivityInterval = await WriteConnectivityIntervalsAsync(profile);
            string networkUsage = await WriteNetworkUsageAsync(profile);
            string providerNetworkUsage = await WriteProviderNetworkUsageAsync(profile);

            string returnString = string.Empty;
            returnString += "Profile Name: " + profile.ProfileName + "\n";
            returnString += "Network Names: " + WriteNetworkNames(profile) + "\n";
            returnString += "Signal Bars: " + WriteSignalBars(profile) + "\n";
            returnString += "Network Connectivity Level: " + profile.GetNetworkConnectivityLevel() + "\n";
            returnString += "Domain Connectivity Level: " + profile.GetDomainConnectivityLevel() + "\n";
            returnString += "Connection Cost: " + WriteConnectionCost(profile) + "\n";
            returnString += "Data Plan Status: " + WriteDataPlanStatus(profile) + "\n";
            returnString += "Network Security Settings: " + WriteNetworkSecuritySettings(profile) + "\n";
            returnString += "Service Provider GUID: " + WriteServiceProviderGuid(profile) + "\n";
            returnString += "Wlan Connection Profile: " + WriteWlanConnectionProfileInfo(profile) + "\n";
            returnString += "Wwan Connection Profile: " + WriteWwanConnectionProfileInfo(profile) + "\n";
            returnString += "Attributed Network Usage: " + attributedNetworkUsage + "\n";
            returnString += "Connectivity Intervals: " + connectivityInterval + "\n";
            returnString += "Network Usage: " + networkUsage + "\n";
            returnString += "Provider Network Usage: " + providerNetworkUsage + "\n";
            returnString += "List Network Adapters: " + WriteNetworkAdapters(profile.NetworkAdapter) + "\n";
            returnString += "================================================================================\n\n";
            return returnString;
        }


        private static string WriteNetworkNames(ConnectionProfile profile)
        {
            var networkNames = profile.GetNetworkNames();
            switch (networkNames.Count)
            {
                case 0:
                    return "none";
                case 1:
                    return networkNames[0];

                default:
                    string returnString = string.Empty;
                    foreach (string name in networkNames)
                    {
                        if (returnString.Length > 1)
                        {
                            returnString += ",";
                        }
                        returnString += name;

                    }
                    return returnString;
            }
        }
        private static string WriteSignalBars(ConnectionProfile profile)
        {
            byte? b = profile.GetSignalBars();
            return !b.HasValue ? "null" : b.Value.ToString();
        }
        private static string WriteConnectionCost(ConnectionProfile profile)
        {
            ConnectionCost connectionCost = profile.GetConnectionCost();
            if (connectionCost == null)
            {
                return "null";
            }

            string returnString = "\n";
            returnString += "\t Approaching Data Limit: " + (connectionCost.ApproachingDataLimit ? "Yes" : "No") + "\n";
            returnString += "\t Background Data Usage Restricted: " + (connectionCost.BackgroundDataUsageRestricted ? "Restricted" : "Not Restricted") + "\n";
            returnString += "\t Roaming: " + (connectionCost.Roaming ? "Roaming" : "Not Roaming") + "\n";
            returnString += "\t Over Data Limit: " + (connectionCost.OverDataLimit ? "Yes" : "No") + "\n";
            returnString += "\t Network Cost Type: " + connectionCost.NetworkCostType;
            return returnString;
        }
        private static string WriteDataPlanStatus(ConnectionProfile profile)
        {
            DataPlanStatus dataPlanStatus = profile.GetDataPlanStatus();
            if (dataPlanStatus == null)
            {
                return "null";
            }

            uint? dataLimitInMegabytes = dataPlanStatus.DataLimitInMegabytes;
            bool hasDataPlanUsage = dataPlanStatus.DataPlanUsage != null;
            ulong? inboundBitsPerSecond = dataPlanStatus.InboundBitsPerSecond;
            uint? maxTransferSizeInMegabytes = dataPlanStatus.MaxTransferSizeInMegabytes;
            DateTimeOffset? nextBillingCycle = dataPlanStatus.NextBillingCycle;
            ulong? outboundBitsPerSecond = dataPlanStatus.OutboundBitsPerSecond;

            string returnString = "\n";
            returnString += "\t Data Limit in Megabytes: " + (dataLimitInMegabytes.HasValue ? dataLimitInMegabytes.Value.ToString() : "null") + "\n";
            returnString += "\t Data Plan Usage (Megabytes Used): " + (hasDataPlanUsage ? dataPlanStatus.DataPlanUsage.MegabytesUsed.ToString() : "null") + "\n";
            returnString += "\t Data Plan Usage (Last Sync Time): " + (hasDataPlanUsage ? dataPlanStatus.DataPlanUsage.LastSyncTime.ToString() : "null") + "\n";
            returnString += "\t Inbound Bits Per Second: " + (inboundBitsPerSecond.HasValue ? inboundBitsPerSecond.Value.ToString() : "null") + "\n";
            returnString += "\t Max Transfer Size in Megabytes: " + (maxTransferSizeInMegabytes.HasValue ? maxTransferSizeInMegabytes.Value.ToString() : "null") + "\n";
            returnString += "\t Next Billing Cycle: " + (nextBillingCycle.HasValue ? nextBillingCycle.Value.ToString() : "null") + "\n";
            returnString += "\t Outbound Bits Per Second: " + (outboundBitsPerSecond.HasValue ? outboundBitsPerSecond.Value.ToString() : "null");
            return returnString;
        }
        private static string WriteNetworkSecuritySettings(ConnectionProfile profile)
        {
            NetworkSecuritySettings networkSecuritySettings = profile.NetworkSecuritySettings;
            if (networkSecuritySettings == null)
            {
                return "null";
            }

            string returnString = "Network EncryptionType: " + networkSecuritySettings.NetworkEncryptionType + ", ";
            returnString += "Network Authentication Type: " + networkSecuritySettings.NetworkAuthenticationType;
            return returnString;
        }
        private static string WriteServiceProviderGuid(ConnectionProfile profile)
        {
            return profile.ServiceProviderGuid != null ? profile.ServiceProviderGuid.ToString() : "null";
        }
        private static string WriteWlanConnectionProfileInfo(ConnectionProfile profile)
        {
            if (!profile.IsWlanConnectionProfile)
            {
                return "(not a WLAN profile)";
            }

            return "Connected SSID: " + profile.WlanConnectionProfileDetails.GetConnectedSsid();
        }
        private static string WriteWwanConnectionProfileInfo(ConnectionProfile profile)
        {
            if (!profile.IsWwanConnectionProfile)
            {
                return "(not a WWAN profile)";
            }

            WwanConnectionProfileDetails wwanConnectionProfileDetails = profile.WwanConnectionProfileDetails;
            string returnString = "\n";
            returnString += "\t Network Registration State: " + wwanConnectionProfileDetails.GetNetworkRegistrationState() + "\n";
            returnString += "\t Current Data Class: " + wwanConnectionProfileDetails.GetCurrentDataClass() + "\n";
            returnString += "\t Home Provider Id: " + wwanConnectionProfileDetails.HomeProviderId + "\n";
            returnString += "\t Access Point Name: " + wwanConnectionProfileDetails.AccessPointName + "\n";
            returnString += "\t Wwan Network IPKind: " + wwanConnectionProfileDetails.IPKind;
            return returnString;
        }
        private static async Task<string> WriteAttributedNetworkUsageAsync(ConnectionProfile profile)
        {
            NetworkUsageStates networkUsageStates;
            networkUsageStates.Roaming = TriStates.DoNotCare;
            networkUsageStates.Shared = TriStates.DoNotCare;

            try
            {
                // attributed network usage requires special privileges
                TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
                var attributedNetworkUsage =
                    await profile.GetAttributedNetworkUsageAsync(
                        DateTimeOffset.Now.Subtract(oneDay),
                        DateTimeOffset.Now,
                        networkUsageStates);
                if (attributedNetworkUsage == null)
                {
                    return "null";
                }

                if (attributedNetworkUsage.Count == 0)
                {
                    return "empty";
                }

                string returnString = " [usage for the last 24 hours]";
                foreach (AttributedNetworkUsage att in attributedNetworkUsage)
                {
                    returnString += "\n";
                    returnString += "\t Attributed Id: " + att.AttributionId + "\n";
                    returnString += "\t Attributed Name: " + att.AttributionName + "\n";
                    returnString += "\t AttributedId: " + att.AttributionThumbnail + "\n";
                    returnString += "\t Bytes Received: " + att.BytesReceived + "\n";
                    returnString += "\t Bytes Sent: " + att.BytesSent + "\n";
                    returnString += "\t --------------------------------------------------";
                }

                return returnString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static async Task<string> WriteConnectivityIntervalsAsync(ConnectionProfile profile)
        {
            NetworkUsageStates networkUsageStates;
            networkUsageStates.Roaming = TriStates.DoNotCare;
            networkUsageStates.Shared = TriStates.DoNotCare;

            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            var intervalList = await profile.GetConnectivityIntervalsAsync(
                DateTimeOffset.Now.Subtract(oneDay),
                DateTimeOffset.Now,
                networkUsageStates);
            if (intervalList == null)
            {
                return "null";
            }
            if (intervalList.Count == 0)
            {
                return "empty";
            }

            string returnString = " [usage for the last 24 hours]";
            foreach (ConnectivityInterval conn in intervalList)
            {
                returnString += "\n";
                returnString += "\t Connection Duration: " + conn.ConnectionDuration + "\n";
                returnString += "\t Start Time: " + conn.StartTime + "\n";
                returnString += "\t --------------------------------------------------";
            }
            return returnString;
        }
        private static async Task<string> WriteNetworkUsageAsync(ConnectionProfile profile)
        {
            NetworkUsageStates networkUsageStates;
            networkUsageStates.Roaming = TriStates.DoNotCare;
            networkUsageStates.Shared = TriStates.DoNotCare;

            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            IReadOnlyList<NetworkUsage> networkUsage = await profile.GetNetworkUsageAsync(
                DateTimeOffset.Now.Subtract(oneDay),
                DateTimeOffset.Now,
                DataUsageGranularity.PerDay,
                networkUsageStates);
            if (networkUsage == null)
            {
                return "null";
            }
            if (networkUsage.Count == 0)
            {
                return "empty";
            }

            string returnString = " [usage for the last 24 hours]";
            foreach (NetworkUsage nu in networkUsage)
            {
                returnString += "\n";
                returnString += "\t Bytes Received: " + nu.BytesReceived + "\n";
                returnString += "\t ByteSend: " + nu.BytesSent + "\n";
                returnString += "\t Connection Duration: " + nu.ConnectionDuration + "\n";
                returnString += "\t --------------------------------------------------";
            }
            return returnString;
        }
        private static async Task<string> WriteProviderNetworkUsageAsync(ConnectionProfile profile)
        {
            NetworkUsageStates networkUsageStates;
            networkUsageStates.Roaming = TriStates.DoNotCare;
            networkUsageStates.Shared = TriStates.DoNotCare;

            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            IReadOnlyList<ProviderNetworkUsage> providerNetworkUsage = await profile.GetProviderNetworkUsageAsync(
                DateTimeOffset.Now.Subtract(oneDay),
                DateTimeOffset.Now,
                networkUsageStates);
            if (providerNetworkUsage == null)
            {
                return "null";
            }
            if (providerNetworkUsage.Count == 0)
            {
                return "empty";
            }

            string returnString = " [usage for the last 24 hours]";
            foreach (ProviderNetworkUsage pnu in providerNetworkUsage)
            {
                returnString += "\n";
                returnString += "\t Provider Id: " + pnu.ProviderId + "\n";
                returnString += "\t Bytes Received: " + pnu.BytesReceived + "\n";
                returnString += "\t Byte Sent: " + pnu.BytesSent + "\n";
                returnString += "\t --------------------------------------------------";
            }
            return returnString;
        }
        private static string WriteNetworkAdapters(NetworkAdapter networkAdapter)
        {
            if (networkAdapter == null)
            {
                return "null";
            }

            string returnString = "\n";
            returnString += "\t Network Adapter LanInterfaceType: " + networkAdapter.IanaInterfaceType + "\n";
            returnString += "\t Inbound Max Bits Per Second: " + networkAdapter.InboundMaxBitsPerSecond + "\n";
            returnString += "\t Outbound Max Bits Per Second: " + networkAdapter.OutboundMaxBitsPerSecond + "\n";
            returnString += "\t Network Adapter Id: " + networkAdapter.NetworkAdapterId + "\n";
            returnString += "\t Network Id: " + networkAdapter.NetworkItem.NetworkId + "\n";
            returnString += "\t Network Type: " + networkAdapter.NetworkItem.GetNetworkTypes();
            return returnString;
        }
    }
}