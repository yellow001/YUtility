using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoExtend
{
    #region 时间事件
    public static int AddTimeEventEx(this MonoBehaviour mono, TimeEvent model)
    {
        return EventMgr.Ins.AddTimeEvent(model);
    }

    public static int AddTimeEventEx(this MonoBehaviour mono, float t, Action overDe, int count = 1, bool doNow = false, bool ignoreTime = false)
    {
        return EventMgr.Ins.AddTimeEvent(new TimeEvent(t, overDe, count, doNow, ignoreTime));
    }

    public static int AddTimeEventEx(this MonoBehaviour mono, float t, Action overDe, Action<float, float> updateDe, int count = 1, bool doNow = false, bool ignoreTime = false)
    {
        return EventMgr.Ins.AddTimeEvent(new TimeEvent(t, overDe, updateDe, count, doNow, ignoreTime));
    }

    public static void RemoveTimeEventEx(this MonoBehaviour mono,int id)
    {
        EventMgr.Ins.RemoveTimeEvent(id);
    }
    #endregion

    #region 注册全局事件名称、监听事件等
    public static void AddEventEx(this MonoBehaviour mono, string name, InvokeDe fun)
    {
        EventMgr.Ins.AddEventFun(name, fun);
    }

    public static void RemoveEventEx(this MonoBehaviour mono, string name, InvokeDe fun)
    {
        EventMgr.Ins.RemoveEventFun(name, fun);
    }

    public static void CallEventEx(this MonoBehaviour mono, string name, params object[] objs)
    {
        EventMgr.Ins.InvokeEventList(name, objs);
    }
    #endregion

    #region 注册物体事件名称、监听事件等

    public static void AddObjEventEx(this MonoBehaviour mono, GameObject obj, string name, InvokeDe fun)
    {
        EventMgr.Ins.AddObjEventFun(obj, name, fun);
    }

    public static void RemoveObjEventEx(this MonoBehaviour mono, GameObject obj, string name, InvokeDe fun)
    {
        EventMgr.Ins.RemoveObjEventFun(obj, name, fun);
    }

    public static void InvokeObjDeEx(this MonoBehaviour mono, GameObject obj, string name, params object[] objs)
    {
        EventMgr.Ins.InvokeObjDeList(obj, name, objs);
    }
    #endregion
}
