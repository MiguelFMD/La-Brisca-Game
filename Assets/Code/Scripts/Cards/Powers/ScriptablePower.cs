using UnityEngine;

public abstract class ScriptablePower : ScriptableObject, IPowerEffect
{
    [SerializeField] private string powerName;
    [TextArea] [SerializeField] private string powerDescription;

    public string Name => powerName;
    public string Description => powerDescription;

    public abstract void Activate(CardData cardData, Player source, Player target);
}
