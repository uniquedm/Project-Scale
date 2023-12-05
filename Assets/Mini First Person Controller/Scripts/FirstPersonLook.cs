using Google.MaterialDesign.Icons;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
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
    }
}
