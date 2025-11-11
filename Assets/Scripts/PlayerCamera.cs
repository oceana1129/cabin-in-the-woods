using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Settings")]
    public float mouseSensitivity = 200f;

    float xRotation = 0f;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // vertical rotation on camera
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // horizontal rotation on the player object
        transform.Rotate(Vector3.up * mouseX);
    }
}
