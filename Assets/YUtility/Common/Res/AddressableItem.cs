using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableItem
{

    string m_assetPath;

    AsyncOperationHandle handle;

    int loadCount;

    Action<AddressableItem> releaseCallback;

    long lastLoadTime;

    public AddressableItem(string path,Action<AddressableItem> releaseCb) {
        m_assetPath = path;
        loadCount = 0;
        releaseCallback = releaseCb;
    }

    public IEnumerator LoadAsset<T>(Action<T> callback){

        lastLoadTime = DateTime.Now.Ticks;

        if (!handle.IsValid()) {
            handle = Addressables.LoadAssetAsync<T>(m_assetPath);
            yield return handle;
        }
        if (!handle.IsDone) {
            yield return handle;
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (callback != null)
            {
                loadCount++;
                callback((T)handle.Result);
            }
        }
        else {
            YDebuger.LogError("load asset fail " + m_assetPath+" handle state:"+handle.Status);
            if (callback != null)
            {
                callback(default(T));
            }
        }
    }


    public void ReleaseAsset() {
        loadCount--;
        loadCount = loadCount < 0 ? 0 : loadCount;
        if (loadCount == 0)
        {
            if (DateTime.Now.Ticks - lastLoadTime >= 10000 * 1000 * ResMgr2.Ins.V_ReleaseSecond) {
                if (releaseCallback != null) {
                    Addressables.Release(handle);
                    releaseCallback(this);
                }
            }
        }
    }

    public int GetLoadCount() {
        return loadCount;
    }

    public string GetPath() {
        return m_assetPath;
    }
}
