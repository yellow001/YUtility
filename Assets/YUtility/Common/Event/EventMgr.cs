using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EventMgr : InsManager<EventMgr>
{

    #region 时间事件相关

    int m_EventIndex = 0;

    Dictionary<int, TimeEvent> tEvents_up_temp = new Dictionary<int, TimeEvent>();
    Dictionary<int, TimeEvent> tEvents_up = new Dictionary<int, TimeEvent>();
    List<int> rmTEvents_up = new List<int>();

    Dictionary<int, TimeEvent> tEvents_fix_temp = new Dictionary<int, TimeEvent>();
    Dictionary<int, TimeEvent> tEvents_fix = new Dictionary<int, TimeEvent>();
    List<int> rmTEvents_fix = new List<int>();

    Dictionary<int, TimeEvent> tEvents_late_temp = new Dictionary<int, TimeEvent>();
    Dictionary<int, TimeEvent> tEvents_late = new Dictionary<int, TimeEvent>();
    List<int> rmTEvents_late = new List<int>();

    // Update is called once per frame
    void Update()
    {
        ExcuteTimeEvent(tEvents_up, tEvents_up_temp, rmTEvents_up);
    }

    private void FixedUpdate()
    {
        ExcuteTimeEvent(tEvents_fix, tEvents_fix_temp,rmTEvents_fix);
    }

    private void LateUpdate()
    {
        ExcuteTimeEvent(tEvents_late, tEvents_late_temp,rmTEvents_late);
    }

    public void ExcuteTimeEvent(Dictionary<int, TimeEvent> eventDic, Dictionary<int, TimeEvent> eventDicTemp, List<int> rmList)
    {
        if (rmList != null && rmList.Count > 0)
        {
            foreach (int item in rmList)
            {
                if (eventDic.ContainsKey(item))
                {
                    eventDic.Remove(item);
                }
            }
            rmList.Clear();
        }

        if (eventDicTemp != null && eventDicTemp.Count > 0) {
            foreach (var item in eventDicTemp.Keys)
            {
                eventDic[item] = eventDicTemp[item];
            }
        }
        eventDicTemp.Clear();

        if (eventDic != null && eventDic.Count > 0)
        {

            foreach (var itemKey in eventDic.Keys)
            {
                TimeEvent item = eventDic[itemKey];
                //如果是立即执行
                if (item.V_ExtuteNow)
                {
                    if (item.V_OverCallBack != null)
                    {
                        item.V_OverCallBack();
                    }
                    item.V_ExtuteNow = false;
                    item.V_Count = item.V_Count > 0 ? --item.V_Count : item.V_Count;
                    if (item.V_Count == 0)
                    {
                        RemoveTimeEvent(itemKey);
                    }
                }

                //已经到达时间
                if (item.V_DeltaTime >= item.V_ExtuteTime)
                {
                    //执行方法，并移除model
                    try
                    {
                        if (item.V_OverCallBack != null)
                        {
                            item.V_OverCallBack();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        RemoveTimeEvent(itemKey);
                        Debug.Log(ex.ToString());
                        return;
                    }

                    item.V_DeltaTime = 0;
                    item.V_Count = item.V_Count > 0 ? --item.V_Count : item.V_Count;
                    if (item.V_Count == 0)
                    {
                        RemoveTimeEvent(itemKey);
                    }


                }
                else
                {
                    float V_DeltaTime = item.V_IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                    item.V_DeltaTime += V_DeltaTime;

                    if (item.V_UpdateCallBack != null)
                    {

                        float leaveTime = item.V_ExtuteTime - item.V_DeltaTime;

                        leaveTime = (int)(leaveTime * 100) / 100f;

                        float percent = (item.V_DeltaTime) / item.V_ExtuteTime;

                        percent = (int)(percent * 100) / 100f;//0.1~1

                        try
                        {
                            item.V_UpdateCallBack(Mathf.Max(0, leaveTime), Mathf.Min(1, percent));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.Log(ex.ToString());
                            RemoveTimeEvent(itemKey);
                        }

                    }
                }
            }
        }
    }

    public int AddTimeEvent(TimeEvent model, TimeEventUpdateMode m = TimeEventUpdateMode.Update)
    {
        if (model == null)
        {
            Debug.Log("the model you add is null");
            return 0;
        }
        model.V_DeltaTime = 0;
        Interlocked.Increment(ref m_EventIndex);
        switch (m)
        {
            case TimeEventUpdateMode.Update:
                tEvents_up_temp.Add(m_EventIndex, model);
                break;
            case TimeEventUpdateMode.FixedUpdate:
                tEvents_fix_temp.Add(m_EventIndex, model);
                break;
            case TimeEventUpdateMode.LateUpdate:
                tEvents_late_temp.Add(m_EventIndex, model);
                break;
            default:
                break;
        }
        return m_EventIndex;
    }

    public void RemoveTimeEvent(int eventID)
    {

        if (tEvents_up.ContainsKey(eventID))
        {
            tEvents_up[eventID].V_DeltaTime = 0;
            if (!rmTEvents_up.Contains(eventID))
            {
                rmTEvents_up.Add(eventID);
            }
        }

        if (tEvents_fix.ContainsKey(eventID))
        {
            tEvents_fix[eventID].V_DeltaTime = 0;
            if (!rmTEvents_fix.Contains(eventID))
            {
                rmTEvents_fix.Add(eventID);
            }
        }

        if (tEvents_late.ContainsKey(eventID))
        {
            tEvents_late[eventID].V_DeltaTime = 0;
            if (!rmTEvents_late.Contains(eventID))
            {
                rmTEvents_late.Add(eventID);
            }
        }

    }
    #endregion

    #region 字符事件
    Dictionary<string, List<InvokeDe>> DeList = new Dictionary<string, List<InvokeDe>>();

    #region 注册、移除、调用事件
    public void AddEventName(string name)
    {
        lock (Ins)
        {
            if (DeList.ContainsKey(name))
            {
                return;
            }

            DeList.Add(name, new List<InvokeDe>());
        }
    }

    public void AddEventName(Enum eName)
    {
        string name = string.Format("{0}{1}", eName.GetType().ToString(), eName.ToString());
        AddEventName(name);
    }

    public void RemoveEventName(string name)
    {
        lock (Ins)
        {
            if (!DeList.ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr: the event name \'{0}\' is not contain", name));
#endif
                return;
            }

            DeList.Remove(name);
        }

    }

    public void RemoveEventName(Enum eName)
    {
        string name = string.Format("{0}{1}", eName.GetType().ToString(), eName.ToString());
        RemoveEventName(name);
    }


    public void AddEventFun(string name, InvokeDe fun)
    {

        lock (Ins)
        {
            AddEventName(name);
            if (DeList[name].Contains(fun))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr: you can not add the function because the event \'{0}\' already had the function", name));
#endif
                return;
            }

            DeList[name].Add(fun);
        }
    }

    public void AddEventFun(Enum eName, InvokeDe fun)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        AddEventFun(name, fun);
    }

    public void RemoveEventFun(string name, InvokeDe fun)
    {

        lock (Ins)
        {
            if (!DeList.ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr: you can not remove the function because the event name \'{0}\' is not contain", name));
#endif
                return;
            }

            if (!DeList[name].Contains(fun))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr: you can not remove the function because the event \'{0}\' do not have the function", name));
#endif
                return;
            }

            DeList[name].Remove(fun);
        }
    }

    public void RemoveEventFun(Enum eName, InvokeDe fun)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        RemoveEventFun(name, fun);
    }

    public void InvokeEventList(string name, params object[] args)
    {
        lock (Ins)
        {
            if (!DeList.ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr: you can not invoke the function because the event name \'{0}\' is not contain", name));
#endif
                return;
            }

            foreach (var item in DeList[name])
            {
                item(args);
            }
        }
    }

    public void InvokeDeList(Enum eName, params object[] args)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        InvokeEventList(name, args);
    }
    #endregion

    #endregion


    #region 注册物体事件名称、监听事件等

    public Dictionary<GameObject, Dictionary<string, List<InvokeDe>>> ObjDelist = new Dictionary<GameObject, Dictionary<string, List<InvokeDe>>>();

    public void AddObjEventName(GameObject obj, string name)
    {
        lock (Ins)
        {
            //存在该物体键值
            if (ObjDelist.ContainsKey(obj))
            {
                //检查该是否该函数名
                if (ObjDelist[obj].ContainsKey(name))
                {
                    return;
                }
                else
                {
                    ObjDelist[obj].Add(name, new List<InvokeDe>());
                }
            }
            else
            {
                ObjDelist.Add(obj, new Dictionary<string, List<InvokeDe>>());
                ObjDelist[obj].Add(name, new List<InvokeDe>());
            }
        }
    }

    public void AddObjEventName(GameObject obj, Enum eName)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        AddObjEventName(obj, name);
    }

    public void RemoveObjEvent(GameObject obj)
    {
        lock (Ins)
        {
            if (!ObjDelist.ContainsKey(obj))
            {
                return;
            }
            ObjDelist.Remove(obj);
        }

    }

    public void RemoveObjEventName(GameObject obj, string name)
    {
        lock (Ins)
        {
            if (!ObjDelist.ContainsKey(obj))
            {
                return;
            }
            if (!ObjDelist[obj].ContainsKey(name))
            {
                return;
            }
            ObjDelist[obj].Remove(name);
        }

    }

    public void RemoveObjEventName(GameObject obj, Enum eName)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        RemoveObjEventName(obj, name);
    }


    public void AddObjEventFun(GameObject obj, string name, InvokeDe fun)
    {
        lock (Ins)
        {
            AddObjEventName(obj, name);
            if (ObjDelist[obj][name].Contains(fun))
            {
                return;
            }
            ObjDelist[obj][name].Add(fun);
        }
    }

    public void AddObjEventFun(GameObject obj, Enum eName, InvokeDe fun)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        AddObjEventFun(obj, name, fun);
    }

    public void RemoveObjEventFun(GameObject obj, string name, InvokeDe fun)
    {
        lock (Ins)
        {
            if (!ObjDelist.ContainsKey(obj)) { return; }
            if (!ObjDelist[obj].ContainsKey(name)) { return; }
            if (!ObjDelist[obj][name].Contains(fun)) { return; }

            ObjDelist[obj][name].Remove(fun);
        }
    }

    public void RemoveObjEventFun(GameObject obj, Enum eName, InvokeDe fun)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        RemoveObjEventFun(obj, name, fun);
    }

    public void InvokeObjDeList(GameObject obj, string name, params object[] args)
    {
        lock (Ins)
        {
            if (!ObjDelist.ContainsKey(obj))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr:{0} has no funList name {1}", obj, name));
#endif
                return;
            }
            if (!ObjDelist[obj].ContainsKey(name))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("EventMgr:  event name \'{0}\' is not contain in {1}", name, obj));
#endif
                return;
            }

            foreach (var item in ObjDelist[obj][name])
            {
                item(args);
            }
        }
    }

    public void InvokeObjDeList(GameObject obj, Enum eName, params object[] args)
    {
        string name = eName.GetType().ToString() + eName.ToString();
        InvokeObjDeList(obj, name, args);
    }
    #endregion
    public void Clear()
    {
        tEvents_up.Clear();
        rmTEvents_up.Clear();

        tEvents_fix.Clear();
        rmTEvents_fix.Clear();

        tEvents_late.Clear();
        rmTEvents_late.Clear();
    }

    private void OnDestroy()
    {
        Clear();
    }
}

public enum TimeEventUpdateMode
{
    Update,
    FixedUpdate,
    LateUpdate
}

/// <summary>
/// 所有监听事件的通用委托(无返回值)
/// </summary>
/// <param name="objs"></param>
public delegate void InvokeDe(params object[] objs);
