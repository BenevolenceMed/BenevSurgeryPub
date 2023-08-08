using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Benev.Events;
using TMPro;

public class SurgSummaryView : MonoBehaviour
{
    [SerializeField] GameObject scrollView;

    //[SerializeField] GameObject MRecentLearningView;
    [SerializeField] GameObject PrefabListItem;
    [SerializeField] CheckButton BtnMode;
    [SerializeField] TMP_Text txtSurgName;

    SurgeDetailInfo mSurgeDetailListInfo;
    EventsGroup Events = new EventsGroup();

    List<GameObject> mListItems = new List<GameObject>();
    float mOriginalScrollHeight;

    // Start is called before the first frame update
    void Awake()
    {
        Events.RegisterEvent("OnSurgSelected", OnSurgSelected);

        ScrollRect rt = scrollView.GetComponent<ScrollRect>();
        mOriginalScrollHeight = rt.content.GetComponent<RectTransform>().sizeDelta.y;
    }
    void Start()
    { }

    // OnEnable comes before Start... hmm...
    private void OnEnable()
    {
    }

    void Refresh(string selectedLearning)
    {
        string strJson = Resources.Load<TextAsset>("Data/Details/AAA").text;
        mSurgeDetailListInfo = JsonUtility.FromJson<SurgeDetailInfo>(strJson);

        txtSurgName.text = selectedLearning;// BootStrap.GetInstance().userData.CurrentLearning;

        BtnMode.SetCheck(false);

        // destroy old ones first.
        for (int k = 0; k < mListItems.Count; ++k)
            GameObject.Destroy(mListItems[k]);


        ScrollRect rt = scrollView.GetComponent<ScrollRect>();
        float height = .0f;
        for (int k = 0; k < mSurgeDetailListInfo.SectionLists.Count; ++k)
        {
            SurgeSectionInfo sectionInfo = mSurgeDetailListInfo.SectionLists[k];
            // Debug.Log($"{sectionInfo.Name}");


            var obj = GameObject.Instantiate(PrefabListItem, rt.content.transform);
            obj.GetComponent<SectionItemViewController>().Refresh(sectionInfo, k);
            height += obj.GetComponent<RectTransform>().sizeDelta.y;

            mListItems.Add(obj);
        }
        rt.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, mOriginalScrollHeight + height);
    }

    void OnSurgSelected(object data)
    {
        Debug.Log("Current Learning Updated.. " + (string)data);
        Refresh((string)data);
    }

    public void OnModeChanged(int index)
    {
        Debug.Log($"Mode changed...{index}");
        EventSystem.DispatchEvent("OnExpertModeChanged", (object)index);
    }
}
