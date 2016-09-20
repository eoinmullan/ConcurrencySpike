using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class ThreadTriggeredTaskRunHeartbeat : Heartbeat {
        public override string Description => "Task Run Heartbeat";

        public ThreadTriggeredTaskRunHeartbeat(Stopwatch stopwatch, DedicatedThreadHeartbeatEventSource dedicatedThreadHeartbeatEventSource) : base(stopwatch) {
            HeartbeatOn = true;

            dedicatedThreadHeartbeatEventSource.Heartbeat += (s, e) => {
                if (HeartbeatOn) {
                    Task.Run(() => DoHeartbeat());
                }
            };
        }
    }
}
