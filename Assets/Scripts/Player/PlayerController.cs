using UnityEngine;

/// <summary>
/// Contrôleur principal du joueur : déplacement horizontal et tir
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Vitesse de déplacement horizontal (en unités par seconde)
    [SerializeField] private float vitesseDeplacement = 8f;

    // Délai minimum entre deux tirs (en secondes)
    [SerializeField] private float cooldownTir = 0.3f;

    // Point d'apparition des projectiles (enfant du joueur, au-dessus du sprite)
    [SerializeField] private Transform pointDeTir;

    // Prefab du projectile joueur
    [SerializeField] private GameObject playerBulletPrefab;

    // Marge par rapport aux bords de l'écran (en unités monde)
    private const float MargeEcran = 0.5f;

    // Référence au Rigidbody2D (mise en cache dans Awake)
    private Rigidbody2D rb;

    // Horodatage du dernier tir (pour le cooldown)
    private float tempsDernierTir;

    // Position de départ sauvegardée pour la réinitialiser à chaque partie
    private Vector2 positionDepart;

    // Limites de déplacement horizontal calculées depuis la caméra
    private float limiteGauche;
    private float limiteDroite;

    /// <summary>
    /// Met en cache les composants et sauvegarde la position de départ
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        positionDepart = transform.position;
    }

    /// <summary>
    /// Réinitialise la position du joueur à chaque activation (nouvelle partie ou rejeu)
    /// </summary>
    private void OnEnable()
    {
        if (rb != null)
            rb.MovePosition(positionDepart);

        // Réinitialise le cooldown pour éviter un tir immédiat
        tempsDernierTir = 0f;
    }

    /// <summary>
    /// Calcule les limites d'écran après que la caméra est initialisée
    /// </summary>
    private void Start()
    {
        CalculerLimitesEcran();
    }

    /// <summary>
    /// Convertit les bords de l'écran (viewport) en coordonnées monde
    /// </summary>
    private void CalculerLimitesEcran()
    {
        Camera cam = Camera.main;
        // Bord gauche et bord droit de la caméra en coordonnées monde
        limiteGauche = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + MargeEcran;
        limiteDroite = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - MargeEcran;
    }

    /// <summary>
    /// Détecte l'appui sur Espace pour déclencher un tir (avec cooldown)
    /// </summary>
    private void Update()
    {
        // Tir si Espace pressée et cooldown écoulé
        if (Input.GetKey(KeyCode.Space) && Time.time >= tempsDernierTir + cooldownTir)
        {
            Tirer();
        }
    }

    /// <summary>
    /// Gère le déplacement horizontal et clamp dans les limites d'écran
    /// </summary>
    private void FixedUpdate()
    {
        float directionInput = 0f;

        // Touche gauche : A (position physique AZERTY = Q sur QWERTY) ou flèche gauche
        // L'ancien Input Manager Unity mappe les KeyCode sur la position physique QWERTY,
        // donc KeyCode.A correspond à la touche "Q" d'un clavier AZERTY sous Windows.
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            directionInput = -1f;

        // Touche droite : D (AZERTY) ou flèche droite
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            directionInput = 1f;

        // Calcule et clamp la nouvelle position horizontale
        float nouvelleX = Mathf.Clamp(
            rb.position.x + directionInput * vitesseDeplacement * Time.fixedDeltaTime,
            limiteGauche,
            limiteDroite
        );

        rb.MovePosition(new Vector2(nouvelleX, rb.position.y));
    }

    /// <summary>
    /// Instancie un projectile au point de tir et recharge le cooldown
    /// </summary>
    private void Tirer()
    {
        if (playerBulletPrefab == null || pointDeTir == null) return;

        Instantiate(playerBulletPrefab, pointDeTir.position, Quaternion.identity);
        tempsDernierTir = Time.time;
    }

    /// <summary>
    /// Déclenche le game over si le joueur est touché par un ennemi ou un projectile ennemi
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
