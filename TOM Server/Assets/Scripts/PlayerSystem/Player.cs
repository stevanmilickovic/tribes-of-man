using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int id;
    public ushort currectClientId;
    public string playerName;

    public Player(int _id, ushort _currectClientId, string _playerName)
    {
        id = _id;
        currectClientId = _currectClientId;
        playerName = _playerName;
    }

}
