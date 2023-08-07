using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class UserDataHolder
{
    public bool ExpertMode { get; set; }            // false to Beginner Mode.
    public string LastRecentLearning { get; set; }  // 

    EventsGroup Events = new EventsGroup();

    public void Init()
    {
        ExpertMode = false;     // should read data from somewhere else ?

        EventsGroup Events = new EventsGroup();
        Events.RegisterEvent("OnExpertModeChanged", OnExpertModeChanged);
        Events.RegisterEvent("OnSurgSectionItemClicked", OnRecentLearningUpdated);
    }


    void OnExpertModeChanged(object data)
    {
        Debug.Log("Mode has been changed.");
        ExpertMode = (int)data == 1 ? true : false;
    }

    void OnRecentLearningUpdated(object data)
    {
        Debug.Log("Last Learning Updated.. " + (string)data);
        LastRecentLearning = (string)data;
    }
}
