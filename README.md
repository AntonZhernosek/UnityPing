### UnityPing
Dotnet Ping has issues around working in builds and improperly resolving addresses. UnityEngine's Ping works well in builds, but it's primitive and needs wrappers to function properly. This extension wraps around Unity Engine's Ping, allowing it to resolve addresses, timeout, support async/await and return a meaningful UnityPingReply object from a ping attempt.\n
All logged exceptions and Debug.Logs in the code are UnityEditor only.

### Example Usage
```csharp
public class ExampleClass : MonoBehaviour 
{
    private async Task PingSomething(string address, int pingTimeout, CancellationToken ct)
    {
        UnityPingReply pingReply = await UnityPing.SendPingAsync(address, pingTimeout, ct);

        if (pingReply.IsSuccessful)
        {
            Debug.Log(pingReply.RoundtripTime);
            // Do stuff
        }
        else
        {
            Debug.Log(pingReply.Error);
            // Do other stuff
        }
    }
}
```

### Samples
Samples contains a small scene and a script that can be used to check if a player is connected to the internet via UnityPing. PingChecker is a small object that will test internet connectivity by using UnityPing and can print the results to console
![unity_inspector](https://i.imgur.com/CJ2PMwb.png)