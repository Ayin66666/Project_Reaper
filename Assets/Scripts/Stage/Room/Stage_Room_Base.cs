using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stage_Room_Base : MonoBehaviour
{
    protected enum RoomType { Normal, Jump, Boss }

    [Header("---Room Setting---")]
    [SerializeField] protected RoomType roomType;
    [SerializeField] protected bool havePlatform;
    [SerializeField] protected bool haveTarp;
    [SerializeField] protected bool haveSpawn;
    [SerializeField] protected bool haveObject;
    [SerializeField] protected bool haveBGM;
    [SerializeField] protected int bgmIndex;
    public bool isActivate;
    public bool isRoomClear;


    [Header("---Start Setting---")]
    [SerializeField] protected Stage_Room_PlayerChecker roomChecker;
    public List<GameObject> players;
    public int playerCount;


    [Header("---Setting - Platform---")]
    [SerializeField] protected Platform_Base[] platform;


    [Header("---Setting - Trap---")]
    [SerializeField] protected Trap_ActivateType trapActivateType;
    [SerializeField] protected float trapTimer;
    [SerializeField] protected Trap_Base[] traps;
    protected enum Trap_ActivateType { Timer, Monster, Jump, Boss }


    [Header("---Setting - Spawner---")]
    [SerializeField] protected Stage_Enemy_Spawner spawner;


    [Header("---Setting - Object---")]
    [SerializeField] protected Stage_Object_Door[] objectDoor;
    [SerializeField] protected int objectDoorCount;


    [Header("--- Room Object & Check ---")]
    [SerializeField] private GameObject[] startDoor;
    [SerializeField] private GameObject[] endDoor;
    [SerializeField] protected Stage_JumpRoom_ClearChecker clearChecker;

    private void Start()
    {
        // Player Check
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    protected void Door_Setting(bool isStageStart)
    {
        if(isStageStart)
        {
            // Start Door Setting
            for (int i = 0; i < startDoor.Length; i++)
            {
                startDoor[i].GetComponent<Stage_Door>().Door_Setting(true);
            }
        }
        else
        {
            Debug.Log("Call");
            // End Door Setting
            for (int i = 0; i < endDoor.Length; i++)
            {
                endDoor[i].GetComponent<Stage_Door>().Door_Setting(false);
            }
        }
    }
    public abstract void RoomStart();
    public abstract void RoomEnd();
}
