using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    [System.Serializable]
    public enum Suit
    {
        Clubs,
        Cups,
        Golds,
        Swords
    }
    [System.Serializable]
    public enum Rank
    {
        Two,
        Four,
        Five,
        Six,
        Seven,
        Jack,
        Knight,
        King,
        Three,
        Ace
    }
    [SerializeField] private Sprite cardSprite;
    [SerializeField] private Suit cardSuit;
    [SerializeField] private Rank cardRank;

    public int CalculateCardValue()
    {
        switch (cardRank)
        {
            case Rank.Jack:
                return 2;
            case Rank.Knight:
                return 3;
            case Rank.King:
                return 4;
            case Rank.Three:
                return 10;
            case Rank.Ace:
                return 11;
        }

        return 1;
    }

    public Sprite GetCardSprite()
    {
        return cardSprite;
    }

    public Suit GetCardSuit()
    {
        return cardSuit;
    }

    public Rank GetCardRank()
    {
        return cardRank;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Only run in Editor (AssetDatabase is Editor-only)
        if (string.IsNullOrWhiteSpace(""+cardRank)) return;

        // Build the new name from serialized fields
        string newName = $"{cardRank}_of_{cardSuit}";

        // Get current asset path
        string assetPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath)) return; // Not an asset yet

        // Get current file name without extension
        string currentName = Path.GetFileNameWithoutExtension(assetPath);

        // Rename only if different
        if (currentName != newName)
        {
            EditorApplication.delayCall += () =>
            {
                if(this != null)
                {
                    AssetDatabase.RenameAsset(assetPath, newName);
                    AssetDatabase.SaveAssets();
                }
            };
        }
    }
    #endif
    
}
