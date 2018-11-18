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
            returnString += "\n================================================================================\n";

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

            if (hostname.IPInformation == null)
            {
                returnString += "IP Information: null\n";
            }
            else
            {
                byte? prefixLength = hostname.IPInformation.PrefixLength;
                returnString += "Prefix Length: " + (prefixLength.HasValue ? prefixLength.Value.ToString() : "null") + "\n";
                returnString += "Network Adapter: ";
                returnString += WriteNetworkAdapters(hostname.IPInformation.NetworkAdapter) + "\n";
            }
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
            returnString += "================================================================================\n\n";
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
            return returnString;
        }


        public static string WriteNetworkNames(ConnectionProfile profile)
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
        public static string WriteNetworkConnectivityLevel(ConnectionProfile profile)
        {
            var connectivityLevel = profile.GetNetworkConnectivityLevel();
            switch (connectivityLevel)
            {
                case NetworkConnectivityLevel.None:
                    return "None";
                case NetworkConnectivityLevel.LocalAccess:
                    return "LocalAccess";
                case NetworkConnectivityLevel.ConstrainedInternetAccess:
                    return "ConstrainedInternetAccess";
                case NetworkConnectivityLevel.InternetAccess:
                    return "InternetAccess";
                default:
                    return "UNKNOWN NetworkConnectivityLevel: " + connectivityLevel;
            }
        }
        public static string WriteDomainConnectivityLevel(ConnectionProfile profile)
        {
            var domainConnectivityLevel = profile.GetDomainConnectivityLevel();
            switch (domainConnectivityLevel)
            {
                case DomainConnectivityLevel.None:
                    return "None";
                case DomainConnectivityLevel.Unauthenticated:
                    return "Unauthenticated";
                case DomainConnectivityLevel.Authenticated:
                    return "Authenticated";
                default:
                    return "UNKNOWN DomainConnectivityLevel: " + domainConnectivityLevel;
            }
        }
        public static string WriteSignalBars(ConnectionProfile profile)
        {
            byte? b = profile.GetSignalBars();
            return !b.HasValue ? "null" : b.Value.ToString();
        }
        public static string WriteConnectionCost(ConnectionProfile profile)
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

            returnString += "\t Network Cost Type: ";
            switch (connectionCost.NetworkCostType)
            {
                case NetworkCostType.Fixed:
                    returnString += "Fixed";
                    break;
                case NetworkCostType.Unknown:
                    returnString += "Unknown";
                    break;
                case NetworkCostType.Unrestricted:
                    returnString += "Unrestricted";
                    break;
                case NetworkCostType.Variable:
                    returnString += "Variable";
                    break;
                default:
                    returnString += "UNKNOWN NetworkCostType: " + connectionCost.NetworkCostType;
                    break;
            }
            return returnString;
        }
        public static string WriteDataPlanStatus(ConnectionProfile profile)
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
        public static string WriteNetworkSecuritySettings(ConnectionProfile profile)
        {
            NetworkSecuritySettings networkSecuritySettings = profile.NetworkSecuritySettings;
            if (networkSecuritySettings == null)
            {
                return "null";
            }

            string returnString = "\n";
            returnString += "\t Network EncryptionType: ";
            switch (networkSecuritySettings.NetworkEncryptionType)
            {
                case NetworkEncryptionType.Ccmp:
                    returnString += "Ccmp";
                    break;
                case NetworkEncryptionType.Ihv:
                    returnString += "Ihv";
                    break;
                case NetworkEncryptionType.None:
                    returnString += "None";
                    break;
                case NetworkEncryptionType.RsnUseGroup:
                    returnString += "RsnUseGroup";
                    break;
                case NetworkEncryptionType.Tkip:
                    returnString += "Tkip";
                    break;
                case NetworkEncryptionType.Unknown:
                    returnString += "Unknown";
                    break;
                case NetworkEncryptionType.Wep:
                    returnString += "Wep";
                    break;
                case NetworkEncryptionType.Wep104:
                    returnString += "Wep104";
                    break;
                case NetworkEncryptionType.Wep40:
                    returnString += "Wep40";
                    break;
                case NetworkEncryptionType.WpaUseGroup:
                    returnString += "WpaUseGroup";
                    break;
                default:
                    returnString += "UNKNOWN NetworkEncryptionType: " + networkSecuritySettings.NetworkEncryptionType;
                    break;
            }
            returnString += "\n";
            returnString += "\t Network Authentication Type: ";
            switch (networkSecuritySettings.NetworkAuthenticationType)
            {
                case NetworkAuthenticationType.Ihv:
                    returnString += "Ihv";
                    break;
                case NetworkAuthenticationType.None:
                    returnString += "None";
                    break;
                case NetworkAuthenticationType.Open80211:
                    returnString += "Open80211";
                    break;
                case NetworkAuthenticationType.Rsna:
                    returnString += "Rsna";
                    break;
                case NetworkAuthenticationType.RsnaPsk:
                    returnString += "RsnaPsk";
                    break;
                case NetworkAuthenticationType.SharedKey80211:
                    returnString += "SharedKey80211";
                    break;
                case NetworkAuthenticationType.Unknown:
                    returnString += "Unknown";
                    break;
                case NetworkAuthenticationType.Wpa:
                    returnString += "Wpa";
                    break;
                case NetworkAuthenticationType.WpaNone:
                    returnString += "WpaNone";
                    break;
                case NetworkAuthenticationType.WpaPsk:
                    returnString += "WpaPsk";
                    break;
                default:
                    returnString += "UNKNOWN NetworkAuthenticationType: " + networkSecuritySettings.NetworkAuthenticationType;
                    break;
            }
            return returnString;
        }
        public static string WriteServiceProviderGuid(ConnectionProfile profile)
        {
            return profile.ServiceProviderGuid != null ? profile.ServiceProviderGuid.ToString() : "null";
        }
        public static string WriteWlanConnectionProfileInfo(ConnectionProfile profile)
        {
            if (!profile.IsWlanConnectionProfile)
            {
                return "(not a WLAN profile)";
            }

            return "Connected SSID: " + profile.WlanConnectionProfileDetails.GetConnectedSsid();
        }
        public static string WriteWwanConnectionProfileInfo(ConnectionProfile profile)
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
        public static async Task<string> WriteAttributedNetworkUsageAsync(ConnectionProfile profile)
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
        public static async Task<string> WriteConnectivityIntervalsAsync(ConnectionProfile profile)
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
        public static async Task<string> WriteNetworkUsageAsync(ConnectionProfile profile)
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
        public static async Task<string> WriteProviderNetworkUsageAsync(ConnectionProfile profile)
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
        public static string WriteNetworkAdapters(NetworkAdapter networkAdapter)
        {
            if (networkAdapter == null)
            {
                return "null";
            }

            string returnString = "\n";
            returnString += "\t Network Adapter LanInterfaceType: " + WriteIanaInterfaceType(networkAdapter.IanaInterfaceType) + "\n";
            returnString += "\t Inbound Max Bits Per Second: " + networkAdapter.InboundMaxBitsPerSecond + "\n";
            returnString += "\t Outbound Max Bits Per Second: " + networkAdapter.OutboundMaxBitsPerSecond + "\n";
            returnString += "\t Network Adapter Id: " + networkAdapter.NetworkAdapterId + "\n";
            returnString += "\t Network Id: " + networkAdapter.NetworkItem.NetworkId + "\n";
            returnString += "\t Network Type: " + networkAdapter.NetworkItem.GetNetworkTypes() + "\n";
            return returnString;
        }
        // from https://www.iana.org/assignments/ianaiftype-mib/ianaiftype-mib
        private static string WriteIanaInterfaceType(uint type)
        {
            switch (type)
            {
                case 1: return "other(1)"; //           --none of the following
                case 2: return "regular1822(2)"; // 
                case 3: return "hdh1822(3)"; // 
                case 4: return "ddnX25(4)"; // 
                case 5: return "rfc877x25(5)"; // 
                case 6: return "ethernetCsmacd(6)"; //  -- for all ethernet-like interfaces, regardless of speed, as per RFC3635
                case 7: return "iso88023Csmacd(7)"; //  --Deprecated via RFC3635, ethernetCsmacd(6) should be used instead
                case 8: return "iso88024TokenBus(8)"; // 
                case 9: return "iso88025TokenRing(9)"; // 
                case 10: return "iso88026Man(10)"; // 
                case 11: return "starLan(11)"; //  --Deprecated via RFC3635, ethernetCsmacd(6) should be used instead
                case 12: return "proteon10Mbit(12)"; // 
                case 13: return "proteon80Mbit(13)"; // 
                case 14: return "hyperchannel(14)"; // 
                case 15: return "fddi(15)"; // 
                case 16: return "lapb(16)"; // 
                case 17: return "sdlc(17)"; // 
                case 18: return "ds1(18)"; //  --DS1 - MIB
                case 19: return "e1(19)"; //  --Obsolete see DS1 - MIB
                case 20: return "basicISDN(20)"; //  --no longer used, see also RFC2127
                case 21: return "primaryISDN(21)"; //  --no longer used, see also RFC2127
                case 22: return "propPointToPointSerial(22)"; //  --proprietary serial
                case 23: return "ppp(23)"; // 
                case 24: return "softwareLoopback(24)"; // 
                case 25: return "eon(25)"; //  --CLNP over IP
                case 26: return "ethernet3Mbit(26)"; // 
                case 27: return "nsip(27)"; //  --XNS over IP
                case 28: return "slip(28)"; //  --generic SLIP
                case 29: return "ultra(29)"; //  --ULTRA technologies
                case 30: return "ds3(30)"; //  --DS3 - MIB
                case 31: return "sip(31)"; //  --SMDS, coffee
                case 32: return "frameRelay(32)"; //  --DTE only.
                case 33: return "rs232(33)"; // 
                case 34: return "para(34)"; //  --parallel - port
                case 35: return "arcnet(35)"; //  --arcnet
                case 36: return "arcnetPlus(36)"; //  --arcnet plus
                case 37: return "atm(37)"; //  --ATM cells
                case 38: return "miox25(38)"; // 
                case 39: return "sonet(39)"; //  --SONET or SDH
                case 40: return "x25ple(40)"; // 
                case 41: return "iso88022llc(41)"; // 
                case 42: return "localTalk(42)"; // 
                case 43: return "smdsDxi(43)"; // 
                case 44: return "frameRelayService(44)"; //  --FRNETSERV - MIB
                case 45: return "v35(45)"; // 
                case 46: return "hssi(46)"; // 
                case 47: return "hippi(47)"; // 
                case 48: return "modem(48)"; //  --Generic modem
                case 49: return "aal5(49)"; //  --AAL5 over ATM
                case 50: return "sonetPath(50)"; // 
                case 51: return "sonetVT(51)"; // 
                case 52: return "smdsIcip(52)"; //  --SMDS InterCarrier Interface
                case 53: return "propVirtual(53)"; //  --proprietary virtual/internal
                case 54: return "propMultiplexor(54)"; // -- proprietary multiplexing
                case 55: return "ieee80212(55)"; //       -- 100BaseVG
                case 56: return "fibreChannel(56)"; //    -- Fibre Channel
                case 57: return "hippiInterface(57)"; //  -- HIPPI interfaces
                case 58: return "frameRelayInterconnect(58)"; //  -- Obsolete, use either frameRelay(32) or frameRelayService(44).
                case 59: return "aflane8023(59)"; //      -- ATM Emulated LAN for 802.3
                case 60: return "aflane8025(60)"; //      -- ATM Emulated LAN for 802.5
                case 61: return "cctEmul(61)"; //         -- ATM Emulated circuit
                case 62: return "fastEther(62)"; //       -- Obsoleted via RFC3635, ethernetCsmacd(6) should be used instead
                case 63: return "isdn(63)"; //            -- ISDN and X.25           
                case 64: return "v11(64)"; //             -- CCITT V.11/X.21             
                case 65: return "v36(65)"; //             -- CCITT V.36                  
                case 66: return "g703at64k(66)"; //       -- CCITT G703 at 64Kbps
                case 67: return "g703at2mb(67)"; //       -- Obsolete see DS1-MIB
                case 68: return "qllc(68)"; //            -- SNA QLLC
                case 69: return "fastEtherFX(69)"; //     -- Obsoleted via RFC3635, ethernetCsmacd(6) should be used instead
                case 70: return "channel(70)"; //         -- channel
                case 71: return "ieee80211(71)"; //       -- radio spread spectrum
                case 72: return "ibm370parChan(72)"; //   -- IBM System 360/370 OEMI Channel
                case 73: return "escon(73)"; //           -- IBM Enterprise Systems Connection
                case 74: return "dlsw(74)"; //            -- Data Link Switching
                case 75: return "isdns(75)"; //           -- ISDN S/T interface
                case 76: return "isdnu(76)"; //           -- ISDN U interface
                case 77: return "lapd(77)"; //            -- Link Access Protocol D
                case 78: return "ipSwitch(78)"; //        -- IP Switching Objects
                case 79: return "rsrb(79)"; //            -- Remote Source Route Bridging
                case 80: return "atmLogical(80)"; //      -- ATM Logical Port
                case 81: return "ds0(81)"; //             -- Digital Signal Level 0
                case 82: return "ds0Bundle(82)"; //       -- group of ds0s on the same ds1
                case 83: return "bsc(83)"; //             -- Bisynchronous Protocol
                case 84: return "async(84)"; //           -- Asynchronous Protocol
                case 85: return "cnr(85)"; //             -- Combat Net Radio
                case 86: return "iso88025Dtr(86)"; //     -- ISO 802.5r DTR
                case 87: return "eplrs(87)"; //           -- Ext Pos Loc Report Sys
                case 88: return "arap(88)"; //            -- Appletalk Remote Access Protocol
                case 89: return "propCnls(89)"; //        -- Proprietary Connectionless Protocol
                case 90: return "hostPad(90)"; //         -- CCITT-ITU X.29 PAD Protocol
                case 91: return "termPad(91)"; //         -- CCITT-ITU X.3 PAD Facility
                case 92: return "frameRelayMPI(92)"; //   -- Multiproto Interconnect over FR
                case 93: return "x213(93)"; //            -- CCITT-ITU X213
                case 94: return "adsl(94)"; //            -- Asymmetric Digital Subscriber Loop
                case 95: return "radsl(95)"; //           -- Rate-Adapt.Digital Subscriber Loop
                case 96: return "sdsl(96)"; //            -- Symmetric Digital Subscriber Loop
                case 97: return "vdsl(97)"; //            -- Very H-Speed Digital Subscrib.Loop
                case 98: return "iso88025CRFPInt(98)"; //  -- ISO 802.5 CRFP
                case 99: return "myrinet(99)"; //         -- Myricom Myrinet
                case 100: return "voiceEM(100)"; //        -- voice recEive and transMit
                case 101: return "voiceFXO(101)"; //       -- voice Foreign Exchange Office
                case 102: return "voiceFXS(102)"; //       -- voice Foreign Exchange Station
                case 103: return "voiceEncap(103)"; //     -- voice encapsulation
                case 104: return "voiceOverIp(104)"; //    -- voice over IP encapsulation
                case 105: return "atmDxi(105)"; //         -- ATM DXI
                case 106: return "atmFuni(106)"; //        -- ATM FUNI
                case 107: return "atmIma(107)"; //        -- ATM IMA
                case 108: return "pppMultilinkBundle(108)"; //  -- PPP Multilink Bundle
                case 109: return "ipOverCdlc(109)"; //    -- IBM ipOverCdlc
                case 110: return "ipOverClaw(110)"; //    -- IBM Common Link Access to Workstn
                case 111: return "stackToStack(111)"; //  -- IBM stackToStack
                case 112: return "virtualIpAddress(112)"; //  -- IBM VIPA
                case 113: return "mpc(113)"; //           -- IBM multi-protocol channel support
                case 114: return "ipOverAtm(114)"; //     -- IBM ipOverAtm
                case 115: return "iso88025Fiber(115)"; //  -- ISO 802.5j Fiber Token Ring
                case 116: return "tdlc(116)"; // 	       -- IBM twinaxial data link control
                case 117: return "gigabitEthernet(117)"; //  -- Obsoleted via RFC3635, ethernetCsmacd(6) should be used instead
                case 118: return "hdlc(118)"; //          -- HDLC
                case 119: return "lapf(119)"; // 	       -- LAP F
                case 120: return "v37(120)"; // 	       -- V.37
                case 121: return "x25mlp(121)"; //        -- Multi-Link Protocol
                case 122: return "x25huntGroup(122)"; //  -- X25 Hunt Group
                case 123: return "transpHdlc(123)"; //    -- Transp HDLC
                case 124: return "interleave(124)"; //    -- Interleave channel
                case 125: return "fast(125)"; //          -- Fast channel
                case 126: return "ip(126)"; // 	       -- IP(for APPN HPR in IP networks)
                case 127: return "docsCableMaclayer(127)"; //   -- CATV Mac Layer
                case 128: return "docsCableDownstream(128)"; //  -- CATV Downstream interface
                case 129: return "docsCableUpstream (129)"; //   -- CATV Upstream interface
                case 130: return "a12MppSwitch (130)"; //  -- Avalon Parallel Processor
                case 131: return "tunnel(131)"; //        -- Encapsulation interface
                case 132: return "coffee (132)"; //        -- coffee pot
                case 133: return "ces(133)"; //           -- Circuit Emulation Service
                case 134: return "atmSubInterface(134)"; //  -- ATM Sub Interface
                case 135: return "l2vlan(135)"; //        -- Layer 2 Virtual LAN using 802.1Q
                case 136: return "l3ipvlan(136)"; //      -- Layer 3 Virtual LAN using IP
                case 137: return "l3ipxvlan(137)"; //     -- Layer 3 Virtual LAN using IPX
                case 138: return "digitalPowerline(138)"; //  -- IP over Power Lines
                case 139: return "mediaMailOverIp(139)"; //  -- Multimedia Mail over IP
                case 140: return "dtm(140)"; //         -- Dynamic syncronous Transfer Mode
                case 141: return "dcn(141)"; //     -- Data Communications Network
                case 142: return "ipForward(142)"; //     -- IP Forwarding Interface
                case 143: return "msdsl(143)"; //        -- Multi-rate Symmetric DSL
                case 144: return "ieee1394(144)"; //  -- IEEE1394 High Performance Serial Bus
                case 145: return "if-gsn(145)"; //        --   HIPPI-6400 
                case 146: return "dvbRccMacLayer(146)"; //  -- DVB-RCC MAC Layer
                case 147: return "dvbRccDownstream(147)"; //   -- DVB-RCC Downstream Channel
                case 148: return "dvbRccUpstream(148)"; //   -- DVB-RCC Upstream Channel
                case 149: return "atmVirtual(149)"; //    -- ATM Virtual Interface
                case 150: return "mplsTunnel(150)"; //    -- MPLS Tunnel Virtual Interface
                case 151: return "srp(151)"; // 	-- Spatial Reuse Protocol
                case 152: return "voiceOverAtm(152)"; //   -- Voice Over ATM
                case 153: return "voiceOverFrameRelay(153)"; //    -- Voice Over Frame Relay
                case 154: return "idsl(154)"; // 		-- Digital Subscriber Loop over ISDN
                case 155: return "compositeLink(155)"; //   -- Avici Composite Link Interface
                case 156: return "ss7SigLink(156)"; //      -- SS7 Signaling Link
                case 157: return "propWirelessP2P(157)"; //   --  Prop.P2P wireless interface
                case 158: return "frForward (158)"; //     -- Frame Forward Interface
                case 159: return "rfc1483(159)"; // 	-- Multiprotocol over ATM AAL5
                case 160: return "usb(160)"; // 		-- USB Interface
                case 161: return "ieee8023adLag(161)"; //   -- IEEE 802.3ad Link Aggregate
                case 162: return "bgppolicyaccounting(162)"; //  -- BGP Policy Accounting
                case 163: return "frf16MfrBundle(163)"; //  -- FRF .16 Multilink Frame Relay
                case 164: return "h323Gatekeeper(164)"; //  -- H323 Gatekeeper
                case 165: return "h323Proxy(165)"; //  -- H323 Voice and Video Proxy
                case 166: return "mpls(166)"; //  -- MPLS
                case 167: return "mfSigLink(167)"; //  -- Multi-frequency signaling link
                case 168: return "hdsl2(168)"; //  -- High Bit-Rate DSL - 2nd generation
                case 169: return "shdsl(169)"; //  -- Multirate HDSL2
                case 170: return "ds1FDL(170)"; //  -- Facility Data Link 4Kbps on a DS1
                case 171: return "pos(171)"; //  -- Packet over SONET/SDH Interface
                case 172: return "dvbAsiIn(172)"; //  -- DVB-ASI Input
                case 173: return "dvbAsiOut(173)"; //  -- DVB-ASI Output
                case 174: return "plc(174)"; //  -- Power Line Communtications
                case 175: return "nfas(175)"; //  -- Non Facility Associated Signaling
                case 176: return "tr008(176)"; //  -- TR008
                case 177: return "gr303RDT(177)"; //  -- Remote Digital Terminal
                case 178: return "gr303IDT(178)"; //  -- Integrated Digital Terminal
                case 179: return "isup(179)"; //  -- ISUP
                case 180: return "propDocsWirelessMaclayer(180)"; //  -- Cisco proprietary Maclayer
                case 181: return "propDocsWirelessDownstream(181)"; //  -- Cisco proprietary Downstream
                case 182: return "propDocsWirelessUpstream(182)"; //  -- Cisco proprietary Upstream
                case 183: return "hiperlan2(183)"; //  -- HIPERLAN Type 2 Radio Interface
                case 184: return "propBWAp2Mp(184)";
                // -- PropBroadbandWirelessAccesspt2multipt
                // use of this iftype for IEEE 802.16 WMAN interfaces as per IEEE Std 802.16f
                // is deprecated and ifType 237 should be used instead.
                case 185: return "sonetOverheadChannel (185)"; //  -- SONET Overhead Channel
                case 186: return "digitalWrapperOverheadChannel (186)"; //  -- Digital Wrapper
                case 187: return "aal2 (187)"; //  -- ATM adaptation layer 2
                case 188: return "radioMAC (188)"; //  -- MAC layer over radio links
                case 189: return "atmRadio (189)"; //  -- ATM over radio links
                case 190: return "imt (190)"; //  -- Inter Machine Trunks
                case 191: return "mvl (191)"; //  -- Multiple Virtual Lines DSL
                case 192: return "reachDSL (192)"; //  -- Long Reach DSL
                case 193: return "frDlciEndPt (193)"; //  -- Frame Relay DLCI End Point
                case 194: return "atmVciEndPt (194)"; //  -- ATM VCI End Point
                case 195: return "opticalChannel (195)"; //  -- Optical Channel
                case 196: return "opticalTransport (196)"; //  -- Optical Transport
                case 197: return "propAtm (197)"; //  --  Proprietary ATM
                case 198: return "voiceOverCable (198)"; //  -- Voice Over Cable Interface
                case 199: return "infiniband (199)"; //  -- Infiniband
                case 200: return "teLink (200)"; //  -- TE Link
                case 201: return "q2931 (201)"; //  -- Q.2931
                case 202: return "virtualTg (202)"; //  -- Virtual Trunk Group
                case 203: return "sipTg (203)"; //  -- SIP Trunk Group
                case 204: return "sipSig (204)"; //  -- SIP Signaling
                case 205: return "docsCableUpstreamChannel (205)"; //  -- CATV Upstream Channel
                case 206: return "econet (206)"; //  -- Acorn Econet
                case 207: return "pon155 (207)"; //  -- FSAN 155Mb Symetrical PON interface
                case 208: return "pon622 (208)"; //  -- FSAN622Mb Symetrical PON interface
                case 209: return "bridge (209)"; //  -- Transparent bridge interface
                case 210: return "linegroup (210)"; //  -- Interface common to multiple lines
                case 211: return "voiceEMFGD(211)"; //  -- voice E&M Feature Group D
                case 212: return "voiceFGDEANA(212)"; //  -- voice FGD Exchange Access North American
                case 213: return "voiceDID(213)"; //  -- voice Direct Inward Dialing
                case 214: return "mpegTransport(214)"; //  -- MPEG transport interface
                case 215: return "sixToFour (215)"; //  -- 6to4 interface (DEPRECATED)
                case 216: return "gtp(216)"; //  -- GTP(GPRS Tunneling Protocol)
                case 217: return "pdnEtherLoop1(217)"; //  -- Paradyne EtherLoop 1
                case 218: return "pdnEtherLoop2(218)"; //  -- Paradyne EtherLoop 2
                case 219: return "opticalChannelGroup(219)"; //  -- Optical Channel Group
                case 220: return "homepna(220)"; //  -- HomePNA ITU-T G.989
                case 221: return "gfp(221)"; //  -- Generic Framing Procedure(GFP)
                case 222: return "ciscoISLvlan(222)"; //  -- Layer 2 Virtual LAN using Cisco ISL
                case 223: return "actelisMetaLOOP(223)"; //  -- Acteleis proprietary MetaLOOP High Speed Link
                case 224: return "fcipLink(224)"; //  -- FCIP Link
                case 225: return "rpr(225)"; //  -- Resilient Packet Ring Interface Type
                case 226: return "qam(226)"; //  -- RF Qam Interface
                case 227: return "lmp(227)"; //  -- Link Management Protocol
                case 228: return "cblVectaStar(228)"; //  -- Cambridge Broadband Networks Limited VectaStar
                case 229: return "docsCableMCmtsDownstream(229)"; //  -- CATV Modular CMTS Downstream Interface
                case 230: return "adsl2(230)"; //  -- Asymmetric Digital Subscriber Loop Version 2 
                // -- (DEPRECATED/OBSOLETED - please use adsl2plus 238 instead)
                case 231: return "macSecControlledIF(231)"; //  -- MACSecControlled
                case 232: return "macSecUncontrolledIF(232)"; //  -- MACSecUncontrolled
                case 233: return "aviciOpticalEther(233)"; //  -- Avici Optical Ethernet Aggregate
                case 234: return "atmbond(234)"; //  -- atmbond
                case 235: return "voiceFGDOS(235)"; //  -- voice FGD Operator Services
                case 236: return "mocaVersion1(236)"; //  -- MultiMedia over Coax Alliance(MoCA) Interface
                // -- as documented in information provided privately to IANA
                case 237: return "ieee80216WMAN(237)"; //  -- IEEE 802.16 WMAN interface
                case 238: return "adsl2plus (238)"; //  -- Asymmetric Digital Subscriber Loop Version 2, 
                // -- Version 2 Plus and all variants
                case 239: return "dvbRcsMacLayer(239)"; //  -- DVB-RCS MAC Layer
                case 240: return "dvbTdm(240)"; //  -- DVB Satellite TDM
                case 241: return "dvbRcsTdma(241)"; //  -- DVB-RCS TDMA
                case 242: return "x86Laps(242)"; //  -- LAPS based on ITU-T X.86/Y.1323
                case 243: return "wwanPP(243)"; //  -- 3GPP WWAN
                case 244: return "wwanPP2(244)"; //  -- 3GPP2 WWAN
                case 245: return "voiceEBS(245)"; //  -- voice P-phone EBS physical interface
                case 246: return "ifPwType (246)"; //  -- Pseudowire interface type
                case 247: return "ilan(247)"; //  -- Internal LAN on a bridge per IEEE 802.1ap
                case 248: return "pip(248)"; //  -- Provider Instance Port on a bridge per IEEE 802.1ah PBB
                case 249: return "aluELP(249)"; //  -- Alcatel-Lucent Ethernet Link Protection
                case 250: return "gpon(250)"; //  -- Gigabit-capable passive optical networks(G-PON) as per ITU-T G.948
                case 251: return "vdsl2(251)"; //  -- Very high speed digital subscriber line Version 2 (as per ITU-T Recommendation G.993.2)
                case 252: return "capwapDot11Profile(252)"; //  -- WLAN Profile Interface
                case 253: return "capwapDot11Bss(253)"; //  -- WLAN BSS Interface
                case 254: return "capwapWtpVirtualRadio(254)"; //  -- WTP Virtual Radio Interface
                case 255: return "bits(255)"; //  -- bitsport
                case 256: return "docsCableUpstreamRfPort(256)"; //  -- DOCSIS CATV Upstream RF Port
                case 257: return "cableDownstreamRfPort(257)"; //  -- CATV downstream RF port
                case 258: return "vmwareVirtualNic(258)"; //  -- VMware Virtual Network Interface
                case 259: return "ieee802154(259)"; //  -- IEEE 802.15.4 WPAN interface
                case 260: return "otnOdu (260)"; //  -- OTN Optical Data Unit
                case 261: return "otnOtu(261)"; //  -- OTN Optical channel Transport Unit
                case 262: return "ifVfiType(262)"; //  -- VPLS Forwarding Instance Interface Type
                case 263: return "g9981(263)"; //  -- G.998.1 bonded interface
                case 264: return "g9982 (264)"; //  -- G.998.2 bonded interface
                case 265: return "g9983 (265)"; //  -- G.998.3 bonded interface
                case 266: return "aluEpon (266)"; //  -- Ethernet Passive Optical Networks(E-PON)
                case 267: return "aluEponOnu(267)"; //  -- EPON Optical Network Unit
                case 268: return "aluEponPhysicalUni(268)"; //  -- EPON physical User to Network interface
                case 269: return "aluEponLogicalLink (269)"; //  -- The emulation of a point-to-point link over the EPON layer
                case 270: return "aluGponOnu(270)"; //  -- GPON Optical Network Unit
                case 271: return "aluGponPhysicalUni(271)"; //  -- GPON physical User to Network interface
                case 272: return "vmwareNicTeam (272)"; //  -- VMware NIC Team
                case 277: return "docsOfdmDownstream(277)"; //  -- CATV Downstream OFDM interface
                case 278: return "docsOfdmaUpstream (278)"; //  -- CATV Upstream OFDMA interface
                case 279: return "gfast (279)"; //  -- G.fast port
                case 280: return "sdci(280)"; //  -- SDCI(IO-Link)
                case 281: return "xboxWireless(281)"; //  -- Xbox wireless
                case 282: return "fastdsl(282)"; //  -- FastDSL
                case 283: return "docsCableScte55d1FwdOob(283)"; //  -- Cable SCTE 55-1 OOB Forward Channel
                case 284: return "docsCableScte55d1RetOob(284)"; //  -- Cable SCTE 55-1 OOB Return Channel
                case 285: return "docsCableScte55d2DsOob(285)"; //  -- Cable SCTE 55-2 OOB Downstream Channel
                case 286: return "docsCableScte55d2UsOob(286)"; //  -- Cable SCTE 55-2 OOB Upstream Channel
                case 287: return "docsCableNdf(287)"; //  -- Cable Narrowband Digital Forward
                case 288: return "docsCableNdr(288)"; //  -- Cable Narrowband Digital Return
                case 289: return "ptm(289)"; //  -- Packet Transfer Mode
                case 290: return "ghn(290)"; //  -- G.hn port
                case 291: return "otnOtsi(291)"; //  -- Optical Tributary Signal
                case 292: return "otnOtuc(292)"; //  -- OTN OTUCn
                case 293: return "otnOduc(293)"; //  -- OTN ODUC
                case 294: return "otnOtsig(294)"; // -- OTN OTUC Signal
                default:
                    return "UNKNOWN IANA Interface Type: " + type;

            }
            /*
                Tunnel types
                    case 1: return "other(1)"; //          -- none of the following
                    case 2: return "direct(2)"; //         -- no intermediate header
                    case 3: return "gre(3)"; //            -- GRE encapsulation
                    case 4: return "minimal(4)"; //        -- Minimal encapsulation
                    case 5: return "l2tp(5)"; //           -- L2TP encapsulation
                    case 6: return "pptp(6)"; //           -- PPTP encapsulation
                    case 7: return "l2f(7)"; //            -- L2F encapsulation
                    case 8: return "udp(8)"; //            -- UDP encapsulation
                    case 9: return "atmp(9)"; //           -- ATMP encapsulation
                    case 10: return "msdp(10)"; //          -- MSDP encapsulation
                    case 11: return "sixToFour(11)"; //     -- 6to4 encapsulation
                    case 12: return "sixOverFour(12)"; //   -- 6over4 encapsulation
                    case 13: return "isatap(13)"; //        -- ISATAP encapsulation
                    case 14: return "teredo(14)"; //        -- Teredo encapsulation
                    case 15: return "ipHttps(15)"; //       -- IPHTTPS
                    case 16: return "softwireMesh(16)" // -- softwire mesh tunnel
                    case 17: return "dsLite(17)" //        -- DS-Lite tunnel
            */
        }
    }
}