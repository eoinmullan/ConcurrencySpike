using System.Diagnostics;
using System.Threading;

namespace ConcurrencySpike {
    internal class ThreadTriggeredSynchronousHeartbeat : DedicatedThreadHeartbeatBase {
        private int heartbeatInterval;

        public override string Description => "Dedicated Thread Heartbeat";

        public ThreadTriggeredSynchronousHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            HeartbeatOn = true;
        }

        protected override void BeginHeartbeat() {
            while (heartbeatOn) {
                Thread.Sleep(heartbeatInterval);
                DoHeartbeat();
            }
        }
    }
}