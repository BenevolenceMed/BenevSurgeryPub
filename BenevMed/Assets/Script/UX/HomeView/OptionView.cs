using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickClearCache()
    {
        PlayerPrefs.DeleteAll();
    }
}
