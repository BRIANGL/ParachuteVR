using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using ZenTarget;

public class VRController : MonoBehaviour
{
    public float m_Gravity = 1962.0f;
    public float m_Sensivity = 0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_RotateIncrement = 45;
    public float m_speedWithParachute = 5.0f;

    public bool parachute = false;
    public SteamVR_Action_Boolean m_RotatePress = null;
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Boolean m_ChutePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;
    private float headDegree = 0.0f;

    //GameObjectives
    private bool touchingWater = false;
    private bool touchingtarget = false;
    private bool touchingPlatform = false;
    public GameObject MyPlayer;
    public Target myTarget;
    public GameObject TargetList;
    public GameObject platforme;
    public GameObject parachutePrefab;
    public GameObject playerCamera;

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
        if (other.tag == "startPlatform")
        {
            touchingPlatform = true;
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
        if (other.tag == "startPlatform")
        {
            touchingPlatform = false;
        }
    }




    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    private void Update()
    {

        //récuperation de la rotation de la caméra
        headDegree = playerCamera.transform.rotation.eulerAngles.y;

        //on met le parachute directement sur la caméra
        parachutePrefab.transform.position = playerCamera.transform.position;

        //on tourne le parachute dans la direction de la camera
        parachutePrefab.transform.rotation = Quaternion.Euler(new Vector3(0,headDegree,0));


        HandleHeight();
        CalculateMovement();
        SnapRotation();

        if (parachute)
        {
            parachutePrefab.SetActive(true);
        }
        else
        {
            parachutePrefab.SetActive(false);
        }



        if (touchingWater)
        {
            parachute = false;
            MyPlayer.transform.position = platforme.transform.position;

            MyPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            MyPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            myTarget.ClearTarget();
            myTarget.NewTarget();
        }
        if (touchingtarget)
        {
            parachute = false;
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
        if (touchingPlatform)
        {
            parachute = false;
        }


    }

    private void HandleHeight()
    {
        float headHeigh = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeigh;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y = m_CharacterController.skinWidth;

        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        m_CharacterController.center = newCenter;
    }

    private void CalculateMovement()
    {
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.Any) && parachute == false)
        {
            m_Speed = 0;
        }

        if (m_ChutePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            parachute = true;
        }

        if (m_MovePress.state && parachute == false)
        {
            m_Speed += m_MoveValue.axis.y * m_Sensivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            movement += orientation * (m_Speed * Vector3.forward);
        }
        else if (parachute == true)
        {
            m_Speed += 10;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            movement += orientation * (m_Speed * Vector3.forward);
        }

        //Gravity
        if (parachute)
        {
            m_MaxSpeed = 10;
            movement.y -= m_speedWithParachute;
        }
        else
        {
            m_MaxSpeed = 2;
            movement.y -= m_Gravity * Time.deltaTime;
        }


        m_CharacterController.Move(movement * Time.deltaTime);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = Mathf.Abs(m_RotateIncrement);
        }

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = -Mathf.Abs(m_RotateIncrement);
        }

        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
}
