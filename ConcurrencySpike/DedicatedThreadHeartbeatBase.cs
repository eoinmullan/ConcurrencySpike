using System.Diagnostics;
using System.Threading;

namespace ConcurrencySpike {
    public abstract class DedicatedThreadHeartbeatBase : Heartbeat {
        private Thread heartbeatThread;
        protected bool heartbeatOn;

        public DedicatedThreadHeartbeatBase(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
        }

        protected abstract void BeginHeartbeat();

        public override bool HeartbeatOn {
            get { return heartbeatOn; }
            set {
                heartbeatOn = value;
                if (value) {
                    heartbeatThread = new Thread(BeginHeartbeat);
                    heartbeatThread.IsBackground = true;
                    heartbeatThread.Start();
                }
                else {
                    heartbeatThread.Join();
                }
                RaisePropertyChanged(nameof(HeartbeatOn));
            }
        }
    }
}
