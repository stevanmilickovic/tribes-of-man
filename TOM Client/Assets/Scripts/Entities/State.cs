using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public ushort tick;
    public Vector2 position;
    public bool[] inputs = new bool[4];

    public State(ushort _tick, Vector2 _position, bool[] _inputs)
    {
        tick = _tick;
        position = _position;
        inputs = _inputs;
    }
}
