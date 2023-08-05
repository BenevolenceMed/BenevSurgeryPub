using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Benev.Events;

public class SurgSummaryView : MonoBehaviour
{
    [SerializeField] GameObject scrollView;

    //[SerializeField] GameObject MRecentLearningView;
    [SerializeField] GameObject PrefabListItem;
    [SerializeField] CheckButton BtnMode;

    SurgeDetailInfo mSurgeDetailListInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        string strJson = Resources.Load<TextAsset>("Data/Details/AAA").text;
        mSurgeDetailListInfo = JsonUtility.FromJson<SurgeDetailInfo>(strJson);

        BtnMode.SetCheck(false);

        ScrollRect rt = scrollView.GetComponent<ScrollRect>();
        float height = .0f;
        for (int k = 0; k < mSurgeDetailListInfo.SectionLists.Count; ++k)
        {
            SurgeSectionInfo sectionInfo = mSurgeDetailListInfo.SectionLists[k];
            Debug.Log($"{sectionInfo.Name}");


            var obj = GameObject.Instantiate(PrefabListItem, rt.content.transform);
            obj.GetComponent<SectionItemViewController>().Refresh(sectionInfo, k);
            height += obj.GetComponent<RectTransform>().sizeDelta.y;
        }
        rt.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,
            rt.content.GetComponent<RectTransform>().sizeDelta.y + height);


    }
}
