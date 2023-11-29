using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogInputData
{
    public string message;
    public string character;
    public bool skippable;
    public AudioClip audio;
    public float waitTime;
    // Method to format the message based on other fields
    public string GetFormattedMessage()
    {
        return $"{message}/wait:{waitTime}//close/";
    }
}


public class Timeloop : MonoBehaviour
{
    [Header("Timer Details")]
    public float timeElapsed;
    [Range(10f, 300f)]
    public float loopLength = 60f;
    private Boolean startTimeloop;
    public bool StartTimeloop { get => startTimeloop; set => startTimeloop = value; }

    [Header("Player Details")]
    public GameObject player;
    public GameObject playerCamera;
    public float cameraLerpSpeed;
    public float playerFalldownHeight;
    private float originalCameraHeight;

    [Header("Respawn Details")]
    public GameObject respawnPoint;
    public String respawnSequence;
    private Boolean startRespawn;
    private Animator animator;
    public float spawnTime = 2f;
    private int respawnCount = 0;
    public List<DialogInputData> respawnDialogs;

    [Header("Misc")]
    public DialogManager dialogManager;
    private Behaviour pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        startTimeloop = false;
        startRespawn = false;
        timeElapsed = 0f;
        animator = GetComponent<Animator>();
        originalCameraHeight = playerCamera.GetComponent<Transform>().localPosition.y;
        pauseMenu = GetComponent<PauseMenu>();
    }

    void FixedUpdate()
    {
        if (startTimeloop) {
            timeElapsed += Time.deltaTime;
        }
        if (timeElapsed > loopLength)
        {
            startTimeloop = false;
            pauseMenu.enabled = false;
            timeElapsed = 0;
            animator.PlayInFixedTime(respawnSequence);
            startRespawn = true;
            StartCoroutine(spawnPlayer(spawnTime));
        }
        if (startRespawn) {
            Transform cameraTransform = playerCamera.GetComponent<Transform>();
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, playerFalldownHeight, cameraLerpSpeed * Time.deltaTime),
                cameraTransform.localPosition.z
            );
        }
    }

    IEnumerator spawnPlayer(float spawnTimeInSeconds)
    {
        yield return new WaitForSeconds(spawnTimeInSeconds);
        DialogInputData data = respawnDialogs[respawnCount < respawnDialogs.Count ? respawnCount : respawnDialogs.Count - 1];
        DialogData dialogData = new DialogData(data.GetFormattedMessage(), data.character, null, data.skippable);
        dialogManager.Show(dialogData);
        startRespawn = false;
        startTimeloop = true;
        pauseMenu.enabled = true;
        Transform playerTransform = player.GetComponent<Transform>();
        playerTransform.SetLocalPositionAndRotation(respawnPoint.transform.localPosition, respawnPoint.transform.localRotation);
        Transform cameraTransform = playerCamera.GetComponent<Transform>();
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            originalCameraHeight,
            cameraTransform.localPosition.z
        );
        cameraTransform.localRotation = Quaternion.identity;
        respawnCount++;
    }
}
