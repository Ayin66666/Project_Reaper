using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_JumpRoom_ClearChecker : MonoBehaviour
{
    [Header("---Clear Checker Setting---")]
    [SerializeField] private Stage_Room_Base room;
    public List<GameObject> playerList;
    public int playerCount;
    public bool isClear;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerCount = room.playerCount;

        // Player Check
        if (collision.CompareTag("Player"))
        {
            // Player Add
            if (!playerList.Contains(collision.gameObject))
            {
                playerList.Add(collision.gameObject);

                // Room Activate
                if (playerList.Count == playerCount)
                {
                    isClear = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player Check
        if (collision.CompareTag("Player"))
        {
            // Player Delete
            if (playerList.Contains(collision.gameObject))
            {
                playerList.Remove(collision.gameObject);
            }
        }
    }
}
