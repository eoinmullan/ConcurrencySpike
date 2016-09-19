using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class TaskDelayHeartbeat : Heartbeat {
        private bool heartbeatOn;
        private int heartbeatInterval;
        private CancellationTokenSource cts;
        private CancellationToken ct;

        public override string Description => "Task Delay Heartbeat";

        public TaskDelayHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            HeartbeatOn = true;
        }

        private async void BeginHeartbeat() {
            while (!ct.IsCancellationRequested) {
                await Task.Delay(heartbeatInterval);
                DoHeartbeat();
            }
        }

        public override bool HeartbeatOn {
            get { return heartbeatOn; }
            set {
                heartbeatOn = value;
                if (value) {
                    cts = new CancellationTokenSource();
                    ct = cts.Token;
                    BeginHeartbeat();
                }
                else {
                    cts.Cancel();
                }
                RaisePropertyChanged(nameof(HeartbeatOn));
            }
        }
    }
}
