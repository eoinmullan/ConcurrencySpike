using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class QueueJumpableTaskScheduler : TaskScheduler {
        private readonly Queue<Task> normalPriorityTasks = new Queue<Task>();
        private readonly Queue<Task> highPriorityTasks = new Queue<Task>();
        private FrontOfQueueTaskScheduler frontOfQueueTaskScheduler;

        public TaskScheduler FrontOfQueue => frontOfQueueTaskScheduler;

        public QueueJumpableTaskScheduler() {
            frontOfQueueTaskScheduler = new FrontOfQueueTaskScheduler(this);
        }

        protected override void QueueTask(Task task) {
            lock (normalPriorityTasks) normalPriorityTasks.Enqueue(task);
            ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
        }

        private void EnqueueTaskAtFrontOfQueue(Task task) {
            lock (highPriorityTasks) highPriorityTasks.Enqueue(task);
            ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks() {
            bool lockTaken = false;
            try {
                Monitor.TryEnter(normalPriorityTasks, ref lockTaken);
                if (lockTaken) return normalPriorityTasks.ToArray();
                else throw new NotSupportedException();
            }
            finally {
                if (lockTaken) Monitor.Exit(normalPriorityTasks);
            }
        }

        private void ProcessNextQueuedItem(object state) {
            Task t = null;
            if (TryTakeTaskFrom(highPriorityTasks, ref t)) {
                frontOfQueueTaskScheduler.TryExecuteTask(t);
            }
            else if (TryTakeTaskFrom(normalPriorityTasks, ref t)) {
                base.TryExecuteTask(t);
            }
        }

        private bool TryTakeTaskFrom(Queue<Task> taskQueue, ref Task task) {
            lock (taskQueue) {
                if (taskQueue.Count > 0) {
                    task = taskQueue.Dequeue();
                    return true;
                }
            }

            return false;
        }

        private class FrontOfQueueTaskScheduler : TaskScheduler {
            private QueueJumpableTaskScheduler parentTaskScheduler;

            public FrontOfQueueTaskScheduler(QueueJumpableTaskScheduler parentTaskScheduler) {
                this.parentTaskScheduler = parentTaskScheduler;
            }

            protected override void QueueTask(Task task) {
                parentTaskScheduler.EnqueueTaskAtFrontOfQueue(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
                return base.TryExecuteTask(task);
            }

            protected override IEnumerable<Task> GetScheduledTasks() {
                return parentTaskScheduler.GetScheduledTasks();
            }

            public new void TryExecuteTask(Task task) {
                base.TryExecuteTask(task);
            }
        }
    }
}
