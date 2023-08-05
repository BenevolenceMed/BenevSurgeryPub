using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class SurgView : MonoBehaviour
{
    [SerializeField] GameObject PreViewUI;
    [SerializeField] GameObject Scene3D;
    [SerializeField] GameObject Scene3DUI;
    [SerializeField] GameObject Scene3DTextBoxUI;

    EventsGroup Events = new EventsGroup();

    // Start is called before the first frame update
    void Start()
    {
        PreViewUI.SetActive(true);
        Scene3D.SetActive(false);
        Scene3DUI.SetActive(false);

        Events.RegisterEvent("OnSurgSectionItemClicked", OnSurgSectionItemClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void On3DBtnClicked()
    {
        PreViewUI.SetActive(false);
        Scene3D.SetActive(true);
        Scene3DUI.SetActive(true);
        Scene3DTextBoxUI.SetActive(true);
    }
    public void On3DSceneQuitClicked()
    {
        PreViewUI.SetActive(true);
        Scene3D.SetActive(false);
        Scene3DUI.SetActive(false);
        Scene3DTextBoxUI.SetActive(false);
    }

    private void OnSurgSectionItemClicked(object data)
    {
        On3DBtnClicked();
    }
}
