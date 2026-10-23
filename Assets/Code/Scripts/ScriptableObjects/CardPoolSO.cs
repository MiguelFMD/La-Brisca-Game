using UnityEngine;

[CreateAssetMenu(menuName = "Services/Card Pool Service")]
public class CardPoolSO : ScriptableObject
{
    public System.Func<CardDisplay> RequestCard;

    public CardDisplay Get()
    {
        if (RequestCard != null) return RequestCard.Invoke();
        Debug.LogError("Card pool not managed.");
        return null;
    }
}