using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Events;

public class StartCount : MonoBehaviour
{
    //public PlayableDirector beforePushed;

    [SerializeField] private UnityEvent OnPush;

    private Button button;

    void OnEnable()
    {
        button = GetComponent<Button>();
    }

    public void PlayTimeline()
    {
        if (GameObject.Find("VRM") != null)
        {
            OnPush.Invoke();
        }
    }
}
