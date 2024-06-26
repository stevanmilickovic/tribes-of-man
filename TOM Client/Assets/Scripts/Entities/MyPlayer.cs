using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{

    public Inventory inventory;
    private bool[] inputs = new bool[4];
    private static int BUFFER_SIZE = 1024;
    private State[] states = new State[BUFFER_SIZE];
    public Chunk currentChunk;
    public Vector2 targetCorrectedPosition;
    public static float TICK_TIME = 0.03125f;
    public static float PLAYER_SPEED = 5f;
    private Vector2 targetPosition;

    public MyPlayer(int _id, string _username) : base(_id, _username) { }

    private void Awake()
    {
        inputs = new bool[4];
        states = new State[BUFFER_SIZE];
        targetPosition = transform.position;
    }

    private void Update()
    {
        GetInputs();
        UpdateChunk();

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        float speedToTarget = distanceToTarget / TICK_TIME;
        // Move the player towards the target position at the calculated speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speedToTarget * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        
        if (!PlayerController.Singleton.isChargingAttack)
        {
            Vector2 movement = GetInputDirection(inputs) * Time.fixedDeltaTime * PLAYER_SPEED;
            targetPosition += new Vector2(movement.x, movement.y);
            states[NetworkManager.Singleton.ServerTick % BUFFER_SIZE] = new State(NetworkManager.Singleton.ServerTick, targetPosition, inputs);
        }

        RefreshInputs();
    }

    public void Reconciliate(ushort serverTick, Vector2 serverPosition)
    {
        if (serverTick > NetworkManager.Singleton.ServerTick)
            states[serverTick % BUFFER_SIZE] = new State(serverTick, targetPosition, inputs);

        if (states[serverTick % BUFFER_SIZE] == null)
            return;

        float positionError = Vector2.Distance(states[serverTick % BUFFER_SIZE].position, serverPosition);

        if (positionError > 0.02f)
        {

            states[serverTick % BUFFER_SIZE].position = serverPosition;
            targetCorrectedPosition = serverPosition;

            ushort tickToProcess = serverTick++;

            while (tickToProcess <= NetworkManager.Singleton.ServerTick)
            {
                State currentState = states[tickToProcess % BUFFER_SIZE];
                currentState.position = RecalculatePosition(currentState.inputs);
                tickToProcess++;
            }

            targetPosition = targetCorrectedPosition;
        }
    }

    private Vector2 RecalculatePosition(bool[] _inputs)
    {
        targetCorrectedPosition += (GetInputDirection(_inputs) * TICK_TIME * PLAYER_SPEED);
        return targetCorrectedPosition;
    }

    private void GetInputs()
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

    private Vector2 GetInputDirection(bool[] inputs)
    {
        Vector2 inputDirection = Vector2.zero;
        if (inputs[0])
            inputDirection.y += 1;
        if (inputs[1])
            inputDirection.x -= 1;
        if (inputs[2])
            inputDirection.y -= 1;
        if (inputs[3])
            inputDirection.x += 1;

        if (inputDirection.magnitude > 0)
        {
            inputDirection = inputDirection.normalized;
        }

        return inputDirection;
    }

    public void UpdateChunk()
    {
        if (currentChunk != MapUtil.GetChunk(transform.position))
        {
            if (currentChunk != null)
            {
                MapManager.Singleton.UpdateVisibleChunks(currentChunk, MapUtil.GetChunk(transform.position));
            }
            currentChunk = MapUtil.GetChunk(transform.position);
        }
    }
}
