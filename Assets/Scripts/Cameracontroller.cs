using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Cameracontroller : MonoBehaviour
{
    public Transform cameraTransform; //Pour pouvoir manipuler les positions de la caméra

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public float zoomSpeed;
    public float maxZoomDist;
    public float minZoomDist;
    
    public Vector3 newPosition;
    public Vector3 zoomAmount;
    public Vector3 newZoom;
    public Vector3 dragStartPos;
    public Vector3 dragCurrentPos;
    public Vector3 rotateStartPos;
    public Vector3 rotateCurrentPos;

    public Quaternion newRotation;
    

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position; // replacement
        newRotation = transform.rotation; // place la rotation
        newZoom = cameraTransform.localPosition; // place le zoom de début sur la bonne position
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
    }

    private Camera cam; // création d'une camera

    private void Awake()
    {
        cam = Camera.main; // attribut la première camera étiquetée "MainCamera" à cam
    }


    

    // Méthode de gestion de la souris
    void HandleMouseInput()
    {
        // le zoom avec le mouseScroll
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount * 8; // On multiplie par 8 car le zoomAmount clavier/souris n'a pas autant de puissance sur l'un que sur l'autre
        }

        // déplacement au clic gauche
        if (Input.GetMouseButtonDown(0)) // Retourne true pendant que l'utilisateur presse le bouton donné, ici 0 (clique gauche)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if(plane.Raycast(ray, out entry))
            {
                dragStartPos = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0)) // retourne true si le bouton donné est maintenu enfoncé
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero); // création d'un plan avec deux vecteurs 0,1,0 et 0,0,0
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Renvoie un rayon allant de la caméra à un point de l'écran, ici la position de la souris
            float entry;

            if (plane.Raycast(ray, out entry)) // vérifie si le rayon et le plan se croisent, si le rayon esst parallèle au plan, entry = 0 et on renvoie false
            {
                dragCurrentPos = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPos - dragCurrentPos;
            }
        }

        // rotation avec le clic du milieu
        if (Input.GetMouseButtonDown(2)) // meme fonctionnement qu'au dessus, le 2 correspond au clic du milieu
        {
            rotateStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPos = Input.mousePosition; // on récupère la position de la souris
            Vector3 difference = rotateStartPos - rotateCurrentPos;
            rotateStartPos = rotateCurrentPos; // actualisation
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f)); // la valeur est négative pour plus d'ergonomie en jeu
        }

    }

    // Méthode de gestion du clavier
    void HandleMovementInput()
    {
        // Sprint de camera
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        // ZQSD CONTROLLER
        if(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        // ROTATION CONTROLLER
        if (Input.GetKey(KeyCode.A))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        // ZOOM CONTROLLER
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }


        // SMOOTH MOVEMENT
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

}
