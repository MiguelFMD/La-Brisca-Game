
public enum DamageType {Trick, Bleeding, Thorns}
public class DamageContext
{
    public PlayerState Source {get; set;}
    public PlayerState Target {get; set;}
    public DamageType DamageType {get; set;}
    public float Amount {get; set;}
    public bool IsRetaliation {get; set;} //Avoid infinite damage loops 
}
