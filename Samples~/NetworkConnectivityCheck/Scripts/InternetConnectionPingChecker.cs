using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities.Networking.Pings;

public class InternetConnectionPingChecker : MonoBehaviour
{
    [Tooltip("Measured in seconds")]
    [SerializeField][Range(1, 30)] private int pingTimeout = 10;
#if UNITY_EDITOR
    [SerializeField] private bool shouldLogResults = true;
#endif

    private CancellationTokenSource pingCheckingCTS;

    private void OnDisable()
    {
        CancelPingCheckingCTS();
    }

    private void Start()
    {
        Task task = IsInternetConnectionAvailablePingCheck();
    }

    private async Task<bool> IsInternetConnectionAvailablePingCheck()
    {
        if (!Application.isPlaying) return false;

        HashSet<string> addresses = UnityPingHelper.GetPingAddresses();

        List<Task<UnityPingReply>> pingTasks = new List<Task<UnityPingReply>>();

        CancelPingCheckingCTS();
        pingCheckingCTS = new CancellationTokenSource();
        CancellationToken pingCheckingToken = pingCheckingCTS.Token;

        foreach (string address in addresses)
        {
            Task<UnityPingReply> pingTask = UnityPing.SendPingAsync(address, pingTimeout);
            pingTasks.Add(pingTask);
        }

        while (Application.isPlaying && pingTasks.Count > 0 && !pingCheckingToken.IsCancellationRequested)
        {
            Task<UnityPingReply> replyTask = await Task.WhenAny(pingTasks);
            pingTasks.Remove(replyTask);

            UnityPingReply reply = replyTask.Result;

            // Sometimes the IPV4 address can resolve to a local IP address with some very odd setups of connecting to the internet
            // I've had cases where the address would resolve to 192.168.0.1 or 127.0.0.1, so it's best to perform this check if having real data about a person's connectivity is essential
            if (UnityPingHelper.IsLocalOrSpecialIPV4Address(reply.Address))
            {
#if UNITY_EDITOR
                if (shouldLogResults)
                {
                    Debug.LogWarning($"Pinged something private at {reply.Address}");
                }
#endif
                continue;
            }

#if UNITY_EDITOR
            if (shouldLogResults)
            {
                Debug.Log($"Ping: {reply.Address},\n" +
                    $"successful: {reply.IsSuccessful},\n" +
                    $"ping time: {reply.RoundtripTime}ms\n" +
                    $"error: {reply.Error}");
            }
#endif

            if (reply.IsSuccessful)
            {
                CancelPingCheckingCTS();
                return true;
            }
        }

        if (!Application.isPlaying) return false;

#if UNITY_EDITOR
        if (shouldLogResults)
        {
            Debug.Log("All pings have failed!\n" +
                $"attempted {addresses.Count} pings");
        }
#endif

        return false;
    }

    private void CancelPingCheckingCTS()
    {
        pingCheckingCTS?.Cancel();
        pingCheckingCTS = null;
    }
}
