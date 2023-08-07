using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Benev.Events;

public class SurgeTopUIController : MonoBehaviour
{
    [SerializeField] TMP_Text txtMode;
    [SerializeField] RectTransform ProgressBar;

    UserDataHolder userDataRef = null;
    EventsGroup Events = new EventsGroup();

    // Start is called before the first frame update
    void Start()
    {
        Events.RegisterEvent("OnTimeLineUpdated", OnTimeLineUpdated);
    }

    private void OnEnable()
    {
        userDataRef = BootStrap.GetInstance().userData;
        txtMode.text = userDataRef.ExpertMode ? "Expert Mode" : "Beginner Mode";

        ProgressBar.localScale = new Vector3(0.0f, 1.0f, 1.0f);
    }

    private void OnTimeLineUpdated(object data)
    {
        ProgressBar.localScale = new Vector3((float)data, 1.0f, 1.0f);
    }
}
