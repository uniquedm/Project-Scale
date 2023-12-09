using Doublsb.Dialog;
using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UI.ThreeDimensional;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct GameObjectToggleEvent
{
    public string eventName;
    public GameObject gameObject;
    public Boolean active;
}

[System.Serializable]
public struct BehaviourToggleEvent
{
    public string eventName;
    public Behaviour behaviour;
    public Boolean active;
}

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;

    // Public property to access the GameManager instance
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // If the instance is null, find the GameManager in the scene
                _instance = FindObjectOfType<GameManager>();

                // If no GameManager is found, create a new GameObject and add the GameManager script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    // Your GameManager logic goes here

    private void Awake()
    {
        // Ensure there is only one instance of the GameManager
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject); // Don't destroy the GameManager when loading new scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
        }
        notificationUIStatic = notificationUI;
        notificationTextStatic = notificationText;
    }

    // Your GameManager methods and properties go here
    public HashSet<string> actionsDone;
    [Header("Generator Events")]
    public UnityEvent generatorEvents;
    [Header("Workbench Inventory")]
    public List<UIObject3D> inventorySlots;
    [Header("Workbench Events")]
    public List<GameObjectToggleEvent> workbenchGameEvents;
    public UnityEvent workbenchEvents;
    [Header("Player Movement Behaviours")]
    public List<BehaviourToggleEvent> playerMovementBehaviours;
    public UnityEvent playerMovementEvents;
    [Header("Start Game Events")]
    public UnityEvent startSceneEvents;
    [Header("Clear Scene Events")]
    public UnityEvent cleanUpSceneEvents;
    [Header("Avalanche Events")]
    public UnityEvent avalancheEvents;
    [Header("Timescale Fix")]
    public int componentsRequiredForFixing = 3;
    public UnityEvent OnTimescaleFix;
    [Header("End Scene")]
    public string sceneName;
    public Image overlayBeforeLoading;
    public float fadeDuration = 3f;
    [Header("Notification Event")]
    public LeanPulse notificationUI;
    public TextMeshProUGUI notificationText;
    public static LeanPulse notificationUIStatic;
    public static TextMeshProUGUI notificationTextStatic;

    // Start is called before the first frame update
    void Start()
    {
        actionsDone = new HashSet<string>();
    }

    public void TriggerEvent(string eventName)
    {
        switch (eventName)
        {
            case "Use Workbench":
                WorkbenchLoad();
                break;
            case "Exit Workbench":
                WorkbenchUnload();
                break;
            case "Generator Start":
                GeneratorStart();
                break;
            case "Enable Player Movement":
                PlayerCanMove(true);
                break;
            case "Disable Player Movement":
                PlayerCanMove(false);
                break;
            case "Start Game":
                StartGame();
                break;
            case "Clear Scene":
                ClearScene();
                break;
            case "Avalanche":
                Avalanche();
                break;
            case "Timescale Fixed":
                TimescaleFixed();
                break;
            case "Game Over":
                EndScene();
                break;
            default:
                Debug.LogWarning($"Event '{eventName}' not handled.");
                break;
        }
    }

    private void EndScene()
    {
        Debug.Log("End Scene!");
        PlayerCanMove(false);
        StartCoroutine(GameEnding());
    }

    private void Avalanche()
    {
        PlayerCanMove(false);
        avalancheEvents.Invoke();
        actionsDone.Add("Avalanche");
        Timeloop timeloop = GetComponent<Timeloop>();
        if (timeloop != null)
        {
            timeloop.TriggerTimeloop();
        }
        actionsDone.Add("TimescalePickedUp");
    }

    private void GeneratorStart()
    {
        actionsDone.Add("GeneratorPowered");
        generatorEvents.Invoke();
    }

    private void ClearScene()
    {
        cleanUpSceneEvents.Invoke();
    }

    private void StartGame()
    {
        startSceneEvents.Invoke();
    }

    public void PlayerCanMove(bool canMove)
    {
        foreach (BehaviourToggleEvent movementBehaviour in playerMovementBehaviours)
        {
            movementBehaviour.behaviour.enabled = canMove;
        }
    }

    // Example method for the "WorkbenchLoad" case
    private void WorkbenchLoad()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        WorkbenchLoadSlots();
        foreach (GameObjectToggleEvent toggleEvent in workbenchGameEvents)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
        Debug.Log("Workbench is loaded!");
    }

    private void WorkbenchUnload()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        foreach (GameObjectToggleEvent toggleEvent in workbenchGameEvents)
        {
            toggleEvent.gameObject.SetActive(!toggleEvent.active);
        }
        Debug.Log("Workbench Exited!");
    }

    private void WorkbenchLoadSlots()
    {
        List<InventoryItem> itemsData = Inventory.Instance.ItemsData;
        HashSet<int> slotsUsed = new HashSet<int>();
        foreach (InventoryItem inventoryItem in itemsData)
        {
            int itemSlot = inventoryItem.slot;
            if (itemSlot == -1)
            {
                continue;
            }
            if (itemSlot < inventorySlots.Count)
            {
                inventorySlots[itemSlot].ObjectPrefab = inventoryItem.prefab.transform;
                slotsUsed.Add(itemSlot);
            }
        }
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            UIObject3DImage renderImage = inventorySlots[i].GetComponent<UIObject3DImage>();
            if (slotsUsed.Contains(i))
            {
                if (renderImage != null)
                {
                    renderImage.enabled = true;
                }
                continue;
            }

            if (renderImage != null)
            {
                renderImage.enabled = false;
            }

        }
    }

    private void TimescaleFixed()
    {
        Debug.Log("Timescale Fixed!");
        Inventory.Instance.itemsData.Clear();
        actionsDone.Add("Timescale Fixed");
        TriggerEvent("Exit Workbench");
        OnTimescaleFix.Invoke();
    }

    public void WorkbenchInteractions(string message)
    {
        DialogManager dialogManager = FindAnyObjectByType<DialogManager>();
        dialogManager.Hide();
        DialogData dialogData = new DialogData(message, "Player", null, true);
        dialogManager.Show(dialogData);
    }

    public void TimescaleComponentFixed()
    {
        componentsRequiredForFixing--;
        if (componentsRequiredForFixing == 0)
        {
            TriggerEvent("Timescale Fixed");
        }
    }

    IEnumerator GameEnding()
    {
        // Ensure the image is not null
        if (overlayBeforeLoading == null)
        {
            Debug.LogError("Image to fade is not assigned!");
            yield break;
        }

        // Set the initial color of the image
        Color currentColor = overlayBeforeLoading.color;

        // Loop until the fade is complete
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            // Calculate the alpha value based on the elapsed time
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            // Set the new color with the updated alpha value
            overlayBeforeLoading.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

            // Wait for the next frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        // Ensure the final color is fully opaque
        overlayBeforeLoading.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);

        // Load the next scene
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    [com.cyborgAssets.inspectorButtonPro.ProPlayButton]
    public void TestNotification()
    {
        Notify("Testing");
    }

    public static void Notify(string message)
    {
        notificationTextStatic.text = message;
        notificationUIStatic.Pulse();
    }
}
