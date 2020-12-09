using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using ZenTarget;

public class VRController : MonoBehaviour
{ 
    //initialisation
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

    //Objectifs
    private bool touchingWater = false;
    private bool touchingtarget = false;
    private bool touchingPlatform = false;
    private bool wasOnPlatform = false;

    public GameObject Fireworks;
    public GameObject MyPlayer;
    public GameObject EasterEggSound;
    public GameObject ParachuteDeploySound;
    public GameObject WindSound;
    public Target myTarget;
    public GameObject TargetList;
    public GameObject platforme;
    public GameObject parachutePrefab;
    public GameObject playerCamera;

    /// <summary>
    /// Quand le joueur touche un de ces éléments, je les gardes en mémoire
    /// </summary>
    /// <param name="other">Collider de l'objet</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "water")
        {
            touchingWater = true;
        }
        if (other.tag == "target")
        {
            touchingtarget = true;
            Fireworks.SetActive(true);
        }
        if (other.tag == "startPlatform")
        {
            touchingPlatform = true;
        }
    }

    /// <summary>
    /// Quand le joueur quitte un de ces éléments, je les gardes en mémoire
    /// </summary>
    /// <param name="other">Collider de l'objet</param>
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "water")
        {
            touchingWater = false;
        }
        if (other.tag == "target")
        {
            touchingtarget = false;
            Fireworks.SetActive(false);
        }
        if (other.tag == "startPlatform")
        {
            touchingPlatform = false;

            if (Random.Range(0,50) == 1 && wasOnPlatform != touchingPlatform)
            {
                EasterEggSound.SetActive(true);
            }
        }
    }




    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// code éxécuté au démarrage du programme
    /// </summary>
    private void Start()
    {
        //on donne le corps au joueur
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    /// <summary>
    /// code éxécuté à chaque image
    /// </summary>
    private void Update()
    {
        //on regarde si le joueur a quitter la platforme
        wasOnPlatform = touchingPlatform;

        //si oui, on joue le son du vent
        if (touchingPlatform == false && touchingtarget == false && touchingWater == false)
        {
            WindSound.SetActive(true);
        }
        else
        {
            WindSound.SetActive(false);
        }
        
        //récuperation de la rotation de la caméra
        headDegree = playerCamera.transform.rotation.eulerAngles.y;

        //on met le parachute directement sur la caméra
        parachutePrefab.transform.position = playerCamera.transform.position;

        //on tourne le parachute dans la direction de la camera
        parachutePrefab.transform.rotation = Quaternion.Euler(new Vector3(0,headDegree,0));

        //on met le parachute directement sur la caméra
        Fireworks.transform.position = playerCamera.transform.position;

        //on tourne le parachute dans la direction de la camera
        Fireworks.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        //on calcule la hauteur de la tête du joueur
        HandleHeight();
        //on calcule le mouvement du joueur
        CalculateMovement();
        //on tourne le joueur de 90°
        SnapRotation();

        //si le parachute est déployer
        if (parachute)
        {
            //on affiche le parachute et on joue le son du parachute
            parachutePrefab.SetActive(true);
            ParachuteDeploySound.SetActive(true);
        }
        else
        {
            //sinon on cache le parachute et on désactive le son du parachute
            parachutePrefab.SetActive(false);
            ParachuteDeploySound.SetActive(false);
        }


        //si le joueur touche l'eau
        if (touchingWater)
        {
            //on supprime le parachute et on remet le joueur sur la platforme de départ
            parachute = false;
            MyPlayer.transform.position = platforme.transform.position;

            MyPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            MyPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            //on recréer une nouvelle platforme
            myTarget.ClearTarget();
            myTarget.NewTarget();
        }
        //si le joueur touche la cible
        if (touchingtarget)
        {
            //on coupe le parachute
            parachute = false;
            //on vérifie si le joueur avais son parachute quand il a toucher la cible
            if (transform.position.y < -15)
            {
                //on renvoie le joueur sur la platforme si il avais pas son parachute ouvert
                Debug.Log(transform.position.y);
                MyPlayer.transform.position = platforme.transform.position;


            }
            else
            {
                //le joueur gagne
                Debug.Log("Win");
            }
        }
        if (touchingPlatform)
        {
            //on coupe le parachute si le joueur est sur la platforme
            parachute = false;
        }

        

    }

    /// <summary>
    /// calcule de la hauteur de le tête du joueur
    /// </summary>
    private void HandleHeight()
    {
        //on ajuste la caméra a la hauteur de la tête du joueur
        float headHeigh = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeigh;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y = m_CharacterController.skinWidth;

        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        //on bouge la "HitBox" du joueur au centre de la caméra
        m_CharacterController.center = newCenter;
    }

    /// <summary>
    /// calcule du mouvement du joueur
    /// </summary>
    private void CalculateMovement()
    {
        //initialisation
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        //on regarde les entrées utilisateur pour l'ouverture du parachute
        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.Any) && parachute == false)
        {
            m_Speed = 0;
        }
        if (m_ChutePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            parachute = true;
        }

        //mouvement du joueur sans le parachute
        if (m_MovePress.state && parachute == false)
        {
            //le joueur peut avancer en avant et en arrière avec une vitesse relative au mouvement du joystick de la manette
            m_Speed += m_MoveValue.axis.y * m_Sensivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            movement += orientation * (m_Speed * Vector3.forward);
        }
        else if (parachute == true)//mouvement du joueur avec le parachute
        {
            //le joueur ne peut que avancer devant avec une vitesse de 10 (horizontal)
            m_Speed += 10;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            movement += orientation * (m_Speed * Vector3.forward);
        }

        //Gravité
        if (parachute)
        {//sous le parachute, le joueur a une vitesse verticale de -10
            m_MaxSpeed = 10;
            movement.y -= m_speedWithParachute;
        }
        else
        {//sinon, le joueur a une gravité de 9.81m/s et une vitesse horizontale maximum de 2
            m_MaxSpeed = 2;
            movement.y -= m_Gravity * Time.deltaTime;
        }

        //on fait bouger le joueur
        m_CharacterController.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// tourner le joueur en fonction de l'appuis des boutons sur les bords des manettes
    /// </summary>
    private void SnapRotation()
    {
        //initialisation
        float snapValue = 0.0f;

        //quand le joueur appuis sur le bouton, on tourne le joueur de -90° (m_RotateIncrement)
        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = -Mathf.Abs(m_RotateIncrement);
        }
        //quand le joueur appuis sur le bouton, on tourne le joueur de 90° (m_RotateIncrement)
        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(m_RotateIncrement);
        }
        //on applique la rotation au joueur
        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
}
