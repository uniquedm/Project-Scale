using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableObject : MonoBehaviour
{
    public Boolean canPickup;
    [Header("Inventory Details")]
    public string itemName;
    public string itemDescription;
    public int slot = -1;
    public GameObject prefab;
    [Header("Dialog")]
    public DialogInputData[] dialogs;
    public DialogManager dialogManager;
    [Header("Game Manager Event")]
    public Boolean triggerEvent;
    [EventList("Use Workbench", "Exit Workbench")]
    public string eventName;

    // Start is called before the first frame update
    void Start()
    {
        if (dialogManager == null) 
        {
            dialogManager = FindAnyObjectByType<DialogManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaction() {
        if (canPickup) {
            Inventory inventory = Inventory.Instance;
            InventoryItem itemData = new InventoryItem(this.itemName, 
                                                        this.itemDescription, 
                                                        this.slot, 
                                                        this.gameObject,
                                                        this.prefab);
            inventory.ItemsData.Add(itemData);
            this.gameObject.SetActive(false);
        }
        if (dialogs.Length > 0) { 
            DialogInputData dialog = dialogs[Random.Range(0, dialogs.Length)];
            DialogData dialogData = new DialogData(dialog.GetFormattedMessage(), dialog.character, null, dialog.skippable);
            dialogManager.Show(dialogData);
        }
        if (triggerEvent)
        {
            GameManager.Instance.TriggerEvent(eventName);
        }
    }
}
