using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResMgr : InsManager<ResMgr>
{
    #region 内部变量
    EM_LoadMode loadMode = EM_LoadMode.Resources;
    AssetBundleManifest manifest;
    List<string> allAssetList = new List<string>();
    Dictionary<string, AssetBundleItem> abLoadedDic = new Dictionary<string, AssetBundleItem>();
    int maxAssetBundleCount = 25;
    bool loading = false;
    #endregion

    public override void Init()
    {
        loadMode = (EM_LoadMode)(AppSetting.Ins.GetIntValue("AssetLoadMode"));
        maxAssetBundleCount = AppSetting.Ins.GetIntValue("MaxAssetBundleCount");

        if (loadMode == EM_LoadMode.AssetBundle)
        {
            //load manifest
            AssetBundle manifestFile = AssetBundle.LoadFromFile(PathHelper.AssetBundlePath + "/StreamingAssets");
            manifest = manifestFile.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            allAssetList.AddRange(manifest.GetAllAssetBundles());
            manifestFile.Unload(false);
        }

        SceneManager.sceneUnloaded += OnSecneUnload;

        this.AddTimeEventEx(100, () => CheckAndReleaseAssetBundle(),-1);

        base.Init();
    }

    void OnSecneUnload(Scene arg0)
    {
        Clear();
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">AB名/文件夹路径</param>
    /// <param name="assetName">资源名/文件名</param>
    /// <param name="callBack">加载回调</param>
    /// <param name="isAsync">是否异步加载</param>
    public void GetAsset<T>(string abName, string assetName, System.Action<T> callBack, bool isAsync = true) where T : Object
    {
        if (loadMode == EM_LoadMode.AssetBundle)
        {
            GetABAsset(abName, assetName, callBack, isAsync);
        }
        else
        {
            GetResAsset(abName, assetName, callBack, isAsync);
        }
    }

    #region AssetBundle
    /// <summary>
    /// 获取AB资源的统一入口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">AB名</param>
    /// <param name="assetName">资源名</param>
    /// <param name="callBack">加载回调</param>
    /// <param name="isAsync">是否异步加载</param>
    void GetABAsset<T>(string abName, string assetName, System.Action<T> callBack, bool isAsync = true) where T : UnityEngine.Object
    {
        if (isAsync)
        {
            LoadABAssetAsync(abName, assetName, callBack);
        }
        else
        {
            if (callBack != null)
            {
                callBack(LoadABAsset<T>(abName, assetName));
            }
        }
    }

    /// <summary>
    /// 通过AssetBundle加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    T LoadABAsset<T>(string abName, string assetName) where T : UnityEngine.Object
    {
        abName = abName.ToLower();
        assetName = assetName.ToLower();

        if (abLoadedDic.ContainsKey(abName) && abLoadedDic[abName].IsAssetLoaded<T>(assetName))
        {
            return abLoadedDic[abName].LoadAsset<T>(assetName);
        }

        if (!LoadAssetBundle(abName))
        {
            return null;
        }

        return abLoadedDic[abName].LoadAsset<T>(assetName);
    }

    /// <summary>
    /// 通过AssetBundle异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    void LoadABAssetAsync<T>(string abName, string assetName, System.Action<T> callBack) where T : UnityEngine.Object
    {
        abName = abName.ToLower();
        assetName = assetName.ToLower();

        if (abLoadedDic.ContainsKey(abName) && abLoadedDic[abName].IsAssetLoaded<T>(assetName))
        {
            callBack(abLoadedDic[abName].LoadAsset<T>(assetName));
        }
        else
        {
            StartCoroutine(LoadAssetAsync(abName, assetName, callBack));
        }
    }

    /// <summary>
    /// 异步加载AB
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <param name="cb"></param>
    /// <returns></returns>
    IEnumerator LoadAssetAsync<T>(string abName, string assetName, System.Action<T> cb) where T : Object
    {
        //AB是否已加载
        if (!abLoadedDic.ContainsKey(abName))
        {//如果还没加载
         //先加载依赖项
            foreach (string item in manifest.GetAllDependencies(abName))
            {
                yield return LoadAssetAsync<T>(item, null, null);
            }
            //加载AB
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathHelper.AssetBundlePath + "/" + abName);

            while (!request.isDone)
            {
                yield return new WaitForSeconds(0.15f);
            }

            AssetBundleItem abItem = new AssetBundleItem(abName, request.assetBundle, manifest.GetAllDependencies(abName));
            abLoadedDic.Add(abName, abItem);
        }
        else
        {
            //AB内存已释放
            if (abLoadedDic[abName].V_AssetBundle == null)
            {
                //先加载依赖项
                foreach (string item in manifest.GetAllDependencies(abName))
                {
                    yield return LoadAssetAsync<T>(item, null, null);
                }
                //加载AB
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathHelper.AssetBundlePath + "/" + abName);

                yield return request;

                if (request.assetBundle != null)
                {
                    abLoadedDic[abName].V_AssetBundle = request.assetBundle;
                }
            }
        }

        if (!string.IsNullOrEmpty(assetName) && cb != null)
        {
            StartCoroutine(abLoadedDic[abName].LoadAssetAsync<T>(assetName, cb));
        }
    }

    /// <summary>
    /// 加载AB
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    bool LoadAssetBundle(string abName)
    {
        //AB是否已加载
        if (!abLoadedDic.ContainsKey(abName))
        {//如果还没加载
         //先加载依赖项
            foreach (string item in manifest.GetAllDependencies(abName))
            {
                if (!LoadAssetBundle(item))
                {
                    return false;
                }
            }
            //加载AB
            AssetBundle ab = AssetBundle.LoadFromFile(PathHelper.AssetBundlePath + "/" + abName);
            AssetBundleItem abItem = new AssetBundleItem(abName, ab, manifest.GetAllDependencies(abName));
            abLoadedDic.Add(abName, abItem);
            return true;
        }
        else
        {
            //AB内存已释放
            if (abLoadedDic[abName].V_AssetBundle == null)
            {
                //先加载依赖项
                foreach (string item in manifest.GetAllDependencies(abName))
                {
                    if (!LoadAssetBundle(item))
                    {
                        return false;
                    }
                }
                //加载AB
                AssetBundle ab = AssetBundle.LoadFromFile(PathHelper.AssetBundlePath + "/" + abName);
                abLoadedDic[abName].V_AssetBundle = ab;
                return true;
            }
            else
            {
                //AB内存未释放
                return true;
            }
        }
    }

    #endregion

    #region Resources
    /// <summary>
    /// 获取Res资源的统一入口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <param name="callBack"></param>
    /// <param name="isAsync"></param>
    void GetResAsset<T>(string abName, string assetName, System.Action<T> callBack, bool isAsync = true) where T : Object
    {
        if (isAsync)
        {
            StartCoroutine(LoadResAssetAsync(abName + "/" + assetName, callBack));
        }
        else
        {
            callBack(LoadResAsset<T>(abName + "/" + assetName));
        }
    }

    /// <summary>
    /// 加载Res资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    T LoadResAsset<T>(string path) where T : Object
    {
        T value = Resources.Load<T>(path);
        return value;
    }

    /// <summary>
    /// 异步加载Res资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator LoadResAssetAsync<T>(string path, System.Action<T> callBack) where T : Object
    {
        ResourceRequest req = Resources.LoadAsync<T>(path);
        yield return req;
        if (req.asset != null)
        {
            callBack((T)req.asset);
        }
        else
        {
            callBack(null);
        }
    }
    #endregion


    /// <summary>
    /// 切换场景时的AB清除
    /// </summary>
    void Clear()
    {
        //把AB内存已经释放掉的AssetBundleItem干掉
        List<string> deleteList = new List<string>();
        var item = abLoadedDic.GetEnumerator();
        while (item.MoveNext())
        {
            if (item.Current.Value.V_AssetBundle == null)
            {
                deleteList.Add(item.Current.Key);
            }
        }
        item.Dispose();

        foreach (string item2 in deleteList)
        {
            abLoadedDic.Remove(item2);
        }

        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 检查并释放AssetBundle（当AssetBundle个数大于某数值时）
    /// </summary>
    void CheckAndReleaseAssetBundle()
    {
        if (loading) { return; }

        if (abLoadedDic.Count <= maxAssetBundleCount)
        {
            return;
        }

        List<AssetBundleItem> list = abLoadedDic.Values.ToList();

        //挑出没有被依赖的
        List<string> templist = new List<string>();
        foreach (AssetBundleItem item in list)
        {

            bool beDepend = false;
            foreach (AssetBundleItem item2 in list)
            {
                if (item == item2) { continue; }
                if (item2.IsDependAB(item.V_AssetName))
                {
                    beDepend = true;
                    break;
                }
            }

            if (!beDepend)
            {
                templist.Add(item.V_AssetName);
            }
        }

        //释放掉内存
        foreach (string item in templist)
        {
            abLoadedDic[item].V_AssetBundle.Unload(false);
        }
    }

    enum EM_LoadMode
    {
        Resources = 0,
        AssetBundle = 1,
    }
}
