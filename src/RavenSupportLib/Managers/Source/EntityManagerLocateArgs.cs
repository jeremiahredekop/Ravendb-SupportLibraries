using System;
using GeniusCode.Framework.Composition;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public class EntityManagerLocateArgs : ObjectLocationArgs
    {
        public Type EntityType { get; set; }
    }
}
