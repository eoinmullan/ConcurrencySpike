using System.Threading;
using System.Diagnostics;
using System;

namespace ConcurrencySpike {
    public class ThreadingTimerHeartbeat : Heartbeat {
        private Timer heartbeatTimer;
        private bool heartbeatOn = true;
        private int heartbeatInterval;

        public override string Description => "System.Threading.Timer Heartbeat";

        public override bool HeartbeatOn {
            get { return heartbeatOn; }
            set {
                heartbeatOn = value;
                if (heartbeatOn) {
                    heartbeatTimer.Change(0, heartbeatInterval);
                }
                else {
                    heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                RaisePropertyChanged(nameof(HeartbeatOn));
            }
        }

        public ThreadingTimerHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            heartbeatTimer = new Timer(DoHeartbeat, null, 0, heartbeatInterval);
        }

        private void DoHeartbeat(object state) {
            DoHeartbeat();
        }
    }
}
