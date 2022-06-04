using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolItem : Item
{
    public int effect;

    public ToolItem(int _id, string _name, bool _stackable, int _effect) : base(Type.Tool, _id, _name, _stackable)
    {
        effect = _effect;
    }
}
