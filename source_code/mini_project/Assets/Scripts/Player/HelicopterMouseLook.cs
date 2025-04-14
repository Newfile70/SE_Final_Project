using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterMouseLook : MonoBehaviour
{
    [Tooltip("Field of view sensitivity")] private float mouseSenstivity;//Mouse sensitivity
    private Transform playerBody;//Player position
    public GameObject body;
    private float yRotation = 0f;//Camera up and down rotation value
    private float xRotation = 0f;//Camera left and right rotation value


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//Lock the cursor in the center
        playerBody = transform.GetComponentInParent<HummerMove>().transform;//Get the PlayerController component in the sub-object, the current position of the player

    }

    // Update is called once per frame
    void Update()
    {

        mouseSenstivity = PlayerPrefs.GetFloat("mouseSenstivity", 400f);//Update mouse sensitivity

        //*Time.deltaTime can change the change per frame to change per second
        float mouseX = Input.GetAxis("Mouse X") * mouseSenstivity * Time.deltaTime;//Get the change of mouse axis value
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenstivity * Time.deltaTime;

        yRotation -= mouseY;//Accumulate the axis value of the mouse up and down rotation and assign it to the camera
        xRotation += mouseX;//Accumulate the axis value of the mouse left and right rotation and assign it to the camera
        xRotation = Mathf.Clamp(xRotation, -78f, 80f);//Limit the size of the camera's left and right angles
        yRotation = Mathf.Clamp(yRotation, -30f, 65f);//Limit the size of the camera's up and down angles
        transform.localRotation = Quaternion.Euler(0f, 0f, xRotation); //Set the camera up and down rotation
        body.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f); //Set the camera up and down rotation
        //body.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f); //Set the camera up and down rotation
        //playerBody.Rotate(Vector3.up * mouseX);//Mouse left and right movement makes the player body rotate left and right
        //xRotation += mouseX;//Accumulate the axis value of the mouse up and down rotation and assign it to the camera
        //xRotation = Mathf.Clamp(xRotation, -18f, 60f);//Limit the size of the camera's up and down angles
        //transform.localRotation = Quaternion.Euler(0f, xRotation, 0f); //Set the camera up and down rotation



    }
}
