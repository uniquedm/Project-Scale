using Google.MaterialDesign.Icons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private RayCaster rayCaster;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnDisable()
    {
        playerCrosshair.enabled = false;
    }

    private void FixedUpdate()
    {
        if (Inventory.Instance.itemsData.Count > 0)
        {
            inventoryPrompt.SetActive(!Inventory.Instance.inventoryUI.activeSelf);
        }
    }

    // Update is called once per frame
    private void Update()
    {
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

    private void InteractionRayCast()
    {
        if (Inventory.Instance.Open())
        {
            playerCrosshair.enabled = false;
            playerInteractionUI.SetActive(false);
            return;
        }

        RaycastHit hit;
        Camera camera = GetComponent<Camera>();
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactionLayerMask))
        {
            Debug.DrawRay(transform.position, camera.transform.forward * hit.distance, Color.green);
            hit.collider.gameObject.layer = 12;
            InteractableObject[] interactableBehaviors = hit.collider.GetComponents<InteractableObject>();
            InteractableObject interactableObject = interactableBehaviors[0];
            foreach (InteractableObject behavior in interactableBehaviors)
            {
                if (behavior.isActiveAndEnabled)
                {
                    interactableObject = behavior;
                    break;
                }
            }

            if (hit.distance < 2 && interactableObject != null)
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
            if (hit.collider != null)
            {
                hit.collider.gameObject.layer = 10;
            }
            Debug.DrawRay(transform.position, camera.transform.forward * hit.distance, Color.red);
            playerCrosshair.enabled = true;
            playerInteractionUI.SetActive(false);
        }
    }

    private void ToggleInteractiveCrosshair(bool isActive)
    {
        playerCrosshair.iconUnicode = isActive ? interactableCrosshair : normalCrosshair;
        playerCrosshair.color = isActive ? interactableColor : normalColor;
    }
}
