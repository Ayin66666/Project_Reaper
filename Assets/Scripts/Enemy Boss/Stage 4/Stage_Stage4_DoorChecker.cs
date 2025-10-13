using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage_Stage4_DoorChecker : MonoBehaviour
{
    [Header("---Door---")]
    [SerializeField] private Stage_Stage4_Door door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!door.playerList.Contains(collision.gameObject) && !door.isOpen)
            {
                Debug.Log("Call" + collision.gameObject);
                door.ListSetting(collision.gameObject, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (door.playerList.Contains(collision.gameObject) && !door.isOpen)
            {
                door.ListSetting(collision.gameObject, false);
            }
        }
    }
}
