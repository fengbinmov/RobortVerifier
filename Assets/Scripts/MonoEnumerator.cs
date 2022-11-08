using System;
using System.Collections;
using UnityEngine;

public static class Enumerator
{
    public static Coroutine TimeDelay(this MonoBehaviour mono, float duetime, Action action)
    {
        if (!mono.gameObject.activeSelf) return null;

        return mono.StartCoroutine(Funcn(duetime, action));
    }

    public static Coroutine TimeProcess(this MonoBehaviour mono, float time, Action actionStar, Action<float> action, Action actionEnd = null)
    {
        if (!mono.gameObject.activeSelf) return null;
        return mono.StartCoroutine(Funcn(time, actionStar, action, actionEnd));
    }

    public static Coroutine TimeProcessOne(this MonoBehaviour mono, float time, Action actionStar, Action<float> action, Action actionEnd = null)
    {
        if (!mono.gameObject.activeSelf) return null;
        return mono.StartCoroutine(Funcno(time, actionStar, action, actionEnd));
    }

    public static Coroutine TimeStepProcess(this MonoBehaviour mono, float time, float setp, Action actionStar, Action<float> action, Action actionEnd = null)
    {
        if (!mono.gameObject.activeSelf) return null;
        return mono.StartCoroutine(Funcn(time, setp, actionStar, action, actionEnd));
    }
    private static IEnumerator Funcn(float duetime, Action action)
    {
        float t = 0;
        while (t < duetime)
        {
            yield return null;

            t += Time.deltaTime;

            if (t >= duetime) break;
        }

        action?.Invoke();
    }
    private static IEnumerator Funcn(float time, float step, Action actionStar, Action<float> action, Action actionEnd)
    {
        actionStar?.Invoke();

        float p = 0;
        float c = time;

        float s = 0;
        while (p < c)
        {
            action.Invoke(p);

            while (s < step)
            {
                s += Time.deltaTime;
                yield return null;
            }
            p += (step + s - step);
            s = 0;

            yield return null;
        }

        actionEnd?.Invoke();
    }

    private static IEnumerator Funcn(float time, Action actionStar, Action<float> action, Action actionEnd)
    {
        actionStar?.Invoke();

        float p = 0;
        float c = time;

        while (p < c)
        {
            p += Time.deltaTime;
            action.Invoke(p > c ? c : p);
            yield return null;
        }

        actionEnd?.Invoke();
    }
    private static IEnumerator Funcno(float time, Action actionStar, Action<float> action, Action actionEnd)
    {
        actionStar?.Invoke();

        float p = 0;
        float c = time;

        while (p < c)
        {
            p += Time.deltaTime;
            action.Invoke((p > c ? c : p) / c);
            yield return null;
        }

        actionEnd?.Invoke();
    }
}
