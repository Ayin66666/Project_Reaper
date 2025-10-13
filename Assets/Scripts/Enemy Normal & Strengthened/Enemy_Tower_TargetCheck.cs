using System.Collections.Generic;
using UnityEngine;


public class Enemy_Tower_TargetCheck : MonoBehaviour
{
    public List<GameObject> targets;
    public bool haveTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            haveTarget = true;
            targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targets.Remove(collision.gameObject);
            if(targets.Count <= 0)
            {
                haveTarget = false;
            }
        }
    }
}
