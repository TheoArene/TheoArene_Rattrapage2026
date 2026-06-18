using UnityEngine;

/// <summary>
/// Comportement d'un projectile : déplacement directionnel et destruction hors écran
/// </summary>
public class Bullet : MonoBehaviour
{
    // Direction normalisée du projectile — (0,1) = vers le haut, (0,-1) = vers le bas
    [SerializeField] private Vector2 direction = Vector2.up;

    // Vitesse de déplacement (en unités par seconde)
    [SerializeField] private float vitesse = 8f;

    // Distance en Y depuis l'origine au-delà de laquelle le projectile est détruit
    private const float LimiteDestructionY = 12f;

    // Distance en X depuis l'origine au-delà de laquelle le projectile est détruit
    private const float LimiteDestructionX = 12f;

    // Référence au Rigidbody2D (mise en cache dans Awake)
    private Rigidbody2D rb;

    /// <summary>
    /// Met en cache le Rigidbody2D
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Déplace le projectile et le détruit s'il dépasse les limites de l'écran
    /// </summary>
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * vitesse * Time.fixedDeltaTime);

        // Détruit le projectile s'il sort des limites de la zone de jeu
        if (Mathf.Abs(rb.position.y) > LimiteDestructionY || Mathf.Abs(rb.position.x) > LimiteDestructionX)
        {
            Destroy(gameObject);
        }
    }
}
