using Easing;
using System.Collections;
using UnityEngine;


public class Stage_Door : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject doorObject;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private Transform[] movePos;

    private Coroutine curCoroutine;

    public void Door_Setting(bool isOn)
    {
        if(curCoroutine != null) StopCoroutine(curCoroutine);
        if (isOn)
        {
            curCoroutine = StartCoroutine(DoorMove(movePos[1].position, true));
        }
        else
        {
            curCoroutine = StartCoroutine(DoorMove(movePos[0].position, false));
        }
    }

    private IEnumerator DoorMove(Vector2 pos, bool isOn)
    {
        // Door On
        if (isOn)
        {
            doorCollider.enabled = true;
        }

        // Door move
        Vector2 startPos = doorObject.transform.position;
        Vector2 endPos = pos;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime / moveSpeed;
            doorObject.transform.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // Door Off
        if(!isOn)
        {
            doorCollider.enabled = false;
        }
    }
}
