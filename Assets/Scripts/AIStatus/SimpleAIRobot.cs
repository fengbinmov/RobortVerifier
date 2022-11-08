using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAI;

public class SimpleAIRobot : MonoBehaviour
{
    protected Behavior behavior;

    protected Dictionary<int, Behavior> behaviors = new Dictionary<int, Behavior>();


    public virtual void Update()
    {
        behavior?.Update();
    }

    public virtual T GetBehavior<T>(int type) where T : Behavior
    {
        return null;
    }

    public Vector2 behaviorWait = new Vector2(0.5f,1f);

    public virtual void SwithStatus(int type)
    {

        Behavior temp = GetBehavior<Behavior>(type);

        behavior?.Leave();
        behavior = null;

        if (behaviorWait.y == 0)
        {
            behavior = temp;
            behavior.Entry();
        }
        else
        {
            this.TimeDelay(UnityEngine.Random.Range(behaviorWait.x, behaviorWait.y), () => {

                behavior = temp;
                behavior.Entry();
#if PrintLog
                sb.AppendLine("behavior.Entry " + Time.realtimeSinceStartup.ToString(".00000") + " " + transform.name + " " + behavior);

                //Debug.Log("behavior.Entry "+Time.realtimeSinceStartup.ToString(".00000")+" " +transform.name+" "+ behavior);
#endif
            });
        }
    }

    [Button("StartUp")]
    public virtual void StartUp()
    {
    }
}
