using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using Benev.Events;

public class SurgAnimationController : MonoBehaviour
{
    [SerializeField] PlayableDirector TimeLineDirector;
    [SerializeField] Transform Camera;
    [SerializeField] GameObject MessageObj;
    [SerializeField] TMP_Text txtDescription;

    // Debug Controller.
    [SerializeField] GameObject BtnPlay, BtnPause;

    bool mIsPaused = false;
    float CamZPos, CamZOffset = .0f;
    bool mIsUpdating = false;
    private Coroutine mCoUpdator = null;
    bool mFreshStart = true;
    EventsGroup Events = new EventsGroup();

    #region Unity CallBacks
    // Start is called before the first frame update
    void Start()
    {
        ///MessageObj.SetActive(false);
        BtnPause.SetActive(false);
        Events.RegisterEvent("OnAnimationJumpTo", OnAnimationJumpTo);
    }

    private void OnEnable()
    {
        mFreshStart = true;

        TimeLineDirector.time = 0;
        TimeLineDirector.Evaluate();

        BtnPlay.SetActive(true);
        BtnPause.SetActive(false);
    }

    private void OnDisable()
    {
        TimeLineDirector.time = 0;
        TimeLineDirector.Stop();
        TimeLineDirector.Evaluate();

        if (mCoUpdator != null)
            StopCoroutine(mCoUpdator);
        mCoUpdator = null;
        mIsUpdating = false;
        mIsPaused = false;
    }
    private void LateUpdate()
    {
        if (mIsPaused)
            Camera.localPosition = new Vector3(Camera.localPosition.x, Camera.localPosition.y, CamZOffset + CamZPos);
    }
    #endregion

    #region Button & Event Receivers
    public void OnPlayClicked()
    {
        if (mFreshStart)
        {
            OnRePlayClicked();
            mFreshStart = false;
        }
        else
        {
            if (mIsPaused) PlayTimeLine();
            else
                TimeLineDirector.Play();

            BtnPlay.SetActive(false);
            BtnPause.SetActive(true);
        }

        mIsPaused = false;
        mCoUpdator = StartCoroutine(coUpdateTimeLine());
    }
    public void OnRePlayClicked()
    {
        TimeLinePlayAt(0);

        BtnPlay.SetActive(false);
        BtnPause.SetActive(true);
    }
    public void OnPauseClicked()
    {
        PauseTimeLine();
        mIsPaused = true;

        BtnPlay.SetActive(true);
        BtnPause.SetActive(false);

        CamZPos = Camera.localPosition.z;
        CamZOffset = .0f;
    }
    public void OnZoomInClicked()
    {
        if(!mIsPaused)
        {
            Debug.Log("Only works at Paused Mode.");
            return;
        }
        CamZOffset += 1.0f;
    }
    public void OnZoomOutClicked()
    {
        if (!mIsPaused)
        {
            Debug.Log("Only works at Paused Mode.");
            return;
        }
        CamZOffset -= 1.0f;
    }
    public void OnJumpToNextSector()
    {
        // find currenct sector.
        long curFrame = Utils.TimeToFrame(TimeLineDirector.time);
        SurgeDetailInfo detailInfo = BootStrap.GetInstance().SurgeDetailInfo;

        int idxDefault = 0;
        SurgeSectionInfo section = detailInfo.SectionLists[idxDefault];
        for (int k = 1; k < section.SectorList.Count; ++k)
        {
            if(curFrame < section.SectorList[k].Frame)
            {
                // Jump To next Frame.
                TimeLinePlayAt(Utils.FrameToTime(section.SectorList[k].Frame));
                break;
            }
        }
    }
    public void OnAddNote()
    {
        if (!mIsPaused)
        {
            Debug.Log("Only works at Paused Mode.");
            return;
        }
        GameObject noteDialog = PopupDialogManager.GetInstance().Trigger("notedialog");

        noteDialog.SetActive(true);
        noteDialog.GetComponent<NoteDialog>().Init(-1, (NoteDialog.ReturnData data) =>
        {
            if (!data.ok) return;

            // data update.
            float fRate = (float)(TimeLineDirector.time / TimeLineDirector.playableAsset.duration);
            Debug.Log($"Note Dialog closed...{fRate}, {data.content}");

            NoteData noteData = new NoteData();
            noteData.fTimeRate = fRate;
            noteData.Content = data.content;
            BootStrap.GetInstance().userData.AddNote(BootStrap.GetInstance().userData.CurrentLearning, noteData);
            Debug.Log("Note Data has been added...");

            // Refresh View.
            EventSystem.DispatchEvent("OnNoteUpdated", null);
        });
    }
    
    void OnAnimationJumpTo(object data)
    {
        float rate = (float)data;
        TimeLinePlayAt(rate * TimeLineDirector.playableAsset.duration);
    }

    public void OnSignalSector(int idx)
    {
        Debug.Log($"Sector id {idx}..");
        // update message.

        // find currenct sector.
        SurgeDetailInfo detailInfo = BootStrap.GetInstance().SurgeDetailInfo;

        int idxDefault = 0;
        SurgeSectionInfo section = detailInfo.SectionLists[idxDefault];
        --idx;
        if (idx < 0 || idx >= section.SectorList.Count)
            return;
        
        txtDescription.text = section.SectorList[idx].Message;

        if (idx > 1 && !BootStrap.GetInstance().userData.ExpertMode)
            OnPauseClicked();
    }
    #endregion

    #region Helper functions
    void TimeLinePlayAt(double time, float speed=1)
    {
        TimeLineDirector.time = time;
        TimeLineDirector.Evaluate();
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        TimeLineDirector.Play();
        mIsPaused = false;
        mFreshStart = false;

        if (mCoUpdator == null)
        {
            mCoUpdator = StartCoroutine(coUpdateTimeLine());
        }
    }
    void PlayTimeLine()
    {
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
    void PauseTimeLine()
    {
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }
    IEnumerator coUpdateTimeLine()
    {
        if (mIsUpdating) yield break;

        mIsUpdating = true;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            float fRate = (float)(TimeLineDirector.time / TimeLineDirector.playableGraph.GetRootPlayable(0).GetDuration());
            EventSystem.DispatchEvent("OnTimeLineUpdated", (object)fRate);
        }
    }
    #endregion


    /*
    IEnumerator coShowMessageObject(int idx)
    {
        yield break;
        //MessageObj.SetActive(true);

        //Debug.Log($"Playing Message idx {idx}..");

        //yield return new WaitForSeconds(1.0f);

        //MessageObj.SetActive(false);
    }

    public void OnPlayAtMidClicked()
    {
        mIsPaused = false;
        TimeLineDirector.time = TimeLineDirector.playableAsset.duration * 0.5f;
        TimeLineDirector.Evaluate();
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        TimeLineDirector.Play();
    }
    */
}
