using UnityEngine;
using UnityEngine.UI;

// Gestionnaire de l'interface — activation des panels et mise à jour des textes
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    // Panel du menu principal
    [SerializeField] private GameObject menuPanel;
    // Panel du HUD en cours de jeu
    [SerializeField] private GameObject hudPanel;
    // Panel de fin de partie
    [SerializeField] private GameObject endPanel;

    [Header("Textes — Menu")]
    // Affiche le meilleur score dans le menu
    [SerializeField] private Text bestScoreText;

    [Header("Textes — HUD")]
    // Affiche le score en temps réel
    [SerializeField] private Text scoreText;

    [Header("Textes — Fin de partie")]
    // Titre : "Game Over" ou "Victoire !"
    [SerializeField] private Text endTitleText;
    // Score affiché sur l'écran de fin
    [SerializeField] private Text finalScoreText;

    /// <summary>
    /// Affiche uniquement le panel Menu
    /// </summary>
    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        hudPanel.SetActive(false);
        endPanel.SetActive(false);
    }

    /// <summary>
    /// Affiche uniquement le panel HUD (pendant le jeu)
    /// </summary>
    public void ShowHUD()
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(true);
        endPanel.SetActive(false);
    }

    /// <summary>
    /// Affiche le panel de fin avec le titre selon le résultat
    /// </summary>
    /// <param name="estVictoire">True = "Victoire !", False = "Game Over"</param>
    public void ShowEnd(bool estVictoire)
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(false);
        endPanel.SetActive(true);

        // Choisit le titre selon le résultat
        endTitleText.text = estVictoire ? "Victoire !" : "Game Over";
    }

    /// <summary>
    /// Met à jour le meilleur score affiché dans le menu
    /// </summary>
    public void UpdateBestScore(int valeur)
    {
        bestScoreText.text = "Meilleur score : " + valeur;
    }

    /// <summary>
    /// Met à jour le score affiché pendant le jeu
    /// </summary>
    public void UpdateScore(int valeur)
    {
        scoreText.text = "Score : " + valeur;
    }

    /// <summary>
    /// Met à jour le score final affiché sur l'écran de fin
    /// </summary>
    public void UpdateFinalScore(int valeur)
    {
        finalScoreText.text = "Score final : " + valeur;
    }

    /// <summary>
    /// Quitte l'application (stoppe le Play mode en éditeur)
    /// </summary>
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
