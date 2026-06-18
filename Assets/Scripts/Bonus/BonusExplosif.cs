using UnityEngine;

/// <summary>
/// Bonus explosif : reste à la position de l'ennemi remplacé et suit le groupe
/// d'ennemis (parent). Si le joueur le collecte, détruit tous les ennemis dans un rayon.
/// </summary>
public class BonusExplosif : MonoBehaviour
{
    // Rayon de l'explosion en unités Unity
    [SerializeField] private float rayonExplosion = 3f;

    // Prefab VFX instancié lors de l'explosion
    [SerializeField] private GameObject vfxExplosionPrefab;

    // Référence au contrôleur de niveau pour signaler les destructions ennemies
    private LevelController levelController;

    /// <summary>
    /// Récupère le LevelController actif dans la scène (appelé une seule fois au spawn)
    /// </summary>
    private void Start()
    {
        levelController = FindAnyObjectByType<LevelController>();
    }

    /// <summary>
    /// Déclenche l'explosion si le joueur ou un tir joueur touche le bonus
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tir joueur : détruit la balle et déclenche l'explosion
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(other.gameObject);
            Exploser();
            return;
        }

        // Contact direct du joueur : déclenche aussi l'explosion
        if (other.CompareTag("Player"))
            Exploser();
    }

    /// <summary>
    /// Instancie le VFX, détruit les ennemis dans le rayon, puis se détruit
    /// </summary>
    private void Exploser()
    {
        // Instancie l'effet visuel d'explosion à la position du bonus
        if (vfxExplosionPrefab != null)
            Instantiate(vfxExplosionPrefab, transform.position, Quaternion.identity);

        // Détecte tous les colliders dans le rayon d'explosion
        Collider2D[] cibles = Physics2D.OverlapCircleAll(transform.position, rayonExplosion);

        foreach (Collider2D cible in cibles)
        {
            // Ne cible que les GameObjects taggés "Enemy"
            if (!cible.CompareTag("Enemy")) continue;

            EnemyController ennemi = cible.GetComponent<EnemyController>();
            if (ennemi != null && levelController != null)
                levelController.DetruireParExplosion(ennemi);
        }

        Destroy(gameObject);
    }
}
