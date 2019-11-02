using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigDBReader
{
    public static Dictionary<string, byte[]> cfgDataDic = new Dictionary<string, byte[]>();

    public static void ReadAllCfg(System.Action cb)
    {

        string fileName = LocalizationMgr.Ins.currentLanguage;

        ResMgr.Ins.GetAsset<TextAsset>("ConfigData", fileName, (tx) =>
        {
            byte[] content = tx.bytes;
            content = CompressHelper.Decompress(content);
            ByteArray ba = new ByteArray(content);
            while (ba.CanRead())
            {
                try
                {
                    //表名
                    string tableName = ba.ReadString();
                    //长度
                    long length = ba.ReadInt64();

                    //数据
                    byte[] data = ba.ReadBytes((int)length);

                    if (cfgDataDic.ContainsKey(tableName))
                    {
                        Debug.Log("表名 " + tableName + " 有重复！！");
                    }
                    cfgDataDic[tableName] = data;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                    break;
                }
            }

            if (cb != null) { cb(); }
        });
    }

    public static void ReadCfg<T>(string tableName, Action<T> action) where T : BaseConfigModel
    {
        if (cfgDataDic != null && cfgDataDic.ContainsKey(tableName))
        {
            ByteArray ba = new ByteArray(cfgDataDic[tableName]);
            while (ba.CanRead())
            {
                T obj = Activator.CreateInstance<T>();
                obj.Read(ba);
                action(obj);
            }

        }
    }
}

