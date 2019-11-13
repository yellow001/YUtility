using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressableTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(ResMgr2.Ins.GetAsset<GameObject>("assets/cube.prefab", (obj) => { Debug.Log(obj.name); }));
        ResMgr2.Ins.GetAsset<GameObject>("Assets/Cube.prefab", (obj) => {
            Instantiate(obj).transform.localScale = Vector3.one * 5;
            AddressableItem item = ResMgr2.Ins.GetAddressableItem("Assets/Cube.prefab");
            Debug.Log(item.GetLoadCount());
            //item.ReleaseAsset();
            //Debug.Log(item.GetLoadCount());
        });

        //ResMgr2.Ins.GetAsset<GameObject>("Assets/Cube.prefab", (obj) => {
        //    Instantiate(obj).transform.localScale = Vector3.one * 5;
        //    AddressableItem item = ResMgr2.Ins.GetAddressableItem("Assets/Cube.prefab");
        //    Debug.Log(item.GetLoadCount());
        //    //item.ReleaseAsset();
        //    //Debug.Log(item.GetLoadCount());
        //});
    }
}
