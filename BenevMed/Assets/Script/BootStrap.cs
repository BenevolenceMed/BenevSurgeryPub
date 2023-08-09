using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootStrap
{
    public UserDataHolder userData { get; private set; }

    // Surgery Data.
    public SurgeListInfo SurgeListInfo { get; private set; }
    public SurgeDetailInfo SurgeDetailInfo { get; private set; }

    public void Init()
    {
        userData = new UserDataHolder();
        userData.Init();

        string strJson = Resources.Load<TextAsset>("Data/surginfo").text;
        SurgeListInfo = JsonUtility.FromJson<SurgeListInfo>(strJson);
    }

    public void LoadSurgeryDetailInfo(string surgName)
    {
        string strJson = Resources.Load<TextAsset>("Data/Details/AAA").text;
        SurgeDetailInfo = JsonUtility.FromJson<SurgeDetailInfo>(strJson);
    }


    static BootStrap sInstance = null;
    public static BootStrap GetInstance()
    {
        if(sInstance == null)
        {
            sInstance = new BootStrap();
            sInstance.Init();
        }
        return sInstance;
    }


}
