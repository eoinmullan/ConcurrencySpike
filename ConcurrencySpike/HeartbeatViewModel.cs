using System.ComponentModel;

namespace ConcurrencySpike {
    public abstract class HeartbeatViewModel : INotifyPropertyChanged {
        private long heartbeatLatency = 0;
        private bool heartbeatIndicatorOn;
        private int heartbeatCount = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public long HeartbeatLatency {
            get { return heartbeatLatency; }
            protected set {
                heartbeatLatency = value;
                RaisePropertyChanged(nameof(HeartbeatLatency));
            }
        }

        public bool HeartbeatIndicatorOn {
            get { return heartbeatIndicatorOn; }
            protected set {
                heartbeatIndicatorOn = value;
                RaisePropertyChanged(nameof(HeartbeatIndicatorOn));
            }
        }

        public int HeartbeatCount {
            get { return heartbeatCount; }
            protected set {
                heartbeatCount = value;
                RaisePropertyChanged(nameof(HeartbeatCount));
            }
        }

        public abstract string Description { get; }
        public abstract bool HeartbeatOn { get; set; }

        protected void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
