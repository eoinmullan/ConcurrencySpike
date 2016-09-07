using System.Threading;
using System.Diagnostics;

namespace ConcurrencySpike {
    public class ThreadingTimerHeartbeat : Heartbeat {
        private Timer heartbeatTimer;

        public string Description => "System.Threading.Timer Heartbeat";

        public ThreadingTimerHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            heartbeatTimer = new Timer(DoHeartbeat, null, 0, heartbeatInterval);
        }

        private void DoHeartbeat(object state) {
            DoHeartbeat();
        }
    }
}
