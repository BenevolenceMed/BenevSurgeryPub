using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurgeTopUIController : MonoBehaviour
{
    [SerializeField] TMP_Text txtMode;

    UserDataHolder userDataRef = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        userDataRef = BootStrap.GetInstance().userData;
        txtMode.text = userDataRef.ExpertMode ? "Expert Mode" : "Beginner Mode";
    }
}
