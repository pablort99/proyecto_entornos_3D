using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider))]

public class TimeLineActivator : MonoBehaviour
{
    // Start is called before the first frame update
  
    public PlayableDirector playableDirector;
    public string playerTag;
    public Transform interactionLocation;
    public bool autoActivate = false;   

    public bool interact { get;  set; }

    [Header("Activation Zone Events")]
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    [Header("Timeline events")]
    public UnityEvent OnTimeLineStart;
    public UnityEvent OnTimeLineEnd;

    public bool isPlaying;
    public bool PlayerInside;
    private Transform playerTransform;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            PlayerInside = true;
            playerTransform = other.transform;
            onPlayerEnter.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals(playerTag))
        {
            PlayerInside = false;
            playerTransform = null;
            onPlayerExit.Invoke();  
        }
    }

    private void PlayTimeLine()
    {
        if(playerTransform && interactionLocation)
        {
            playerTransform.SetPositionAndRotation(interactionLocation.position, interactionLocation.rotation);

            if (autoActivate)
            {
                PlayerInside = false;
            }
            if (playableDirector)
            {
                playableDirector.Play();
            }

            isPlaying = true;
            interact=false;

            StartCoroutine(WaitForTimeLineToEnd());
        }
    }

    private IEnumerator WaitForTimeLineToEnd()
    {
        OnTimeLineStart.Invoke();

        float timeLineDuration = (float)playableDirector.duration;

        while(timeLineDuration > 0)
        {
            timeLineDuration -= Time.deltaTime;
            yield return null;
        }

        isPlaying = false;
        OnTimeLineEnd.Invoke(); 
    }

    private void Update()
    {
        if (PlayerInside && !isPlaying)
        {
            if (interact || autoActivate)
            {
                PlayTimeLine();
            }
        }
    }

}

