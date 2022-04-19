using UnityEngine;

public class Structure
{

    public enum Type
    {
        Natural,
        Manmade
    }

    public Type type;
    public int id;
    public string name;
    public Item item;
    public int maxHealth;
    public int maxBrokenHealth;
    public Sprite sprite;

    public Structure(Type _type, int _id, string _name, Item _item, int _maxHealth, int _maxBrokenHealth, Sprite _sprite)
    {
        type = _type;
        id = _id;
        name = _name;
        item = _item;
        maxHealth = _maxHealth;
        maxBrokenHealth = _maxBrokenHealth;
        sprite = _sprite;
    }
}

