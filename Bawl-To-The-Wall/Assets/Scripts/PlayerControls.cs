using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControls : MonoBehaviour
{
    public InputActionReference GripReference = null;
    public InputActionReference ThrustReference = null;

    public GameObject player;
    public GameObject LeftController;
    public GameObject RightController;




    public float maxSpeed = 10;

    private void Awake()
    {

        GripReference.action.started += Grip;
        ThrustReference.action.started += Thrust;
    }

    private void OnDestroy()
    {
        GripReference.action.started -= Grip;
        ThrustReference.action.started -= Thrust;
    }









    private void Grip(InputAction.CallbackContext context)
    {
        Debug.Log("Grip pressed on " + gameObject.name);

        Debug.Log(context.control.device.name + " Grip");

        if (context.control.device.name == "OculusTouchControllerLeft")
        {
            Debug.Log("Left Grip");
        }
        else if (context.control.device.name == "OculusTouchControllerRight")
        {
            Debug.Log("Right Grip");
        }
    }


    private void Thrust(InputAction.CallbackContext context)
    {
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();

        Debug.Log(context.control.device.name + " Thrust");

        Vector3 handDirection = Vector3.zero;

        if (context.control.device.name == "OculusTouchControllerLeft")
        {
            Debug.Log("Left Thrust");
            handDirection = LeftController.transform.forward;
        }
        else if (context.control.device.name == "OculusTouchControllerRight")
        {
            Debug.Log("Right Thrust");
            handDirection = RightController.transform.forward;
        }

        float velocity = rigidbody.velocity.magnitude;
        rigidbody.AddForce(handDirection * maxSpeed, ForceMode.Impulse);
    }










}
