using UnityEngine;

/// <summary>
/// Construit et reconstruit un bouclier composé de blocs destructibles.
/// Appelé par LevelController au démarrage du niveau 3.
/// La forme est une arche typique Space Invaders (haut plein, bas évidé au centre).
/// </summary>
public class ShieldBuilder : MonoBehaviour
{
    // Prefab du bloc élémentaire constituant le bouclier
    [SerializeField] private GameObject blocPrefab;

    // Espacement entre les centres des blocs (en unités Unity)
    private const float TailleBloc = 0.4f;

    // Forme du bouclier : 1 = bloc présent, 0 = vide (lue de haut en bas)
    private static readonly int[,] Forme = new int[,]
    {
        { 0, 1, 1, 1, 0 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 0, 1, 1 },
    };

    /// <summary>
    /// Détruit tous les blocs existants et reconstruit le bouclier complet
    /// </summary>
    public void Reconstruire()
    {
        // Supprime les blocs résiduels d'un niveau précédent
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (blocPrefab == null)
        {
            Debug.LogWarning($"ShieldBuilder ({name}) : blocPrefab non assigné !");
            return;
        }

        int lignes = Forme.GetLength(0);
        int colonnes = Forme.GetLength(1);

        // Calcule les offsets pour centrer la forme sur la position du Transform parent
        float offsetX = -(colonnes - 1) * TailleBloc * 0.5f;
        float offsetY = -(lignes - 1) * TailleBloc * 0.5f;

        for (int l = 0; l < lignes; l++)
        {
            for (int c = 0; c < colonnes; c++)
            {
                if (Forme[l, c] == 0) continue;

                // Les lignes sont lues de haut en bas — l'axe Y est inversé
                Vector3 posLocale = new Vector3(
                    offsetX + c * TailleBloc,
                    offsetY + (lignes - 1 - l) * TailleBloc,
                    0f
                );

                Instantiate(blocPrefab, transform.position + posLocale, Quaternion.identity, transform);
            }
        }
    }
}
