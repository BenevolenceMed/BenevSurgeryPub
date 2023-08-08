using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Benev.Events;

public class SurgeTopUIController : MonoBehaviour
{
    [SerializeField] TMP_Text txtMode;
    [SerializeField] RectTransform ProgressBar;
    [SerializeField] GameObject PrefabNote;
    [SerializeField] Vector2 NoteAreaX;

    UserDataHolder userDataRef = null;
    EventsGroup Events = new EventsGroup();
    List<GameObject> mListNoteObjs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Events.RegisterEvent("OnTimeLineUpdated", OnTimeLineUpdated);
        Events.RegisterEvent("OnNoteUpdated", OnNoteUpdated);
    }

    private void OnEnable()
    {
        userDataRef = BootStrap.GetInstance().userData;
        txtMode.text = userDataRef.ExpertMode ? "Expert Mode" : "Beginner Mode";

        ProgressBar.localScale = new Vector3(0.0f, 1.0f, 1.0f);

        RefreshNoteUX();
    }

    private void OnTimeLineUpdated(object data)
    {
        ProgressBar.localScale = new Vector3((float)data, 1.0f, 1.0f);
    }
    private void OnNoteUpdated(object data)
    {
        RefreshNoteUX();
    }

    void RefreshNoteUX()
    { 
        // Remove first. 
        for(int k = 0; k < mListNoteObjs.Count; ++k)
        {
            GameObject.Destroy(mListNoteObjs[k]);
        }
        mListNoteObjs.Clear();


        if (!userDataRef.DictNotes.ContainsKey(userDataRef.CurrentLearning))
            return;

        // Insert all again.
        List<NoteData> listData = userDataRef.DictNotes[userDataRef.CurrentLearning];
        for(int k = 0; k < listData.Count; ++k)
        {
            var obj = GameObject.Instantiate(PrefabNote, ProgressBar.transform.parent);

            var noteData = listData[k];
            float fX = NoteAreaX.x + (NoteAreaX.y - NoteAreaX.x) * noteData.fTimeRate;
            obj.transform.localPosition = new Vector3(fX, obj.transform.localPosition.y, obj.transform.localPosition.z);
            obj.GetComponent<NoteItemController>().Init(listData[k].fTimeRate);
            mListNoteObjs.Add(obj);
        }
    }
}
