using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

// Define the InteractionAction enum
[System.Serializable]
public enum InteractionAction
{
    Interact,
    Pickup,
    Inspect, // Item which requires some actions
    Use
}

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Details")]
    public InteractionAction interactionAction;
    [Header("Inventory Details")]
    public string itemName;
    public string itemDescription;
    public int slot = -1;
    public GameObject prefab;
    [Header("Dialog")]
    public DialogInputData[] dialogs;
    public DialogInputData[] inspectDialogs;
    public DialogManager dialogManager;
    [Header("Game Manager Event")]
    [EventList("Use Workbench", "Exit Workbench")]
    public string triggerEvent;
    public InteractionAction nextActionAfterUsage;
    [Header("Item Required")]
    public InteractionAction nextActionAfterCompletion;
    public string itemRequired;
    public List<GameObjectToggleEvent> completionToggles;
    public List<BehaviourToggleEvent> completionBehaviors;
    // Start is called before the first frame update
    void Start()
    {
        if (dialogManager == null) 
        {
            dialogManager = FindAnyObjectByType<DialogManager>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void Interaction()
    {
        switch (interactionAction)
        {
            case InteractionAction.Pickup:
                PickUp();
                ShowDialogs(dialogs);
                break;
            case InteractionAction.Use:
                GameManager.Instance.TriggerEvent(triggerEvent);
                interactionAction = nextActionAfterUsage;
                break;
            case InteractionAction.Inspect:
                ItemsRequiredForNextAction();
                break;
            default:
                ShowDialogs(dialogs);
                break;
        }
    }

    private void ItemUsage()
    {
        throw new NotImplementedException();
    }

    private void ItemsRequiredForNextAction()
    {
        Inventory.Instance.CheckInventory(this);
        if (!Inventory.Instance.Open())
        {
            ShowDialogs(inspectDialogs);
        }
    }

    public void ItemsFound()
    {
        interactionAction = nextActionAfterCompletion;
    }

    private void ShowDialogs(DialogInputData[] dialogs)
    {
        if (dialogs.Length > 0)
        {
            if (dialogManager == null)
            {
                Debug.LogError("Dialog Manager Not Found! Dialogs are Skipped");
            }
            else
            {
                dialogManager.Hide();
                DialogInputData dialog = dialogs[Random.Range(0, dialogs.Length)];
                DialogData dialogData = new DialogData(dialog.GetFormattedMessage(), dialog.character, null, dialog.skippable);
                dialogManager.Show(dialogData);
            }
        }
    }

    private void PickUp()
    {
        Inventory inventory = Inventory.Instance;
        InventoryItem itemData = new InventoryItem(this.itemName,
                                                    this.itemDescription,
                                                    this.slot,
                                                    this.gameObject,
                                                    this.prefab);
        inventory.ItemsData.Add(itemData);
        this.gameObject.SetActive(false);
    }
}