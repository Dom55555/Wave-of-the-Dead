using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;

    public bool cursorVisible = false;

    public Transform cameraTransform;
    public Image aim;
    public Gamemanager game;
    public GameObject arms;

    float verticalRotation = 0f;
    float recoilToReach = 0f;
    float currentRecoil = 0f;
    float recoilReachSpeed = 50f;
    float recoilReturnSpeed = 6f;
    float recoilDecreaseSpeed = 3f;
    float currentArmsRotation = 0f;
    bool armsRotating = false;
    bool reachedRecoilRotation = true;
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
            MouseRotateCamera();
        }
        recoilToReach += Time.deltaTime * recoilDecreaseSpeed;
        recoilToReach = Mathf.Clamp(recoilToReach, -4, 0);
        if (!reachedRecoilRotation)
        {
            currentRecoil = Mathf.Lerp(currentRecoil, recoilToReach, Time.deltaTime * recoilReachSpeed);
            if (currentRecoil <= recoilToReach)
            {
                currentRecoil = recoilToReach;
                reachedRecoilRotation = true;
            }
        }
        if (reachedRecoilRotation)
        {
            currentRecoil = Mathf.Lerp(currentRecoil, 0, Time.deltaTime * recoilReturnSpeed);
        }
        if(armsRotating)
        {
            currentArmsRotation = Mathf.MoveTowards(currentArmsRotation,-2f,Time.deltaTime * 35);
            if (currentArmsRotation<=-2f)
            {
                armsRotating = false;
            }
        }
        else
        {
            currentArmsRotation = Mathf.MoveTowards(currentArmsRotation,0f, Time.deltaTime * 5);
        }
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation + currentRecoil, 0f, 0f);
        arms.transform.localRotation = Quaternion.Euler(currentArmsRotation,0f,0f);

    }

    void MovePlayer()
    {
        Vector2 velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
        rb.velocity = transform.rotation * new Vector3(velocity.x, rb.velocity.y, velocity.y);
    }

    void MouseRotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
    }
    public void ToggleCursor()
    {
        cursorVisible = !cursorVisible;
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        aim.gameObject.SetActive(!cursorVisible);
    }
    public void Recoil(float recoilPower)
    {
        reachedRecoilRotation = false;
        armsRotating = true;
        recoilToReach -= recoilPower;
    }
}