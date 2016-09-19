using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class ThreadTriggeredTaskRunHeartbeat : DedicatedThreadHeartbeatBase {
        private readonly int heartbeatInterval;

        public ThreadTriggeredTaskRunHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            HeartbeatOn = true;
        }

        public override string Description => "Task Run Heartbeat";

        protected override void BeginHeartbeat() {
            while (heartbeatOn) {
                Thread.Sleep(heartbeatInterval);
                Task.Run(() => DoHeartbeat());
            }
        }
    }
}
