using System.Collections;
using UnityEngine;

// Gestionnaire principal du jeu — Singleton
public class GameManager : MonoBehaviour
{
    // Instance unique accessible depuis n'importe quel script
    public static GameManager Instance { get; private set; }

    // Clé PlayerPrefs pour sauvegarder le meilleur score
    private const string CléMeilleurScore = "BestScore";

    // Score actuel du joueur
    private int score;

    // Meilleur score sauvegardé localement
    private int meilleurScore;

    // Niveau en cours (1 ou 2)
    private int niveauActuel;

    // Référence au gestionnaire d'interface
    [SerializeField] private UIManager uiManager;

    // Référence au contrôleur de niveau
    [SerializeField] private LevelController levelController;

    // Référence au GameObject du joueur — activé/désactivé selon l'état du jeu
    [SerializeField] private GameObject playerGameObject;

    /// <summary>
    /// Initialise le Singleton et charge le meilleur score sauvegardé
    /// </summary>
    private void Awake()
    {
        // Détruit le doublon si une instance existe déjà
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Charge le meilleur score depuis PlayerPrefs
        meilleurScore = PlayerPrefs.GetInt(CléMeilleurScore, 0);
    }

    /// <summary>
    /// Affiche le menu principal au démarrage
    /// </summary>
    private void Start()
    {
        uiManager.ShowMenu();
        uiManager.UpdateBestScore(meilleurScore);
    }

    /// <summary>
    /// Lance le jeu depuis le niveau 1
    /// </summary>
    public void StartGame()
    {
        // Remet le temps à la normale (en cas de Game Over ou victoire finale précédents)
        Time.timeScale = 1f;
        score = 0;
        niveauActuel = 1;
        playerGameObject.SetActive(true);
        uiManager.ShowHUD();
        uiManager.UpdateScore(score);
        levelController.StartLevel(1);
    }

    /// <summary>
    /// Ajoute des points au score et met à jour l'affichage
    /// </summary>
    public void AddScore(int points)
    {
        score += points;
        uiManager.UpdateScore(score);
    }

    /// <summary>
    /// Déclenche la défaite du joueur — fige le jeu immédiatement
    /// </summary>
    public void GameOver()
    {
        // Fige le jeu pour stopper la grille et tous les mouvements
        Time.timeScale = 0f;
        playerGameObject.SetActive(false);
        SauvegarderMeilleurScore();
        uiManager.ShowEnd(false);
        uiManager.UpdateFinalScore(score);
    }

    /// <summary>
    /// Déclenche la victoire — passe au niveau suivant ou affiche l'écran de fin
    /// </summary>
    public void Victory()
    {
        if (niveauActuel < 2)
        {
            // Passage au niveau suivant — affiche la transition puis lance le niveau 2
            niveauActuel++;
            uiManager.ShowLevelTransition("Niveau 2 !");
            StartCoroutine(LancerNiveauApresTransition(2));
        }
        else
        {
            // Fin réelle du jeu après le niveau 2 — fige le jeu
            Time.timeScale = 0f;
            playerGameObject.SetActive(false);
            SauvegarderMeilleurScore();
            uiManager.ShowEnd(true);
            uiManager.UpdateFinalScore(score);
        }
    }

    /// <summary>
    /// Attend en temps réel puis masque la transition et lance le niveau donné
    /// </summary>
    private IEnumerator LancerNiveauApresTransition(int niveau)
    {
        // Durée de la transition en secondes réels (indépendant de Time.timeScale)
        const float DureeTransition = 1.5f;

        yield return new WaitForSecondsRealtime(DureeTransition);

        uiManager.HideLevelTransition();
        levelController.StartLevel(niveau);
    }

    /// <summary>
    /// Relance le jeu depuis le niveau 1 (bouton Rejouer)
    /// </summary>
    public void RestartGame()
    {
        StartGame();
    }

    /// <summary>
    /// Retourne au menu principal
    /// </summary>
    public void ReturnToMenu()
    {
        // Remet le temps à la normale par sécurité
        Time.timeScale = 1f;
        playerGameObject.SetActive(false);
        levelController.ClearLevel();
        uiManager.ShowMenu();
        uiManager.UpdateBestScore(meilleurScore);
    }

    /// <summary>
    /// Sauvegarde le meilleur score si le score actuel est supérieur
    /// </summary>
    private void SauvegarderMeilleurScore()
    {
        if (score > meilleurScore)
        {
            meilleurScore = score;
            PlayerPrefs.SetInt(CléMeilleurScore, meilleurScore);
            PlayerPrefs.Save();
        }
    }

    // Propriétés en lecture seule
    public int Score => score;
    public int MeilleurScore => meilleurScore;
    public int NiveauActuel => niveauActuel;
}
