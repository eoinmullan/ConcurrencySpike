using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConcurrencySpike {
    public class MainWindowViewModel : INotifyPropertyChanged { 
        private const int HeartbeatInterval = 1000;

        private int threadPoolThreadsInUse;
        private int workUnitDurationMs = 1000;
        private int workUnitIntervalMs = 1000;
        private long taskLatency = 0;

        private Stopwatch stopwatch = new Stopwatch();
        private Thread threadPoolMonitorThread;
        private Thread variableWorkloadThread;

        public event PropertyChangedEventHandler PropertyChanged;

        public Heartbeat ThreadingTimerHeartbeat => new ThreadingTimerHeartbeat(stopwatch, HeartbeatInterval);
        public Heartbeat TimersTimerHeartbeat => new TimersTimerHeartbeat(stopwatch, HeartbeatInterval);

        public MainWindowViewModel() {
            stopwatch.Start();
            StartVariableWorkloadThread();
            StartThreadPoolMonitor();
        }

        private void StartThreadPoolMonitor() {
            threadPoolMonitorThread = new Thread(() => {
                int maxThreads = 0;
                int completionPortThreads = 0;
                int availableThreads = 0;
                while (true) {
                    ThreadPool.GetMaxThreads(out maxThreads, out completionPortThreads);
                    ThreadPool.GetAvailableThreads(out availableThreads, out completionPortThreads);
                    ThreadPoolthreadsInUse = maxThreads - availableThreads;
                }
            });
            threadPoolMonitorThread.IsBackground = true;
            threadPoolMonitorThread.Start();
        }

        public int ThreadPoolthreadsInUse {
            get { return threadPoolThreadsInUse; }
            set {
                if (value != threadPoolThreadsInUse) {
                    threadPoolThreadsInUse = value;
                    RaisePropertyChanged(nameof(ThreadPoolthreadsInUse));
                }
            }
        }

        public int WorkUnitDurationMs {
            get { return workUnitDurationMs; }
            set {
                workUnitDurationMs = value;
                RaisePropertyChanged(nameof(WorkUnitDurationMs));
            }
        }

        public int WorkUnitIntervalMs {
            get { return workUnitIntervalMs; }
            set {
                workUnitIntervalMs = value;
                RaisePropertyChanged(nameof(WorkUnitIntervalMs));
            }
        }

        public long TaskLatency {
            get { return taskLatency; }
            set {
                taskLatency = value;
                RaisePropertyChanged(nameof(TaskLatency));
            }
        }

        private void StartVariableWorkloadThread() {
            variableWorkloadThread = new Thread(() => {
                while (true) {
                    Thread.Sleep(workUnitIntervalMs);
                    var queueTime = stopwatch.ElapsedMilliseconds;
                    Task.Run(() => {
                        TaskLatency = stopwatch.ElapsedMilliseconds - queueTime;
                        Thread.Sleep(WorkUnitDurationMs);
                    });
                }
            });
            variableWorkloadThread.IsBackground = true;
            variableWorkloadThread.Start();
        }

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}