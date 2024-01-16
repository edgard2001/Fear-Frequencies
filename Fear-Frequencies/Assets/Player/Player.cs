using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float sphereRadius = 0.1f;
    private Vector3 sphereCheckOffset = new Vector3(0, -1, 0);
    
    private CharacterController controller;
    private float _gravityForce = -9.81f;
    private Vector3 currentVelocity = Vector3.zero;

    private float _cameraAngleX;
    private Transform _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 2, 0);

        _cameraAngleX += Input.GetAxis("Mouse Y") * -2;
        _cameraAngleX = Mathf.Clamp(_cameraAngleX, -90, 90);
        _camera.localRotation = Quaternion.Euler(_cameraAngleX, 0, 0);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
            controller.Move(4f * transform.forward * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.S))
            controller.Move(4f *  -transform.forward * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.D))
            controller.Move(4f * transform.right * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.A))
            controller.Move(4f *  -transform.right * Time.fixedDeltaTime);
        
        if (Physics.CheckSphere(transform.position + sphereCheckOffset, sphereRadius))
        {
            // Add a small force to make sure we stay on the ground
            currentVelocity.y = -0.1f;
            if (Input.GetKey(KeyCode.Space))
                currentVelocity.y = 4f;
        }
        else
        {
            // Accumulate gravity force over time
            currentVelocity.y += _gravityForce * Time.fixedDeltaTime;
        }

        controller.Move(currentVelocity * Time.fixedDeltaTime);
    }
}
