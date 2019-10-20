using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float jumpSpeed = 5f;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D)) {
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.A)) {
            transform.position -= transform.right * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W)) {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.S)) {
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().velocity += Vector3.up * jumpSpeed;
        }

        // mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(/*rotX*/0f, rotY, 0.0f);
        transform.rotation = localRotation;
    }
}
