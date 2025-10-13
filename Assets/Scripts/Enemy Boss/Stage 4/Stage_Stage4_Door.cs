using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Stage4_Door : MonoBehaviour
{
    [Header("---Setting---")]
    public bool isOpen;

    [Header("---Player Check---")]
    [SerializeField] private int playerCount;
    public List<GameObject> playerList;

    [Header("---Object---")]
    [SerializeField] private GameObject[] door;

    private void Start()
    {
        playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    public void ListSetting(GameObject target, bool isAdd)
    {
        // Open Check
        if (isOpen)
        {
            return;
        }
        else
        {
            if (isAdd)
            {
                // Add
                if (!playerList.Contains(target))
                {
                    playerList.Add(target);
                }
            }
            else
            {
                // Remove
                if (playerList.Contains(target))
                {
                    playerList.Remove(target);
                }
            }

            // Player Check
            if (playerList.Count == playerCount)
            {
                // Door Open
                isOpen = true;
                for (int i = 0; i < door.Length; i++)
                {
                    door[i].SetActive(false);
                }
            }
        }
    }
}
