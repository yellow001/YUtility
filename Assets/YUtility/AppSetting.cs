using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AppSetting
{
    public Dictionary<string, string> Settings = new Dictionary<string, string>();

    public static AppSetting Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new AppSetting();
            }

            return ins;
        }
    }

    static AppSetting ins;


    private AppSetting()
    {
        InitSetting();
    }


    public string GetValue(string name)
    {

#if UNITY_EDITOR
        ReloadSetting();
#endif

        if (Ins.Settings.ContainsKey(name))
        {
            return Ins.Settings[name];
        }
        else
        {

            return string.Empty;
        }
    }

    public int GetIntValue(string name)
    {

#if UNITY_EDITOR
        ReloadSetting();
#endif
        try
        {
            if (Ins.Settings.ContainsKey(name))
            {
                return int.Parse(Ins.Settings[name]);
            }
            else
            {
                return 0;
            }
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    void InitSetting()
    {
        try
        {
#if UNITY_EDITOR
            string content = Resources.Load<TextAsset>("AppSetting").text;
#else
            string content = new StreamReader(PathHelper.AssetBundlePath + "/AppSetting.txt",Encoding.UTF8).ReadToEnd();
#endif
            string[] settings = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in settings)
            {
                if (item.StartsWith("//")) { continue; }
                string[] item2 = item.Split(new string[] { ":" },2,StringSplitOptions.RemoveEmptyEntries);
                if (item2.Length == 2)
                {
                    Settings.Add(item2[0], item2[1]);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    void ReloadSetting()
    {
        try
        {
#if UNITY_EDITOR
            string content = Resources.Load<TextAsset>("AppSetting").text;
#else
            string content = new StreamReader(PathHelper.AssetBundlePath + "/AppSetting.txt",Encoding.UTF8).ReadToEnd();
#endif
            string[] settings = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in settings)
            {
                if (item.StartsWith("//")) { continue; }

                string[] item2 = item.Split(new char[] {':'},2,StringSplitOptions.RemoveEmptyEntries);
                if (item2.Length == 2)
                {
                    Settings[item2[0]] = item2[1];
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
