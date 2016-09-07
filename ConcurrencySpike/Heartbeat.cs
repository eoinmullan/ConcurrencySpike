using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public abstract class Heartbeat : INotifyPropertyChanged {
        private long lastHeartbeatTime;
        private long heartbeatLatency = 0;
        private bool heartbeatIndicatorOn;
        private readonly Stopwatch stopwatch;

        public event PropertyChangedEventHandler PropertyChanged;

        public Heartbeat(Stopwatch stopwatch, int heartbeatInterval) {
            this.stopwatch = stopwatch;
        }

        protected void DoHeartbeat() {
            HeartbeatLatency = stopwatch.ElapsedMilliseconds - lastHeartbeatTime;
            lastHeartbeatTime = stopwatch.ElapsedMilliseconds;
            HeartbeatIndicatorOn = true;
            Task.Run(async () => {
                await Task.Delay(150);
                HeartbeatIndicatorOn = false;
            });
        }

        public long HeartbeatLatency {
            get { return heartbeatLatency; }
            set {
                heartbeatLatency = value;
                RaisePropertyChanged(nameof(HeartbeatLatency));
            }
        }

        public bool HeartbeatIndicatorOn {
            get { return heartbeatIndicatorOn; }
            set {
                heartbeatIndicatorOn = value;
                RaisePropertyChanged(nameof(HeartbeatIndicatorOn));
            }
        }

        public abstract bool HeartbeatOn { get; set; }

        protected void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
