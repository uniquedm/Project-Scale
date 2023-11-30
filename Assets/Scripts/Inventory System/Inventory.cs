using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.ThreeDimensional;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct InventoryItem
{
    public string itemName;
    public string itemDescription;
    public int slot;
    public GameObject item;
    public GameObject prefab;

    public InventoryItem(string itemName, string itemDescription, int slot, GameObject item, GameObject prefab) : this()
    {
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.slot = slot;
        this.item = item;
        this.prefab = prefab;
    }
}

public class Inventory : MonoBehaviour
{
    // Singleton instance
    private static Inventory _instance;

    // Public property to access the Inventory instance
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
            {
                // If the instance is null, find the Inventory in the scene
                _instance = FindObjectOfType<Inventory>();

                // If no Inventory is found, create a new GameObject and add the Inventory script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("Inventory");
                    _instance = singletonObject.AddComponent<Inventory>();
                }
            }

            return _instance;
        }
    }

    public List<InventoryItem> ItemsData { 
        get => itemsData;
        set => itemsData = value;
    }

    [Header("Inventory UI")]
    public GameObject inventoryUI;
    public GameObject inventoryCamera;
    public Volume renderVolume;
    public VolumeProfile inventoryProfile;
    public VolumeProfile renderProfile;
    public GameObject itemPreviewLocation;
    public int focusedLayer;
    private GameObject currentItem;
    public float rotationSpeed = 10f;
    private int currentIndex = 0;
    public TextMeshProUGUI itemNameUIText;
    public TextMeshProUGUI itemDescriptionUIText;
    [Header("Inventory Data")]
    public List<InventoryItem> itemsData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I)) {
            ToggleInventory(!inventoryUI.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory(false);
        }

        if (inventoryUI.activeSelf)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0f) // Scroll Up
            {
                ToggleInventory(inventoryUI.activeSelf, currentIndex + 1);
            }
            else if (scrollInput < 0f) // Scroll Down
            {
                ToggleInventory(inventoryUI.activeSelf, currentIndex - 1);
            }
        }

        if (currentItem != null)
        {
            // Calculate the rotation angle based on time
            float angle = Mathf.Repeat(Time.time * rotationSpeed, 360f);

            // Set the rotation of the object around the y-axis
            currentItem.transform.localRotation = Quaternion.Euler(-66f, angle, 0f);
        }
    }

    public void ToggleInventory(bool toggle, int index = 0)
    {
        if (toggle && itemsData.Count == 0)
        {
            ToggleInventory(false, index);
            return;
        }
        if (!toggle)
        {
            StopAllCoroutines();
        }
        inventoryUI.SetActive(toggle);
        inventoryCamera.SetActive(toggle);
        Behaviour behaviour = this.GetComponent<FirstPersonMovement>();
        behaviour.enabled = !toggle;
        renderVolume.profile = toggle ? inventoryProfile : renderProfile;
        if (index < 0)
        {
            index = itemsData.Count - 1;
        }
        else if (itemsData.Count > 0)
        {
            index %= itemsData.Count;
        }
        currentIndex = index;
        if (itemsData.Count > 0)
        {
            itemNameUIText.enabled = toggle;
            itemDescriptionUIText.enabled = toggle;
            itemsData[index].item.transform.parent = itemPreviewLocation.transform;
            itemsData[index].item.transform.localPosition = Vector3.zero;
            itemsData[index].item.tag = "Untagged";
            itemsData[index].item.layer = focusedLayer;
            itemsData[index].item.GetComponent<BoxCollider>().enabled = false;
            currentItem = itemsData[index].item;
            InteractableObject interactableObject = itemsData[index].item.GetComponent<InteractableObject>();
            itemNameUIText.text = interactableObject.itemName;
            itemDescriptionUIText.text = interactableObject.itemDescription;
            for (int i = 0; i < itemsData.Count; i++) {
                if (i==index) {
                    itemsData[i].item.SetActive(toggle);
                }
                else
                {
                    itemsData[i].item.SetActive(false);
                }
            }
        }
    }

    internal void CheckInventory(InteractableObject interactableObject)
    {
        ToggleInventory(true);
        StartCoroutine(CheckItemSelection(interactableObject));
    }

    private IEnumerator CheckItemSelection(InteractableObject interactableObject)
    {
        bool itemMatched = false;
        // Continue checking until the item names match
        while (!itemMatched)
        {
            // Wait for a left mouse click
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            Debug.Log("Checking!");
            Debug.Log(itemsData[currentIndex].itemName + " -> " + interactableObject.itemRequired);
            // Mouse click detected, perform your logic
            if (itemsData[currentIndex].itemName != interactableObject.itemRequired)
            {
                yield return null;
            }
            else
            {
                interactableObject.ItemsFound();
                ToggleInventory(false);
                itemMatched = true; // Set the flag to exit the loop
            }
        }
    }

    internal bool Open()
    {
        return this.inventoryUI.activeSelf;
    }
}
