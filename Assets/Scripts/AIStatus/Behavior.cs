using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SAI
{
    public class Behavior
    {
        public SimpleAIRobot mono;
        public Transform transform => mono.transform;

        public virtual void Entry() { }
        public virtual void Update() { }
        public virtual void Leave() { }

        public void ChangeStatus(int type)
        {
            mono.SwithStatus(type);
        }

        public T GetBehavior<T>(int type) where T : Behavior
        {
            return mono.GetBehavior<T>(type);
        }

    }
}