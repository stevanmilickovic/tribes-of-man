using UnityEngine;

public class PlayerPacket
{

    public PlayerPacket(int _id, string _username, Vector2 _position, Equipment _clothes, Equipment _tools)
    {
        id = _id;
        username = _username;
        position = _position;
        clothes = _clothes;
        tools = _tools;
    }

    public PlayerPacket(int _id, Vector2 _position)
    {
        id = _id;
        position = _position;
    }

    public int id;
    public string username;
    public Vector2 position;

    public Equipment clothes;
    public Equipment tools;

}
