using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager singleton;

    public static PlayerManager Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(PlayerManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject myPlayerPrefab;
    [SerializeField] private Interpolator interpolator;
    public MyPlayer myPlayer;
    public Dictionary<int, Player> players;
    public int myId = -1;

    public Camera mainCamera;
    public bool cinematicMode;

    private void Awake()
    {
        Singleton = this;
        players = new Dictionary<int, Player>();
    }

    public void SpawnPlayer(PlayerPacket packet)
    {
        if (players.ContainsKey(packet.id)) return;

        GameObject player = Instantiate(packet.id == myId ? myPlayerPrefab : playerPrefab);
        player.transform.position = new Vector3(packet.position.x, packet.position.y, 0f);
        player.name = packet.username;
        Player playerScript = SetPlayerScript(player, packet);
        players.Add(packet.id, playerScript);

        if (packet.id != myId)
            MapUtil.GetChunk(packet.position).players.Add(packet.id, playerScript);
    }

    private Player SetPlayerScript(GameObject newPlayer, PlayerPacket packet)
    {

        Player newPlayerScript = GetScriptFromPlayerGameObject(newPlayer, packet);
        newPlayerScript.FillPlayerData(packet);

        if (packet.id == myId)
        {
            if (!cinematicMode)
            {
                //mainCamera.transform.SetParent(newPlayer.transform);
                mainCamera.transform.position = newPlayer.transform.position;
                mainCamera.transform.GetComponent<CameraFollow>().playerTransform = newPlayer.transform;
            }
            newPlayer.GetComponentInChildren<Transform>().position = newPlayer.transform.position; //Possible difficulties with children position

            PlayerController.Singleton.hit = newPlayer.transform.GetChild(2).gameObject;
            PlayerController.Singleton.playerAnimator = newPlayer.gameObject.GetComponent<PlayerAnimator>();
            PlayerController.Singleton.playerAnimator.isMyPlayer = true;
        }

        return newPlayerScript;
    }

    private Player GetScriptFromPlayerGameObject(GameObject newPlayer, PlayerPacket packet)
    {
        if (packet.id == myId)
        {
            MyPlayer myPlayerScript = newPlayer.AddComponent<MyPlayer>();
            myPlayer = myPlayerScript;
            return myPlayerScript;
        }
        else
        {
            return newPlayer.AddComponent<Player>();
        }
    }

    public void SetMyId(int id)
    {
        myId = id;
        if (players.ContainsKey(id))
        {
            GameObject oldPlayer = players[id].gameObject;

            Vector2 position = new Vector2(oldPlayer.transform.position.x, oldPlayer.transform.position.y);
            string username = players[id].username;
            Equipment clothes = players[id].clothes;
            Equipment tools = players[id].tools;

            Destroy(oldPlayer);
            players.Remove(id);

            SpawnPlayer(new PlayerPacket(id, username, position, clothes, tools));
        }
    }

    public void UpdatePlayerPosition(int id, ushort tick, Vector2 position)
    {
        if (players.TryGetValue(id, out Player player) && id != myId)
        {
            player.Move(tick, position);
        }
        else if (id == myId)
        {
            myPlayer.Reconciliate(tick, position);
        }
    }

    public void UpdatePlayerEquipment(int id, Equipment clothes, Equipment tools)
    {
        if (players.TryGetValue(id, out Player player) && id != myId)
        {
            player.clothes = clothes;
            player.tools = tools;
            player.CheckEquipment();
        }
    }

    public void ChargePlayerMeleeAttack(int id, MeleeAttackTypes type, Vector2 direction, ushort tick)
    {
        Player player = players[id];
        PlayerAnimator playerAnimator = player.gameObject.GetComponent<PlayerAnimator>();

        if (myId != id)
        {
            playerAnimator.MeleeCharge(type, direction);
        }
    }

    public void ExecutePlayerAttack(int id, MeleeAttackTypes type, Vector2 direction)
    {

        Player player = players[id];
        PlayerAnimator playerAnimator = player.gameObject.GetComponent<PlayerAnimator>();

        playerAnimator.ExecuteMeleeAttack(type, direction);

        PlayerController.Singleton.isChargingAttack = false;
    }

    
}
