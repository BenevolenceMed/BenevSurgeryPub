using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class EventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnSectionStarted(int idx)
    {
        Debug.Log($"TimeLine Event triggered! {idx}");

        EventSystem.DispatchEvent("OnSectionStarted", (object)idx);
    }
}