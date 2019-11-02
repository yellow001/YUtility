using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationMgr : MonoBehaviour
{
    static LocalizationMgr ins;

    Dictionary<string, string> languageDic = new Dictionary<string, string>();
    List<string> languageKeys = new List<string>();

    public System.Action changeAction;
    public string currentLanguage;

    public static LocalizationMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new LocalizationMgr();
            }
            return ins;
        }
    }

    LocalizationMgr()
    {
        Init();
    }

    void Init()
    {
        currentLanguage = PlayerPrefs.GetString("Language", "chinese");
        ReadTx(currentLanguage, null);
    }

    void ReadTx(string key, System.Action cb)
    {
        languageDic.Clear();

        ResMgr.Ins.GetAsset<TextAsset>("Localization", key, (tx) => {

            if (tx == null)
            {
                Debug.Log("localization for " + key + " is null");
                return;
            }

            string[] allLines = tx.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            string[] allKeys = allLines[0].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            languageKeys.AddRange(allKeys);

            for (int j = 1; j < allLines.Length; j++)
            {

                if (allLines[j].StartsWith("//")) { continue; }

                string[] allpart = allLines[j].Split(new char[] { ',' }, 2);
                if (allpart.Length > 1)
                {
                    languageDic[allpart[0]] = allpart[1];
                }
            }

            if (cb != null)
            {
                cb();
            }

        }, false);
    }

    public string Get(string key)
    {
        if (languageDic.ContainsKey(key))
        {
            return languageDic[key];
        }
        return key;
    }

    public void ChangeLanguage(string key)
    {
        PlayerPrefs.SetString("Language", key);
        currentLanguage = key;
        ReadTx(currentLanguage, changeAction);
    }

    public List<string> GetAllLanguageKeys()
    {
        return languageKeys;
    }
}
