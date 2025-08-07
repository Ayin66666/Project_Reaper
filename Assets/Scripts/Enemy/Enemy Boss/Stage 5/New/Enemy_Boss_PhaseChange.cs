using Easing;
using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
{
    [SerializeField] private string attackName;

    [Header("--- Collider & Warring ---")]
    public GameObject[] attackWarring;
    public GameObject[] attackCollider;

    [Header("---Prefab---")]
    public GameObject[] bullet;
    public GameObject[] waveBullet;

    [Header("--- Pos ---")]
    public Transform[] movePos;
    public Transform shotPos;


    [Header("---VFX---")]
    public GameObject[] vfx;
}


public class Enemy_Boss_PhaseChange : MonoBehaviour
{
    [Header("--- Component ---")]
    [SerializeField] private Animator anim;
    [SerializeField] private Enemy_Sound sound;
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private LineRenderer line;


    [Header("--- Setting ---")]
    [SerializeField] private AttackData[] attackData;
    [SerializeField] private Type type;
    [SerializeField] private LayerMask layer;
    [SerializeField] protected CurLook curLook;
    public bool isAttack;
    private bool isBodyFade;
    private enum Type { Stage1, Stage2 };
    protected enum CurLook { None, Left, Right }

    [SerializeField] private Transform[] explosionPosL;
    [SerializeField] private Transform[] explosionPosR;


    [Header("---Target---")]
    [SerializeField] private GameObject curTarget;
    private Vector2 targetVector;
    private float targetDir;
    private bool haveTarget;
    private List<GameObject> targetList = new List<GameObject>();


    public void Use()
    {
        switch (type)
        {
            case Type.Stage1:
                StartCoroutine(Use_Type1());
                break;

            case Type.Stage2:
                StartCoroutine(Use_Type2());
                break;
        }
    }
    #region Type 1
    private IEnumerator Use_Type1()
    {
        isAttack = true;

        StartCoroutine(Body_Fade(true));

        // 공격 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAOE", true);
        anim.SetBool("isAttack", true);
        anim.SetBool("isAttackReady", true);

        // 워닝 표시
        attackData[0].attackWarring[0].SetActive(true);

        // 준비모션 대기
        while(anim.GetBool("isAttackReady"))
        {
            yield return null;
        }

        // 워닝 표시
        attackData[0].attackWarring[0].SetActive(false);

        // 공격
        StartCoroutine(Bullet_Spawn360());
        yield return new WaitForSeconds(10f);
        anim.SetBool("isAOE", false);

        // 종료
        while(anim.GetBool("isAttack"))
        {
            yield return null;
        }

        StartCoroutine(Body_Fade(false));
        yield return new WaitWhile(() => isBodyFade);

        isAttack = false;
        Destroy(gameObject);
    }

