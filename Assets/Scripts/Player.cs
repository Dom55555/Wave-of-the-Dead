using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float mouseSensitivity = 2f;

    public Transform playerBody;
    public Transform cameraTransform;
    public Image aim;
    public Gamemanager game;

    float verticalRotation = 0f;
    public bool cursorVisible = false;

    Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        aim.gameObject.SetActive(true);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        if (!cursorVisible)
        {
            RotateCamera();
        }
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * 0.01f;
        rb.MovePosition(newPosition);
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        playerBody.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    public void ToggleCursor()
    {
        cursorVisible = !cursorVisible;
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        aim.gameObject.SetActive(!cursorVisible);
    }
}