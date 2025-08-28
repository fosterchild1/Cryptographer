using System.Collections;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

// batched work stealing priority queue 
// sorry about the name not being descriptive, i didnt want it to be long :(
public class SearchQueue<TElement, TPriority>
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

        [Obsolete("Slower, because since its a priority queue if it hits a priority 0 it still has to process everything left in the batch.")]
        public bool TryDequeueBatch(out List<(TElement, TPriority)> batch)
        {
            lock (lockObj) {
                if (queue.Count == 0)
                {
                    batch = default!;
                    return false;
                }

                int batchSize = Math.Min((int)MathF.Ceiling(queue.Count / 2f), 32);

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
    public SearchQueue(int workerCount)
    {
        workers_ = workerCount;

        for (int i = 0; i < workerCount; i++)
        {
            queues.Add(new LocalQueue());
        }

        int counter = 0;
        index = new(() => Interlocked.Increment(ref counter) % workerCount);
    }

    /// <summary>
    ///  Adds the specified element with associated priority to one <see cref="LocalQueue"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="SearchQueue{TElement, TPriority}"/>.</param>
    /// <param name="priority">The priority with which to associate the new element.</param>
    public void Enqueue(TElement element, TPriority priority)
    {
        queues[index.Value].Enqueue(element, priority);
    }

    /// <summary>
    ///  Removes the minimal element from one <see cref="LocalQueue"/>,
    ///  and copies it to the <paramref name="element"/> parameter,
    ///  and its associated priority to the <paramref name="priority"/> parameter.
    /// </summary>
    /// <param name="element">The removed element.</param>
    /// <param name="priority">The priority associated with the removed element.</param>
    /// <returns>
    ///  <see langword="true"/> if the element is successfully removed;
    ///  <see langword="false"/> if the <see cref="SearchQueue{TElement, TPriority}"/> is empty or it fails to remove.
    /// </returns>
    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        // try on self first
        if (queues[index.Value].TryDequeue(out element, out priority)) return true;

        // try stealing from others
        for (int i = 0; i < workers_; i++)
        {
            if (i == index.Value) continue;

            if (queues[i].TryDequeue(out element, out priority)) return true;
        }

        element = default!;
        priority = default!;
        return false;
    }

    /// <summary>
    ///  Removes half of the minimal elements from one <see cref="LocalQueue"/>,
    ///  and copies it and their priority to the <paramref name="batch"/> parameter,
    /// </summary>
    /// <param name="batch">List of tuples</param>
    /// <returns>
    ///  <see langword="true"/> if the elements are successfully removed;
    ///  <see langword="false"/> if the <see cref="SearchQueue{TElement, TPriority}"/> is empty or it fails to remove.
    /// </returns>
    [Obsolete("Slower, because since its a priority queue if it hits a priority 0 it still has to process everything left in the batch.")]
    public bool TryDequeueBatch(out List<(TElement, TPriority)> batch)
    {
        // try on self first
        if (queues[index.Value].TryDequeueBatch(out batch)) return true;

        // try stealing from the biggest
        LocalQueue? bestQueue = null;
        int maxCount = 0;

        for (int i = 0; i < workers_; i++)
        {
            if (i == index.Value) continue;

            LocalQueue q = queues[i];
            lock (q.lockObj)
            {
                int count = q.queue.Count;
                if (count <= maxCount) continue;

                maxCount = count;
                bestQueue = q;
            }
        }

        // none had items
        if (bestQueue == null || maxCount == 0)
        {
            batch = default!;
            return false;
        }

        // batch failed
        if (bestQueue.TryDequeueBatch(out batch))
        {
            return batch.Count > 0;
        }

        batch = default!;
        return false;
    }
    public int[] GetQueueSizes()
    {
        int[] sizes = new int[workers_];
        for (int i = 0; i < workers_; i++)
        {
            lock (queues[i].lockObj)
            {
                sizes[i] = queues[i].queue.Count;
            }
        }
        return sizes;
    }
    /// <returns>
    /// <see langword="true"/> if the <see cref="SearchQueue{TElement, TPriority}"/> is empty;
    /// <see langword="false"/> if it isn't.
    /// </returns>
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