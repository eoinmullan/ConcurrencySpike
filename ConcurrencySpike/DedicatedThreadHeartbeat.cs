using System;
using System.Diagnostics;
using System.Threading;

namespace ConcurrencySpike {
    internal class DedicatedThreadHeartbeat : Heartbeat {
        private Thread heartbeatThread;
        private bool heartbeatOn;
        private int heartbeatInterval;

        public override string Description => "Dedicated Thread Heartbeat";

        public DedicatedThreadHeartbeat(Stopwatch stopwatch, int heartbeatInterval) : base(stopwatch, heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            HeartbeatOn = true;
        }

        private void BeginHeartbeat() {
            while (heartbeatOn) {
                Thread.Sleep(heartbeatInterval);
                DoHeartbeat();
            }
        }

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