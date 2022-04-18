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

    private AudioSource audioSource;
    public AudioClip thrustSound;

    public float boost = 15;
    public float maxSpeed = 40;



    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }


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


        audioSource.PlayOneShot(thrustSound);

        rigidbody.AddForce(handDirection * boost, ForceMode.Impulse);

        if (rigidbody.velocity.magnitude > maxSpeed)
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);

    }










}
