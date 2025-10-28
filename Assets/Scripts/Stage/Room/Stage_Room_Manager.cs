using System.Collections;
using UnityEngine;


public class Stage_Room_Manager : Stage_Room_Base
{
    [SerializeField] private Stage_Manager room_Manager;
    private float curTrapTimer;

    public override void RoomStart()
    {
        isRoomClear = false;
        Door_Setting(true);

        // Spawn
        if(haveSpawn)
        {
            StartCoroutine(SpawnCheck());
        }

        // Platform
        if(havePlatform)
        {
            for(int i = 0; i < platform.Length; i++) 
            {
                platform[i].gameObject.SetActive(true);
                platform[i].PlatformActivate(true);
            }
        }
        
        // Trap
        if (haveTarp)
        {
            // Trap On
            for (int i = 0; i < traps.Length; i++)
            {
                traps[i].gameObject.SetActive(true);
                traps[i].TrapActivate(true);
            }

            // Trap Type Check
            if(trapActivateType == Trap_ActivateType.Timer)
            {
                StartCoroutine(Trap_Check_Timer());
            }
        }

        // Object
        if (haveObject)
        {
            for (int i = 0; i < objectDoor.Length; i++)
            {
                objectDoor[i].gameObject.SetActive(true);
                objectDoor[i].Object_Setting();
            }
        }


        switch (roomType)
        {
            case RoomType.Normal:
                StartCoroutine(SpawnCheck());
                break;

            case RoomType.Jump:
                StartCoroutine(Room_Check_TypeJump());
                break;

            case RoomType.Boss:
                StartCoroutine(Boss_Check());
                break;
        }
    }

    private IEnumerator Boss_Check()
    {
        while(spawner != null)
        {
            yield return null;
        }

        RoomEnd();
    }

    private IEnumerator Room_Check_TypeJump()
    {
        // Player Check
        while(!clearChecker.isClear)
        {
            yield return null;
        }

        room_Manager.Room_Clear();
        RoomEnd();
    }

    private IEnumerator Trap_Check_Timer()
    {
        curTrapTimer = trapTimer;
        while (curTrapTimer > 0)
        {
            curTrapTimer -= Time.deltaTime;
            yield return null;
        }

        // Room Claer
        switch (roomType)
        {
            case RoomType.Normal:
            case RoomType.Boss:
                RoomEnd();
                break;

            case RoomType.Jump:
                break;
        }
    }

    private IEnumerator SpawnCheck()
    {
        // Enemy Spawn
        spawner.Spawn();

        // Spawner Check
        if (haveSpawn)
        {
            while(spawner != null)
            {
                yield return null;
            }
        }

        // Room Claer
        switch (roomType)
        {
            case RoomType.Normal:
                room_Manager.Room_Clear();
                RoomEnd();
                break;

            case RoomType.Boss:
                room_Manager.Stage_Clear();
                RoomEnd();
                break;

            case RoomType.Jump:
                room_Manager.Room_Clear();
                break;
        }
    }

    public override void RoomEnd()
    {
        isRoomClear = true;

        // Platform
        if (havePlatform)
        {
            for (int i = 0; i < platform.Length; i++)
            {
                platform[i].PlatformActivate(false);
            }
        }

        // Trap
        if (haveTarp)
        {
            for (int i = 0; i < traps.Length; i++)
            {
                if (traps[i].trapType == Trap_Base.TrapType.Spike || traps[i].trapType == Trap_Base.TrapType.electric)
                {
                    traps[i].TrapActivate(false);
                }

                traps[i].gameObject.SetActive(false);
            }
        }

        Door_Setting(false);
    }
}
