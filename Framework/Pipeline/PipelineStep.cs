using System;
using UnityEngine;
using Random = System.Random;

namespace Framework.Pipeline
{
    public abstract class PipelineStep : MonoBehaviour
    {
        public Random random;
        public abstract Type[] RequiredGuarantees { get; }
        
        public virtual bool AddToDebugStackedView => false;

        public abstract GameWorld Apply(GameWorld world);
        
    }
}