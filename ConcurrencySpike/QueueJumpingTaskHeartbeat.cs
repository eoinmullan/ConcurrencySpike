using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class ThreadTriggeredQueueJumpingHeartbeat : DedicatedThreadHeartbeatBase {
        private readonly int heartbeatInterval;
        private QueueJumpableTaskScheduler queueJumpableTaskScheduler;

        public ThreadTriggeredQueueJumpingHeartbeat(Stopwatch stopwatch, int heartbeatInterval, QueueJumpableTaskScheduler queueJumpableTaskScheduler)
            : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            this.queueJumpableTaskScheduler = queueJumpableTaskScheduler;
            HeartbeatOn = true;
        }

        public override string Description => "Queue Jumping Task Heartbeat";

        protected override void BeginHeartbeat() {
            while (heartbeatOn) {
                Thread.Sleep(heartbeatInterval);
                Task.Factory.StartNew(
                    () => DoHeartbeat(),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    queueJumpableTaskScheduler.FrontOfQueue);
            }
        }
    }
}
