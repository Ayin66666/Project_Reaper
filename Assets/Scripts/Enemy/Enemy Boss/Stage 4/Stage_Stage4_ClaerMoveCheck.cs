using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Stage4_ClaerMoveCheck : MonoBehaviour
{
    [SerializeField] private Stage_Room_Base room;
    [SerializeField] private Enemy_Boss_Stage4 boss;
    [SerializeField] private int moveIndex;

    private void Start()
    {
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        // Claer Check
        while(!room.isRoomClear)
        {
            yield return null;
        }

        boss.MoveCall(moveIndex, gameObject);
    }
}
