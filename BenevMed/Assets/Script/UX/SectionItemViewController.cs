using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Benev.Events;

public class SectionItemViewController : MonoBehaviour
{
    [SerializeField] TMP_Text txtTitle;
    [SerializeField] TMP_Text txtDesc;

    int mIndex;

    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(this.OnClick);
    }

    public void Refresh(SurgeSectionInfo info, int idx)
    {
        txtTitle.text = info.Name;
        txtDesc.text = info.Desc;
        mIndex = idx;
    }

    public void OnClick()
    {
        Debug.Log($"Section Item Button {mIndex} Clciked!");
        EventSystem.DispatchEvent("OnSurgSectionItemClicked", null);
    }
}
