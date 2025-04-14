using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera rotation
/// Mouse left and right movement controls body rotation
/// Mouse up and down movement controls camera up and down rotation
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Tooltip("Field of view sensitivity")] private float mouseSenstivity;//Mouse sensitivity
    private Transform playerBody;//Player position
    private float yRotation = 0f;//Camera up and down rotation value

    private CharacterController characterController;
    [Tooltip("Initial height of the current camera")] public float height = 1.8f;
    private float interpolationSpeed = 12f;//Smooth value of height change

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//Lock the cursor in the center
        playerBody = transform.GetComponentInParent<PlayerController>().transform;//Get the PlayerController component in the sub-object, the current position of the player
        characterController = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseSenstivity = PlayerPrefs.GetFloat("mouseSenstivity", 400f);//Update mouse sensitivity

        //*Time.deltaTime can change the change per frame to change per second
        float mouseX = Input.GetAxis("Mouse X") * mouseSenstivity * Time.deltaTime;//Get the change of mouse axis value
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenstivity * Time.deltaTime;

        yRotation -= mouseY;//Accumulate the axis value of the mouse up and down rotation and assign it to the camera
        yRotation = Mathf.Clamp(yRotation, -60f, 60f);//Limit the size of the camera's up and down angles
        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f); //Set the camera up and down rotation
        playerBody.Rotate(Vector3.up * mouseX);//Mouse left and right movement makes the player body rotate left and right

        /* When the character crouches or stands, with the change of height, the height of the camera should also change */
        float heightTarget = characterController.height * 0.9f;
        height = Mathf.Lerp(height, heightTarget, interpolationSpeed * Time.deltaTime);//Smooth change
        transform.localPosition = Vector3.up * height;//Set the camera height when crouching and standing


    }
}
