using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;

namespace GeniusCode.RavenDb.Loops
{
    internal class AyendesObjectLoop<T> : AyendesLoop<T>
    {
        private readonly IRavenQueryable<T> _input;

        public AyendesObjectLoop(IRavenQueryable<T> input)
        {
            _input = input;
        }

        protected override List<T> PerformQuery(int queryStartPosition)
        {
            return _input.Skip(queryStartPosition).ToList();
        }
    }
}