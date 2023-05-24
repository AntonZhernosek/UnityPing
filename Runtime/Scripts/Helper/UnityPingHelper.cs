using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UnityUtilities.Networking.Pings
{
    public class UnityPingHelper
    {
        private static HashSet<string> PingAddresses = new HashSet<string>()
        {
            // Cloudflare
            "1.1.1.1",
            "1.0.0.1",
            // Level3 open DNS server
            "4.2.2.2",
            // Open DNS
            "208.67.222.222",
            "208.67.220.220",
            // Google DNS
            "8.8.8.8",
            "8.8.4.4",
            // Websites you'd always expect to be up and running
            "www.google.com",
            "www.microsoft.com",
        };

        #region IPV4 Addresses

        public static async Task<string> ResolveToIPV4Address(string address)
        {
            try
            {
                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(address);
                IPAddress ipAddress = ipHostInfo.AddressList
                    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                return ipAddress.ToString();
            }
#if UNITY_EDITOR
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                return null;
            }
#else
            catch 
            { 
                return null; 
            }
#endif
        }

        public static bool IsLocalOrSpecialIPV4Address(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return false;

            try
            {
                int[] addressInts = address.Split('.').Select(x => int.Parse(x)).ToArray();

                int zero = addressInts[0];
                // 10.0.0.0- 10.255.255.255
                if (zero == 10) return true;

                // 172.16.0.0-172.31.255.255
                if (zero == 172)
                {
                    int one = addressInts[1];
                    return one >= 16 && one <= 31;
                }

                // 192.168.0.0-192.168.255.255
                if (zero == 192) return addressInts[1] == 168;

                // 100.x.x.x
                if (zero == 100) return true;

                // 169.254.1.0-169.254.254.255
                if (zero == 169) return addressInts[1] == 254;

                return string.Equals(address, "127.0.0.1");
            }
            catch 
            { 
                return false; 
            }
        }

        #endregion

        #region Item List

        /// <summary>
        /// Get a list of DNS addresses to try and send pings to.
        /// Provides a default list of commonly used DNS addresses and websites that you would expect to always be up and running
        /// </summary>
        /// <param name="additionalAddresses">Adds a set of additional addresses that you can try pinging. When using this function, this really shouldn't be needed</param>
        /// <returns>A HashSet of unique addresses that you can try pinging to check internet connectivity</returns>
        public static HashSet<string> GetPingAddresses(IEnumerable<string> additionalAddresses = null)
        {
            HashSet<string> addresses = new HashSet<string>(PingAddresses);

            if (additionalAddresses != null && additionalAddresses.Any())
            {
                foreach (string address in additionalAddresses)
                {
                    addresses.Add(address);
                }
            }

            string cultureName = CultureInfo.InstalledUICulture.Name;
            switch (cultureName)
            {
                // China
                case string when cultureName.StartsWith("zh"):
                    addresses.Add("www.baidu.com");
                    break;

                // Iran
                case string when cultureName.StartsWith("fa"):
                    addresses.Add("www.aparat.com");
                    break;
            }

            return addresses;
        }

        #endregion
    }
}