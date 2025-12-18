using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Temel Bilgiler")]
    public string levelName = "Seviye 1";
    [TextArea] public string description = "Isınma turu.";
    public Image lvlImage;
    [Header("Görsel Tema")]
    public Color backgroundColor = Color.blue; 

    [Header("Düşman Yapısı")]
    public List<WavePattern> wavePatterns; 
    public int totalWaves = 5; 

    [Header("Zorluk Çarpanı")]
    public float difficultyMultiplier = 1.0f; 
}