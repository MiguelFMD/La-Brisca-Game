
public enum DamageType {Trick, Bleeding, Thorns}
public class DamageContext
{
    public Player Source {get; set;}
    public Player Target {get; set;}
    public DamageType DamageType {get; set;}
    public int Amount {get; set;}
    public bool IsRetaliation {get; set;} //Avoid infinite damage loops 
}
