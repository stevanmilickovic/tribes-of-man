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

    public GameObject attackDirectionObject;

    private float speed;
    private Vector2 lastPosition;
    public bool isMyPlayer = false;

    public bool chargingLeft = false;
    public bool chargingRight = false;
    public bool chargingUp = false;
    public bool chargingDown = false;

    public bool isCharging = false;

    public GameObject hit;

    private int runBuffer = 2;

    // Start is called before the first frame update
    void Start()
    {
        //this is disgusting
        animator = transform.GetChild(1).gameObject.GetComponent<Animator>();
        slashObject = transform.GetChild(2).transform.GetChild(0).gameObject;
        slashAnimator = slashObject.GetComponent<Animator>();
        stabObject = transform.GetChild(2).transform.GetChild(1).gameObject;
        stabAnimator = stabObject.GetComponent<Animator>();
        crushObject = transform.GetChild(2).transform.GetChild(2).gameObject;
        crushAnimator = crushObject.GetComponent<Animator>();
        attackDirectionObject = transform.GetChild(2).transform.GetChild(3).gameObject;
        body = transform.GetChild(1).gameObject;
        torsoObject = body.transform.GetChild(0).gameObject;
        headObject = body.transform.GetChild(1).gameObject;
        armLeftObject = body.transform.GetChild(4).gameObject;
        armRightObject = body.transform.GetChild(5).gameObject;
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Speed", CalculateSpeed());
    /**
        animator.SetBool("Charging Left", chargingLeft);
        animator.SetBool("Charging Right", chargingRight);
        animator.SetBool("Charging Up", chargingUp);
        animator.SetBool("Charging Down", chargingDown);
    */

        if (!isMyPlayer && !isCharging)
        {
            TurnPlayerBasedOnMovement();
        }

        lastPosition = transform.position;
    }

    private float CalculateSpeed()
    {
        if (lastPosition == null) return 0f;
        float speed = Vector2.Distance(transform.position, lastPosition) / Time.deltaTime;
        if (speed == 0)
        {
            if (runBuffer != 0)
            {
                speed = 1f;
                runBuffer--;
            }
        } 
        else
        {
            runBuffer = 2;
        }
        return speed;
    }

    private void TurnPlayerBasedOnMovement()
    {
        Vector2 currentPositon = transform.position;

        if (currentPositon.x + 0.01f < lastPosition.x) TurnPlayerLeft();
        if (currentPositon.x - 0.01f > lastPosition.x) TurnPlayerRight();
    }

    public void TurnPlayerBasedOnInputs(bool[] inputs)
    {
        bool left = inputs[1];
        bool right = inputs[3];

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
        bool right = isDirectionRight(direction);

        if (right) TurnPlayerRight();
        else TurnPlayerLeft();

        chargingDown = true;

        hit.transform.right = direction;
        attackDirectionObject.GetComponent<SpriteRenderer>().sprite = TextureUtil.GetSprite("attack-direction");
        isCharging = true;
    }

    public void ExecuteRangedAttack()
    {
        isCharging = false;
        attackDirectionObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public void ExecuteMeleeAttack(MeleeAttackTypes type, Vector2 direction)
    {
        chargingLeft = false;
        chargingRight = false;
        chargingUp = false;
        chargingDown = false;
        isCharging = false;

        attackDirectionObject.GetComponent<SpriteRenderer>().sprite = null;
        hit.transform.right = direction;

        stabAnimator.SetTrigger("Execute");

        /**
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
        }*/
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
