using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject interactionWorldUI;
    public GameObject interactionWorldUIInstance;
    [Header("Inventory Details")]
    public string itemName;
    public string itemDescription;
    public int slot = -1;
    public GameObject prefab;
    [Header("Dialog")]
    public DialogInputData[] dialogs;
    public DialogInputData[] inspectDialogs;
    public DialogManager dialogManager;
    [Header("Use Trigger Events")]
    public Boolean gameManagerTrigger;
    #if UNITY_EDITOR
    [EventList("Use Workbench", "Exit Workbench", "Generator Start", "Game Over")]
    #endif
    public string gameManagerEventName;
    public InteractionAction nextActionAfterUsage;
    public Boolean animationTrigger;
    public string animationTriggerName;
    [Header("Item/Action Required")]
    public InteractionAction nextActionAfterCompletion;
    public string itemRequired;
    public Boolean actionBased;
    public string actionName;
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
                Use();
                break;
            case InteractionAction.Inspect:
                ItemsRequiredForNextAction();
                break;
            default:
                ShowDialogs(dialogs);
                break;
        }
    }

    private void Use()
    {
        if (gameManagerTrigger)
        {
            GameManager.Instance.TriggerEvent(gameManagerEventName);
        }
        Animator animator = GetComponent<Animator>();
        if (animationTrigger && animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }
        interactionAction = nextActionAfterUsage;
    }

    private void ItemUsage()
    {
        throw new NotImplementedException();
    }

    private void ItemsRequiredForNextAction()
    {
        if (actionBased)
        {
            if (GameManager.Instance.actionsDone.Contains(actionName))
            {
                this.ItemsFound();
                return;
            }
        }
        else
        {
            Inventory.Instance.CheckInventory(this);
            if (Inventory.Instance.Open())
            {
                return;
            }
        }
        ShowDialogs(inspectDialogs);
    }

    public void ItemsFound()
    {
        interactionAction = nextActionAfterCompletion;
        foreach (GameObjectToggleEvent toggleEvent in completionToggles)
        {
            toggleEvent.gameObject.SetActive(toggleEvent.active);
        }
        foreach (BehaviourToggleEvent toggleBehaviors in completionBehaviors)
        {
            toggleBehaviors.behaviour.enabled = toggleBehaviors.active;
        }
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

    public void InteractionUI(Boolean active)
    {
        if (interactionWorldUIInstance == null)
        {
            interactionWorldUIInstance = GameObject.Instantiate(interactionWorldUI, transform, true);
            interactionWorldUIInstance.transform.localPosition = Vector3.zero;
        }
        TextMeshProUGUI interactionText = interactionWorldUIInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (interactionText != null)
        {
            interactionText.text = itemName;
        }
        interactionWorldUIInstance.SetActive(active);
        if (active==false)
        {
            Destroy(interactionWorldUIInstance);
            interactionWorldUIInstance = null;
        }
    }
}