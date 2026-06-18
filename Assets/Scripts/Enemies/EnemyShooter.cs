using UnityEngine;

/// <summary>
/// Tir aléatoire d'un ennemi — utilisé uniquement en niveau 2 sur les colonnes 0 et 1.
/// Ce composant est désactivé par défaut dans le prefab Enemy.
/// LevelController l'active au moment du spawn selon le niveau et la colonne.
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    // Prefab du projectile ennemi instancié lors d'un tir
    [SerializeField] private GameObject enemyBulletPrefab;

    // Couleur violette pour distinguer visuellement les ennemis tireurs
    [SerializeField] private Color couleurTireur = new Color(0.6f, 0.1f, 0.8f);

    // Intervalle entre deux tentatives de tir (en secondes)
    private const float IntervalleTir = 3f;

    // Probabilité de tirer à chaque tentative (0 = jamais, 1 = toujours)
    private const float ProbabiliteTir = 0.3f;

    // Référence au SpriteRenderer mis en cache dans Awake
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Met en cache le SpriteRenderer du GameObject
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Colorise l'ennemi en violet et lance la boucle de tir dès que le composant est activé
    /// </summary>
    private void OnEnable()
    {
        // Applique la couleur tireur avant le premier tir pour une lisibilité immédiate
        if (spriteRenderer != null)
            spriteRenderer.color = couleurTireur;

        InvokeRepeating(nameof(TenterTir), IntervalleTir, IntervalleTir);
    }

    /// <summary>
    /// Arrête la boucle de tir quand le composant est désactivé ou l'objet détruit
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke(nameof(TenterTir));
    }

    /// <summary>
    /// Tente de tirer selon la probabilité définie
    /// </summary>
    private void TenterTir()
    {
        // Tire uniquement si le tirage aléatoire dépasse le seuil
        if (Random.value <= ProbabiliteTir)
            Tirer();
    }

    /// <summary>
    /// Instancie un projectile ennemi à la position de cet ennemi
    /// </summary>
    private void Tirer()
    {
        if (enemyBulletPrefab == null) return;
        Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
    }
}
