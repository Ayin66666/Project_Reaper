using System.Collections;
using UnityEngine;

public class Enemy_Boss_Stage3_Part : Enemy_Base
{
    // 부속품 공격
    // Laser180 : (좌 -> 우 / 우 -> 좌) 로 레이져 공격
    // ChaseShot : 플레이어 방향으로 n초간 탄막 발사
    // SwingLR : 좌 우 휘둘기 공격
    // Smash : N초간 플레이어 위치 추적 후 내려찍기 + 좌우 폭팔

    // 이동 관련 이징함수 전부 대충 박아넣은거라 나중에 그래프보고 조절해야함!

    [Header("---Component---")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private TrailRenderer trail;

    [Header("Part State")]
    public bool isActivate;
    public bool isPartAction;
    [SerializeField] private Enemy_Boss_Stage3 boss;
    [SerializeField] private bool isMove;
    private Vector3 movePos;
    private Coroutine partCoroutine;

    public enum Attack { None, Laser180, ChaseShot, SwingLR, Smash }

    [Header("---Attack Collider---")]
    [SerializeField] private GameObject swingLRCollider;
    [SerializeField] private GameObject smashCollider;
    [SerializeField] private GameObject[] laserCollider;

    [Header("---Prefabs---")]
    [SerializeField] private GameObject[] bullet;
    [SerializeField] private GameObject smashExplosion;
    [SerializeField] private GameObject dieEffect;

    [Header("---Move Pos---")]
    [SerializeField] private GameObject lookTarget;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform shotPos;
    [SerializeField] private Transform[] swingLRPos;
    [SerializeField] private Transform[] laserLRPos;

    [Header("---Explosion Pos---")]
    [SerializeField] private Transform[] smashExPosL;
    [SerializeField] private Transform[] smashExPosR;

    public void ReSpawn()
    {
        Spawn();
    }

    public void Destory()
    {
        Die();
    }

    public void PartAttack(Attack attack, Vector3 movePos)
    {
        if (isActivate == true)
        {
            Target_Setting();
            partCoroutine = StartCoroutine(Action(attack, movePos));
        }
        else
        {
            return;
        }
    }

    private IEnumerator Action(Attack attack, Vector3 movePos)
    {
        isPartAction = true;
        this.movePos = movePos;
        partCoroutine = StartCoroutine(Move());

        // Move Wait
        while(isMove)
        {
            yield return null;
        }

        // Attack
        switch (attack)
        {
            case Attack.Laser180:
                partCoroutine = StartCoroutine(Laser180());
                break;

            case Attack.ChaseShot:
                partCoroutine = StartCoroutine(ChaseShot());
                break;

            case Attack.SwingLR:
                partCoroutine = StartCoroutine(SwingLR());
                break;

            case Attack.Smash:
                partCoroutine = StartCoroutine(Smash());
                break;
        }
    }

    private IEnumerator Move()
    {
        state = State.Move;
        isMove = true;

        Vector3 startPos = transform.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, movePos, Easing.EasingFunctions.InOutCubic(timer));
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.15f);
        state = State.Idle;
        isMove = false;
    } // End

    private IEnumerator Laser180()
    {
        state = State.Attack;
        isAttack = true;

        // Physics2D.OverlapBox() 랑 레이케스트 둘다 쓰면 될듯?
        // 레이져 크기는 사이즈 vecteor2 함수에서 조절하는데 길이만 내가 레이케스트로 잡아주고
        // 두깨는 임의 지정하면 원하는 모양이 나올듯

        // Attack
        int attactPos = Random.Range(0, 2);
        laserCollider[boss.isPhase2 ? 1 : 0].transform.rotation = (attactPos == 0) ? Quaternion.Euler(new Vector3(0, 0, -120)) :Quaternion.Euler(new Vector3(0, 0, 120));
        laserCollider[boss.isPhase2 ? 1 : 0].SetActive(true);
        Quaternion startRot = laserCollider[boss.isPhase2 ? 1 : 0].transform.rotation;
        Quaternion endRot = (attactPos == 0) ? Quaternion.Euler(new Vector3(0, 0, 120)) : Quaternion.Euler(new Vector3(0, 0, -120));

        float rotSpeed = boss.isPhase2 ? 1.5f : 3f;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / rotSpeed;

            laserCollider[boss.isPhase2 ? 1 : 0].transform.rotation = Quaternion.Lerp(startRot, endRot, Easing.EasingFunctions.InOutCubic(timer));

            yield return null;
        }
        laserCollider[boss.isPhase2 ? 1 : 0].SetActive(false);

        // End Delay ?
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isPartAction = false;
    } // End

    private IEnumerator ChaseShot()
    {
        state = State.Attack;
        isAttack = true;

        // Attack
        Target_Setting();
        Vector3 shotDir;
        int shotType = Random.Range(0, 2);
        int shotCount = Random.Range(5, 10);
        for (int i = 0; i < shotCount; i++)
        {
            shotDir = (curTarget.transform.position - shotPos.position).normalized;
            GameObject obj = Instantiate(bullet[shotType == 0 ? 0 : 1], shotPos.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting((shotType == 0 ? Enemy_Bullet.BulletType.Red : Enemy_Bullet.BulletType.Blue), shotDir, 10, 30, 15);

            // Shot Delay
            yield return new WaitForSeconds(boss.isPhase2 ? Random.Range(0.1f, 0.15f) : Random.Range(0.1f, 0.15f));
        }

        // End Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isPartAction = false;
    } // End

    private IEnumerator SwingLR()
    {
        state = State.Attack;
        isAttack = true;

        // Move to Attack Pos
        Vector3 startPos = transform.position;
        int ran = Random.Range(0, 2);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, (ran == 0 ? swingLRPos[0].position : swingLRPos[1].position), Easing.EasingFunctions.InOutQuart(timer));
            yield return null;
        }

        // Delay F
        yield return new WaitForSeconds(boss.isPhase2 ? 0.5f : 0.3f);

        // Attack
        swingLRCollider.SetActive(true);
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = 
                Vector3.Lerp((ran == 0 ? swingLRPos[0].position : swingLRPos[1].position), 
                (ran == 0 ? swingLRPos[1].position : swingLRPos[0].position), Easing.EasingFunctions.InOutQuart(timer));
            yield return null;
        }
        swingLRCollider.SetActive(false);

        // End Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isPartAction = false;
    } // End

    private IEnumerator Smash()
    {
        state = State.Attack;
        isAttack = true;
        Vector3 targetPos;


        // Chase
        Vector3 chasePos = new Vector3(curTarget.transform.position.x, 0, 0);
        Vector3 startPos = transform.position;
        float timer = 0f;
        float updateTimer = 0;
        float dir = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / (boss.isPhase2 ? 1.5f : 2f);

            // Chase Pos Update
            updateTimer += Time.deltaTime;
            if (updateTimer >= 0.15f)
            {
                startPos = transform.position;
                chasePos = new Vector3(curTarget.transform.position.x, 5, 0);
                updateTimer = 0;
                dir = 0;
            }

            if (dir < 1)
            {
                dir += Time.deltaTime * (boss.isPhase2 ? 1.2f : 1f);
            }

            // Chase
            transform.position = Vector3.Lerp(startPos, chasePos, dir);

            yield return null;
        }


        /*
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / (boss.isPhase2 ? 1.5f : 2f);
            targetPos = new Vector3(curTarget.transform.position.x, 5, 0);
            transform.position = Vector3.Lerp(transform.position, targetPos, timer);
            yield return null;
        }
        */


        // Attack
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 15f, groundLayer);
        targetPos = hit.point;
        smashCollider.SetActive(true);

        startPos = transform.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / (boss.isPhase2 ? 1.5f : 1f);
            transform.position = Vector3.Lerp(startPos, targetPos, Easing.EasingFunctions.InElastic(timer));
            yield return null;
        }
        smashCollider.SetActive(false);

        // Smash Explosion
        for (int i = 0; i < smashExPosL.Length; i++)
        {
            Instantiate(smashExplosion, smashExPosL[i].position, Quaternion.identity);
            Instantiate(smashExplosion, smashExPosR[i].position, Quaternion.identity);
            yield return new WaitForSeconds(boss.isPhase2 ? 0.06f : 0.08f);
        }

        // Move Up
        startPos = transform.position;
        Vector3 endPos = spawnPos.position;
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.InOutQuart(timer));
            yield return null;
        }

        // End Delay ?
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isPartAction = false;
    } // End

    public void Respawn()
    {
        if(!isActivate)
        {
            Spawn();
        }
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        isInvincibility = true;
        trail.enabled = true;

        // Move
        Vector3 startPos = transform.position;
        Vector3 endPos = spawnPos.position;
        float timer = 0;
        while(timer < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutSine(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        
        // Delay
        yield return new WaitForSeconds(0.1f);

        Status_Setting();
        state = State.Idle;
        trail.enabled = false;

        isHit = false;
        isInvincibility = false;
        isPartAction = false;
        isActivate = true;
        isMove = false;
    }

    protected override void Stagger()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        StopCoroutine(partCoroutine);
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        state = State.Die;
        isDie = true;
        isPartAction = false;
        isActivate = false;
        isMove = false;

        // Die Effect
        dieEffect.SetActive(true);

        // Move
        Vector3 startPos = transform.position;
        Vector3 endPos = Physics2D.Raycast(transform.position, Vector3.down, 100, groundLayer).point;
        float timer = 0;
        while(timer < 1) 
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, Easing.EasingFunctions.OutElastic(timer));
            yield return null;
        }
    }
}
