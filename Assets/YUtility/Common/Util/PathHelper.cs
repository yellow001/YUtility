using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper
{
#if UNITY_EDITOR_WIN
    public static readonly string AssetBundlePath = Application.streamingAssetsPath;
#else
        public static readonly string AssetBundlePath = Application.persistentDataPath;
#endif
    public static readonly string StreamingPath = Application.streamingAssetsPath;
    public static readonly string ResPath = Application.dataPath + "/Resources";

    /// <summary>
    /// 获取相对于项目的路径(完整路径是不会打包的)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativeProjectPath(string path)
    {

        string[] temp = path.Split(new string[] { Application.dataPath }, StringSplitOptions.RemoveEmptyEntries);
        string realPath = "Assets" + temp[temp.Length - 1];
        //Debug.Log(realPath);

        return realPath;
    }

    /// <summary>
    /// 获取相对路径
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static string GetRelativePath(string child, string parent)
    {
        if (!child.Contains(parent))
        {
            Debug.Log("the path you given is not a chid of the parent path");
            return string.Empty;
        }

        if (child.Equals(parent))
        {
            return "";
        }

        string[] tempDirs = child.Split(new string[] { parent + "/" }, StringSplitOptions.RemoveEmptyEntries);
        child = tempDirs[tempDirs.Length - 1];

        return child;
    }

    /// <summary>
    /// 获取相对于资源路径的相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativeResPath(string path)
    {
        return GetRelativePath(path, ResPath);
    }

    /// <summary>
    /// 获取相对于打包路径的相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativeStreamingPath(string path)
    {
        return GetRelativePath(path, AssetBundlePath);
    }
}
