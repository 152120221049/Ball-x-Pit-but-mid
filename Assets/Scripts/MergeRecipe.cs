using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Game/Merge Recipe")]
public class MergeRecipe : ScriptableObject
{
    [Header("Gereken Malzemeler")]
    public ItemData inputA;
    public ItemData inputB;

    [Header("Sonuç")]
    public ItemData resultItem;

    [Header("Usta Parası")]
    public int cost = 500; // Birleştirme maliyeti
}