using System.Collections.Generic;

public class DBMgr
{
    static DBMgr ins;

    bool init = false;

    public static DBMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new DBMgr();
            }
            return ins;
        }
    }

    Dictionary<System.Type, DBBaseCfg> allDBCfg = new Dictionary<System.Type, DBBaseCfg>() {
            { typeof(DBResPathCfg),new DBResPathCfg() },
        };

    public void F_InitDB()
    {
        foreach (var item in allDBCfg.Values)
        {
            item.InitDB();
        }
        init = true;
    }


    public void F_ReleaseDB()
    {
        foreach (var item in allDBCfg.Values)
        {
            item.F_ReleaseDB();
        }
        init = false;
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetConfgDB<T>() where T : DBBaseCfg
    {

        if (!init)
        {
            F_InitDB();
        }

        if (allDBCfg.ContainsKey(typeof(T)))
        {
            return allDBCfg[typeof(T)] as T;
        }

        return null;
    }
}

