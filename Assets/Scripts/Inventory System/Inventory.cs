using com.cyborgAssets.inspectorButtonPro;
using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TextMeshProUGUI itemNameUIText;
    public TextMeshProUGUI itemDescriptionUIText;
    private DialogManager dialogManager;
    public PagedRect pagedRect;
    public Dictionary<InventoryItem, Page> itemPageMap;
    public Dictionary<Page, InventoryItem> pageItemMap;
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
        pageItemMap = new Dictionary<Page, InventoryItem>();
        dialogManager = FindAnyObjectByType<DialogManager>();
        itemsData = new List<InventoryItem>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.deltaTime !=0) {
            if (IsOpen())
            {
                CloseInventory();
            }
            else 
            {
                OpenInventory();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    public void PageChanged()
    {
        Debug.Log("Called from Paginator!");
        PlaySFX(scrollSFX);
        if (IsOpen())
        {
            ChangeInventoryPage();
        }
    }

    internal void CheckInventory(InteractableObject interactableObject)
    {
        if (itemsData.Count > 0)
        {
            OpenInventory();
            StartCoroutine(CheckItemSelection(interactableObject));
        }
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
            InventoryItem currentItem = pageItemMap[pagedRect.GetCurrentPage()];
            Debug.Log(currentItem.itemName + " -> " + interactableObject.itemRequired);
            // Mouse click detected, perform your logic
            if (currentItem.itemName != interactableObject.itemRequired)
            {
                dialogManager.Hide();
                DialogData dialogData = new DialogData(wrongItemDialog, "Player", null, true);
                PlaySFX(wrongItemSFX);
                dialogManager.Show(dialogData);
                yield return null;
            }
            else
            {
                RemoveItem(currentItem);
                PlaySFX(correctItemSFX);
                interactableObject.ItemsFound();
                // Wait for 1 second
                yield return new WaitForSeconds(0.01f);
                Debug.Log("Done Invoking");
                CloseInventory();
                // Set the flag to exit the loop
                itemMatched = true;
            }
        }
    }

    private void RemoveItem(InventoryItem currentItem)
    {
        Debug.Log("Removing");
        GameObject gameObject = currentItem.item;
        itemsData.Remove(currentItem);
        pageItemMap.Remove(itemPageMap[currentItem]);
        itemPageMap.Remove(currentItem);
        Debug.Log(pagedRect.Pages.Count);
        pagedRect.RemoveCurrentPage();
        Debug.Log(pagedRect.Pages.Count);
        Destroy(gameObject);
    }

    public void OpenInventory()
    {
        if (itemsData.Count == 0)
        {
            CloseInventory();
            return;
        }
        foreach (InventoryItem item in itemsData)
        {
            if (!itemPageMap.ContainsKey(item))
            {
                Page newPage = AddPage(item);
                itemPageMap.Add(item, newPage);
                pageItemMap.Add(newPage, item);
            }
        }
        inventoryUI.SetActive(true);
        inventoryUIBlur.SetActive(true);
        Behaviour behaviour = this.GetComponent<FirstPersonMovement>();
        behaviour.enabled = false;
        Debug.Log("Called from Open Inventory!");
        ChangeInventoryPage();
    }

    public void ChangeInventoryPage()
    {
        InventoryItem currentItem = pageItemMap[pagedRect.GetCurrentPage()];
        itemNameUIText.enabled = true;
        itemDescriptionUIText.enabled = true;
        itemNameUIText.text = currentItem.itemName;
        itemDescriptionUIText.text = currentItem.itemDescription;
    }

    public void CloseInventory()
    {
        StopAllCoroutines();
        inventoryUI.SetActive(false);
        inventoryUIBlur.SetActive(false);
        Behaviour behaviour = this.GetComponent<FirstPersonMovement>();
        behaviour.enabled = true;
    }

    internal bool IsOpen()
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
}
