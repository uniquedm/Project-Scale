using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Boolean timebasedRestart;
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
    private Boolean startRespawn;
    public float spawnTime = 5f;
    private int respawnCount = 0;
    public List<DialogInputData> respawnDialogs;
    public Image fadeOverlay;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;
    public List<GameObjectToggleEvent> respawnGameEvents;
    public List<BehaviourToggleEvent> respawnBehaviours;

    [Header("Misc")]
    public DialogManager dialogManager;
    private Behaviour pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        startTimeloop = false;
        startRespawn = false;
        timeElapsed = 0f;
        originalCameraHeight = playerCamera.GetComponent<Transform>().localPosition.y;
        pauseMenu = GetComponent<PauseMenu>();
    }

    void FixedUpdate()
    {
        if (startTimeloop) {
            timeElapsed += Time.deltaTime;
        }
        if (timebasedRestart && timeElapsed > loopLength)
        {
            TriggerTimeloop();
        }
        if (startRespawn)
        {
            RespawnStart();
        }
    }

    public void TriggerTimeloop()
    {
        startTimeloop = false;
        pauseMenu.enabled = false;
        timeElapsed = 0;
        startRespawn = true;
        StartCoroutine(spawnPlayer(spawnTime));
        StartCoroutine(FadeInOut());
    }

    private void RespawnStart()
    {
        Transform cameraTransform = playerCamera.GetComponent<Transform>();
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            Mathf.Lerp(cameraTransform.localPosition.y, playerFalldownHeight, cameraLerpSpeed * Time.deltaTime),
            cameraTransform.localPosition.z
        );
    }

    IEnumerator spawnPlayer(float spawnTimeInSeconds)
    {
        yield return new WaitForSeconds(spawnTimeInSeconds);
        dialogManager.Hide();
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
        GameManager.Instance.PlayerCanMove(true);
        foreach (GameObjectToggleEvent toggleEvent in respawnGameEvents)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
        foreach (BehaviourToggleEvent behaviour in respawnBehaviours)
        {
            behaviour.behaviour.enabled = behaviour.active;
        }
    }

    IEnumerator FadeInOut()
    {
        // Fade in
        yield return Fade(fadeOverlay, 1f, fadeInTime);

        // Wait for a moment before fading out (you can adjust this time)
        yield return new WaitForSeconds(1f);

        // Fade out
        yield return Fade(fadeOverlay, 0f, fadeOutTime);

        // You can repeat the process or perform other actions here
    }

    IEnumerator Fade(Image image, float targetAlpha, float fadeTime)
    {
        // Store the original alpha value
        float startAlpha = image.color.a;

        // Calculate the rate of change for the alpha value
        float rate = 1.0f / fadeTime;

        // Keep track of time passed
        float timePassed = 0f;

        while (timePassed < 1.0f)
        {
            // Interpolate the alpha value over time
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timePassed);

            // Update the image's color
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            // Increment the time passed based on the frame rate
            timePassed += Time.deltaTime * rate;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final alpha value is set
        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
    }
}
