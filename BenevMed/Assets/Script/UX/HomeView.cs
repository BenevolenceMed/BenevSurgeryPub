using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Benev.Events;

public class HomeView : MonoBehaviour
{
    [SerializeField] GameObject scrollView;

    [SerializeField] GameObject MRecentLearningView;
    [SerializeField] GameObject PrefabListItem;

    SurgeListInfo mSurgeListInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!MRecentLearningView.activeSelf)
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1800);

        string strJson = Resources.Load<TextAsset>("Data/surginfo").text;
        mSurgeListInfo = JsonUtility.FromJson<SurgeListInfo>(strJson);


        ScrollRect rt = scrollView.GetComponent<ScrollRect>();
        float height = .0f;
        for (int k = 0; k < mSurgeListInfo.SurgeryLists.Count; ++k)
        {
            var surgInfo = mSurgeListInfo.SurgeryLists[k];
            // Debug.Log($"{surgInfo.Name}");

            var obj = GameObject.Instantiate(PrefabListItem, rt.content.transform);
            obj.GetComponent<SurgItemViewController>().Refersh(surgInfo, k);
            height += obj.GetComponent<RectTransform>().sizeDelta.y;
        }
        rt.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }
}
