using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.XR;
// Les commentaires seront utilisé pour expliquer le code, pour comprendre leur utilité et leur objectif dans le code
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour
{
    // --- VARIABLE REGLABLAS DANS L'INSPECTEUR ---
    // Quand c'est marqué "public" c'est que c'est visible dans Unity, il est possible de modifier la valeur dans l'inspecteur.

    public Transform cameraTransform; //la caméra du joueur (sa tête)
    public Light flashlight; //la lampe torche (peut rester vide)

    public float walkSpeed = 3.0f; // la vitesse de marche en m/s
    public float runSpeed = 5.0f; // vitesse de course (input shift)
    public float mouseSensitivity = 2.0f; // sensibilité de la souris
    public float gravity = -9.81f; // la gravité terrestre

    // --- VARIABLES INTERNES ---7
    // Sans "public" : elles ne servent qu'à l'intérieur du script.

    CharacterController controller; // réf au Character Controller
    float verticalRotation = 0f; // angle haut/bas de la caméra
    float verticalVelocity = 0f; // vitesse de chute actuelle


    // Start() est appelée UNE fois, au lancement du jeu.
    void Start()
    {
        // On récupère le Character Controller qui est sur le même objet.
        controller = GetComponent<CharacterController>();

        // On cache le curseur et on le bloque au centre de l'écran,
        // comme dans tous les jeux à la première personne.

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update est appelée à CHAQUE image (60 fois par seconde ou plus).
    void Update()
    {
      Look() //tourner la tête avec la souris
      Move() //se déplacer avec le clavier

        // Touche F: allume / éteint la lampe.
        // GetKeyDown = vrai UNE seul fois, à l'instant où on appuie sur la touche.
       // "!" inverse la valeur : allumé devient éteint, et inversement.

    if (Input.GetKeyDown(KeyCode.f) && flashlight != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
