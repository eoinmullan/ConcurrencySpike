using System.Diagnostics;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public abstract class Heartbeat : HeartbeatViewModel {
        private long lastHeartbeatTime;
        private readonly Stopwatch stopwatch;

        public Heartbeat(Stopwatch stopwatch, int heartbeatInterval) {
            this.stopwatch = stopwatch;
        }

        protected void DoHeartbeat() {
            HeartbeatLatency = stopwatch.ElapsedMilliseconds - lastHeartbeatTime;
            lastHeartbeatTime = stopwatch.ElapsedMilliseconds;
            HeartbeatIndicatorOn = true;
            HeartbeatCount++;
            Task.Run(async () => {
                await Task.Delay(150);
                HeartbeatIndicatorOn = false;
            });
        }
    }
}
