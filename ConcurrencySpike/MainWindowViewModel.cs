using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConcurrencySpike {
    public class MainWindowViewModel : INotifyPropertyChanged {
        private int threadPoolThreadsInUse;
        private int workUnitDurationMs = 1000;
        private int workUnitIntervalMs = 1000;
        private long taskLatency = 0;

        private System.Timers.Timer checkThreadUsageTimer = new System.Timers.Timer(1000) { AutoReset = true };
        private Stopwatch stopwatch = new Stopwatch();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel() {
            stopwatch.Start();
            StartVariableWorkload();
            checkThreadUsageTimer.Elapsed += CheckThreadUsage;
            checkThreadUsageTimer.Start();
        }

        private void CheckThreadUsage(object sender, ElapsedEventArgs e) {
            int maxThreads = 0;
            int completionPortThreads = 0;
            int availableThreads = 0;
            ThreadPool.GetMaxThreads(out maxThreads, out completionPortThreads);
            ThreadPool.GetAvailableThreads(out availableThreads, out completionPortThreads);
            ThreadPoolthreadsInUse = maxThreads - availableThreads;
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

        private void StartHeartbeat() {

        }

        private void StartVariableWorkload() {
            Task.Factory.StartNew(() => {
                while (true) {
                    Thread.Sleep(workUnitIntervalMs);
                    var queueTime = stopwatch.ElapsedMilliseconds;
                    Task.Run(() => {
                        TaskLatency = stopwatch.ElapsedMilliseconds - queueTime;
                        Thread.Sleep(WorkUnitDurationMs);
                    });
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}