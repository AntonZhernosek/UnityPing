using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Ping = UnityEngine.Ping;

namespace UnityUtilities.Networking.Pings
{
    public static class UnityPingExtensions
    {
        /// <summary>
        /// Await for a UnityPing using an awaiter and get an informative object of the ping's result in return
        /// </summary>
        /// <param name="ping"></param>
        /// <param name="timeout">Ping timeout in seconds</param>
        /// <param name="ct">CancellationToken for the async operation. Will dispose of the ping if it's cancelled</param>
        /// <returns>A UnityPingReply object</returns>
        public static async Task<UnityPingReply> AwaitAsync(this Ping ping, int timeout = 20, CancellationToken ct = default)
        {
            if (ct.CanBeCanceled)
            {
                ct.Register(ping.DestroyPing);
            }

            DateTime startTime = DateTime.Now;

            while (Application.isPlaying 
                && ping != null 
                && !ping.isDone
                && !ct.IsCancellationRequested
                && !HasTimedOut(startTime, timeout)) await Task.Yield();

            if (ping == null)
            {
                return new UnityPingReply(string.Empty, UnityPingReply.Ping_Disposed_Error);
            }

            if (!Application.isPlaying)
            {
                return new UnityPingReply(ping.ip, UnityPingReply.Application_Exit_Error);
            }

            if (ct.IsCancellationRequested)
            {
                return new UnityPingReply(ping.ip, UnityPingReply.Cancellation_Requested_Error);
            }

            if (HasTimedOut(startTime, timeout))
            {
                return new UnityPingReply(ping.ip, UnityPingReply.Timeout_Error);
            }

            // Unity ping is successful only if both conditions are true
            // If the address is invalid or any other error occurs, the ping will return as done with time = -1
            if (ping.isDone && ping.time > 0)
            {
                return new UnityPingReply(true, ping.ip, ping.time);
            }

            // Should probably not see this scenario, unless deliberately providing an invalid ping address
            return new UnityPingReply(ping.ip, UnityPingReply.Unaccounted_Error);
        }

        private static bool HasTimedOut(DateTime startTime, int timeout)
        {
            TimeSpan span = DateTime.Now - startTime;
            return span.TotalSeconds >= timeout;
        }
    }
}