using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    private Interpolator interpolator;

    public int id;
    public string username;

    public Player(int _id, string _username)
    {
        id = _id;
        username = _username;
        try
        {
            interpolator = gameObject.GetComponent<Interpolator>();
        }
        catch (Exception) { }
    }

    public void Move(ushort tick, Vector2 position)
    {
        interpolator = gameObject.GetComponent<Interpolator>();
        interpolator.NewUpdate(tick, position);
    }

}
