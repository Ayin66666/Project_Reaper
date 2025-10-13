using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Enemy_Boss_Stage4 : MonoBehaviour
{
    [Header("---Boss---")]
    [SerializeField] private Stage_Manager stage_Manager;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject bossBody;
    [SerializeField] private Enemy_Sound sound;

    [Header("---Boss Object---")]
    [SerializeField] private GameObject bossObject;

    [Header("---Room Setting---")]
    [SerializeField] private Stage_Room_Manager[] room_manager;

    [Header("---Boss Move---")]
    [SerializeField] private Transform[] stageMovePos;
    [SerializeField] private Transform[] stage7MovePos;
    private bool isMove;


    [Header("---Setting - Platform---")]
    [SerializeField] protected Platform_Base[] platform;

    [Header("---Setting - Trap---")]
    [SerializeField] protected float trapTimer;
    [SerializeField] protected Trap_Base[] traps;

    [Header("--- Room Object & Check ---")]
    [SerializeField] private GameObject[] startDoor;
    [SerializeField] private GameObject[] endDoor;

    // Sound Index
    // 0 : Move
    // 1 : Move Stop

    public void MoveCall(int moveIndex, GameObject obj)
    {
        Debug.Log(obj);
        if(isMove)
        {
            Debug.Log("Boss Already Move!");
            return;
        }
        else
        {
            StartCoroutine(Move(moveIndex));
        }
    }

    private IEnumerator Move(int moveIndex)
    {
        // Sound On
        sound.SoundPlay_Other(0);

        isMove = true;
        Vector3 startPos = bossBody.transform.position;
        Vector3 endPos = stageMovePos[moveIndex].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / 5;
            bossBody.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InOutExpo(timer));
            yield return null;
        }

        // Sound On
        sound.SoundPlay_Other(1);

        isMove = false;
    }

    public void LastRoomStartCall()
    {
        StartCoroutine(LastRoomStart());
    }

    private void RoomOn()
    {
        // Start Door Setting
        for (int i = 0; i < startDoor.Length; i++)
        {
            startDoor[i].SetActive(true);
        }

        // Platform
        for (int i = 0; i < platform.Length; i++)
        {
            platform[i].gameObject.SetActive(true);
            platform[i].PlatformActivate(true);
        }

        // Trap
        for (int i = 0; i < traps.Length; i++)
        {
            traps[i].gameObject.SetActive(true);
            traps[i].TrapActivate(true);
        }
    }

    private IEnumerator LastRoomStart()
    {
        Debug.Log("Call C");

        // Sound On
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Spawn);

        // Move Up
        Vector3 startPos = stage7MovePos[0].position;
        Vector3 endPos = stage7MovePos[1].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / 5;
            bossBody.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InOutQuart(timer));
            yield return null;
        }

        // object Spawn
        bossObject.SetActive(true);
        Enemy_Base enemy = bossObject.GetComponent<Enemy_Base>();
        while (enemy.state == Enemy_Base.State.Spawn)
        {
            yield return null;
        }

        // Room Systeam Activate
        RoomOn();

        // Stage Check
        while (bossObject != null)
        {
            yield return new WaitForSeconds(1f);
        }

        // Die Call
        DieCall();
    }

    public void DieCall()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        // Start Door Setting
        for (int i = 0; i < startDoor.Length; i++)
        {
            startDoor[i].SetActive(false);
        }

        // Platform
        for (int i = 0; i < platform.Length; i++)
        {
            platform[i].gameObject.SetActive(false);
            platform[i].PlatformActivate(false);
        }

        // Trap
        for (int i = 0; i < traps.Length; i++)
        {
            traps[i].gameObject.SetActive(false);
            traps[i].TrapActivate(false);
        }

        // Sound On
        sound.SoundPlay_public(Enemy_Sound.PublicSound.Die);

        // Cam Effect ?

        // Die Move
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 5;
            transform.position = Vector2.Lerp(stage7MovePos[1].position, stage7MovePos[0].position, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // UI Call
        stage_Manager.Stage_Clear();
        while (stage_Manager.isUI)
        {
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(1f);


        // End Door Setting
        for (int i = 0; i < endDoor.Length; i++)
        {
            endDoor[i].SetActive(false);
        }

        // Destory
        Destroy(container);
    }
}
