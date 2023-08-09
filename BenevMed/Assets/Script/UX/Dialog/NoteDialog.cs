using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteDialog : MonoBehaviour
{
    public class ReturnData
    {
        public bool ok { get; set; }
        public bool delete { get; set; }
        public bool jump { get; set; }
        public string content { get; set; }
        public void Clear()
        {
            ok = false; delete = false;     jump = false; content = "";
        }
    }
    [SerializeField] TMP_Text TxtContent;
    [SerializeField] GameObject ScrollView;
    [SerializeField] GameObject InputField;
    [SerializeField] GameObject BtnEdit;
    [SerializeField] GameObject BtnDone;
    [SerializeField] GameObject BtnDelete;

    System.Action<ReturnData> mCloseCallback = null;
    ReturnData mReturnData = new ReturnData();
    NoteData mNoteData = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // index : note index ( -1 : new one )
    public void Init(int index, System.Action<ReturnData> closeCallBack)
    {
        mReturnData.Clear();

        // Update Mode.
        if (index >= 0)
        {
            mNoteData = BootStrap.GetInstance().userData.GetNoteByIndex(BootStrap.GetInstance().userData.CurrentLearning, index);
            TxtContent.text = mNoteData.Content;
            InputField.SetActive(false);
            ScrollView.SetActive(true);
            BtnEdit.SetActive(true);
            BtnDone.SetActive(false);
            BtnDelete.SetActive(true);
        }
        // New Mode.
        else
        {
            ScrollView.SetActive(false);
            InputField.SetActive(true);
            BtnEdit.SetActive(false);
            BtnDone.SetActive(true);
            BtnDelete.SetActive(false);
        }

        mCloseCallback = closeCallBack;
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        
        mReturnData.ok = false;
        mReturnData.delete = false;
        mReturnData.content = TxtContent.text;
        if (mCloseCallback != null)
            mCloseCallback.Invoke(mReturnData);
    }

    public void OnEdit()
    {
        InputField.GetComponent<TMP_InputField>().text = TxtContent.text;

        ScrollView.SetActive(false);
        InputField.SetActive(true);
        BtnEdit.SetActive(false);
        BtnDone.SetActive(true);
    }

    public void OnDeleteClicked()
    {
        gameObject.SetActive(false);

        mReturnData.ok = true;
        mReturnData.delete = true;
        if (mCloseCallback != null)
            mCloseCallback.Invoke(mReturnData);
    }

    public void OnJumpToPosition()
    {
        gameObject.SetActive(false);
        mReturnData.ok = true;
        mReturnData.delete = true;
        mReturnData.jump = true;
        if (mCloseCallback != null)
            mCloseCallback.Invoke(mReturnData);
    }

    public void OnBtnOK()
    {
        // save this change to data and close popup.
        //
        gameObject.SetActive(false);
        
        mReturnData.ok = true;
        mReturnData.delete = false;
        mReturnData.content = TxtContent.text;
        if (mCloseCallback != null)
            mCloseCallback.Invoke(mReturnData);
    }

    public void OnEditDone()
    {
        TxtContent.text = InputField.GetComponent<TMP_InputField>().text;

        ScrollView.SetActive(true);
        InputField.SetActive(false);
        BtnEdit.SetActive(true);
        BtnDone.SetActive(false);
    }
}
