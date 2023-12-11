using com.cyborgAssets.inspectorButtonPro;
using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Pagination;
using UI.ThreeDimensional;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public GameObject inventoryUIBlur;
    public GameObject inventoryCamera;
    public GameObject itemPreviewLocation;
    public int focusedLayer;
    private GameObject currentItem;
    public float rotationSpeed = 10f;
    private int currentIndex = 0;
    public TextMeshProUGUI itemNameUIText;
    public TextMeshProUGUI itemDescriptionUIText;
    private DialogManager dialogManager;
    public PagedRect pagedRect;
    public Dictionary<InventoryItem, Page> itemPageMap;
    [Header("Inventory Data")]
    public List<InventoryItem> itemsData;
    [Header("Inventory SFX")]
    private AudioSource audioSource;
    public AudioClip inventoryOpenSFX;
    public AudioClip inventoryCloseSFX;
    public AudioClip scrollSFX;
    public AudioClip itemUseSFX;
    [Header("Inventory Interaction")]
    [TextArea(3, 10)]
    public string wrongItemDialog = "/color:#e84200/I don't think it can be used here.../close/";
    public AudioClip wrongItemSFX;
    public AudioClip correctItemSFX;

    private void PlaySFX(AudioClip sfxClip)
    {
        if (audioSource == null || sfxClip == null)
        {
            return;
        }
        audioSource.PlayOneShot(sfxClip);
    }

    // Start is called before the first frame update
    void Start()
    {
        itemPageMap = new Dictionary<InventoryItem, Page>();
        dialogManager = FindAnyObjectByType<DialogManager>();
        itemsData = new List<InventoryItem>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
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
        else if (index == 0)
        {
            pagedRect.ShowFirstPage();
        }
        inventoryUI.SetActive(toggle);
        inventoryUIBlur.SetActive(toggle);
        // TODO: Check the issues with this
        /*inventoryCamera.SetActive(toggle);*/
        Behaviour behaviour = this.GetComponent<FirstPersonMovement>();
        behaviour.enabled = !toggle;
        if (index < 0)
        {
            index = itemsData.Count - 1;
        }
        else if (itemsData.Count > 0)
        {
            index %= itemsData.Count;
        }
        currentIndex = index;
        foreach (InventoryItem item in itemsData)
        {
            if (!itemPageMap.ContainsKey(item))
            {
                itemPageMap.Add(item, AddPage(item));
            }
        }
        if (itemsData.Count > 0 && itemsData[index].item != currentItem)
        {
            PlaySFX(scrollSFX);
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
        if (itemsData.Count > 0)
        {
            ToggleInventory(true);
            StartCoroutine(CheckItemSelection(interactableObject));
        }
    }

    private IEnumerator CheckItemSelection(InteractableObject interactableObject)
    {
        bool itemMatched = false;
        // Continue checking until the item names match
        while (!itemMatched)
        {
            Debug.Log("Running Coroutine!");
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
                dialogManager.Hide();
                DialogData dialogData = new DialogData(wrongItemDialog, "Player", null, true);
                // TODO: Play wrong item SFX
                dialogManager.Show(dialogData);
                yield return null;
            }
            else
            {
                GameObject gameObject = itemsData[currentIndex].item;
                itemsData.Remove(itemsData[currentIndex]);
                RemoveCurrentPage();
                Destroy(gameObject);
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

    public Page AddPage(InventoryItem item)
    {
        Page newPage = pagedRect.AddPageUsingTemplate();
        UIObject3D objectPreview = newPage.GetComponentInChildren<UIObject3D>();
        UIObject3DImage objectPreviImage = newPage.GetComponentInChildren<UIObject3DImage>();
        objectPreview.ObjectPrefab = item.prefab.transform;
        objectPreviImage.color = Color.white;
        return newPage;
    }

    public void RemoveCurrentPage()
    {
        pagedRect.RemoveCurrentPage();
    }
}
