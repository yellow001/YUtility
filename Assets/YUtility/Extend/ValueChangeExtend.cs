using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ValueChangeExtend
{
    public static void ChangeValue(this float src, float dst, float duration, Action<float> changeFun, AnimationCurve curve = null, bool ignoreTime = false, Action callback = null)
    {
        float temp = src;
        float offset = dst - src;
        EventMgr.Ins.AddTimeEventEx(duration, callback, (t, p) => {
            if (curve != null && curve.length > 0)
            {
                p = curve.Evaluate(p);
            }
            src = temp + offset * p;
            changeFun(src);
        },1,false,ignoreTime);
    }

    public static void ChangeValue(this Vector2 src, Vector2 dst, float duration, Action<Vector2> changeFun, AnimationCurve curve = null, bool ignoreTime = false, Action callback = null)
    {
        Vector2 temp = src;
        Vector2 offset = dst - src;
        EventMgr.Ins.AddTimeEventEx(duration, callback, (t, p) => {
            if (curve != null && curve.length > 0)
            {
                p = curve.Evaluate(p);
            }
            src = temp + offset * p;
            changeFun(src);
        },1,false,ignoreTime);
    }

    public static void ChangeVaule(this Vector3 src, Vector3 dst, float duration, Action<Vector3> changeFun, AnimationCurve curve = null, bool ignoreTime = false, Action callback = null)
    {
        Vector3 temp = src;
        Vector3 offset = dst - src;
        EventMgr.Ins.AddTimeEventEx(duration, callback, (t, p) => {
            if (curve != null && curve.length > 0)
            {
                p = curve.Evaluate(p);
            }
            src = temp + offset * p;
            changeFun(src);
        },1,false,ignoreTime);
    }

    public static void ChangeVaule(this Color src, Color dst, float duration, Action<Color> changeFun, AnimationCurve curve = null, bool ignoreTime = false, Action callback = null)
    {
        Color temp = src;
        Color offset = dst - src;
        EventMgr.Ins.AddTimeEventEx(duration, callback, (t, p) => {
            if (curve != null && curve.length > 0)
            {
                p = curve.Evaluate(p);
            }
            src = temp + offset * p;
            changeFun(src);
        },1,false,ignoreTime);
    }
}
