public class Structure
{

    public enum Type
    {
        Natural,
        Manmade
    }

    public enum SizeType
    {
        Large,
        Medium,
        Small
    }

    public Type type;
    public SizeType sizeType;
    public bool collapsable;
    public int id;
    public string name;
    public Item item;
    public int maxHealth;
    public int maxCollapsedHealth;

    public Structure(Type _type, SizeType _sizeType, bool _collapsable, int _id, string _name, Item _item, int _maxHealth, int _maxCollapsedHealth)
    {
        type = _type;
        sizeType = _sizeType;
        collapsable = _collapsable;
        id = _id;
        name = _name;
        item = _item;
        maxHealth = _maxHealth;
        maxCollapsedHealth = _maxCollapsedHealth;
    }
}

