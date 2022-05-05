using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{

    public Inventory inventory;
    private bool[] inputs = new bool[4];
    private static int BUFFER_SIZE = 1024;
    private State[] states = new State[BUFFER_SIZE];

    public MyPlayer(int _id, string _username) : base(_id, _username) { }

    private void Awake()
    {
        inputs = new bool[4];
        states = new State[BUFFER_SIZE];
    }

    private void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        gameObject.transform.Translate(GetInputDirection(inputs) * Time.deltaTime * 3);
        RefreshInputs();
        states[NetworkManager.Singleton.ServerTick % BUFFER_SIZE] = new State(NetworkManager.Singleton.ServerTick, gameObject.transform.position, inputs);
    }

    public void Reconciliate(ushort serverTick, Vector2 serverPosition)
    {
        if (states[serverTick % BUFFER_SIZE] == null)
            return;

        if (serverTick > NetworkManager.Singleton.ServerTick)
            states[serverTick % BUFFER_SIZE] = new State(serverTick, gameObject.transform.position, inputs);

        float positionError = Vector2.Distance(states[serverTick % BUFFER_SIZE].position, serverPosition);

        if (positionError > 0.02f)
        {
            Debug.Log($"Reconciliation from {states[serverTick % BUFFER_SIZE].position} to {serverPosition}");

            states[serverTick % BUFFER_SIZE].position = serverPosition;
            transform.position = serverPosition;

            ushort tickToProcess = serverTick++;

            while (tickToProcess < NetworkManager.Singleton.ServerTick)
            {
                State currentState = states[tickToProcess % BUFFER_SIZE];
                currentState.position = RecalculatePosition(currentState.inputs);
                tickToProcess++;
            }
        }
    }

    private Vector2 RecalculatePosition(bool[] _inputs)
    {
        transform.Translate(GetInputDirection(_inputs) * 0.03125f * 3);
        Vector2 newPosition = transform.position;
        return newPosition;
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
        return inputDirection;
    }
}
