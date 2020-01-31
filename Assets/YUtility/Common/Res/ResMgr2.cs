using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class ResMgr2 : InsManager<ResMgr2>
{
    #region 内部变量
    Dictionary<string, AddressableItem> assetList;
    int maxAssetBundleCount = 250;
    int m_releaseUnuseAssetTime = 600;
    #endregion

    #region 公有变量
    public int V_ReleaseSecond = 60;
    #endregion

    public override void Init()
    {
        maxAssetBundleCount = AppSetting.Ins.GetIntValue("MaxAssetBundleCount");
        maxAssetBundleCount = maxAssetBundleCount <= 0 ? 250 : maxAssetBundleCount;

        V_ReleaseSecond = AppSetting.Ins.GetIntValue("ReleaseSecond");

        m_releaseUnuseAssetTime = AppSetting.Ins.GetIntValue("ReleaseUnuseSecond");
        m_releaseUnuseAssetTime = m_releaseUnuseAssetTime <= 200 ? 200 : m_releaseUnuseAssetTime;

        assetList = new Dictionary<string, AddressableItem>();

        SceneManager.sceneUnloaded += OnSecneUnload;

        this.AddTimeEventEx(m_releaseUnuseAssetTime, () => Clear(), -1);

        base.Init();
    }

    public void GetAsset<T>(string path, Action<T> callback) {

        if (!assetList.ContainsKey(path)) {
            AddressableItem item = new AddressableItem(path, ReleaseAssetCallBack);
            assetList.Add(path, item);
        }
        StartCoroutine(assetList[path].LoadAsset<T>(callback));
    }

    public AddressableItem GetAddressableItem(string path) {
        if (assetList.ContainsKey(path)) {
            return assetList[path];
        }
        return null;
    }

    public void ReleaseAssetCallBack(AddressableItem item) {
        if (assetList.ContainsKey(item.GetPath())) {
            assetList.Remove(item.GetPath());
        }
    }

    public void Clear() {
        List<string> delList = new List<string>();
        foreach (var item in assetList)
        {
            if (item.Value.GetLoadCount() <= 0) {
                delList.Add(item.Key);
            }
        }

        foreach (var item in delList)
        {
            assetList.Remove(item);
        }
    }

    public void OnSecneUnload(Scene scene) {
        Clear();
    }
}
