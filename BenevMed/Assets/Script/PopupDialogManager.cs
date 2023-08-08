using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

[System.Serializable]
public class DialogObject
{
    public string name;
    public GameObject Object;
}

public class PopupDialogManager : MonoBehaviour
{
    static PopupDialogManager instance;
    public static PopupDialogManager GetInstance()
    {
        if (instance == null)
        {
            var foundObjects = FindObjectsOfType<PopupDialogManager>();
            instance = foundObjects[0];
        }
        return instance;
    }

    [SerializeField] List<DialogObject> Dialogs = new List<DialogObject>();

    EventsGroup Events = new EventsGroup();

    private void Awake()
    {
        GetInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Events.RegisterEvent("OnDialogTriggered", OnDialogTriggered);
    }

    public void Init()
    {
    }
    public GameObject Trigger(string dlgKey)
    {
        for(int k = 0; k < Dialogs.Count; ++k)
        {
            if(dlgKey == Dialogs[k].name)
            {
                Dialogs[k].Object.SetActive(true);
                return Dialogs[k].Object;
            }
        }
        return null;
    }

    void OnDialogTriggered(object data)
    {

    }
}
