using UnityEngine;

/// <summary>
/// Comportement d'un ennemi : détection des collisions avec le joueur et ses projectiles.
/// Note : pas de Rigidbody2D sur les ennemis — ils sont déplacés via Transform par LevelController
/// (mouvement en bloc, pas de physique). La détection de triggers fonctionne car le joueur
/// et ses projectiles possèdent un Rigidbody2D.
/// </summary>
public class EnemyController : MonoBehaviour
{
    // Référence au contrôleur de niveau — injectée par LevelController lors du spawn
    private LevelController levelController;

    // Prefab à instancier à la position de l'ennemi quand il est détruit (null par défaut)
    [SerializeField] private GameObject bonusADropper;

    /// <summary>
    /// Initialise la référence au LevelController (appelé par LevelController.SpawnerGrille)
    /// </summary>
    public void Init(LevelController controller)
    {
        levelController = controller;
    }

    /// <summary>
    /// Assigne le prefab de bonus à déposer à la destruction de cet ennemi
    /// </summary>
    public void DefinirBonusADropper(GameObject prefab)
    {
        bonusADropper = prefab;
    }

    /// <summary>
    /// Gère la collision avec un projectile joueur : drop du bonus éventuel, score, destructions
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Touché par un projectile joueur : dépose le bonus si configuré, puis +1 point
        if (other.CompareTag("PlayerBullet"))
        {
            // Instancie le bonus comme enfant du groupe d'ennemis (même parent) pour qu'il suive la grille
            if (bonusADropper != null)
                Instantiate(bonusADropper, transform.position, Quaternion.identity, transform.parent);

            GameManager.Instance.AddScore(1);
            Destroy(other.gameObject);
            levelController?.SignalerEnnemiDetruit(gameObject);
            Destroy(gameObject);
        }
    }
}
