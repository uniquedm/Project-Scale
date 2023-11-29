using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public int interactionLayer;
    public List<string> actions;

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

        #region Interaction Raycaster
        InteractionRayCast();
        #endregion
    }

    private void InteractionRayCast()
    {
        int layerMask = 1 << interactionLayer;
        RaycastHit hit;
        Camera camera = GetComponent<Camera>();
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, camera.transform.forward * hit.distance, Color.green);
            if (hit.distance < 2 && actions.Contains(hit.collider.tag))
            {
                interactionUI.SetActive(true);
                interactionText.text = hit.collider.tag;
                if (Input.GetKeyDown(KeyCode.E)) {
                    InteractableObject interactableObject = hit.collider.GetComponent<InteractableObject>();
                    if (interactableObject != null) {
                        interactableObject.Interaction();
                    }
                }
            }
            else
            {
                interactionUI.SetActive(false);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, camera.transform.forward * hit.distance, Color.red);
            interactionUI.SetActive(false);
        }
    }
}
