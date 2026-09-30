using UnityEngine;

[CreateAssetMenu(fileName = "DeckType", menuName = "Scriptable Objects/DeckType")]
public class DeckType : ScriptableObject
{
    [SerializeField] public Card[] cards;
}
