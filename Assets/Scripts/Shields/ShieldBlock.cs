using UnityEngine;

/// <summary>
/// Bloc destructible composant un bouclier — encaisse les tirs joueurs et ennemis,
/// change de couleur quand endommagé, et se détruit quand sa santé atteint zéro.
/// </summary>
public class ShieldBlock : MonoBehaviour
{
    // Points de vie du bloc (2 = survit au premier tir, détruit au second)
    [SerializeField] private int pointsDeVie = 2;

    // Couleur du bloc lorsqu'il est intact
    private static readonly Color CouleurSain = new Color(0.35f, 0.8f, 0.35f);

    // Couleur du bloc lorsqu'il est endommagé (un seul point de vie restant)
    private static readonly Color CouleurEndommage = new Color(0.85f, 0.55f, 0.1f);

    // Référence au SpriteRenderer mis en cache dans Awake
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Met en cache le SpriteRenderer et applique la couleur de bloc intact
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = CouleurSain;
    }

    /// <summary>
    /// Réagit aux collisions : seuls les tirs ennemis endommagent le bloc
    /// Les tirs joueurs le traversent sans interaction
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tir ennemi uniquement : détruit la balle et inflige un dégât au bloc
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Toucher();
        }
    }

    /// <summary>
    /// Inflige un dégât au bloc — met à jour la couleur et détruit si santé nulle
    /// </summary>
    private void Toucher()
    {
        pointsDeVie--;

        if (pointsDeVie <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Indique visuellement que le bloc est endommagé
        if (spriteRenderer != null)
            spriteRenderer.color = CouleurEndommage;
    }
}
