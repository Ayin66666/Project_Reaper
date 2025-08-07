using System.Collections;
using UnityEngine;
using Easing;

public class Player_Move : MonoBehaviour
{
    public Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Player_Attack playerAttack;
    [SerializeField] private Player_Animation playerAnim;

    public Transform dashPos;
    private float dashTimer;
    private float attackMoveTimer;


    public Vector2 moveVector;
    public Vector2 dashVector;
    public Vector2 myVector;

    public RaycastHit2D dashWallCheck;
    public RaycastHit2D attackWallCheck;
    public RaycastHit2D skillWallCheck;

    private Coroutine myCoroutine;

    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        Player_Status.instance.action += CoroutineStop;
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        Jump();
        Move();

        if (Input.GetKeyDown(KeyCode.C) && Player_Status.instance.canDash)
        {
            Player_Status.instance.action?.Invoke();
            myCoroutine = StartCoroutine(Dash());
        }

        playerAnim.anim.SetBool("isGround", Player_Status.instance.isGround);

        if (rigidBody2D.velocity.y < 0)
        {
            if (rigidBody2D.velocity.y < -15)
            {
                //최대 낙하속도 조절
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, -15);
            }
            //낙하 모션
            playerAnim.anim.SetBool("isFall", true);
        }

        if (rigidBody2D.velocity.y == 0)
        {
            //애니메이션
            playerAnim.anim.SetBool("isJump", false);
            playerAnim.anim.SetBool("isDoubleJump", false);
            playerAnim.anim.SetBool("isFall", false);
        }
    }

    private void FixedUpdate()
    {
        //지면감지
        Debug.DrawRay(transform.position, Vector3.down, new Color(1, 0, 0), boxCollider.bounds.extents.y + 0.1f);
        Player_Status.instance.isGround = Physics2D.Raycast(transform.position, Vector3.down, boxCollider.bounds.extents.y + 0.1f, Player_Status.instance.groundCheck);

        //벽 감지
        Player_Status.instance.isWall = Physics2D.Raycast(transform.position, dashPos.position - transform.position, boxCollider.bounds.extents.x + 0.5f, Player_Status.instance.groundCheck);

        //대각선 감지
        Player_Status.instance.isSide = Physics2D.Raycast(transform.position, (new Vector2(transform.localScale.x, 0) + Vector2.down).normalized, 2f, Player_Status.instance.groundCheck);
    }

    void CoroutineStop()
    {
        if(myCoroutine != null)
        {
            StopCoroutine(myCoroutine);
        }
    }

    void Movement()
    {
        rigidBody2D.velocity = new Vector2(moveVector.normalized.x * Player_Status.instance.speed, rigidBody2D.velocity.y);
    }

    void ScaleControl()
    {
        //스케일을 - 해버리니 공격 콜라이더 스케일도 영향받아서 문제 발생 (충돌에 영향가는지 알아봐야함)
        if (moveVector.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (moveVector.x < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }


    void Move()
    {
        if(!Player_Status.instance.canMove)
        {
            return;
        }

        if (moveVector.magnitude > 0.1f)
        {
            playerAnim.anim.SetBool("isMove", true);

            Movement();
            ScaleControl();
        }
        else
        {
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);

            playerAnim.anim.SetBool("isMove", false);
        }
    }

    void Jump()
    {
        if(Player_Status.instance.isGround)
        {
            Player_Status.instance.jumpCount = 0;
        }

        if (!Player_Status.instance.canJump)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X) && Player_Status.instance.jumpCount < 2)
        {
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.AddForce(Vector2.up * Player_Status.instance.jumpPower, ForceMode2D.Impulse);
            Player_Status.instance.jumpCount++;
            Player_Sound.instance.SFXPlay(Player_Sound.instance.utility_Sound[2]);

            //점프 모션
            if (Player_Status.instance.jumpCount == 1)
            {
                playerAnim.anim.SetBool("isJump", true);
            }
            else if(Player_Status.instance.jumpCount == 2)
            {
                playerAnim.anim.SetBool("isDoubleJump", true);
            }
        }
    }

    IEnumerator Dash()
    {
        if (Player_Status.instance.canDash)
        {
            Debug.Log("대쉬");
            Player_Status.instance.canMove = false;
            Player_Status.instance.isDash = true;
            Player_Status.instance.isGroundAttack = false;
            Player_Status.instance.isAirAttack = false;
            Player_Status.instance.canDash = false;
            Player_Status.instance.canJump = false;
            Player_Status.instance.canAttack = false;

            rigidBody2D.velocity = Vector2.zero;
            Player_Status.instance.GravityCheck(true);

            Player_Status.instance.dashPower = 0f;
            Player_Status.instance.dashDistance = 80f;
            Player_Status.instance.dashCoolTime = 0.5f;
            dashTimer = 0.3f;

            Vector3 startPos = transform.position;
            //Vector3 endPos = dashWallCheck ? dashWallCheck.point - (transform.localScale.x > 0 ? new Vector2(capsuleCollider.bounds.extents.x, 0) : new Vector2(-capsuleCollider.bounds.extents.x, 0)) : dashPos.position;
            Vector3 endPos = dashPos.position;

            Player_Sound.instance.SFXPlay(Player_Sound.instance.utility_Sound[1]);

            while (dashTimer > 0 && !Player_Status.instance.isWall)
            {
                dashTimer -= Time.deltaTime;

                Player_Status.instance.dashPower += 3f * Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(Player_Status.instance.dashPower));
                yield return null;
            }
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);

            Player_Status.instance.isDash = false;
            Player_Status.instance.isGroundAttack = false;
            Player_Status.instance.isSkill = false;
            Player_Status.instance.isAirAttack = false;
            Player_Status.instance.canMove = true;
            Player_Status.instance.canDash = true;
            Player_Status.instance.canJump = true;
            Player_Status.instance.canAttack = true;
            Player_Status.instance.canNextAttack = true;

            Player_Status.instance.GravityCheck(false);
            playerAttack.curComboCount = 0;
        }
    }

    public void Call_AttackMovement()
    {
        StartCoroutine(AttackMovement());
    }

    public IEnumerator AttackMovement()
    {
        Debug.Log("공격 이동");
        Player_Status.instance.attackMovePower = 0f;
        attackMoveTimer = 0.2f;

        Vector2 colliderLength = transform.localScale.x == 1 ? new Vector2(boxCollider.bounds.extents.x, 0) : new Vector2(-boxCollider.bounds.extents.x, 0);
        Vector3 moveDir = transform.localScale.x == 1 ? new Vector3(0.8f, 0) : new Vector3(-0.8f, 0);
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + moveDir;

        while (attackMoveTimer > 0 && !Player_Status.instance.isWall)
        {
            attackMoveTimer -= Time.deltaTime;
            Player_Status.instance.attackMovePower += 5f * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(Player_Status.instance.attackMovePower));

            yield return null;
        }
    }
}
