using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootStrap
{
    public UserDataHolder userData { get; private set; }

    public void Init()
    {
        userData = new UserDataHolder();
        userData.Init();
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
