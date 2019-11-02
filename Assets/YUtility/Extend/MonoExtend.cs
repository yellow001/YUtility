using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoExtend
{
    #region 时间事件
    public static void AddTimeEvent(this MonoBehaviour mono, TimeEvent model)
    {
        EventMgr.Ins.AddTimeEvent(model);
    }

    public static void AddTimeEvent(this MonoBehaviour mono, float t, Action overDe, int count = 1, bool doNow = false, bool ignoreTime = false)
    {
        EventMgr.Ins.AddTimeEvent(new TimeEvent(t, overDe, count, doNow, ignoreTime));
    }

    public static void AddTimeEvent(this MonoBehaviour mono, float t, Action overDe, Action<float, float> updateDe, int count = 1, bool doNow = false, bool ignoreTime = false)
    {
        EventMgr.Ins.AddTimeEvent(new TimeEvent(t, overDe, updateDe, count, doNow, ignoreTime));
    }

    public static void RemoveTimeEvent(this MonoBehaviour mono, TimeEvent model)
    {
        EventMgr.Ins.RemoveTimeEvent(model);
    }
    #endregion

    #region 注册全局事件名称、监听事件等
    public static void AddEvent(this MonoBehaviour mono, string name, InvokeDe fun)
    {
        EventMgr.Ins.AddEventFun(name, fun);
    }

    public static void RemoveEvent(this MonoBehaviour mono, string name, InvokeDe fun)
    {
        EventMgr.Ins.RemoveEventFun(name, fun);
    }

    public static void CallEvent(this MonoBehaviour mono, string name, params object[] objs)
    {
        EventMgr.Ins.InvokeEventList(name, objs);
    }
    #endregion

    #region 注册物体事件名称、监听事件等

    public static void AddObjEvent(this MonoBehaviour mono, GameObject obj, string name, InvokeDe fun)
    {
        EventMgr.Ins.AddObjEventFun(obj, name, fun);
    }

    public static void RemoveObjEvent(this MonoBehaviour mono, GameObject obj, string name, InvokeDe fun)
    {
        EventMgr.Ins.RemoveObjEventFun(obj, name, fun);
    }

    public static void InvokeObjDe(this MonoBehaviour mono, GameObject obj, string name, params object[] objs)
    {
        EventMgr.Ins.InvokeObjDeList(obj, name, objs);
    }
    #endregion
}
