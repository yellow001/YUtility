using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源路径配置类
/// </summary>
public class DBResPathCfg : DBBaseCfg
{

    Dictionary<int, CResPath> resDic;

    public override void F_ReleaseDB()
    {
        if (resDic != null)
        {
            resDic.Clear();
        }
    }

    public override void InitDB()
    {
        resDic = new Dictionary<int, CResPath>();
        ConfigDBReader.ReadCfg<CResPath>(CResPath.tableName, (cfg) => {
            resDic[cfg.q_id] = cfg;
        });
    }

    /// <summary>
    /// 通过id获取配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CResPath F_GetResPathCfg(int id)
    {
        if (resDic != null && resDic.ContainsKey(id))
        {
            return resDic[id];
        }
        return null;
    }

    /// <summary>
    /// 通过id获取路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string F_GetResPath(int id)
    {
        if (resDic != null && resDic.ContainsKey(id))
        {
            return resDic[id].q_path;
        }
        return "";
    }
}
