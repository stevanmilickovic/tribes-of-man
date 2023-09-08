using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public GameObject body;
    public static float BODY_SCALE = 1.5f;

    public Sprite headFront;
    public Sprite headBack;
    public Sprite torsoFront;
    public Sprite torsoBack;

    public GameObject headObject;
    public GameObject torsoObject;
    public GameObject armLeftObject;
    public GameObject armRightObject;

    public static float ATTACK_SPRITE_SCALE = 5f;
    public Animator slashAnimator;
    public GameObject slashObject;
    public Animator stabAnimator;
    public GameObject stabObject;
    public Animator crushAnimator;
    public GameObject crushObject;

    private float speed;
    private Vector2 lastPosition;
    public bool isMyPlayer = false;

    public bool chargingLeft = false;
    public bool chargingRight = false;
    public bool chargingUp = false;
    public bool chargingDown = false;

    public bool isCharging = false;

    public GameObject hit;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(1).gameObject.GetComponent<Animator>();
        slashObject = transform.GetChild(2).transform.GetChild(0).gameObject;
        slashAnimator = slashObject.GetComponent<Animator>();
        stabObject = transform.GetChild(2).transform.GetChild(1).gameObject;
        stabAnimator = stabObject.GetComponent<Animator>();
        crushObject = transform.GetChild(2).transform.GetChild(2).gameObject;
        crushAnimator = crushObject.GetComponent<Animator>();
        body = transform.GetChild(1).gameObject;
        torsoObject = body.transform.GetChild(0).gameObject;
        headObject = body.transform.GetChild(1).gameObject;
        armLeftObject = body.transform.GetChild(4).gameObject;
        armRightObject = body.transform.GetChild(5).gameObject;
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Speed", CalculateSpeed());

        animator.SetBool("Charging Left", chargingLeft);
        animator.SetBool("Charging Right", chargingRight);
        animator.SetBool("Charging Up", chargingUp);
        animator.SetBool("Charging Down", chargingDown);

        if (!isMyPlayer && !isCharging)
        {
            TurnPlayerBasedOnMovement();
        }

        lastPosition = transform.position;
    }

    private float CalculateSpeed()
    {
        if (lastPosition == null) return 0f;
        else return Vector2.Distance(transform.position, lastPosition) / Time.deltaTime;
    }

    private void TurnPlayerBasedOnMovement()
    {
        Vector2 currentPositon = transform.position;

        if (currentPositon.x + 0.01f < lastPosition.x) TurnPlayerLeft();
        if (currentPositon.x - 0.01f > lastPosition.x) TurnPlayerRight();
        if (currentPositon.y - 0.01f > lastPosition.y) TurnPlayerUpwards();
        if (currentPositon.y + 0.01f < lastPosition.y) TurnPlayerDownwards();
    }

    public void TurnPlayerBasedOnInputs(bool[] inputs)
    {
        bool up = inputs[0];
        bool down = inputs[2];
        bool left = inputs[1];
        bool right = inputs[3];

        if (up) TurnPlayerUpwards();
        else if (down) TurnPlayerDownwards();
        else TurnPlayerDownwards();

        if (right) TurnPlayerRight();
        else if (left) TurnPlayerLeft();
    }

    public void TurnPlayerLeft()
    {
        body.transform.localScale = new Vector3(-BODY_SCALE, BODY_SCALE, 1);
    }

    public void TurnPlayerRight()
    {
        body.transform.localScale = new Vector3(BODY_SCALE, BODY_SCALE, 1);
    }

    public void TurnPlayerUpwards()
    {
        headObject.GetComponent<SpriteRenderer>().sprite = headBack;
        torsoObject.GetComponent<SpriteRenderer>().sprite = torsoBack;
        armLeftObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    public void TurnPlayerDownwards()
    {
        headObject.GetComponent<SpriteRenderer>().sprite = headFront;
        torsoObject.GetComponent<SpriteRenderer>().sprite = torsoFront;
        armLeftObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void MeleeCharge(MeleeAttackTypes type, Vector2 direction)
    {
        bool up = isDirectionUp(direction);
        bool right = isDirectionRight(direction);

        if (up) TurnPlayerUpwards();
        else TurnPlayerDownwards();

        if (right) TurnPlayerRight();
        else TurnPlayerLeft();

        if (type == MeleeAttackTypes.Up) chargingUp = true;

        if (type == MeleeAttackTypes.Down)
        {
            chargingDown = true;
            if (up) armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            else armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        if (type == MeleeAttackTypes.Left)
        {
            if (up)
            {
                if (right)
                {
                    chargingRight = true;
                    armRightObject.GetComponent<SpriteRenderer>().sortingOrder  = 0;
                }
                else chargingLeft = true;
            } 
            else
            {
                if (right) chargingLeft = true;
                else 
                {
                    armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    chargingRight = true; 
                }
            }
        }

        if (type == MeleeAttackTypes.Right)
        {
            if (up)
            {
                if (right) chargingLeft = true;
                else
                {
                    chargingRight = true;
                    armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }
            }
            else
            {
                if (right)
                {
                    armRightObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    chargingRight = true;
                }
                else chargingLeft = true;
            }
        }

        isCharging = true;
    }

    public void ExecuteMeleeAttack(MeleeAttackTypes type, Vector2 direction)
    {
        chargingLeft = false;
        chargingRight = false;
        chargingUp = false;
        chargingDown = false;
        isCharging = false;
        TurnPlayerDownwards();

        hit.transform.right = direction;

        if (type == MeleeAttackTypes.Left)
        {
            slashObject.transform.localScale = new Vector3(ATTACK_SPRITE_SCALE, ATTACK_SPRITE_SCALE, 1);
            Vector3 currentAngle = slashObject.transform.localEulerAngles;
            slashObject.transform.localEulerAngles = new Vector3(currentAngle.x, currentAngle.y, -45);
            slashAnimator.SetTrigger("Execute");
        } 
        else if (type == MeleeAttackTypes.Right)
        {
            slashObject.transform.localScale = new Vector3(ATTACK_SPRITE_SCALE, -ATTACK_SPRITE_SCALE, 1);
            Vector3 currentAngle = slashObject.transform.localEulerAngles;
            slashObject.transform.localEulerAngles = new Vector3(currentAngle.x, currentAngle.y, 45);
            slashAnimator.SetTrigger("Execute");
        } 
        else if (type == MeleeAttackTypes.Down)
        {
            stabAnimator.SetTrigger("Execute");
        }
        else if (type == MeleeAttackTypes.Up)
        {
            crushObject.transform.eulerAngles = new Vector3(0, 0, 0);
            crushAnimator.SetTrigger("Execute");
        }
    }

    public bool isDirectionUp(Vector2 direction)
    {
        return direction.y > 0;
    }

    public bool isDirectionRight(Vector2 direction)
    {
        return direction.x > 0;
    }
}
