using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleItem
{
    public string V_AssetName;
    public AssetBundle V_AssetBundle;
    Dictionary<string, Object> AssetList = new Dictionary<string, Object>();
    //Dictionary<string, List<Object>> AssetInsList=new Dictionary<string, List<Object>>();
    List<string> depends = new List<string>();
    List<string> allAssetNames = new List<string>();

    public AssetBundleItem(string n, AssetBundle ab, string[] dependAB)
    {
        V_AssetName = n;
        V_AssetBundle = ab;
        if (dependAB != null && dependAB.Length > 0)
        {
            depends.AddRange(dependAB);
        }
        allAssetNames.AddRange(ab.GetAllAssetNames());
    }

    /// <summary>
    /// 是否依赖某AB
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public bool IsDependAB(string abName)
    {
        return depends.Contains(abName);
    }

    /// <summary>
    /// 对应资源是否已加载过
    /// </summary>
    /// <returns></returns>
    public bool IsAssetLoaded<T>(string assetName) where T : Object
    {
        return AssetList.ContainsKey(assetName + typeof(T).ToString());
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="n"></param>
    /// <returns></returns>
    public T LoadAsset<T>(string n) where T : Object
    {
        string assetName = n + typeof(T).ToString();

        if (!AssetList.ContainsKey(assetName))
        {
            bool hasAsset = false;
            foreach (var item in allAssetNames)
            {
                if (item.Contains(n))
                {
                    hasAsset = true;
                    break;
                }
            }
            if (!hasAsset)
            {
                return null;
            }

            try
            {
                T asset = V_AssetBundle.LoadAsset<T>(n) as T;

                if (asset == null)
                {
                    Debug.Log("asset type error");
                    return null;
                }

                AssetList[assetName] = asset;
                //List<Object> list = new List<Object>();
                //AssetInsList[assetName] = list;
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
                return null;
            }
        }

        T result = Object.Instantiate(AssetList[assetName]) as T;
        //AssetInsList[assetName].Add(result);
        return result;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="n"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetAsync<T>(string n, System.Action<T> callBack) where T : Object
    {
        string assetName = n + typeof(T).ToString();

        if (!AssetList.ContainsKey(assetName))
        {
            bool hasAsset = false;
            foreach (var item in allAssetNames)
            {
                if (item.Contains(n))
                {
                    hasAsset = true;
                    break;
                }
            }
            if (!hasAsset)
            {
                callBack(null);
                yield break;
            }

            AssetBundleRequest request = V_AssetBundle.LoadAssetAsync<T>(n);

            yield return request;

            if (request.asset == null)
            {
                Debug.Log("asset type error");
                callBack(null);
                yield break;
            }

            AssetList[assetName] = request.asset;
            //List<Object> list = new List<Object>();
            //AssetInsList[assetName] = list;
        }

        T result = Object.Instantiate(AssetList[assetName]) as T;
        //AssetInsList[assetName].Add(result);
        callBack(result);
    }
}
