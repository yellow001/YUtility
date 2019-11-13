using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablesPackHelper
{

    public static void BuildAssetGroup() {

    }

    [MenuItem("YUtil/Test")]
    public static void BuildAssetPack() {
        AddressableAssetSettings setting = new AddressableAssetSettings();
        AddressableAssetGroup group = setting.FindGroup("AssetsGroup");
        Debug.Log(group.entries);
    }
}
