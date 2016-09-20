using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class ThreadTriggeredQueueJumpingHeartbeat : Heartbeat {
        private QueueJumpableTaskScheduler queueJumpableTaskScheduler;

        public override string Description => "Queue Jumping Task Heartbeat";

        public ThreadTriggeredQueueJumpingHeartbeat(Stopwatch stopwatch, QueueJumpableTaskScheduler queueJumpableTaskScheduler, DedicatedThreadHeartbeatEventSource dedicatedThreadHeartbeatEventSource)
            : base(stopwatch) {
            this.queueJumpableTaskScheduler = queueJumpableTaskScheduler;
            HeartbeatOn = true;

            dedicatedThreadHeartbeatEventSource.Heartbeat += (s, e) => {
                if (HeartbeatOn) {
                    Task.Factory.StartNew(
                        () => DoHeartbeat(),
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        queueJumpableTaskScheduler.FrontOfQueue);
                }
            };
        }
    }
}
