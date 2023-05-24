using System.Threading;
using System.Threading.Tasks;
using Ping = UnityEngine.Ping;

namespace UnityUtilities.Networking.Pings
{
    public static class UnityPing
    {
        /// <summary>
        /// Send a UnityPing with a custom awaiter and an informative UnityPingReply object to receive the results of the ping
        /// </summary>
        /// <param name="address">The address that you want to ping. This address wil resolve to an IPV4 address if it's not</param>
        /// <param name="timeout">Ping timeout in seconds</param>
        /// <param name="ct">Cancellation Token for the async operation. Will dispose of the ping if it's cancelled</param>
        /// <returns>A UnityPingReply object</returns>
        public static async Task<UnityPingReply> SendPingAsync(string address, int timeout = 20, CancellationToken ct = default)
        {
            string resolvedAddress = await UnityPingHelper.ResolveToIPV4Address(address);

            if (string.IsNullOrWhiteSpace(resolvedAddress))
            {
                return new UnityPingReply(address, UnityPingReply.Invalid_Address_Error);
            }

            Ping ping = new Ping(resolvedAddress);
            return await ping.AwaitAsync(timeout, ct);
        }
    }
}