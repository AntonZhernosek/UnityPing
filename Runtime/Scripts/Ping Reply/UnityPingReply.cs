namespace UnityUtilities.Networking.Pings
{
    public class UnityPingReply
    {
        protected bool isSuccessful;
        protected string address;
        protected string error;
        protected int roundtripTime;

        public const string Invalid_Address_Error = "Invalid address";
        public const string Ping_Disposed_Error = "Ping has been disposed";
        public const string Application_Exit_Error = "Application exit";
        public const string Cancellation_Requested_Error = "Ping has been cancelled";
        public const string Timeout_Error = "Ping has timed out";
        public const string Unaccounted_Error = "Unaccounted ping error occured";

        public bool IsSuccessful { get { return isSuccessful; } }
        public string Address { get { return address; } }
        public string Error { get { return error; } }
        public int RoundtripTime { get { return roundtripTime; } }

        public UnityPingReply(bool isSuccessful, string address, int roundtripTime)
        {
            this.isSuccessful = isSuccessful;
            this.address = address;
            this.roundtripTime = roundtripTime;
        }

        public UnityPingReply(string address, string error)
        {
            this.address = address;
            SetAsUnsuccessfulInternal(error);
        }

        public virtual void SetAsUnsuccessful(string error)
        {
            SetAsUnsuccessfulInternal(error);
        }

        protected virtual void SetAsUnsuccessfulInternal(string error)
        {
            isSuccessful = false;
            this.error = error;
            roundtripTime = -1;
        }
    }
}