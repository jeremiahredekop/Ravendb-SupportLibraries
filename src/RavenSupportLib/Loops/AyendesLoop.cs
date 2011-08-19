using System;
using System.Collections.Generic;

namespace GeniusCode.RavenDb.Loops
{
    internal abstract class AyendesLoop<T>
    {
        protected abstract List<T> PerformQuery(int queryStartPosition);

        public void Do(Action<List<T>> batchAction)
        {
            int count = 0;
            do
            {
                List<T> q = PerformQuery(count);

                if (q.Count == 0)
                    break;

                count += q.Count;
                batchAction(q);
            } while (true);
        }

        public List<T> GetAll()
        {
            var output = new List<T>();
            Do(output.AddRange);

            return output;
        }
    }
}