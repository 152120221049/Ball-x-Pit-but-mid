using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Zorunlu Yöneticiler")]
    public GameObject playerDataManagerPrefab; // Prefab'ı buraya atacağız

    void Awake()
    {
        // Eğer sahnede PlayerDataManager YOKSA, oluştur.
        if (PlayerDataManager.Instance == null)
        {
            Instantiate(playerDataManagerPrefab);
            Debug.Log("GameInitializer: Yönetici oluşturuldu.");
        }
        else
        {
            Debug.Log("GameInitializer: Yönetici zaten var, yenisine gerek yok.");
        }
    }
}