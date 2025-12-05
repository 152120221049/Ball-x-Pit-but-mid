using UnityEngine;

[CreateAssetMenu(fileName = "NewWavePattern", menuName = "Game/Wave Pattern")]
public class WavePattern : ScriptableObject
{
    [Header("Bu Satırın Deseni")]
    // Spawn noktası sayısı kadar eleman tutacak dizi.
    // Eğer bir noktada düşman istemiyorsan orayı boş bırakacaksın.
    public GameObject[] enemies;
}