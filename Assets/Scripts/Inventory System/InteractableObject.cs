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
    [Header("Dialog")]
    public DialogInputData[] dialogs;
    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaction() {
        if (canPickup) {
            Inventory inventory = Inventory.Instance;
            inventory.items.Add(this.gameObject);
            this.gameObject.SetActive(false);
        }
        if (dialogs.Length > 0) { 
            DialogInputData dialog = dialogs[Random.Range(0, dialogs.Length)];
            DialogData dialogData = new DialogData(dialog.GetFormattedMessage(), dialog.character, null, dialog.skippable);
            dialogManager.Show(dialogData);
        }
    }
}
