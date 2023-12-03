using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TimeScaleDevice : MonoBehaviour
{
    // Singleton instance
    private static TimeScaleDevice _instance;

    // Public property to access the singleton instance
    public static TimeScaleDevice Instance
    {
        get
        {
            // If the instance is null, find the existing instance in the scene
            if (_instance == null)
            {
                _instance = FindObjectOfType<TimeScaleDevice>();

                // If no instance is found, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("TimeScaleDevice");
                    _instance = singletonObject.AddComponent<TimeScaleDevice>();
                }
            }

            // Return the singleton instance
            return _instance;
        }
    }

    // Optional: Add your other class members here
    public GameObject timescaleGameObject;
    public GameObject timescaleOpenUI;
    public GameObject timescaleUseUI;
    public KeyCode timecaleOpenKey = KeyCode.F;
    public Boolean timescaleHeld;

    [Header("Reverse Event Detail")]
    public Animator reverseEventAnimator;
    public float reverseEventTime;
    public AudioClip reverseSFX;
    public GameObject reverseVFX;
    public String triggerEvent;
    private AudioSource audioSource;
    public VolumeProfile reverseProfile;
    private VolumeProfile currentProfile;
    private Camera mainCamera;
    public List<GameObjectToggleEvent> gameEventsAfterReversal;
    public List<BehaviourToggleEvent> behavioursAfterReversal;
    public Boolean disableTimeScale = false;
    private bool isBeingReversedCoroutineRunning = false;
    public GameObject eventAvailableUI;


    // Ensure the instance is not destroyed when reloading the scene
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            // If an instance already exists, destroy this new instance
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        timescaleOpenUI.SetActive(false);
        timescaleUseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            timescaleOpenUI.SetActive(true);
        }
        else
        {
            timescaleOpenUI.SetActive(false);
            timescaleUseUI.SetActive(false);
        }
        
        if (Input.GetKeyDown(timecaleOpenKey))
        {
            timescaleHeld = !timescaleHeld;
        }
        timescaleGameObject.SetActive(timescaleHeld);
        if (CanReverseTime() && timescaleHeld && Input.GetMouseButtonDown(0))
        {
            isBeingReversedCoroutineRunning = true;
            StartCoroutine(ReverseTime());
            timescaleHeld = false;
        }
        if (timescaleHeld)
        {
            timescaleUseUI.SetActive(timescaleHeld);
        }
        if (disableTimeScale && !isBeingReversedCoroutineRunning)
        {
            disableTimeScale = false;
            this.enabled = false;
        }
    }

    private IEnumerator ReverseTime()
    {
        PlaySFX(reverseSFX);
        mainCamera = Camera.main;
        Volume postProcessing = mainCamera.GetComponent<Volume>();
        currentProfile = postProcessing.profile;
        postProcessing.profile = reverseProfile;
        reverseVFX.SetActive(true);
        reverseEventAnimator.SetTrigger(triggerEvent);
        yield return new WaitForSeconds(reverseEventTime);
        postProcessing.profile = currentProfile;
        foreach (BehaviourToggleEvent behaviour in behavioursAfterReversal)
        {
            behaviour.behaviour.enabled = behaviour.active;
        }
        foreach (GameObjectToggleEvent toggleEvent in gameEventsAfterReversal)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
        isBeingReversedCoroutineRunning = false;
    }

    private Boolean CanReverseTime()
    {
        Boolean canReverse = reverseEventAnimator != null && triggerEvent != null;
        eventAvailableUI.SetActive(canReverse);
        return canReverse;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource == null)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }
}
