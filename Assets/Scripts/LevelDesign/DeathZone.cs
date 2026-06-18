using UnityEngine;

/// <summary>
/// Zone de mort positionnée en bas de l'écran.
/// Déclenche le game over si un ennemi la franchit.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class DeathZone : MonoBehaviour
{
    // Référence au contrôleur de niveau pour signaler la défaite
    [SerializeField] private LevelController levelController;

    /// <summary>
    /// Détecte les ennemis qui descendent jusqu'en bas de l'écran
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Un ennemi franchit la limite basse = défaite
        if (other.CompareTag("Enemy"))
        {
            levelController?.SignalerEnnemiEnBas();
        }
    }
}
