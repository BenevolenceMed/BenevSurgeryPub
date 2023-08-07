using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class ScreenHeightFitter : MonoBehaviour
{
    [SerializeField] RectTransform SafeArea;
    [SerializeField] float BottomOffset = 0.0f;
    [SerializeField] List<RectTransform> Others;

    EventsGroup Events = new EventsGroup();

    // Start is called before the first frame update
    void Start()
    {
        OnNewSafeAreaApplied((object)SafeArea);
        Events.RegisterEvent("OnNewSafeAreaApplied", OnNewSafeAreaApplied);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnNewSafeAreaApplied(object data)
    {
        RectTransform rtSafe = (RectTransform)data;
        float top = Screen.height * rtSafe.anchorMax.y;
        float bottom = Screen.height * rtSafe.anchorMin.y;

        float screenHeight = top - bottom;
        for (int k = 0; k < Others.Count; ++k)
            screenHeight -= Others[k].sizeDelta.y;

        screenHeight -= BottomOffset;

        var panel = GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, screenHeight);
        Debug.Log($"Height Fitter Has been update...{screenHeight}");
    }
}
