using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HomeView : MonoBehaviour
{
    [SerializeField] GameObject scrollView;

    [SerializeField] GameObject MRecentLearningView;

    // Start is called before the first frame update
    void Start()
    {
        if(!MRecentLearningView.activeSelf)
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1800);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
