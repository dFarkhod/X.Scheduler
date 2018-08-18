using System.Collections.Generic;

namespace X.Scheduler
{
    public class ListHelper
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> source, int chunkSize)
        {
            var chunk = new List<T>(chunkSize);
            foreach (var item in source)
            {
                chunk.Add(item);
                if (chunk.Count == chunkSize)
                {
                    yield return chunk;
                    chunk.Clear();
                }
            }
            if (chunk.Count > 0)
            {
                yield return chunk;
            }
        }
    }
}


