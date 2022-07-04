public class Item
{
    public enum Type
    {
        Weapon,
        Resource,
        Tool,
        Shield,
        Clothing,
        Armor,
        Food
    }

    public Type type;
    public int id;
    public string name;
    public bool stackable;

    public Item(Type _type, int _id, string _name, bool _stackable)
    {
        type = _type;
        id = _id;
        name = _name;
        stackable = _stackable;
    }
}
