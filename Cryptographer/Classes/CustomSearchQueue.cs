using System.Collections;
using System.Diagnostics;

// batched work stealing priority queue 
// sorry about the name not being descriptive, i didnt want it to be long :(
public class CustomSearchQueue<TElement, TPriority>
{
    // thread-level queue
    private class LocalQueue
    {
        public object lockObj = new();
        public PriorityQueue<TElement, TPriority> queue = new();

        public void Enqueue(TElement item, TPriority priority)
        {
            lock (lockObj)
            {
                queue.Enqueue(item, priority);
            }
        }

        public bool TryDequeue(out TElement element, out TPriority priority)
        {
            lock (lockObj)
            {
                // check if empty then try dequeueing
                if (queue.Count == 0) { element = default!; priority = default!; return false; }

                if (!queue.TryDequeue(out element!, out priority!)) return false;

                return true;
            }

        }

        public bool TryDequeueBatch(out List<(TElement, TPriority)> batch)
        {
            lock (lockObj) {
                if (queue.Count == 0)
                {
                    batch = default!;
                    return false;
                }

                int batchSize = (int)MathF.Ceiling(queue.Count / 2f);

                batch = new();

                while (batch.Count < batchSize && queue.Count > 0)
                {
                    if (!queue.TryDequeue(out TElement? item, out TPriority? priority)) break;
                    batch.Add((item, priority));
                }

                return true;
            }
        }
    }

    private List<LocalQueue> queues = new();
    private ThreadLocal<int> index;
    private int workers_ = 1;

    // global queue
    public CustomSearchQueue(int workerCount)
    {
        workers_ = workerCount;

        for (int i = 0; i < workerCount; i++)
        {
            queues.Add(new LocalQueue());
        }

        int counter = 0;
        index = new(() => Interlocked.Increment(ref counter) % workerCount);
    }

    public void Enqueue(TElement item, TPriority priority)
    {
        queues[index.Value].Enqueue(item, priority);
    }

    public bool TryDequeue(out TElement item, out TPriority priority)
    {
        // try on self first
        if (queues[index.Value].TryDequeue(out item, out priority)) return true;

        // try stealing from others
        for (int i = 0; i < workers_; i++)
        {
            if (i == index.Value) continue;

            if (queues[i].TryDequeue(out item, out priority)) return true;
        }

        item = default!;
        priority = default!;
        return false;
    }

    public bool TryDequeueBatch(out List<(TElement, TPriority)> batch)
    {
        // try on self first
        if (queues[index.Value].TryDequeueBatch(out batch)) return true;

        // try stealing from others
        for (int i = 0; i < workers_; i++)
        {
            if (i == index.Value) continue;

            if (queues[i].TryDequeueBatch(out batch)) return true;
        }

        batch = default!;
        return false;
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < workers_; i++)
        {
            lock (queues[i].lockObj)
            {
                if (queues[i].queue.Count > 0) return false;
            }
        }
        return true;
    }
}