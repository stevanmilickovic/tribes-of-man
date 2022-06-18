using UnityEngine;

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
    public int maxBrokenHealth;
    public Sprite sprite;
    public Sprite collapsedSprite;

    public Structure(Type _type, SizeType _sizeType, int _id, string _name, Item _item, int _maxHealth, int _maxBrokenHealth, Sprite _sprite, Sprite _collapsedSprite)
    {
        type = _type;
        sizeType = _sizeType;
        id = _id;
        name = _name;
        item = _item;
        maxHealth = _maxHealth;
        maxBrokenHealth = _maxBrokenHealth;
        sprite = _sprite;
        collapsedSprite = _collapsedSprite;
    }
}

