using System;
using System.Diagnostics;
using System.Threading;

namespace ConcurrencySpike {
    internal class ThreadTriggeredSynchronousHeartbeat : Heartbeat {
        public override string Description => "Dedicated Thread Heartbeat";

        public ThreadTriggeredSynchronousHeartbeat(Stopwatch stopwatch, DedicatedThreadHeartbeatEventSource dedicatedThreadHeartbeatEventSource) : base(stopwatch) {
            HeartbeatOn = true;

            dedicatedThreadHeartbeatEventSource.Heartbeat += (s, e) => {
                if (HeartbeatOn) {
                    DoHeartbeat();
                }
            };
        }
    }
}