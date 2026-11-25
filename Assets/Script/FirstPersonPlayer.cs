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
        Look(); //tourner la tête avec la souris
        Move(); //se déplacer avec le clavier

        // Touche F: allume / éteint la lampe.
       // GetKeyDown = vrai UNE seul fois, à l'instant où on appuie sur la touche.
      // "!" inverse la valeur : allumé devient éteint, et inversement.
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
        // Touche Echap : libère la souris (utile pour quitter le jeu 
       // dans l'éditeur Unity). A retirer dans le jeu final si besoin.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //---REGARDER AVEC LA SOURIS---
    void Look()
    {
        // De combien la souris a bougé depuis la dernière image.
       // Pas besoin de multiplier par Time.delatTime ici :
      // Unity le gère déjà pour la souris.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Gauche/droite : on fait tourner TOUT le corps du joueur.
       // C'est ce qui change la direction dans laquelle on avance.
        transform.Rotate(0f, mouseX, 0f); // tourner le corps du joueur à gauche/droite

        // Haut/bas : on fait tourner SEULEMENT la caméra (la tête).
       // Le "-=" est là pour le bouger la souris vers le haut
      // fasse regarder vers le haut.
        verticalRotation -= mouseY;

        // Clamp = on bloque l'ange entre -80° et +80°.
       // Sans ça, on pourrait faire un tour complet de la tête.
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        // On applique la rotation verticale à la caméra.
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void Move()
    {
        // Horizontal = touche Q/D (ou A/D ou flèches gauche/droite)
       // Vertical = touche Z/S (ou W/S ou flèches haut/bas)
      // Chaque valeur va d -1 à 1.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Si on maintient Shift on court, sinon on marche.
       // Syntaxe : condition ? valeur_si_vrai : valeur_si_faux
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // La direction du déplacement, par rapport à où regarde le joueur.
       // transform.right = droite du joueur, transform.forward = devant lui.
        Vector3 move = transform.right * x + transform.forward * z;


        //Quand on va en diagonale, la vitesse serait plus grande.
       // ClampMagnitude(..., 1) l'empêche de dépasser 1, puis on
      // multiplie par la vitesse voulue.
        move = Vector3.ClampMagnitude(move, 1f) * speed;



        //--- GRAVITE ---
       // Si on touche le sol, on remet la vitesse de chute presque à zéro 
      // (-2 plutot que 0 bien "coller" au sol).
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        // Sinon la vitesse de chute augmente petit à petit.
       // Time.deltaTime = temps écoulé depuis la dernière image,
      // pour que ça marche pareil quelle que soit la puissance du PC.
        verticalVelocity += gravity * Time.deltaTime;

        // On ajoute la chute au déplacement (axe Y = vertical). 
        move.y = verticalVelocity;

        // On déplace enfin le joueur. Character Controller bloque
       // tout seul contre les murs.
        controller.Move(move * Time.deltaTime);
    }
}

