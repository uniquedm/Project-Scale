using System;
using System.Collections.Generic;
using UI.ThreeDimensional;
using Unity.VisualScripting;
using UnityEngine;

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
    }

    // Your GameManager methods and properties go here
    public HashSet<string> actionsDone;
    [Header("Generator Events")]
    public List<GameObjectToggleEvent> generatorGameEvents;
    public List<BehaviourToggleEvent> generatorBehaviours;
    [Header("Workbench Inventory")]
    public List<UIObject3D> inventorySlots;
    [Header("Workbench Events")]
    public List<GameObjectToggleEvent> workbenchGameEvents;
    [Header("Player Movement Behaviours")]
    public List<BehaviourToggleEvent> playerMovementBehaviours;
    [Header("Start Game Events")]
    public List<GameObjectToggleEvent> startGameEvents;
    [Header("Clear Scene Events")]
    public List<GameObjectToggleEvent> clearSceneEvents;

    // Start is called before the first frame update
    void Start()
    {
        actionsDone = new HashSet<string>();
    }

    // Update is called once per frame
    void Update()
    {

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
            default:
                Debug.LogWarning($"Event '{eventName}' not handled.");
                break;
        }
    }

    private void GeneratorStart()
    {
        actionsDone.Add("GeneratorPowered");
        foreach (GameObjectToggleEvent toggleEvent in generatorGameEvents)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
        foreach (BehaviourToggleEvent generatorBehaviors in generatorBehaviours)
        {
            generatorBehaviors.behaviour.enabled = generatorBehaviors.active;
        }
    }

    private void ClearScene()
    {
        foreach (GameObjectToggleEvent toggleEvent in clearSceneEvents)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
    }

    private void StartGame()
    {
        foreach (GameObjectToggleEvent toggleEvent in startGameEvents)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
    }

    private void PlayerCanMove(bool canMove)
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
        foreach (InventoryItem inventoryItem in itemsData)
        {
            int itemSlot = inventoryItem.slot;
            Debug.Log(inventoryItem.item + " will be assigned to " + itemSlot.ToString());
            if (itemSlot == -1)
            {
                continue;
            }
            Debug.Log("Inventory has " + inventorySlots.Count.ToString() + "Slots");
            if (inventorySlots.Count >= itemSlot)
            {
                inventorySlots[itemSlot].ObjectPrefab = inventoryItem.prefab.transform;
                Debug.Log(inventoryItem.item + " is assigned to " + itemSlot.ToString());
            }
        }
    }
}
