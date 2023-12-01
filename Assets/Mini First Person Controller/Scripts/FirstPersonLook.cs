using Google.MaterialDesign.Icons;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public struct InteractionActionUI
{
    public InteractionAction action;
    public Sprite icon;
    public string materialIcon;
}

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    [Range(1, 10)]
    private float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;
    Quaternion initialCharacterRotation;

    [Header("Interaction")]
    public MaterialIcon playerCrosshair;
    public string interactableCrosshair = "F230";
    public string normalCrosshair = "E3C2";
    public Color normalColor = Color.white;
    public Color interactableColor = Color.grey;
    public GameObject playerInteractionUI;
    public TextMeshProUGUI interactionText;
    public MaterialIcon interactionIcon;
    public int interactionLayer;
    public List<InteractionActionUI> actionUI;
    public GameObject inventoryPrompt;

    public float Sensitivity { get => sensitivity; set => sensitivity = value; }

    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Store the initial character rotation.
        initialCharacterRotation = character.localRotation;
    }

    void FixedUpdate()
    {
        #region Camera Movement
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * Sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = initialCharacterRotation * Quaternion.AngleAxis(velocity.x, Vector3.up);
        #endregion

        if (Inventory.Instance.itemsData.Count > 0)
        {
            inventoryPrompt.SetActive(!Inventory.Instance.inventoryUI.activeSelf);
        }
    }

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

    public void OnDisable()
    {
        playerCrosshair.enabled = false;
    }

    private void InteractionRayCast()
    {
        if (Inventory.Instance.Open())
        {
            playerCrosshair.enabled = false;
            playerInteractionUI.SetActive(false);
            return;
        }

        int layerMask = 1 << interactionLayer;
        RaycastHit hit;
        Camera camera = GetComponent<Camera>();
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, camera.transform.forward * hit.distance, Color.green);
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
            
            if (hit.distance < 2 && interactableObject!=null)
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
                if (Input.GetKeyDown(KeyCode.E)) {
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
