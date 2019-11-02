using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEvent
{
    #region 变量

    /// <summary>
    /// 执行等待时间
    /// </summary>
    public float V_ExtuteTime;

    /// <summary>
    /// 累计时间
    /// </summary>
    public float V_DeltaTime;

    /// <summary>
    /// 事件执行回调
    /// </summary>
    public Action V_OverCallBack;

    /// <summary>
    /// 等待执行的帧事件回调（可获取剩余时间）
    /// </summary>
    public Action<float, float> V_UpdateCallBack;

    /// <summary>
    /// 执行次数(负数 循环执行不会停)
    /// </summary>
    public int V_Count;

    /// <summary>
    /// 立即执行一次
    /// </summary>
    public bool V_ExtuteNow;

    /// <summary>
    /// 是否忽略时间缩放
    /// </summary>
    public bool V_IgnoreTimeScale = false;

    #endregion

    /// <summary>
    /// 时间事件初始化
    /// </summary>
    /// <param name="time">延迟执行</param>
    /// <param name="callback">执行回调</param>
    /// <param name="count">执行次数</param>
    /// <param name="extuteNow">立即执行一次</param>
    /// <param name="ignoreTimeScale">忽略时间缩放</param>
    public TimeEvent(float time, Action callback, int count = 1, bool extuteNow = false, bool ignoreTimeScale = false)
    {
        V_ExtuteTime = time;
        V_OverCallBack = callback;
        V_UpdateCallBack = null;
        V_Count = count;
        V_Count = V_Count == 0 ? 1 : V_Count;
        V_ExtuteNow = extuteNow;
        V_IgnoreTimeScale = ignoreTimeScale;
    }

    /// <summary>
    /// 时间事件初始化
    /// </summary>
    /// <param name="time">延迟执行</param>
    /// <param name="callback">执行回调</param>
    /// <param name="updateCallback">倒计时回调</param>
    /// <param name="count">执行次数</param>
    /// <param name="extuteNow">立即执行一次</param>
    /// <param name="ignoreTimeScale">忽略时间缩放</param>
    public TimeEvent(float time,Action callback,Action<float,float> updateCallback=null,int count=1,bool extuteNow=false,bool ignoreTimeScale=false) {
        V_ExtuteTime = time;
        V_OverCallBack = callback;
        V_UpdateCallBack = updateCallback;
        V_Count = count;
        V_Count = V_Count == 0 ? 1 : V_Count;
        V_ExtuteNow = extuteNow;
        V_IgnoreTimeScale = ignoreTimeScale;
    }
}
