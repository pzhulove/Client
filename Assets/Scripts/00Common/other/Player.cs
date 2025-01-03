using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public enum PlayerAxis
    {
        Null,
        Right,
        Left,
        Up,
        Down,
    }

    public enum PlayerOrient
    {
        Forward,
        Idle,
        Back,
    }

    public float WalkSpeed = 5;
    public float RunSpeed = 10;

    public Animator Anim;
    public Vector3 ForwardScale = Vector3.one;
    public Vector3 BackScale = Vector3.one;

    float mForward;
    float mRight;

    public float Speed { get; set; }

    private PlayerAxis mLastAxis = 0;
    private float mLastAxisTime = 0.1F;
    public float AxisSensitivity = 0.1F;

    private float mRunParameter = 0.5f;

    private PlayerOrient mPlayerOrient;
    public bool IsForward { get; set; }

    void Start()
    {
        mPlayerOrient = PlayerOrient.Idle;
        IsForward = true;

        StartCoroutine("SpecialIdle");
    }

    IEnumerator SpecialIdle()
    {
        float t = 2;
        while (true)
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !Anim.IsInTransition(0))
            {
                t -= Time.deltaTime;
                if (t <= 0)
                {
                    Anim.SetBool("SpecialIdle", true);
                    t = Random.Range(2F, 4F);
                }
            }
            else
            {
                Anim.SetBool("SpecialIdle", false);
            }
            yield return null;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        CharacterController controller = this.GetComponent<CharacterController>();

        mLastAxisTime -= Time.deltaTime;
        if (mLastAxisTime <= 0)
            mLastAxis = PlayerAxis.Null;

        PlayerAxis pa = AxisButton();
        if (pa != PlayerAxis.Null)
        {
            if (mLastAxis == pa)
            {
                mRunParameter = 1F;
            }
            mLastAxisTime = AxisSensitivity;
            mLastAxis = pa;
        }

        bool attacking = ((Anim.GetCurrentAnimatorStateInfo(0).IsName("Kick") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("KickInOut"))
         || ((Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("AttackInOut"))
         || ((Anim.GetCurrentAnimatorStateInfo(0).IsName("AttackLoop") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("AttackLoopInOut"))
         || ((Anim.GetCurrentAnimatorStateInfo(0).IsName("SpecialAttack") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("SpecialAttackInOut"));
        
        bool jumpingBack = (Anim.GetCurrentAnimatorStateInfo(0).IsName("JumpBack") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("JumpBackInOut");

        bool jumping = (Anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("JumpInOut");

        bool shoveling = (Anim.GetCurrentAnimatorStateInfo(0).IsName("Shovel") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("ShovelInOut");

        bool walkShooting = (Anim.GetCurrentAnimatorStateInfo(1).IsName("WalkShoot") && !Anim.IsInTransition(1)) || Anim.GetAnimatorTransitionInfo(1).IsUserName("WalkShootInOut");

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) <= 0.1F && Mathf.Abs(Input.GetAxisRaw("Vertical")) <= 0.1F && !jumping && !shoveling)
            mRunParameter = 0.5F;

        if (attacking)
        {
            mForward = 0;
            mRight = 0;
            Speed = WalkSpeed;
            Anim.SetFloat("Run", 0);

            if ((Anim.GetCurrentAnimatorStateInfo(0).IsName("AttackLoop") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("AttackLoopInOut"))
            {
                if (!Input.GetKey(KeyCode.J))
                {
                    Anim.SetBool("AttackLoop", false);
                }
            }
            else if ((Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !Anim.IsInTransition(0)) || Anim.GetAnimatorTransitionInfo(0).IsUserName("AttackInOut"))
            {
                if (!Input.GetKey(KeyCode.J))
                {
                    Anim.SetBool("AttackLoop", false);
                }
            }
        }
        else if (jumpingBack)
        {
            mForward = -1;
            mRight = 0;
            Speed = WalkSpeed;
            Anim.SetFloat("Run", 0);
        }
        else
        {
            if (jumping || shoveling)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) <= 0.1F && Mathf.Abs(Input.GetAxisRaw("Vertical")) <= 0.1F)
                {
                    Anim.SetFloat("Run", 0);
                }
                else
                {
                    Anim.SetFloat("Run", mRunParameter);
                }
            }
            else
            {
                mForward = Input.GetAxisRaw("Horizontal");
                mRight = -Input.GetAxisRaw("Vertical");
                if (Mathf.Abs(mForward) > 0.1F || Mathf.Abs(mRight) > 0.1F)
                {
                    Anim.SetFloat("Run", mRunParameter);
                }
                else
                {
                    Anim.SetFloat("Run", 0);
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    Anim.SetTrigger("Jump");
                    Anim.SetFloat("JumpAttack", 0);
                }
                else if (Input.GetKey(KeyCode.M))
                {
                    Anim.SetTrigger("Jump");
                    Anim.SetFloat("JumpAttack", 1);
                }
                else if (SpecialSkill())
                {
                }
                else if (Input.GetKey(KeyCode.Z))
                {
                    Anim.SetTrigger("JumpBack");
                }
                else if (Input.GetKey(KeyCode.U))
                {
                    Anim.SetTrigger("Kick");
                    Anim.SetFloat("KickType", 0);
                }
                else if (Input.GetKey(KeyCode.I))
                {
                    Anim.SetTrigger("Kick");
                    Anim.SetFloat("KickType", 1);
                }
                else if (Input.GetKey(KeyCode.J))
                {
                    if (!walkShooting)
                    {
                        Anim.SetTrigger("Attack");
                        Anim.SetBool("AttackLoop", true);
                        Anim.SetBool("WalkShootLoop", true);
                    }
                }
            }

            Speed = mRunParameter > 0.7F ? RunSpeed : WalkSpeed;
        }

        if (walkShooting)
        {
            if (!Input.GetKey(KeyCode.J) || Anim.GetFloat("Run") < 0.1F || Anim.GetFloat("Run") > 0.8F)
            {
                Anim.SetBool("WalkShootLoop", false);
            }
        }

        PlayerOrient playerOrient = CalcOrient();
        if (playerOrient != mPlayerOrient)
        {
            mPlayerOrient = playerOrient;
            ApplyOrient();
        }

        var movement = (transform.forward * mForward + transform.right * mRight) * Speed;
        movement *= Time.deltaTime;
        controller.Move(movement);
    }

    PlayerAxis AxisButton()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            float axis = Input.GetAxisRaw("Horizontal");
            if (axis > 0.5F)
                return PlayerAxis.Right;
            if (axis < -0.5F)
                return PlayerAxis.Left;
        }
        if (Input.GetButtonDown("Vertical"))
        {
            float axis = Input.GetAxisRaw("Vertical");
            if (axis > 0.5F)
                return PlayerAxis.Up;
            if (axis < -0.5F)
                return PlayerAxis.Down;
        }
        return PlayerAxis.Null;
    }

    bool SpecialSkill()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 1);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 2);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 3);
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 4);
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 5);
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 6);
        }
        else if (Input.GetKey(KeyCode.Alpha7))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 7);
        }
        else if (Input.GetKey(KeyCode.Alpha8))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 8);
        }
        else if (Input.GetKey(KeyCode.Alpha9))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 9);
        }
        else if (Input.GetKey(KeyCode.Alpha0))
        {
            Anim.SetTrigger("Special");
            Anim.SetFloat("SpecialAttack", 0);
        }
        else
        {
            return false;
        }
        return true;
    }

    PlayerOrient CalcOrient()
    {
        if (mForward > 0.1F)
            return PlayerOrient.Forward;
        else if (mForward < -0.1F)
            return PlayerOrient.Back;
        else
            return PlayerOrient.Idle;
    }

    void ApplyOrient()
    {
        if (mPlayerOrient == PlayerOrient.Forward)
        {
            Anim.transform.localScale = ForwardScale;
            IsForward = true;
        }
        else if (mPlayerOrient == PlayerOrient.Back)
        {
            Anim.transform.localScale = BackScale;
            IsForward = false;
        }
    }
}
