using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolItem : Item
{

    int effect;

    public ToolItem(int _id, string _name, bool _stackable, Sprite _sprite, int _effect) : base(Type.Tool, _id, _name, _stackable, _sprite)
    {
        effect = _effect;
    }
}
