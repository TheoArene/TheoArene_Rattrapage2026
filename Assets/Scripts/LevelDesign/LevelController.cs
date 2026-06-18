using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôleur de niveau : spawn de la grille ennemie, mouvement en bloc et suivi de la victoire
/// </summary>
public class LevelController : MonoBehaviour
{
    // Nombre de lignes d'ennemis
    private const int NombreLignes = 5;

    // Nombre de colonnes d'ennemis
    private const int NombreColonnes = 10;

    // Espacement horizontal entre chaque ennemi (en unités)
    private const float EspacementX = 1.1f;

    // Espacement vertical entre chaque ligne (en unités)
    private const float EspacementY = 1.0f;

    // Position X du premier ennemi — grille centrée à X=0 pour 10 ennemis
    private const float OrigineX = -4.95f;

    // Position Y de la première ligne d'ennemis
    private const float OrigineY = 3.0f;

    // Vitesse de déplacement horizontal du groupe (en unités par seconde)
    private const float VitesseHorizontale = 2f;

    // Distance de descente à chaque rebond latéral (en unités)
    private const float PasDescente = 0.5f;

    // Limite gauche au-delà de laquelle le groupe rebondit
    private const float LimiteGauche = -7.0f;

    // Limite droite au-delà de laquelle le groupe rebondit
    private const float LimiteDroite = 7.0f;

    // Référence au gestionnaire principal du jeu
    [SerializeField] private GameManager gameManager;

    // Prefab de l'ennemi à instancier dans la grille
    [SerializeField] private GameObject enemyPrefab;

    // Prefab du bonus explosif — assigné uniquement à l'ennemi central au niveau 2
    [SerializeField] private GameObject bonusPrefab;

    // Parent commun de tous les ennemis — déplacé comme un seul bloc
    [SerializeField] private Transform enemyGroupParent;

    // Tableau des quatre boucliers — activés et reconstruits uniquement au niveau 3
    [SerializeField] private ShieldBuilder[] boucliers;

    // Liste des ennemis encore vivants
    private readonly List<GameObject> ennemisActifs = new List<GameObject>();

    // Direction de déplacement actuelle : 1 = droite, -1 = gauche
    private float directionActuelle = 1f;

    // Vrai si un niveau est actuellement en cours
    private bool niveauEnCours;

