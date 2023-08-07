using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class DirectorMain : MonoBehaviour
{
    [SerializeField] GameObject HomeView;
    [SerializeField] GameObject OptionView;
    //[SerializeField] GameObject MRecentLearningView;
    [SerializeField] TabButtonGroup TabButtons;

    [SerializeField] GameObject UIRoot2D;
    [SerializeField] GameObject UIRoot3D;

    EventsGroup Events = new EventsGroup();

    BootStrap bootStrap = null;

    private void Awake()
    {
        bootStrap = BootStrap.GetInstance();

        HomeView.SetActive(true);
        OptionView.SetActive(false);
        TabButtons.TurnOnTabButton(0);
        //if(MRecentLearningView.activeSelf)

        Events.RegisterEvent("OnSurgItemClicked", OnSurgItemClicked);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnHomeBtnClicked()
    {
        HomeView.SetActive(true);
        OptionView.SetActive(false);
        TabButtons.TurnOnTabButton(0);
    }
    public void OnOptionBtnClicked()
    {
        HomeView.SetActive(false);
        OptionView.SetActive(true);
        TabButtons.TurnOnTabButton(1);
    }
    public void On3DSceneBtnClicked()
    {
        UIRoot2D.SetActive(false);
        UIRoot3D.SetActive(true);

    }
    public void On2DSceneBtnClicked()
    {
        UIRoot2D.SetActive(true);
        UIRoot3D.SetActive(false);
    }

    private void OnSurgItemClicked(object data)
    {
        On3DSceneBtnClicked();
    }
}
