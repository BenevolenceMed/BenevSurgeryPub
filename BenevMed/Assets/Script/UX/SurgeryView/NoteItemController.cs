using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benev.Events;

public class NoteItemController : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button BtnNote;

    float mTimeRate;
    // Start is called before the first frame update
    void Start()
    {
        BtnNote.onClick.AddListener(this.OnClick);
    }

    public void Init(float fTimeRate)
    {
        mTimeRate = fTimeRate;
    }


    // Update is called once per frame
    public void OnClick()
    {
        GameObject noteDialog = PopupDialogManager.GetInstance().Trigger("notedialog");

        int idx = BootStrap.GetInstance().userData.GetNoteIndex(BootStrap.GetInstance().userData.CurrentLearning, mTimeRate);
        noteDialog.SetActive(true);
        noteDialog.GetComponent<NoteDialog>().Init(idx, (NoteDialog.ReturnData data) =>
        {
            if (!data.ok) return;

            // data update.
            //float fRate = (float)(TimeLineDirector.time / TimeLineDirector.playableAsset.duration);
            //Debug.Log($"Note Dialog closed...{fRate}, {data.content}");

            if (data.delete)
            {
                BootStrap.GetInstance().userData.RemoveNote(BootStrap.GetInstance().userData.CurrentLearning, mTimeRate);
                Debug.Log("Note Data has been Deleted...");
            }
            else
            {
                NoteData noteData = new NoteData();
                noteData.fTimeRate = mTimeRate;
                noteData.Content = data.content;
                BootStrap.GetInstance().userData.UpdateNote(BootStrap.GetInstance().userData.CurrentLearning, noteData);
                Debug.Log("Note Data has been Updated...");
            }

            // Refresh View.
            EventSystem.DispatchEvent("OnNoteUpdated", null);
        });
    }
}
