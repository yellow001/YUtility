using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalText : MonoBehaviour
{
    public string key;
    Text tx;
    void OnEnable()
    {
        tx = GetComponent<Text>();
        if (tx != null)
        {
            tx.text = LocalizationMgr.Ins.Get(key);
            LocalizationMgr.Ins.changeAction += ChangeLanguage;
        }
    }

    void ChangeLanguage()
    {
        if (tx != null)
        {
            tx.text = LocalizationMgr.Ins.Get(key);
        }
    }
}
