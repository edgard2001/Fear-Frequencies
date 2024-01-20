using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float sphereRadius = 0.1f;
    private Vector3 sphereCheckOffset = new Vector3(0, 0, 0);
    
    private CharacterController controller;

    [SerializeField] private float _baseSpeed = 5f;
    
    private float _totalSpeedMultiplier = 1f;
    [SerializeField] private float _sprintSpeedMultiplier = 2f;
    
    private float _gravityAcceleration = -30f;
    private Vector3 _currentVelocity = Vector3.zero;
    private bool _jumping;

    private float _cameraAngleX;
    private Transform _camera;

    private bool _inputInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _camera = Camera.main.transform;
 
        StartCoroutine(InitializeInputAfterDelay(0.5f)); //To stop player faceing ground in Game scene
    }

    // Update is called once per frame
    void Update()
    {
        if (!_inputInitialized) return;
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _totalSpeedMultiplier *= _sprintSpeedMultiplier;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _totalSpeedMultiplier /= _sprintSpeedMultiplier;
        
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 2, 0);

        _cameraAngleX += Input.GetAxis("Mouse Y") * -2;
        _cameraAngleX = Mathf.Clamp(_cameraAngleX, -90, 90);
        _camera.localRotation = Quaternion.Euler(_cameraAngleX, 0, 0);
    }


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
            controller.Move(_baseSpeed * _totalSpeedMultiplier * Time.fixedDeltaTime * transform.forward );
        if (Input.GetKey(KeyCode.S))
            controller.Move(_baseSpeed * _totalSpeedMultiplier * Time.fixedDeltaTime * -transform.forward);
        if (Input.GetKey(KeyCode.D))
            controller.Move( _baseSpeed * _totalSpeedMultiplier * Time.fixedDeltaTime * transform.right);
        if (Input.GetKey(KeyCode.A))
            controller.Move( _baseSpeed * _totalSpeedMultiplier * Time.fixedDeltaTime * -transform.right);
        
        if (Physics.CheckSphere(transform.position + sphereCheckOffset, sphereRadius))
        {
            // Add a small force to make sure we stay on the ground
            _currentVelocity.y = -1f;
            if (Input.GetKey(KeyCode.Space))
            {
                _currentVelocity.y = 10f;
            }
        }
        else
        {
            // Accumulate gravity force over time
            _currentVelocity.y += _gravityAcceleration * Time.fixedDeltaTime;
        }

        controller.Move(_currentVelocity * Time.fixedDeltaTime);
    }

    IEnumerator InitializeInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _inputInitialized = true;
    }
}
