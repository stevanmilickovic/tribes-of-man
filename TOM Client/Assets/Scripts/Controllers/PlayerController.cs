using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RiptideNetworking;
using System.Linq;

public class PlayerController : MonoBehaviour
{

    private static PlayerController singleton;

    public static PlayerController Singleton
    {
        get => singleton;
        private set
        {
            if (singleton == null)
                singleton = value;
            else if (singleton != value)
            {
                Debug.Log($"{nameof(PlayerController)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private bool[] inputs = new bool[4];
    public Camera mainCamera;
    public GameObject hit;
    public PlayerAnimator playerAnimator;
    public bool isChargingAttack = false;
    Vector2 hitDirection;

    private void Awake()
    {
        Singleton = this;
    }

    private void FixedUpdate()
    {
        if (HasInput())
        {
            PlayerSender.SendInputs(inputs);
        }
        RefreshInputs();
    }

    private void Update()
    {
        Aim();
        HandleInputs();
    }

    private void HandleInputs()
    {
        GetMovementInputs();
        GetMouseInputs();

        if (playerAnimator != null && !playerAnimator.isCharging) playerAnimator.TurnPlayerBasedOnInputs(inputs);
    }

    private void GetMouseInputs()
    {
        if (!EventSystem.current.IsPointerOverGameObject()) 
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                Pickup();

            if (!isChargingAttack)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Attack(MeleeAttackTypes.Left);
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Attack(MeleeAttackTypes.Right);
                }

                if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
                {
                    Attack(MeleeAttackTypes.Up);
                }

                if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
                {
                    Attack(MeleeAttackTypes.Down);
                }
            }
        }
    }

    public void ExecutedAttack()
    {
        
        isChargingAttack = false;
    }

    private void Aim()
    {
        if (hit == null) return;
        Vector2 mouseScreenPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - (Vector2)hit.transform.position).normalized;
        hitDirection = direction;
        //hit.transform.right = direction;
    }

    private void Attack(MeleeAttackTypes attackType)
    {
        if (hit == null) return;
        isChargingAttack = true;
        Vector2 direction = hitDirection;
        playerAnimator.MeleeCharge(attackType, direction);
        PlayerSender.SendAttackMessage(attackType, direction);
    }

    public void Pickup()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        int x = (int)mousePosition.x;
        int y = (int)mousePosition.y;
        if (MapManager.Singleton.tiles[(x, y)].itemObject != null)
        {
            InventorySender.SendPickupMessage(x, y);
        }
    }

    private void GetMovementInputs()
    {
        if (Input.GetKey(KeyCode.W))
        {
            inputs[0] = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputs[1] = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputs[2] = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputs[3] = true;
        }
    }

    private void RefreshInputs()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    private bool HasInput() //Returns true if at least one input is pressed
    {
        return inputs.Contains(true);
    }
}
