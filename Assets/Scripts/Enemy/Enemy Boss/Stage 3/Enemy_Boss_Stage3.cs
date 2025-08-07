using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;
using System.Net.NetworkInformation;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using Unity.VisualScripting;

public class Enemy_Boss_Stage3 : Enemy_Base
{
    [Header("---Stage 3 Boss Setting---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject[] handObj;
    [SerializeField] private Enemy_Boss_Stage3_Part[] parts;
    [SerializeField] private Collider2D moveCollider;
    [SerializeField] private GameObject container;
    public bool isPhase2;

    [SerializeField] private int count;
    [SerializeField] private float platformTimer;
    [SerializeField] private float partRespawnTimerR;
    [SerializeField] private float partRespawnTimerL;
    [SerializeField] private PlatformType curPlatformType;

    private Coroutine platformCoroutine;
    private enum PlatformType { TypeA, TypeB, TypeC }

    [Header("---Move Pos---")]
    [SerializeField] private Transform[] laserAOEPos;
    [SerializeField] private Transform safeZonePos;


    [Header("---Prefabs & Object---")]
    [SerializeField] private GameObject[] bullet;
    [SerializeField] private GameObject[] centerExplosionPrefab;
    [SerializeField] private GameObject[] platformObjectA;
    [SerializeField] private GameObject[] platformObjectB;
    [SerializeField] private GameObject[] platformObjectC;


    [Header("---Effect---")]
    [SerializeField] private GameObject spewnEffect;
    [SerializeField] private GameObject[] dieEffect;

    [SerializeField] private GameObject centerChargeEffect;
    [SerializeField] private GameObject laser360ChargeEffect;
    [SerializeField] private GameObject chaseWarringEffect;
    [SerializeField] private GameObject[] aoeWarringEffect;


    [Header("---Explosion Pos---")]
    [SerializeField] private Transform[] centerExlposionPosL;
    [SerializeField] private Transform[] centerExlposionPosR;
    [SerializeField] private Transform[] laser360ExplosionPos;


    [Header("---Attack Collider---")]
    [SerializeField] private GameObject[] laserAOECollider;
    [SerializeField] private GameObject centerLaserCollider;
    [SerializeField] private GameObject laser360Collider;
    [SerializeField] private GameObject chaseLaserCollider;

    // AOE Attack
    [SerializeField] private GameObject aoeChargeEffect;
    [SerializeField] private GameObject aoeMainCollider;
    [SerializeField] private GameObject[] aoeLaesrBCollider;
    [SerializeField] private GameObject[] aoeLaesrWCollider;

    // Ÿ���� + �μ�ǰ 2�� ����
    // �μ�ǰ �ı� �� �����Ⱓ ��ü ���� ����
    // 2�� ������ ���� (1,2 ������)
    // ������ �� ��ü ���� ���� �ð� ����

    // ��ü ����
    // LaserAoE : �� �� ������ �̵� ����
    // Laser360 : ������ 360�� 1ȸ��  + �ٴ� ����
    // CenterExlposion : �߾� ������ ���� �� �¿� ����
    // ChaseLaser : N�ʰ� �÷��̾ õõ�� �����ϴ� ������ ���� + N2�ʸ��� ������ ���� ���� ����

    // (�÷��� on/off)(�нú� ?)

    // �μ�ǰ ����
    // Laser180 : (�� -> �� / �� -> ��) �� ������ ����
    // ChaseShot : �÷��̾� �������� n�ʰ� ź�� �߻�
    // SwingLR : �� �� �ֵѱ� ����
    // Smash : N�ʰ� �÷��̾� ��ġ ���� �� ������� + �¿� ����

    // �ʻ��
    // AOE : �������� ���� -> N�ʰ� ���� ���� -> �������� �̵� -> �������� �ۿ����� ���� ������
    // +A �� ���� ���� ���𰡰� �־�� �Ұ� ������

    // ������ ��ȯ
    // 2������ ��ȯ -> �ٴ� �ı� / ���� ���� ����
    // 1������ -> �μ�ǰ 1�� / 2������ -> �μ�ǰ 2�� ����

    // Sound Index
    // 0 : LaserAoE, Laser360, ChaseLaser
    // 1 : CenterExlposion
    // 2 : Phase 2
    // 3 : AOE

    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if (state == State.Spawn || state == State.Die)
        {
            return;
        }

        // Part & Platform Timer
        Timer();

        // Platform Setting
        if (platformTimer <= 0)
        {
             platformCoroutine = StartCoroutine(PlatformSetting(true));
        }

        // Phase 2 Check
        if (!isPhase2 && hp <= 500)
        {
            StartCoroutine(Phase2On());
        }

        // Find Target & Reset Enemy
        if (!haveTarget)
        {
            Target_Setting();
            if (!haveTarget && state != State.Await)
            {
                state = State.Await;
            }
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(AOE2());
        }

        // Think
        if (state == State.Idle && !isAttack && !isDie)
        {
            Think();
        }
        else
        {
            return;
        }
    }

    private void Think()
    {
        state = State.Think;

        if(count >= 5)
        {
            StartCoroutine(AOE());
        }
        else
        {
            // Attack Setting
            int ran = Random.Range(0, 100);
            if (ran <= 25)
            {
                StartCoroutine(LaserAOE());
            }
            else if (ran <= 50)
            {
                StartCoroutine(Laser360());
            }
            else if (ran <= 75)
            {
                StartCoroutine(CenterExlposion());
            }
            else
            {
                StartCoroutine(ChaseLaser());
            }
        }
    }

    private void Timer()
    {
        // Platform Timer
        if(platformTimer > 0)
            platformTimer -= Time.deltaTime;

        // Part Timer
        if (partRespawnTimerL > 0)
            partRespawnTimerL -= Time.deltaTime;

        if (partRespawnTimerR > 0)
            partRespawnTimerR -= Time.deltaTime;
    }

    private IEnumerator Phase2On()
    {
        state = State.Spawn;
        isAttack = true;
        isPhase2 = true;

        // Sound On
        sound.SoundPlay_Other(5);

        // Effect -> ȭ�� ��鸲 + A?
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Part Spawn
        PartSpawn();

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    }

    private void PartSpawn()
    {
        if(isPhase2)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (!parts[i].isActivate)
                {
                    parts[i].ReSpawn();
                }
            }
        }
        else
        {
            if(parts[0].isActivate == false)
            {
                parts[0].ReSpawn();
            }
        }
    }

    private void PartOrder(Enemy_Boss_Stage3_Part.Attack attackType, int part)
    {
        if (parts[part].isActivate && !parts[part].isPartAction)
        {
            switch (attackType)
            {
                case Enemy_Boss_Stage3_Part.Attack.Laser180:
                    parts[part].PartAttack(Enemy_Boss_Stage3_Part.Attack.Laser180, PartMovePos());
                    break;

                case Enemy_Boss_Stage3_Part.Attack.ChaseShot:
                    parts[part].PartAttack(Enemy_Boss_Stage3_Part.Attack.ChaseShot, PartMovePos());
                    break;

                case Enemy_Boss_Stage3_Part.Attack.SwingLR:
                    parts[part].PartAttack(Enemy_Boss_Stage3_Part.Attack.SwingLR, PartMovePos());
                    break;

                case Enemy_Boss_Stage3_Part.Attack.Smash:
                    parts[part].PartAttack(Enemy_Boss_Stage3_Part.Attack.Smash, PartMovePos());
                    break;
            }
        }
    }

    private Vector2 PartMovePos()
    {
        float x = moveCollider.bounds.size.x;
        float y = moveCollider.bounds.size.y;

        x = Random.Range((x / 2) * -1, x / 2);
        y = Random.Range((y / 2) * -1, y / 2);
        Debug.Log("x : " + x + "y : " + y);

        Vector2 moveDir = new(x + moveCollider.transform.position.x, y + moveCollider.transform.position.y);
        Debug.Log(moveDir);
        return moveDir;
    }

    private IEnumerator PlatformSetting(bool OnOff)
    {
        if(OnOff)
        {
            // Platform Setting
            switch (curPlatformType)
            {
                case PlatformType.TypeA:
                    // Platform Off
                    for (int i = 0; i < platformObjectA.Length; i++)
                    {
                        platformObjectA[i].SetActive(false);
                    }

                    // Delay
                    yield return new WaitForSeconds(0.25f);

                    // Platform On
                    for (int i = 0; i < platformObjectB.Length; i++)
                    {
                        platformObjectB[i].SetActive(true);
                    }

                    curPlatformType = PlatformType.TypeB;
                    break;

                case PlatformType.TypeB:
                    // Platform Off
                    for (int i = 0; i < platformObjectB.Length; i++)
                    {
                        platformObjectB[i].SetActive(false);
                    }

                    // Delay
                    yield return new WaitForSeconds(0.25f);

                    // Platform On
                    for (int i = 0; i < platformObjectC.Length; i++)
                    {
                        platformObjectC[i].SetActive(true);
                    }

                    curPlatformType = PlatformType.TypeB;
                    break;

                case PlatformType.TypeC:
                    // Platform Off
                    for (int i = 0; i < platformObjectC.Length; i++)
                    {
                        platformObjectC[i].SetActive(false);
                    }

                    // Delay
                    yield return new WaitForSeconds(0.25f);

                    // Platform On
                    for (int i = 0; i < platformObjectA.Length; i++)
                    {
                        platformObjectA[i].SetActive(true);
                    }

                    curPlatformType = PlatformType.TypeB;
                    break;
            }

            // Timer Reset
            platformTimer = 10f;
        }
        else
        {
            // Platform Off
            for (int i = 0; i < platformObjectA.Length; i++)
            {
                platformObjectA[i].SetActive(false);
            }

            for (int i = 0; i < platformObjectB.Length; i++)
            {
                platformObjectB[i].SetActive(false);
            }

            for (int i = 0; i < platformObjectC.Length; i++)
            {
                platformObjectC[i].SetActive(false);
            }
        }
    } // End

    private IEnumerator LaserAOE()
    {
        // LaserAoE : �� �� ������ �̵� ����
        state = State.Attack;
        isAttack = true;
        count++;

        // Laser On
        for (int i = 0; i < laserAOECollider.Length; i++)
        {
            laserAOECollider[i].SetActive(true);
            laserAOECollider[i].transform.position = laserAOEPos[i].position;
        }

        // Part Attack
        if(isPhase2)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                int ran = Random.Range(0, 100);
                Debug.Log("A" + ran);
                PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.Smash : Enemy_Boss_Stage3_Part.Attack.ChaseShot, i);
            }
        }
        else
        {

            int ran = Random.Range(0, 100);
            Debug.Log("B" + ran);
            PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.Smash : Enemy_Boss_Stage3_Part.Attack.ChaseShot, 0);
        }

        // Delay
        yield return new WaitForSeconds(isPhase2 ? 2f : 1f);
        Debug.Log("C");

        // Sound On
        sound.SoundPlay_Other(0);

        // Laser Move
        Vector3 laserLStartPos = laserAOECollider[0].transform.position;
        Vector3 laserRStartPos = laserAOECollider[1].transform.position;
        float timer = 0f;
        while(timer < 1)
        {
            timer += Time.deltaTime / (isPhase2 ? 1.5f : 2f);
            laserAOECollider[0].transform.position = Vector3.Lerp(laserLStartPos, laserAOEPos[1].position, EasingFunctions.InOutCubic(timer));
            laserAOECollider[1].transform.position = Vector3.Lerp(laserRStartPos, laserAOEPos[0].position, EasingFunctions.InOutCubic(timer));
            yield return null;
        }

        // Off Delay
        yield return new WaitForSeconds(0.2f);

        // Laser Off
        for (int i = 0; i < laserAOECollider.Length; i++)
        {
            laserAOECollider[i].SetActive(false);
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    } // End

    private IEnumerator Laser360()
    {
        state = State.Attack;
        isAttack = true;

        // Rotate Setting -> �̻��
        int ran = Random.Range(0, 2);
        bool isRight = ran == 0 ? true : false;

        // Laser On + Deley F + Laser Rotation Reset
        laser360Collider.SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 0.15f : 0.25f);

        // Part Attack
        if (isPhase2)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                ran = Random.Range(0, 100);
                PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.ChaseShot : Enemy_Boss_Stage3_Part.Attack.SwingLR, i);
            }
        }
        else
        {
            ran = Random.Range(0, 100);
            PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.ChaseShot : Enemy_Boss_Stage3_Part.Attack.SwingLR, 0);
        }

        // Sound On
        sound.SoundPlay_Other(0);

        // Rotate + Attack
        Vector3 startRotation = laser360Collider.transform.eulerAngles;
        Vector3 targetRotation = startRotation + new Vector3(0f, 0f, 360f);

        float rotationDuration = isPhase2 ? 1 : 1.5f; // ȸ���ϴ� �� �ɸ��� �ð� (��)
        float elapsed = 0f; // ����� �ð��� �����մϴ�.

        while (elapsed < rotationDuration)
        {
            // 0���� 1�� ����ȭ�� �ð��� ����մϴ�.
            float t = Mathf.Clamp01(elapsed / rotationDuration);

            // ������ ����Ͽ� ����ȭ�� �ð��� ���� ȸ������ ����մϴ�.
            float curveValue = Mathf.SmoothStep(0f, 1f, t);
            float rotationAmount = 360f * curveValue;

            // ���� ������������ ��ǥ �������� �����Ͽ� ȸ���մϴ�.
            Vector3 currentRotation = Vector3.Lerp(startRotation, targetRotation, EasingFunctions.InOutQuart(t));
            laser360Collider.transform.eulerAngles = currentRotation;

            // ��� �ð��� ������Ʈ�մϴ�.
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Explosion
        ran = Random.Range(0, centerExplosionPrefab.Length);
        for (int i = 0; i < laser360ExplosionPos.Length; i++)
        {
            Instantiate(centerExplosionPrefab[ran], laser360ExplosionPos[i].position, Quaternion.identity);
            yield return new WaitForSeconds(isPhase2 ? 0.0015f : 0.0025f);
        }

        // Delay M
        yield return new WaitForSeconds(0.2f);

        // Laser Off
        laser360Collider.SetActive(false);

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    } // End

    private IEnumerator CenterExlposion() 
    {
        state = State.Attack;
        isAttack = true;

        // Sound On
        sound.SoundPlay_Other(1);

        // Charge
        centerChargeEffect.SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1f : 1.5f);
        centerChargeEffect.SetActive(false); // -> ���߿� ����Ʈ�� �ڵ����� ������ �� ��!

        // Part Attack
        if (isPhase2)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                PartOrder(Enemy_Boss_Stage3_Part.Attack.Laser180, i);
            }
        }
        else
        {
            PartOrder(Enemy_Boss_Stage3_Part.Attack.Laser180, 0);
        }

        // Delay F
        yield return new WaitForSeconds(isPhase2 ? 0.2f : 0.3f);

        // Sound On
        sound.SoundPlay_Other(2);

        // Attack
        centerLaserCollider.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        centerLaserCollider.SetActive(false);

        // Ground Explosion
        int ran = Random.Range(0, centerExplosionPrefab.Length);
        for (int i = 0; i < centerExlposionPosL.Length; i++)
        {
            Instantiate(centerExplosionPrefab[ran], centerExlposionPosL[i].transform.position, Quaternion.identity);
            Instantiate(centerExplosionPrefab[ran], centerExlposionPosR[i].transform.position, Quaternion.identity);
            
            // Explosion Delay
            yield return new WaitForSeconds(isPhase2 ? 0.2f : 0.25f);
        }

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    } // End

    private IEnumerator ChaseLaser()
    {
        state = State.Attack;
        isAttack = true;

        // Warring Effect
        chaseWarringEffect.transform.position = new Vector3(curTarget.transform.position.x, 0, 0);
        chaseWarringEffect.SetActive(true);
        float timer = isPhase2 ? 0.5f : 1f;
        while(timer > 0)
        {
            chaseWarringEffect.transform.position = new Vector3(curTarget.transform.position.x, 0, 0);
            timer -= Time.deltaTime;
            yield return null;
        }
        chaseWarringEffect.SetActive(false);

        // Delay F
        yield return new WaitForSeconds(isPhase2 ? 0.25f : 0.35f);

        // Part Attack
        if (isPhase2)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                int ran = Random.Range(0, 100);
                PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.ChaseShot : Enemy_Boss_Stage3_Part.Attack.Smash, i);
            }
        }
        else
        {
            int ran = Random.Range(0, 100);
            PartOrder(ran <= 50 ? Enemy_Boss_Stage3_Part.Attack.ChaseShot : Enemy_Boss_Stage3_Part.Attack.Smash, 0);
        }

        // Sound On
        sound.SoundPlay_Other(0);

        // Attack & chase
        chaseLaserCollider.transform.position = chaseWarringEffect.transform.position;
        chaseLaserCollider.SetActive(true);

        Vector3 chasePos = new Vector3(curTarget.transform.position.x, 0, 0);
        Vector3 startPos = chaseLaserCollider.transform.position;
        timer = 0f;
        float updateTimer = 0;
        float dir = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / (isPhase2 ? 3f : 5f);

            // Chase Pos Update
            updateTimer += Time.deltaTime;
            if (updateTimer >= 0.2f)
            {
                startPos= chaseLaserCollider.transform.position;
                chasePos = new Vector3(curTarget.transform.position.x, 0, 0); 
                updateTimer = 0;
                dir = 0;
            }

            if(dir < 1)
            {
                dir += Time.deltaTime * (isPhase2 ? 1f : 0.7f);
            }

            // Chase
            chaseLaserCollider.transform.position = Vector3.Lerp(startPos, chasePos, dir);

            yield return null;
        }
        chaseLaserCollider.SetActive(false);

        // Delay
        yield return new WaitForSeconds(delay);

        state = State.Idle;
        isAttack = false;
    } // End

    private IEnumerator AOE()
    {
        state = State.Attack;
        isAttack = true;
        count = 0;

        // charge
        aoeMainCollider.SetActive(true);
        aoeChargeEffect.SetActive(true);
        yield return new WaitForSeconds(isPhase2 ?  1f : 1.5f);
        aoeChargeEffect.SetActive(false);

        // Delay F
        yield return new WaitForSeconds(isPhase2 ? 0.25f : 0.5f);

        // Attack
        int ran = Random.Range(3, 5);
        for (int i = 0; i < ran; i++)
        {
            // Warring On
            int count = 0;
            for (int i1 = 0; i1 < aoeWarringEffect.Length; i1++)
            {
                ran = Random.Range(0, 100);
                if(ran <= 80)
                {
                    aoeWarringEffect[i1].SetActive(true);
                    count++;
                }

                if(count == 6)
                {
                    break;
                }
            }

            // Warring Rotation
            ran = Random.Range(0, 100);
            Quaternion rot = Quaternion.Euler(0, 0, (ran <= 50 ? 8000 : -8000));
            Quaternion startRotation = aoeMainCollider.transform.rotation;
            Quaternion endrotation = startRotation * rot;
            float timer = 0;
            while (timer < 1)
            {
                Debug.Log(timer);
                timer += Time.deltaTime / (isPhase2 ? 1.5f : 2f);
                aoeMainCollider.transform.rotation = Quaternion.Lerp(startRotation, endrotation, EasingFunctions.InOutQuad(timer));
                yield return null;
            }

            // Attack
            for (int i3 = 0; i3 < aoeLaesrBCollider.Length; i3++)
            {
                if (aoeWarringEffect[i3].activeSelf)
                {
                    ran = Random.Range(0, 100);
                    if (ran <= 50)
                    {
                        aoeLaesrBCollider[i3].SetActive(true);
                    }
                    else
                    {
                        aoeLaesrWCollider[i3].SetActive(true);
                    }
                }
            }

            // Warring Off
            for (int i2 = 0; i2 < aoeWarringEffect.Length; i2++)
            {
                aoeWarringEffect[i2].SetActive(false);
            }

            // Attack Rotation
            rot = Quaternion.Euler(0, 0, (ran <= 50 ? -8000 : 8000));
            startRotation = aoeMainCollider.transform.rotation;
            endrotation = startRotation * rot;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / (isPhase2 ? 0.5f : 1f);
                aoeMainCollider.transform.rotation = Quaternion.Lerp(startRotation, endrotation, EasingFunctions.InOutQuad(timer));
                yield return null;
            }

            // Attack Off
            for (int i3 = 0; i3 < aoeLaesrBCollider.Length; i3++)
            {
                aoeLaesrBCollider[i3].SetActive(false);
                aoeLaesrWCollider[i3].SetActive(false);
            }

            // Delay
            yield return new WaitForSeconds(isPhase2 ? 0.35f : 0.25f);
        }
        aoeMainCollider.SetActive(false);

        // Delay
        yield return new WaitForSeconds(delay);
        state = State.Idle;
        isAttack = false;
    } // ȸ�� �̽� �ִ� ���� (360�� �̻� ȸ�� �Ұ�)

    private IEnumerator AOE2() // ���� ȸ�� �̽� �ִ� ���� (ȸ���� 0���� �ʱ�ȭ��)
    {
        state = State.Attack;
        isAttack = true;
        count = 0;

        // charge
        aoeMainCollider.SetActive(true);
        aoeChargeEffect.SetActive(true);
        yield return new WaitForSeconds(isPhase2 ? 1f : 1.5f);
        aoeChargeEffect.SetActive(false);

        // Delay F
        yield return new WaitForSeconds(isPhase2 ? 0.25f : 0.5f);

        // Attack
        int ran = Random.Range(3, 5);
        for (int i = 0; i < ran; i++)
        {
            // Warring On
            int count = 0;
            for (int i1 = 0; i1 < aoeWarringEffect.Length; i1++)
            {
                int ran2 = Random.Range(0, 100);
                if (ran2 <= 80)
                {
                    aoeWarringEffect[i1].SetActive(true);
                    count++;
                }

                if (count == 6)
                {
                    break;
                }
            }

            // Sound On
            sound.SoundPlay_Other(3);

            // Warring Rotation
            float startZ = aoeMainCollider.transform.rotation.z;
            float endZ = startZ + ((i % 2 != 0) ? Random.Range(300, 400) : Random.Range(-300, -400));
            float z = startZ;
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / (isPhase2 ? 1f : 1.5f);
                z = Mathf.Lerp(startZ, endZ, EasingFunctions.InOutQuad(timer));
                aoeMainCollider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, z));
                yield return null;
            }

            // Part Attack
            PartOrder(Enemy_Boss_Stage3_Part.Attack.SwingLR, 0);
            if (isPhase2)
                PartOrder(Enemy_Boss_Stage3_Part.Attack.SwingLR, 1);

            // Attack Delay
            yield return new WaitForSeconds(isPhase2 ? 0.3f : 0.5f);

            // Sound On
            sound.SoundPlay_Other(4);

            // Attack
            for (int i3 = 0; i3 < aoeLaesrBCollider.Length; i3++)
            {
                if (aoeWarringEffect[i3].activeSelf)
                {
                    int ran2 = Random.Range(0, 100);
                    if (ran2 <= 50)
                    {
                        aoeLaesrBCollider[i3].SetActive(true);
                        aoeWarringEffect[i3].SetActive(false);
                    }
                    else
                    {
                        aoeLaesrWCollider[i3].SetActive(true);
                        aoeWarringEffect[i3].SetActive(false);
                    }
                }
            }

            // Delay
            yield return new WaitForSeconds(isPhase2 ? 0.45f : 0.55f);

            // Bullet Spawn
            BulletSpawn360(30, (i % 2 != 0) ? true : false);

            // Attack Rotation
            Quaternion rot = Quaternion.Euler(0, 0, ((i % 2 != 0) ? -8000 : 8000));
            Quaternion startRotation = aoeMainCollider.transform.rotation;
            Quaternion endrotation = startRotation * rot;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / (isPhase2 ? 1.5f : 2f);
                aoeMainCollider.transform.rotation = Quaternion.Lerp(startRotation, endrotation, EasingFunctions.InOutQuad(timer));
                yield return null;
            }

            // Attack Off
            for (int i3 = 0; i3 < aoeLaesrBCollider.Length; i3++)
            {
                aoeLaesrBCollider[i3].SetActive(false);
                aoeLaesrWCollider[i3].SetActive(false);
            }

            // Delay
            yield return new WaitForSeconds(isPhase2 ? 0.35f : 0.25f);
        }

        // Explosion
        aoeMainCollider.SetActive(false);
        for (int i4 = 0; i4 < (isPhase2 ? 5 : 3); i4++)
        {
            BulletSpawn360(30, (i4 % 2 != 0));
            yield return new WaitForSeconds(isPhase2 ? 0.15f : 0.25f);
        }

        // Delay
        yield return new WaitForSeconds(delay);
        state = State.Idle;
        isAttack = false;
    }

    private void BulletSpawn360(int count, bool isWhite)
    {
        float angleStep = 360f / count; // count = ��ȯ�� ź�� ����
        float anlge = 0;
        float radius = 1f;

        Transform startPos = aoeMainCollider.transform;
        for (int i = 0; i < count; i++)
        {
            float bulletX = startPos.position.x + Mathf.Sin((anlge * Mathf.PI) / 180) * radius;
            float bulletY = startPos.position.y + Mathf.Cos((anlge * Mathf.PI) / 180) * radius;

            Vector3 projectVec = new Vector3(bulletX, bulletY, 0);
            Vector3 projectDir = (projectVec - startPos.position).normalized;

            // Bullet Spawn
            if (isWhite)
            {
                GameObject obj = Instantiate(bullet[0], aoeMainCollider.transform.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Red, projectDir, 5, 30, 10);
            }
            else
            {
                GameObject obj = Instantiate(bullet[1], aoeMainCollider.transform.position, Quaternion.identity);
                obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.Blue, projectDir, 5, 30, 10);
            }

            anlge += angleStep;
        }
    }

    protected override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        state = State.Spawn;
        isInvincibility = true;

        // Sound On
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // Effect On
        spewnEffect.SetActive(true);

        // Spawn UI
        statusUI_Boss.StartNameFadeCall();

        // Delay
        yield return new WaitForSeconds(2f);

        // Effect Off
        spewnEffect.SetActive(false);

        state = State.Idle;
        isInvincibility = false;
    }
    protected override void Stagger()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        state = State.Die;
        isDie = true;
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        // Effect Off
        spewnEffect.SetActive(false);
        centerChargeEffect.SetActive(false);
        laser360ChargeEffect.SetActive(false);
        chaseWarringEffect.SetActive(false);
        for (int i = 0; i < aoeWarringEffect.Length; i++)
        {
            aoeWarringEffect[i].SetActive(false);
        }

        // Collider Off
        centerLaserCollider.SetActive(false);
        laser360Collider.SetActive(false);
        chaseLaserCollider.SetActive(false);
        aoeChargeEffect.SetActive(false);
        aoeMainCollider.SetActive(false);
        for (int i = 0; i < laserAOECollider.Length; i++)
        {
            laserAOECollider[i].SetActive(false);
        }
        for (int i = 0; i < aoeLaesrBCollider.Length; i++)
        {
            aoeLaesrBCollider[i].SetActive(false);
        }
        for (int i = 0; i < aoeLaesrWCollider.Length; i++)
        {
            aoeLaesrWCollider[i].SetActive(false);
        }

        // Sound On
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Die VFX
        for (int i = 0; i < dieEffect.Length; i++)
        {
            dieEffect[i].SetActive(true);

        }

        // UI Call
        stage_Manager.Stage_Clear();
        while (stage_Manager.isUI)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(0.25f);

        Destroy(container);
    }
}
