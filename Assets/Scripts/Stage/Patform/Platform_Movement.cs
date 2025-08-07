using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing;

public class Platform_Movement : Platform_Base
{
    [Header("---Setting ( Movement )---")]
    [SerializeField] public MovementType movementType;
    [SerializeField] private bool startActivate;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDelay;
    [SerializeField] private GameObject body;
    [SerializeField] private Transform[] waypoints;

    public enum MovementType { always, pressed }
    private Coroutine movementCoroutine;
    public List<GameObject> playerList;

    private void Start()
    {
        if(startActivate)
        {
            PlatformActivate(true);
        }
    }

    public override void PlatformActivate(bool isActivate)
    {
        this.isActivate = isActivate;
        if(isActivate)
        {
            switch (movementType)
            {
                case MovementType.always:
                    movementCoroutine = StartCoroutine(PlatformMovement());
                    break;

                case MovementType.pressed:
                    break;
            }
        }
        else
        {
            switch (movementType)
            {
                case MovementType.always:
                    if (movementCoroutine != null)
                    {
                        StopCoroutine(movementCoroutine);
                    }
                    break;

                case MovementType.pressed:
                    break;
            }
        }
    }

    public void StartMovement()
    {
        movementCoroutine = StartCoroutine(PlatformMovement());
    }

    public void StopMovement()
    {
        if(movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        movementCoroutine = StartCoroutine(PlatformReset());
    }

    private IEnumerator PlatformMovement()
    {
        isActivate = true;
        isReset = false;

        while (isActivate)
        {
            switch (movementType)
            {
                case MovementType.always:            
                    for (int i = 0; i < waypoints.Length; i++)
                    {
                        Vector3 startPos = body.transform.position;
                        Vector3 endPos = waypoints[i].position;
                        float timer = 0;

                        // Movement
                        while (timer < 1)
                        {
                            timer += moveSpeed * Time.deltaTime;
                            body.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InOutCubic(timer));
                            yield return null;
                        }
                        body.transform.position = endPos;

                        // Delay
                        yield return new WaitForSeconds(moveDelay);
                    }
                    break;

                case MovementType.pressed:
                    if(body.transform.position != waypoints[1].position)
                    {
                        Vector3 startPos = body.transform.position;
                        float timer = 0;
                        while (timer < 1)
                        {
                            timer += moveSpeed * Time.deltaTime;
                            body.transform.position = Vector3.Lerp(startPos, waypoints[1].position, EasingFunctions.InOutCubic(timer));
                            yield return null;
                        }
                    }
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator PlatformReset()
    {
        isActivate = false;
        isReset = true;

        Vector3 startPos = body.transform.position;
        float timer = 0;

        // Pressed - Reset
        while (isReset && timer < 1)
        {
            timer += Time.deltaTime * moveSpeed * 0.5f;
            body.transform.position = Vector3.Lerp(startPos, waypoints[0].position, EasingFunctions.InOutCubic(timer));
            yield return null;
        }
        body.transform.position = waypoints[0].position;
        isReset = false;
    }
}
