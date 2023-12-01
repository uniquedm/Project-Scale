using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TimescaleEventTrigger : MonoBehaviour
{
    [Header("Reverse Event Detail")]
    public Animator reverseEventAnimator;
    public float reverseEventTime;
    public String triggerEvent;
    public List<GameObjectToggleEvent> gameEventsAfterReversal;
    public List<BehaviourToggleEvent> behavioursAfterReversal;

    private TimeScaleDevice timeScaleDevice;

    private void OnEnable()
    {
        timeScaleDevice = TimeScaleDevice.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            timeScaleDevice.reverseEventAnimator = reverseEventAnimator;
            timeScaleDevice.reverseEventTime = reverseEventTime;
            timeScaleDevice.triggerEvent = triggerEvent;
            timeScaleDevice.gameEventsAfterReversal = gameEventsAfterReversal;
            timeScaleDevice.behavioursAfterReversal = behavioursAfterReversal;
        }
    }

    private void OnDisable()
    {
        ClearTimeScale();
        timeScaleDevice.disableTimeScale = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ClearTimeScale();
        }
    }

    private void ClearTimeScale()
    {
        timeScaleDevice.reverseEventAnimator = null;
        timeScaleDevice.reverseEventTime = 0f;
        timeScaleDevice.triggerEvent = null;
        timeScaleDevice.gameEventsAfterReversal = null;
        timeScaleDevice.behavioursAfterReversal = null;
    }
}
