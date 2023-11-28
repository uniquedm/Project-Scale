using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Timeloop : MonoBehaviour
{
    public TextMeshProUGUI timeUI;
    public float timeElapsed;

    public GameObject player;
    public GameObject playerCamera;
    public GameObject respawnPoint;
    public String respawnSequence;

    [Range(10f, 300f)]
    public float loopLength = 60f;

    private Boolean startTimeloop;
    private Animator animator;
    private float originalCameraHeight;
    private Boolean startRespawn;
    private Behaviour pauseMenu;

    public float targetCameraHeight;
    public float lerpSpeed;

    public bool StartTimeloop { get => startTimeloop; set => startTimeloop = value; }

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
            timeUI.text = timeElapsed.ToString();
        }
        if (timeElapsed > loopLength)
        {
            startTimeloop = false;
            pauseMenu.enabled = false;
            timeElapsed = 0;
            animator.PlayInFixedTime(respawnSequence);
            startRespawn = true;
            StartCoroutine(spawnPlayer(2));
        }
        if (startRespawn) {
            Transform cameraTransform = playerCamera.GetComponent<Transform>();
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, targetCameraHeight, lerpSpeed * Time.deltaTime),
                cameraTransform.localPosition.z
            );
        }
    }

    IEnumerator spawnPlayer(int spawnTimeInSeconds)
    {
        yield return new WaitForSeconds(spawnTimeInSeconds);
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
    }
}
