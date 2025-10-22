using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Room_PlayerChecker : MonoBehaviour
{
    [SerializeField] private Stage_Room_Base room;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Check
        if (collision.CompareTag("Player") && !room.isActivate)
        {
            // Player Add
            if (!room.players.Contains(collision.gameObject))
            {
                room.players.Add(collision.gameObject);

                // Room Activate
                if (room.players.Count == room.playerCount)
                {
                    room.RoomStart();
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player Check
        if (collision.CompareTag("Player") && !room.isActivate)
        {
            // Player Delete
            if (room.players.Contains(collision.gameObject))
            {
                room.players.Remove(collision.gameObject);
            }
        }
    }
}
