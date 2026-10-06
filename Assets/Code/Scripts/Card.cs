using UnityEngine;

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
    
}
