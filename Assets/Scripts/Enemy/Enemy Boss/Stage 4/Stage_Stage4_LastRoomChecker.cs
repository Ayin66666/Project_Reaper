using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Stage4_LastRoomChecker : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Stage4 boss;
    [SerializeField] private List<GameObject> players;
    [SerializeField] private int playerCount;
    [SerializeField] private bool isActivate;

    private void Start()
    {
        // Player Check
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Call Aa");
        // Player Check
        if (collision.CompareTag("Player") && !isActivate)
        {
            Debug.Log("Call A");

            // Player Add
            if (!players.Contains(collision.gameObject))
            {
                players.Add(collision.gameObject);

                // Room Activate
                if (players.Count == playerCount)
                {
                    Debug.Log("Call B");
                    isActivate = true;
                    boss.LastRoomStartCall();
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Player Check
        if (collision.CompareTag("Player") && !isActivate)
        {
            // Player Delete
            if (players.Contains(collision.gameObject))
            {
                players.Remove(collision.gameObject);
            }
        }
    }
}
