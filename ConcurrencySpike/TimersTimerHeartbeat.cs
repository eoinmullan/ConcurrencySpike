using System.Diagnostics;
using System.Timers;

namespace ConcurrencySpike {
    public class TimersTimerHeartbeat : Heartbeat {
        private Timer timersTimerHeartbeat;

        public override string Description => "System.Timers.Timer Heartbeat";

        public override bool HeartbeatOn {
            get { return timersTimerHeartbeat.Enabled; }
            set {
                timersTimerHeartbeat.Enabled = value;
                RaisePropertyChanged(nameof(HeartbeatOn));
            }
        }

        public TimersTimerHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            timersTimerHeartbeat = new Timer(heartbeatInterval) { AutoReset = true };
            timersTimerHeartbeat.Elapsed += DoTimersTimerHeartbeat;
            timersTimerHeartbeat.Start(); ;
        }

        private void DoTimersTimerHeartbeat(object sender, ElapsedEventArgs e) {
            DoHeartbeat();
        }
    }
}
