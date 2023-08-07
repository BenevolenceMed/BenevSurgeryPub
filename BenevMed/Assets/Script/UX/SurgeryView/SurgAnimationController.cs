using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class SurgAnimationController : MonoBehaviour
{
    [SerializeField] PlayableDirector TimeLineDirector;
    [SerializeField] Transform Camera;
    [SerializeField] GameObject MessageObj;
    [SerializeField] TMP_Text txtDescription;

    bool mIsPaused = false;
    float CamZPos, CamZOffset = .0f;

    // Start is called before the first frame update
    void Start()
    {
        ///MessageObj.SetActive(false);
    }

    void PlayTimeLine()
    {
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }
    void PauseTimeLine()
    {
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    public void OnPlayClicked()
    {
        if (mIsPaused) PlayTimeLine();
        else
            TimeLineDirector.Play();

        mIsPaused = false;
    }
    public void OnRePlayClicked()
    {
        mIsPaused = false;
        TimeLineDirector.time = .0f;
        TimeLineDirector.Evaluate();
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        TimeLineDirector.Play();
    }
    public void OnPauseClicked()
    {
        PauseTimeLine();
        mIsPaused = true;

        CamZPos = Camera.localPosition.z;
        CamZOffset = .0f;
    }
    public void OnZoomInClicked()
    {
        CamZOffset += 1.0f;
    }
    public void OnZoomOutClicked()
    {
        CamZOffset -= 1.0f;
    }
    public void OnPlayAtMidClicked()
    {
        mIsPaused = false;
        TimeLineDirector.time = TimeLineDirector.playableAsset.duration * 0.5f;
        TimeLineDirector.Evaluate();
        TimeLineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        TimeLineDirector.Play();
    }
    private void LateUpdate()
    {
        if(mIsPaused)
            Camera.localPosition = new Vector3( Camera.localPosition.x, Camera.localPosition.y, CamZOffset + CamZPos);
    }

    public void OnSignalSector(int idx)
    {
        // StartCoroutine(coShowMessageObject(idx));

        Debug.Log($"Sector Signal idx {idx}..");
        // update message.

        txtDescription.text = idx.ToString() + " " + txtDescription.text;

        if (!BootStrap.GetInstance().userData.ExpertMode)
            OnPauseClicked();
    }

    IEnumerator coShowMessageObject(int idx)
    {
        yield break;
        //MessageObj.SetActive(true);

        //Debug.Log($"Playing Message idx {idx}..");

        //yield return new WaitForSeconds(1.0f);

        //MessageObj.SetActive(false);
    }
}
