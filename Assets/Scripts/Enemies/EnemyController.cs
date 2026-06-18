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

    /// <summary>
    /// Initialise la référence au LevelController (appelé par LevelController.SpawnerGrille)
    /// </summary>
    public void Init(LevelController controller)
    {
        levelController = controller;
    }

    /// <summary>
    /// Gère les collisions avec les éléments du joueur
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Touché par un projectile joueur : +1 point, destruction de l'ennemi et du projectile
        if (other.CompareTag("PlayerBullet"))
        {
            GameManager.Instance.AddScore(1);
            Destroy(other.gameObject);
            levelController?.SignalerEnnemiDetruit(gameObject);
            Destroy(gameObject);
            return;
        }

        // Contact direct avec le joueur = game over immédiat
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
