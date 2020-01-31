using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutSize : MonoBehaviour
{
    [SerializeField]
    LayoutGroup LayoutGroup;


    private void Awake()
    {
        if (LayoutGroup == null)
        {
            LayoutGroup = GetComponent<LayoutGroup>();
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //RefreshSize();
    }

    public void RefreshSize()
    {
        if (LayoutGroup != null)
        {

            if (LayoutGroup is VerticalLayoutGroup)
            {
                RefreshSizeVertical();
            }
            else if (LayoutGroup is HorizontalLayoutGroup) {
                RefreshSizeHorizontal();
            }

        }
    }

    public void RefreshSizeVertical()
    {
        VerticalLayoutGroup layout = LayoutGroup as VerticalLayoutGroup;
        if (layout != null)
        {
            float height = 0;
            int count = 0;
            for (int i = 0; i < layout.transform.childCount; i++)
            {
                RectTransform tra = layout.transform.GetChild(i) as RectTransform;
                if (tra != null && tra.gameObject.activeSelf)
                {
                    height += tra.sizeDelta.y;
                    count++;
                }
            }

            height += layout.spacing * (count - 1) + layout.padding.top + layout.padding.bottom;

            Vector2 size = (layout.transform as RectTransform).sizeDelta;

            (layout.transform as RectTransform).sizeDelta = new Vector2(size.x, height);

        }
    }

    public void RefreshSizeHorizontal()
    {
        HorizontalLayoutGroup layout = LayoutGroup as HorizontalLayoutGroup;
        if (layout != null)
        {
            float width = 0;
            int count = 0;
            for (int i = 0; i < layout.transform.childCount; i++)
            {
                RectTransform tra = layout.transform.GetChild(i) as RectTransform;
                if (tra != null && tra.gameObject.activeSelf)
                {
                    width += tra.sizeDelta.x;
                    count++;
                }
            }

            width += layout.spacing * (count - 1) + layout.padding.left + layout.padding.right;

            Vector2 size = (layout.transform as RectTransform).sizeDelta;

            (layout.transform as RectTransform).sizeDelta = new Vector2(width, size.y);

        }
    }

}

public enum EM_LayoutType
{

    /// <summary>
    /// 垂直布局
    /// </summary>
    Vertical,

    /// <summary>
    /// 水平布局
    /// </summary>
    Horizontal,

    /// <summary>
    /// 网格布局
    /// </summary>
    Grid,
}
