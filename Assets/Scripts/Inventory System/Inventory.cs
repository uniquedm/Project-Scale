using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    // Singleton instance
    private static Inventory _instance;
    public GameObject inventoryUI;
    public GameObject inventoryCamera;
    public Volume renderVolume;
    public VolumeProfile inventoryProfile;
    public VolumeProfile renderProfile;
    public GameObject itemPreviewLocation;
    public FirstPersonLook playerLookScript;
    public int focusedLayer;

    private GameObject currentItem;
    public float rotationSpeed = 10f;
    private int currentIndex = 0;
    public TextMeshProUGUI itemNameUIText;
    public TextMeshProUGUI itemDescriptionUIText;

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

    public List<GameObject> items;

    // Start is called before the first frame update
    void Start()
    {
        items = new List<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) {
            ToggleInventory(!inventoryUI.activeSelf);
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
        inventoryUI.SetActive(toggle);
        inventoryCamera.SetActive(toggle);
        Behaviour behaviour = this.GetComponent<FirstPersonMovement>();
        behaviour.enabled = !toggle;
        playerLookScript.enabled = !toggle;
        renderVolume.profile = toggle ? inventoryProfile : renderProfile;
        if (index < 0)
        {
            index = items.Count - 1;
        }
        else if (items.Count > 0)
        {
            index %= items.Count;
        }
        currentIndex = index;
        if (items.Count > 0)
        {
            itemNameUIText.enabled = toggle;
            itemDescriptionUIText.enabled = toggle;
            items[index].transform.parent = itemPreviewLocation.transform;
            items[index].transform.localPosition = Vector3.zero;
            items[index].tag = "Untagged";
            items[index].layer = focusedLayer;
            currentItem = items[index];
            InteractableObject interactableObject = items[index].GetComponent<InteractableObject>();
            itemNameUIText.text = interactableObject.itemName;
            itemDescriptionUIText.text = interactableObject.itemDescription;
            for (int i = 0; i < items.Count; i++) {
                if (i==index) {
                    items[i].SetActive(toggle);
                }
                else
                {
                    items[i].SetActive(false);
                }
            }
        }
    }
}
