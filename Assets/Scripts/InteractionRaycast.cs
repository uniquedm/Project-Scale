using Google.MaterialDesign.Icons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InteractionRaycast : MonoBehaviour
{
    [Header("Interaction")]
    public MaterialIcon playerCrosshair;
    public string interactableCrosshair = "F230";
    public string normalCrosshair = "E3C2";
    public Color normalColor = Color.white;
    public Color interactableColor = Color.grey;
    public GameObject playerInteractionUI;
    public TextMeshProUGUI interactionText;
    public MaterialIcon interactionIcon;
    public LayerMask interactionLayerMask;
    public int interactionLayer;
    public int interactionOutlineLayer;
    public List<InteractionActionUI> actionUI;
    public GameObject inventoryPrompt;

    [Header("InteractionRayCaster")]
    private RayCaster interactionRayCaster;
    public float interactionMaxDistance = 2f;
    private Camera playerCamera;

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
        if (Inventory.Instance.itemsData.Count > 0)
        {
            inventoryPrompt.SetActive(!Inventory.Instance.inventoryUI.activeSelf);
        }

        #region New Interaction Raycaster
        InteractionRayCastTrigger();
        #endregion
        #region Interaction Raycaster
        InteractionRayCast();
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

    private void InteractionRayCast()
    {
        if (Inventory.Instance.Open())
        {
            playerCrosshair.enabled = false;
            playerInteractionUI.SetActive(false);
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, interactionMaxDistance, interactionLayerMask))
        {
            Debug.DrawRay(transform.position, playerCamera.transform.forward * hit.distance, Color.green);
            InteractableObject[] interactableBehaviors = hit.collider.GetComponents<InteractableObject>();
            InteractableObject interactableObject = null;
            if (interactableBehaviors != null && interactableBehaviors.Length > 0)
            {
                interactableObject = interactableBehaviors[0];
                foreach (InteractableObject behavior in interactableBehaviors)
                {
                    if (behavior.isActiveAndEnabled)
                    {
                        interactableObject = behavior;
                        break;
                    }
                }
            }
            if (interactableObject != null)
            {
                foreach (InteractionActionUI actionElement in actionUI)
                {
                    if (interactableObject.interactionAction != actionElement.action)
                    {
                        continue;
                    }
                    interactionIcon.iconUnicode = actionElement.materialIcon;
                    break;

                }
                playerCrosshair.enabled = false;
                playerInteractionUI.SetActive(true);
                interactionText.text = interactableObject.interactionAction.ToString();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactableObject.Interaction();
                }
            }
            else
            {
                ToggleInteractiveCrosshair(true);
                playerCrosshair.enabled = true;
                playerInteractionUI.SetActive(false);
            }
        }
        else
        {
            ToggleInteractiveCrosshair(false);
            Debug.DrawRay(transform.position, playerCamera.transform.forward * hit.distance, Color.red);
            playerCrosshair.enabled = true;
            playerInteractionUI.SetActive(false);
        }
    }

    private void ToggleInteractiveCrosshair(bool isActive)
    {
        playerCrosshair.iconUnicode = isActive ? interactableCrosshair : normalCrosshair;
        playerCrosshair.color = isActive ? interactableColor : normalColor;
    }

    private static void InteractionUI(Collider collider, Boolean isEnable)
    {
        InteractableObject[] interaction = collider.GetComponents<InteractableObject>();
        foreach (InteractableObject interactableObject in interaction)
        {
            if (interactableObject.enabled)
            {
                interactableObject.InteractionUI(isEnable);
            }
        }
    }
}