    private IEnumerator Bullet_Spawn360()
    {
        // Sound
        // sound.SoundPlay_Other(3);

        for (int i = 0; i < 10; i++)
        {
            // Bullet Type & Movement Setting
            int ran = Random.Range(0, 2);
            BulletSpawn360(31, ran == 0);

            // Big Wave Attack 
            if (i % 3 == 0)
            {
                ran = Random.Range(0, 2);
                BulletSpawnRL(ran == 0);
            }

            // Spawn Delay
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void BulletSpawn360(int count, bool isWhite)
    {
        float angleStep = 360f / count; // count = 소환할 탄의 갯수
        float anlge = 0;
        float radius = 1f;

        Transform startPos = attackData[0].shotPos;
        for (int i = 0; i < count; i++)
        {
            float bulletX = startPos.position.x + Mathf.Sin((anlge * Mathf.PI) / 180) * radius;
            float bulletY = startPos.position.y + Mathf.Cos((anlge * Mathf.PI) / 180) * radius;

            Vector3 projectVec = new Vector3(bulletX, bulletY, 0);
            Vector3 projectDir = (projectVec - startPos.position).normalized;
            float rotationZ = Mathf.Atan2(projectDir.y, projectDir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

            // Bullet Spawn
            GameObject obj = Instantiate(attackData[0].bullet[isWhite ? 0 : 1], attackData[0].shotPos.position, rotation);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(isWhite ? Enemy_Bullet.BulletType.Red : Enemy_Bullet.BulletType.Blue, projectDir, 5, 30, 10);

            anlge += angleStep;
        }
    }

    private void BulletSpawnRL(bool isRed)
    {
        GameObject obj = Instantiate(attackData[0].waveBullet[isRed ? 0 : 1], attackData[0].shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(isRed ? Enemy_Bullet.BulletType.Red : Enemy_Bullet.BulletType.Blue, -attackData[0].shotPos.right, 10, 25, 15);

        GameObject obj1 = Instantiate(attackData[0].waveBullet[isRed ? 0 : 1], attackData[0].shotPos.position, Quaternion.identity);
        obj1.GetComponent<Enemy_Bullet>().Bullet_Setting(isRed ? Enemy_Bullet.BulletType.Red : Enemy_Bullet.BulletType.Blue, attackData[0].shotPos.right, 10, 25, 15);
    }
    #endregion

    #region Type 2
    private IEnumerator Use_Type2()
    {
        isAttack = true;

        // Attack
        int count = Random.Range(3, 4); // Attack Count
        for (int i = 0; i < count; i++)
        {
            Target_Setting();
            CurTarget_Check();
            LookAt();

            // Animation
            anim.SetTrigger("Attack");
            anim.SetBool("isRushReady", true);
            anim.SetBool("isAirRush", true);
            anim.SetBool("isAirRushLanding", true);

            // Move Up
            int posRan = Random.Range(0, 2); // 0 => Left  |  1 => Right
            rigid.velocity = Vector2.zero;
            rigid.gravityScale = 0;
            if (posRan == 0)
            {
                posRan = Random.Range(0, 2);
                transform.position = attackData[0].movePos[posRan].position;
            }
            else
            {
                posRan = Random.Range(2, 4);
                transform.position = attackData[0].movePos[posRan].position;
            }

            // Effect
            attackData[0].vfx[0].SetActive(true);

            // Attack Delay
            line.enabled = true;
            line.SetPosition(0, transform.position);
            float timer = 0;
            while (timer < 0.45f)
            {
                line.SetPosition(1, curTarget.transform.position);
                timer += Time.deltaTime;
                LookAt();
                yield return null;
            }
            line.enabled = false;

            // Move Setting
            Vector3 startPos = transform.position;
            Vector3 rayDir = (curTarget.transform.position - transform.position).normalized;
            Vector3 endPos = Physics2D.Raycast(transform.position, rayDir, 150, layer).point;
            endPos.x += endPos.x < 0 ? 0.5f : -0.5f;
            endPos.y += 0.5f;

            rigid.velocity = Vector2.zero;
            anim.SetBool("isRushReady", false);

            // Sound
            // sound.SoundPlay_Other(5);

            // Attack => Move Down
            attackData[0].attackCollider[0].SetActive(true);
            timer = 0;
            while (timer < 1)
            {
                timer += 1.5f * Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InQuart(timer));
                yield return null;
            }
            anim.SetBool("isAirRush", false);
            attackData[0].attackCollider[0].SetActive(false);
            rigid.gravityScale = 1;

            StartCoroutine(AirExplosion(0.1f));

            // Animation Wait
            while (anim.GetBool("isAirRushLanding"))
            {
                yield return null;
            }

            // Next Attack Delay
            yield return new WaitForSeconds(0.25f);
        }

        isAttack = false;
        Destroy(gameObject);
    } // End

    public void AirShotCall1()
    {
        // Sound
        // sound.SoundPlay_Other(1);

        // Shot Dir & Rotation Setting
        Vector3 shotDir = (curTarget.transform.position - transform.position).normalized;
        GameObject obj = Instantiate(attackData[0].waveBullet[0], transform.position, Quaternion.identity);

        // Sword Aura Move
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.BulletType.None, shotDir, 20, 40, 15);

        // Sword Aura Explosion
        obj.GetComponent<Enemy_WaveExplosion>().ExplosionSetting(Enemy_WaveExplosion.Type.None, 15f);
    }

    private IEnumerator AirExplosion(float timer)
    {
        int ran = Random.Range(0, 100);
        for (int i = 0; i < explosionPosL.Length; i++)
        {
            // Explosion
            Instantiate(attackData[0].bullet[0], explosionPosL[i].position, Quaternion.identity);
            Instantiate(attackData[0].bullet[0], explosionPosR[i].position, Quaternion.identity);

            // Sound
            // sound.SoundPlay_Other(6);

            // Delay
            yield return new WaitForSeconds(timer);
        }
    } // End
    #endregion


    #region Other
    private IEnumerator Body_Fade(bool OnOff)
    {
        isBodyFade = true;

        // 바디 등장
        if (OnOff)
        {
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 0.75f;
                bodySprite.color = new Color(bodySprite.color.r, bodySprite.color.g, bodySprite.color.b, timer);
                yield return null;
            }
            bodySprite.color = new Color(bodySprite.color.r, bodySprite.color.g, bodySprite.color.b, 1);
        }
        else
        {
            float timer = 1;
            while (timer > 0)
            {
                timer -= Time.deltaTime * 0.75f;
                bodySprite.color = new Color(bodySprite.color.r, bodySprite.color.g, bodySprite.color.b, timer);
                yield return null;
            }
            bodySprite.color = new Color(bodySprite.color.r, bodySprite.color.g, bodySprite.color.b, 1);
        }

        isBodyFade = false;
    }

    protected void LookAt()
    {
        if (haveTarget)
        {
            // 이거 연속으로 호출되면 이상하게 쳐다보는 문제가 있는거 같은데 확인해볼것!
            targetVector = curTarget.transform.position - transform.position;
            if (targetVector.x > 0)
            {
                curLook = CurLook.Right;
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (targetVector.x < 0)
            {
                curLook = CurLook.Left;
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                return;
            }
        }
    }

    protected void Target_Setting()
    {
        if (targetList.Count <= 0)
        {
            // Find Player
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < targets.Length; i++)
            {
                targetList.Add(targets[i]);
                Debug.Log(targetList[i]);
            }

            if (targets.Length > 0)
            {
                haveTarget = true;

                // Cal Target Dir / Range
                curTarget = targets[Random.Range(0, targets.Length)];
                targetVector = curTarget.transform.position - transform.position;
                targetDir = targetVector.magnitude;
            }
            else
            {
                Debug.Log("타겟이 없습니다!");
                haveTarget = false;
            }
        }
        else
        {
            // List Check
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == null)
                {
                    targetList.RemoveAt(i);
                }
            }

            // Target Setting
            if (targetList.Count > 0)
            {
                int ran = Random.Range(0, targetList.Count);
                curTarget = targetList[ran];
            }
            else
            {
                Debug.Log("타겟이 없습니다!");
                haveTarget = false;
            }
        }
    }

    protected void CurTarget_Check()
    {
        if (haveTarget)
        {
            targetVector = curTarget.transform.position - transform.position;
            targetDir = targetVector.magnitude;
        }
    }
    #endregion
}
