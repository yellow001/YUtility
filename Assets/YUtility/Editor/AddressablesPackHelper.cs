using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesPackHelper
{
    public static List<string> assetPathStrs;
    public static List<string> filterStrs;

    public static bool Init = true;

    public static void BuildAssetGroup() {

    }

    [MenuItem("YUtil/Test")]
    public static void BuildAssetPack() {

        Init = true;

        AddressableAssetSettings setting = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetGroup group = setting.FindGroup("AssetGroup");
        List<AddressableAssetEntry> temp = group.entries.ToList();
        foreach (var item in temp)
        {
            setting.RemoveAssetEntry(item.guid);
        }

        string[] p = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < p.Length; i++)
        {
            if (CanPack(p[i])) {
                setting.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(p[i]), group);
            }
        }
    }


    public static bool CanPack(string path) {
        if (Init) {
            if (assetPathStrs == null) {
                assetPathStrs = new List<string>();
            }
            assetPathStrs.Clear();

            if (filterStrs == null)
            {
                filterStrs = new List<string>();
            }
            filterStrs.Clear();

            assetPathStrs.AddRange(AppSetting.Ins.GetValue("AssetPath").Split(new char[] { ';'},StringSplitOptions.RemoveEmptyEntries));

            filterStrs.AddRange(AppSetting.Ins.GetValue("DontPack").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

            Init = true;
        }

        bool pack = false;
        foreach (var item in assetPathStrs)
        {
            if (path.StartsWith(item)) {
                pack = true;
                break;
            }
        }

        if (!pack) { return false; }

        foreach (var item in filterStrs)
        {
            if (path.EndsWith(item)) {
                return false;
            }
        }


        return true;
    }
}
