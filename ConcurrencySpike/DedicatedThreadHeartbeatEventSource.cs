using System;
using System.Threading;

namespace ConcurrencySpike {
    public class DedicatedThreadHeartbeatEventSource {
        public event EventHandler Heartbeat;
        private Thread heartbeatThread;
        private int heartbeatInterval;

        public DedicatedThreadHeartbeatEventSource(int heartbeatInterval) {
            this.heartbeatInterval = heartbeatInterval;
            heartbeatThread = new Thread(GenerateHeartbeat);
            heartbeatThread.IsBackground = true;
            heartbeatThread.Start();
        }

        private void GenerateHeartbeat() {
            while (true) {
                Thread.Sleep(heartbeatInterval);
                Heartbeat?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
