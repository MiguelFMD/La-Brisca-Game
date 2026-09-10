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
    [SerializeField] private Texture2D cardImage;
    [SerializeField] public Suit cardSuit;
    [Range(1, 12)]
    [SerializeField] public int number;

    /// <summary>
    /// Calculates the card's value based on the card number and the rules.
    /// </summary>
    private int CalculateCardValue()
    {
        switch (number)
        {
            case 1:
                return 11;
            case 3:
                return 10;
            case 12:
                return 4;
            case 11:
                return 3;
            case 10:
                return 2;
        }

        return 0;
    }
}
