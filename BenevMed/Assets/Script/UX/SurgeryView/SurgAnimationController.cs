using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using Benev.Events;

public class SurgAnimationController : MonoBehaviour
{
    [SerializeField] PlayableDirector TimeLineDirector;
    [SerializeField] GameObject MessageObj;
    [SerializeField] TMP_Text txtDescription;
    [SerializeField] float ScreenDragRate = 0.001f;
    [SerializeField] float ScreenZoomRate = 0.001f;
    [SerializeField] float ScreenRotateRate = 0.05f;
    [SerializeField] float ScreenRotateAroundDist = 10.0f;
    [SerializeField] Transform DummyCamSimulation;  // need dummy transform, since tr for camera should be overriden by anim.
    [SerializeField] Transform RotationTarget;

    // Debug Controller.
    [SerializeField] GameObject BtnPlay, BtnPause;
    [SerializeField] float TestingMoveSpeed = 0.5f;
    [SerializeField] TMP_Dropdown ScreenControlMode;

    enum eScreenControlMode { eDrag=0, eZoom, eRotate }

    
    bool mIsPaused = false;
    private Coroutine mCoUpdator = null;
    bool mFreshStart = true;
    EventsGroup Events = new EventsGroup();

    // Camera Control.
    Camera aniCamera;
    bool mIsUpdating = false;
    private Vector3 vDifference;
    private bool Drag = false;
    private Vector3 vOrigin;
    eScreenControlMode mControlMode = eScreenControlMode.eDrag;

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
        ScreenControlMode.value = 0;
        mControlMode = eScreenControlMode.eDrag;

        var foundObjects = FindObjectsOfType<Camera>();
        System.Diagnostics.Debug.Assert(foundObjects.Length == 1);
        aniCamera = foundObjects[0];
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

    private void Update()
    {
        if (mIsPaused) return;

        // stay in sync for screen control.
        DummyCamSimulation.position = aniCamera.transform.position;
        DummyCamSimulation.localRotation = aniCamera.transform.localRotation;
    }

    private void LateUpdate()
    {
        if (!mIsPaused) return;

        UpdateMouseDrag();

        ReflectDummyTransformToAniCamera();

        switch (mControlMode)
        {
        case eScreenControlMode.eDrag:
            UpdateDrag();
            break;
        case eScreenControlMode.eZoom:
            UpdateZoom();
            break;
        case eScreenControlMode.eRotate:
            //UpdateRotation();
            UpdateRotationAround();
            break;
        default:
            return;
        }
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

        vDifference = Vector3.zero;
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

    public void OnScreenControlModeChanged(int index)
    {
        Debug.Log($"Screen Mode changed! {index}");
        mControlMode = (eScreenControlMode)index;
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

    // Camera Control. - May need to optimize code.
    void ReflectDummyTransformToAniCamera()
    {
        // aniCamera data is dirty since its animation transform data.
        aniCamera.transform.position = DummyCamSimulation.position;
        aniCamera.transform.rotation = DummyCamSimulation.rotation;
    }
    void UpdateZoom()
    {
        DummyCamSimulation.position += DummyCamSimulation.forward * vDifference.y * ScreenZoomRate;
        aniCamera.transform.position += DummyCamSimulation.forward * vDifference.y * ScreenZoomRate;
    }

    void UpdateDrag()
    {
        DummyCamSimulation.position -= DummyCamSimulation.right * vDifference.x * ScreenDragRate;
        DummyCamSimulation.position -= DummyCamSimulation.up * vDifference.y * ScreenDragRate;
        aniCamera.transform.position -= DummyCamSimulation.right * vDifference.x * ScreenDragRate;
        aniCamera.transform.position -= DummyCamSimulation.up * vDifference.y * ScreenDragRate;
    }

    void UpdateRotationAround()
    {
        Vector3 vTarget = RotationTarget != null ? RotationTarget.position : DummyCamSimulation.position + DummyCamSimulation.forward * ScreenRotateAroundDist;
        Debug.DrawLine(DummyCamSimulation.position, vTarget);

        aniCamera.transform.RotateAround(vTarget, Vector3.up, vDifference.x * ScreenRotateRate);
        DummyCamSimulation.RotateAround(vTarget, Vector3.up, vDifference.x * ScreenRotateRate);

        // Maybe x aswell ?
        aniCamera.transform.RotateAround(vTarget, aniCamera.transform.right, -vDifference.y * ScreenRotateRate);
        DummyCamSimulation.RotateAround(vTarget, aniCamera.transform.right, -vDifference.y * ScreenRotateRate);
    }

    void UpdateMouseDrag()
    {
        if (Input.GetMouseButton(0))
        {
            if (Drag == false)
            {
                Drag = true;
                vOrigin = Input.mousePosition;
            }
            vDifference = Input.mousePosition - vOrigin;
        }
        else
            Drag = false;
        
        if (Drag == true)
            vOrigin = Input.mousePosition;
    }
    #endregion


    public void OnZoomInClicked()
    {
        if (!mIsPaused)
        {
            Debug.Log("Only works at Paused Mode.");
            return;
        }
        //CamZoomOffset += (aniCamera.transform.forward * TestingMoveSpeed);
        //vRotOffset += new Vector3(2, 0, 0);
    }
    public void OnZoomOutClicked()
    {
        if (!mIsPaused)
        {
            Debug.Log("Only works at Paused Mode.");
            return;
        }
        //vRotOffset += new Vector3(-2, 0, 0);
        //CamZoomOffset -= (aniCamera.transform.forward * TestingMoveSpeed);
    }

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
    void UpdateRotation()
    {
        if (Input.GetMouseButton(0))
        {
            if (Drag == false)
            {
                Drag = true;
                vOrigin = Input.mousePosition;
            }
            vDifference = vOrigin - Input.mousePosition;
        }
        else
        {
            if (Drag) vCamRot += vRotOffset;
            Drag = false;
        }

        vRotOffset = Vector3.zero;
        if (Drag == true)
        {
            vRotOffset = new Vector3(-vDifference.y, vDifference.x, .0f) * ScreenRotateRate;
        }
    }
    */
}