    /// <summary>
    /// Lance un niveau : réinitialise la grille, démarre le mouvement et gère les boucliers
    /// </summary>
    public void StartLevel(int niveau)
    {
        ClearLevel();
        SpawnerGrille(niveau);
        directionActuelle = 1f;
        niveauEnCours = true;

        // Niveau 3 : active et reconstruit les boucliers ; autres niveaux : boucliers masqués
        if (boucliers != null)
        {
            foreach (ShieldBuilder bouclier in boucliers)
            {
                if (bouclier == null) continue;

                if (niveau == 3)
                {
                    bouclier.gameObject.SetActive(true);
                    bouclier.Reconstruire();
                }
                else
                {
                    bouclier.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Supprime tous les ennemis présents, réinitialise l'état du niveau et masque les boucliers
    /// </summary>
    public void ClearLevel()
    {
        // Détruit chaque enfant du groupe ennemi
        for (int i = enemyGroupParent.childCount - 1; i >= 0; i--)
            Destroy(enemyGroupParent.GetChild(i).gameObject);

        ennemisActifs.Clear();
        niveauEnCours = false;

        // Recentre le groupe à l'origine pour le prochain niveau
        enemyGroupParent.position = Vector3.zero;

        // Sécurité : désactive tous les boucliers au retour menu ou en cas de Game Over
        if (boucliers != null)
        {
            foreach (ShieldBuilder bouclier in boucliers)
            {
                if (bouclier != null)
                    bouclier.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Instancie la grille 5×10 et configure chaque ennemi selon le niveau
    /// </summary>
    private void SpawnerGrille(int niveau)
    {
        for (int ligne = 0; ligne < NombreLignes; ligne++)
        {
            for (int col = 0; col < NombreColonnes; col++)
            {
                float posX = OrigineX + col * EspacementX;
                float posY = OrigineY - ligne * EspacementY;
                Vector3 position = new Vector3(posX, posY, 0f);

                GameObject ennemi = Instantiate(enemyPrefab, position, Quaternion.identity, enemyGroupParent);
                ennemi.name = $"Enemy_L{ligne}_C{col}";

                // Injecte le LevelController dans l'EnemyController de cet ennemi
                EnemyController controller = ennemi.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.Init(this);

                    // Niveau 2 : assigne le bonus à l'ennemi central (ligne 2, colonne 4)
                    if (niveau == 2 && ligne == 2 && col == 4 && bonusPrefab != null)
                        controller.DefinirBonusADropper(bonusPrefab);
                }

                // Désactive le tir par défaut sur tous les ennemis
                EnemyShooter shooter = ennemi.GetComponent<EnemyShooter>();
                if (shooter != null)
                {
                    // Niveaux 2 et 3 : active le tir uniquement sur les lignes 0 et 1
                    shooter.enabled = (niveau >= 2 && ligne < 2);

                    // Règle la probabilité de tir selon le niveau (50% au niveau 3, 30% au niveau 2)
                    if (shooter.enabled)
                        shooter.DefinirProbabiliteTir(niveau == 3 ? 0.5f : 0.3f);
                }

                ennemisActifs.Add(ennemi);
            }
        }
    }

    /// <summary>
    /// Déplace le groupe ennemi horizontalement et gère les rebonds avec descente
    /// </summary>
    private void Update()
    {
        if (!niveauEnCours) return;

        // Supprime les références nulles (ennemis déjà détruits)
        ennemisActifs.RemoveAll(e => e == null);
        if (ennemisActifs.Count == 0) return;

        // Calcule et applique le déplacement horizontal de ce frame
        float deplacement = directionActuelle * VitesseHorizontale * Time.deltaTime;
        enemyGroupParent.Translate(Vector3.right * deplacement);

        // Vérifie si un ennemi dépasse la limite côté actif
        bool horsLimite = false;
        foreach (GameObject ennemi in ennemisActifs)
        {
            if (ennemi == null) continue;
            float x = ennemi.transform.position.x;
            if (directionActuelle > 0f && x > LimiteDroite) { horsLimite = true; break; }
            if (directionActuelle < 0f && x < LimiteGauche) { horsLimite = true; break; }
        }

        if (horsLimite)
        {
            // Annule le déplacement, fait descendre le groupe et inverse la direction
            enemyGroupParent.Translate(Vector3.right * -deplacement);
            enemyGroupParent.Translate(Vector3.down * PasDescente);
            directionActuelle *= -1f;
        }
    }

    /// <summary>
    /// Appelé par EnemyController quand un ennemi est détruit par un tir joueur
    /// </summary>
    public void SignalerEnnemiDetruit(GameObject ennemi)
    {
        ennemisActifs.Remove(ennemi);

        // Plus aucun ennemi sur le terrain = victoire
        if (ennemisActifs.Count == 0)
            gameManager.Victory();
    }

    /// <summary>
    /// Détruit un ennemi via l'explosion d'un bonus : réutilise la logique de score et de victoire
    /// </summary>
    public void DetruireParExplosion(EnemyController ennemi)
    {
        if (ennemi == null) return;

        // Même comportement qu'un tir joueur classique : score, retrait de liste, vérification victoire
        GameManager.Instance.AddScore(1);
        SignalerEnnemiDetruit(ennemi.gameObject);
        Destroy(ennemi.gameObject);
    }

    /// <summary>
    /// Appelé par DeathZone quand un ennemi franchit la limite basse de l'écran
    /// </summary>
    public void SignalerEnnemiEnBas()
    {
        gameManager.GameOver();
    }
}
