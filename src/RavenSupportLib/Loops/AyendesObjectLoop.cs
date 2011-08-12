using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;

namespace RavenSupportLib
{
    internal class AyendesObjectLoop<T> : AyendesLoop<T>
    {

        private readonly IRavenQueryable<T> _Input;
        public AyendesObjectLoop(IRavenQueryable<T> input)
        {
            _Input = input;
        }

        protected override List<T> PerformQuery(int queryStartPosition)
        {
            return _Input.Skip(queryStartPosition).ToList();
        }

    }
}
