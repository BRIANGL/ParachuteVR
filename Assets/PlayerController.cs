using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using ZenTarget;

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public float speed = 1;
    private bool touchingWater = false;
    private bool touchingtarget = false;
    public GameObject MyPlayer;
    public Target myTarget;
    public GameObject TargetList;
    public GameObject platforme;
    //public GameObject VrCamera;
    private CharacterController characterController;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "water")
        {
            touchingWater = true;
        }
        if (other.tag == "target")
        {
            touchingtarget = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "water")
        {
            touchingWater = false;
        }
        if (other.tag == "target")
        {
            touchingtarget = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        //characterController.Move(speed * Time.deltaTime* Vector3.ProjectOnPlane(direction, Vector3.up) - new Vector3(input.axis.x, 9.81f, input.axis.y) * Time.deltaTime);

        transform.position += speed * Time.deltaTime * new Vector3(input.axis.x, 0, input.axis.y);

        if (touchingWater)
        {
            MyPlayer.transform.position = platforme.transform.position;

            MyPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            MyPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            myTarget.ClearTarget();
            myTarget.NewTarget();

        }
        if (touchingtarget)
        {
            if (transform.position.y < -15)
            {
                Debug.Log(transform.position.y);
                MyPlayer.transform.position = platforme.transform.position;


            }
            else
            {
                Debug.Log("Win");
            }
        }

    }
}
