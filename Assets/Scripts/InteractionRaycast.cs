using Google.MaterialDesign.Icons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

[System.Serializable]

public struct InteractionActionUI
{
    public InteractionAction action;
    public Sprite icon;
    public string materialIcon;
}

public class InteractionRaycast : MonoBehaviour
{
    [Header("Interaction")]
    public LayerMask interactionLayerMask;
    public int interactionLayer;
    public int interactionOutlineLayer;

    [Header("InteractionRayCaster")]
    private RayCaster interactionRayCaster;
    public float interactionMaxDistance = 2f;
    private Camera playerCamera;
    private InteractableObject currentInteractableObject;

    [Header("InteractionUI")]
    public List<InteractionActionUI> actionUI;
    public GameObject inventoryPrompt;
    public MaterialIcon playerCrosshair;
    public string interactableCrosshair = "F230";
    public string normalCrosshair = "E3C2";
    public Color normalColor = Color.white;
    public Color interactableColor = Color.grey;
    public GameObject playerInteractionUI;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI interactionItemNameText;
    public MaterialIcon interactionIcon;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        interactionRayCaster = new RayCaster();
        interactionRayCaster.OnRayEnter += InteractionOnEnter;
        interactionRayCaster.OnRayExit += InteractionOnExit;
        interactionRayCaster.RayCastLayerMask = interactionLayerMask;
        interactionRayCaster.RayLength = interactionMaxDistance;
        interactionRayCaster.StartTransform = playerCamera.transform;
        interactionRayCaster.Direction = playerCamera.transform.forward;
    }

    void InteractionOnEnter(Collider collider)
    {
        collider.gameObject.layer = interactionOutlineLayer;
        InteractionUI(collider, true);
    }

    void InteractionOnExit(Collider collider)
    {
        collider.gameObject.layer = interactionLayer;
        InteractionUI(collider, false);
    }

    public void OnDisable()
    {
        if (playerCrosshair != null)
        {
            playerCrosshair.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractableObject!=null)
        {
            currentInteractableObject.Interaction();
        }
    }

    private void FixedUpdate()
    {
        if (Inventory.Instance.itemsData.Count > 0)
        {
            inventoryPrompt.SetActive(!Inventory.Instance.inventoryUI.activeSelf);
        }
        #region New Interaction Raycaster
        if (Inventory.Instance.Open())
        {
            playerCrosshair.enabled = false;
            playerInteractionUI.SetActive(false);
            return;
        }
        else
        {
            InteractionRayCastTrigger();
        }
        #endregion

        if (Time.timeScale == 0 || TimeScaleDevice.Instance.timescaleHeld)
        {
            playerCrosshair.enabled = false;
            playerInteractionUI.SetActive(false);
            inventoryPrompt.SetActive(false);
        }
    }

    private void InteractionRayCastTrigger()
    {
        interactionRayCaster.StartTransform = playerCamera.transform;
        interactionRayCaster.Direction = playerCamera.transform.forward;
        interactionRayCaster.CastRay();
    }

    private void ToggleInteractiveCrosshair(bool isActive)
    {
        playerCrosshair.iconUnicode = isActive ? interactableCrosshair : normalCrosshair;
        playerCrosshair.color = isActive ? interactableColor : normalColor;
    }

    private void InteractionUI(Collider collider, Boolean isEnable)
    {
        if (!isEnable)
        {
            interactionItemNameText.text = "";
            ToggleInteractiveCrosshair(false);
            playerCrosshair.enabled = true;
            playerInteractionUI.SetActive(false);
            currentInteractableObject = null;
            return;
        }
        InteractableObject[] interaction = collider.GetComponents<InteractableObject>();
        InteractableObject activeInteractableObject = null;
        foreach (InteractableObject interactableObject in interaction)
        {
            if (!interactableObject.enabled)
            {
                continue;
            }
            activeInteractableObject = interactableObject;
            interactionItemNameText.text = interactableObject.itemName;
            break;
        }
        if (activeInteractableObject == null)
        {
            return;
        }
        currentInteractableObject = activeInteractableObject;
        foreach (InteractionActionUI actionElement in actionUI)
        {
            if (activeInteractableObject.interactionAction != actionElement.action)
            {
                continue;
            }
            interactionIcon.iconUnicode = actionElement.materialIcon;
            break;

        }
        playerCrosshair.enabled = false;
        playerInteractionUI.SetActive(true);
        interactionText.text = activeInteractableObject.interactionAction.ToString();
    }
}
