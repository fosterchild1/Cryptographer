// work stealing priority queue 
// sorry about the name not being descriptive, i didnt want it to be long :(
[Obsolete("multithreading is not needed anymore", true)]
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
    }

    private List<LocalQueue> queues = new();
    private int workers_ = 1;

    // global queue
    public SearchQueue(int workerCount)
    {
        workers_ = workerCount;

        for (int i = 0; i < workerCount; i++)
        {
            queues.Add(new());
        }
    }

    /// <summary>
    ///  Adds the specified element with associated priority to one <see cref="LocalQueue"/>.
    /// </summary>
    /// <param name="element">The element to add to the <see cref="SearchQueue{TElement, TPriority}"/>.</param>
    /// <param name="priority">The priority with which to associate the new element.</param>
    public void Enqueue(TElement element, TPriority priority, int index)
    {
        queues[index].Enqueue(element, priority);
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
    public bool TryDequeue(out TElement element, out TPriority priority, int index)
    {
        // try on self first
        if (queues[index].TryDequeue(out element, out priority)) return true;

        // try stealing from others
        for (int i = 0; i < workers_; i++)
        {
            if (i == index) continue;

            if (queues[i].TryDequeue(out element, out priority)) return true;
        }

        element = default!;
        priority = default!;
        return false;
    }
    public bool TryPeek(out TElement element, out TPriority priority, int index)
    {
        lock (queues[index].lockObj)
            return queues[index].queue.TryPeek(out element!, out priority!);
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