using UnityEngine;
using UnityEngine.InputSystem;
// Les commentaires seront utilisé pour expliquer le code, pour comprendre leur utilité et leur objectif dans le code
[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour
{
    // --- VARIABLE REGLABLAS DANS L'INSPECTEUR ---
   // Quand c'est marqué "public" c'est que c'est visible dans Unity, il est possible de modifier la valeur dans l'inspecteur.

    public Transform cameraTransform; //la caméra du joueur (sa tête)
    public Light flashlight; //la lampe torche (peut rester vide)

    public float walkSpeed = 3.0f; // la vitesse de marche en m/s
    public float runSpeed = 5f; // vitesse de course (input shift)
    public float mouseSensitivity = 0.1f; // sensibilité de la souris
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
        // Keyboard.current / Mouse.current = le clavier et la souris branchés.
        // Ils valent "null" s'il n'y en a pas : dans ce cas on ne fait rien.
        if (Keyboard.current == null || Mouse.current == null) return;

        Look(); //tourner la tête avec la souris
        Move(); //se déplacer avec le clavier

        // Touche F: allume / éteint la lampe.
       // GetKeyDown = vrai UNE seul fois, à l'instant où on appuie sur la touche.
      // "!" inverse la valeur : allumé devient éteint, et inversement.
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
            Debug.Log("F pressé, lampe allumée" + flashlight.enabled);
        }
        // Touche Echap : libère la souris (utile pour quitter le jeu 
       // dans l'éditeur Unity). A retirer dans le jeu final si besoin.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //---REGARDER AVEC LA SOURIS---
    void Look()
    {
        // delta = de combien la souris a bougé depuis la dernière image.
        // (x = gauche/droite, y = haut/bas)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Gauche/droite : on tourne tout le corps du joueur.
        transform.Rotate(0f, mouseDelta.x, 0f);

        // Haut/bas : on tourne seulement la caméra, limitée à -80°/+80°.
        verticalRotation -= mouseDelta.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void Move()
    {
        Keyboard kb = Keyboard.current;

        // x = gauche/droite, z = avant/arrière. Chaque valeur va de -1 à 1.
        // Les touches sont repérées par leur POSITION physique : wKey est
        // la touche Z sur un clavier AZERTY, aKey est la touche Q, etc.
        float x = 0f;
        float z = 0f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) z += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) z -= 1f;

        // Shift maintenu = on court, sinon on marche.
        float speed = kb.leftShiftKey.isPressed ? runSpeed : walkSpeed;

        // Direction par rapport à où regarde le joueur, vitesse limitée
        // en diagonale puis multipliée par la vitesse voulue.
        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f) * speed;

        // --- GRAVITÉ ---
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; // on "colle" au sol
        }
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}

