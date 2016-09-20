using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencySpike {
    public class QueueJumpableTaskScheduler {
        private readonly Queue<Task> normalPriorityTasks = new Queue<Task>();
        private readonly Stack<Task> highPriorityTasks = new Stack<Task>();
        private FrontOfQueueTaskScheduler frontOfQueueTaskScheduler;
        private BackOfQueueTaskScheduler backOfQueueTaskScheduler;

        public TaskScheduler FrontOfQueue => frontOfQueueTaskScheduler;
        public TaskScheduler BackOfQueue => backOfQueueTaskScheduler;

        public QueueJumpableTaskScheduler() {
            frontOfQueueTaskScheduler = new FrontOfQueueTaskScheduler(this);
            backOfQueueTaskScheduler = new BackOfQueueTaskScheduler(this);
        }

        private void EnqueueTaskAtFrontOfQueue(Task task) {
            lock (highPriorityTasks) highPriorityTasks.Push(task);
            ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
        }

        private void EnqueueTaskAtBackOfQueue(Task task) {
            lock (highPriorityTasks) normalPriorityTasks.Enqueue(task);
            ThreadPool.UnsafeQueueUserWorkItem(ProcessNextQueuedItem, null);
        }

        private void ProcessNextQueuedItem(object state) {
            Task t = null;
            if (TryTakeTaskFrom(highPriorityTasks, ref t)) {
                frontOfQueueTaskScheduler.ExecuteTask(t);
            }
            else if (TryTakeTaskFrom(normalPriorityTasks, ref t)) {
                backOfQueueTaskScheduler.ExecuteTask(t);
            }
        }

        private bool TryTakeTaskFrom(Stack<Task> taskQueue, ref Task task) {
            lock (taskQueue) {
                if (taskQueue.Count > 0) {
                    task = taskQueue.Pop();
                    return true;
                }
            }

            return false;
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

        private IEnumerable<Task> GetScheduledTasks() {
            bool lockTaken = false;
            try {
                Monitor.TryEnter(normalPriorityTasks, ref lockTaken);
                if (lockTaken) return highPriorityTasks.ToList().Concat(normalPriorityTasks.ToList());
                else throw new NotSupportedException();
            }
            finally {
                if (lockTaken) Monitor.Exit(normalPriorityTasks);
            }
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

            public void ExecuteTask(Task task) {
                base.TryExecuteTask(task);
            }
        }

        private class BackOfQueueTaskScheduler : TaskScheduler {
            private QueueJumpableTaskScheduler parentTaskScheduler;

            public BackOfQueueTaskScheduler(QueueJumpableTaskScheduler parentTaskScheduler) {
                this.parentTaskScheduler = parentTaskScheduler;
            }

            protected override void QueueTask(Task task) {
                parentTaskScheduler.EnqueueTaskAtBackOfQueue(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
                return false;
            }

            protected override IEnumerable<Task> GetScheduledTasks() {
                return parentTaskScheduler.GetScheduledTasks();
            }

            public void ExecuteTask(Task task) {
                base.TryExecuteTask(task);
            }
        }
    }
}
