using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // --- EKONOMİ & İLERLEME ---
    public int bankedXP;
    public int maxBudget;
    public int budgetUpgradeCost;
    public int controlModeIndex;
    public int maxLevelReached;

    // --- EŞYALAR (Dictionary JSON'a çevrilemez, List kullanıyoruz) ---
    public List<SavedItemEntry> savedItems = new List<SavedItemEntry>();

    // --- DESTE (Hangi eşyadan kaç tane var) ---
    public List<SavedDeckEntry> savedDeck = new List<SavedDeckEntry>();

    // --- PERKLER ---
    public List<string> ownedPerkIDs = new List<string>();
    public List<string> equippedPerkIDs = new List<string>();
    public SaveData()
    {
        
        bankedXP = 500;

        
        maxBudget = 10;
        budgetUpgradeCost = 100;

        
        controlModeIndex = 1; // 1: Hybrid (Tek El), 0: Dual
        maxLevelReached = 1;

        
    }
}

[System.Serializable]
public class SavedItemEntry
{
    public string itemID;
    public bool isUnlocked;
    public int stock;
    public int level;

    public SavedItemEntry(string id, bool unlocked, int st, int lvl)
    {
        itemID = id;
        isUnlocked = unlocked;
        stock = st;
        level = lvl;
    }
}

[System.Serializable]
public class SavedDeckEntry
{
    public string itemID;
    public int amount; // Stack miktarı

    public SavedDeckEntry(string id, int amt)
    {
        itemID = id;
        amount = amt;
    }
}