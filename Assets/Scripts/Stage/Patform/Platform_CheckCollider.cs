using UnityEngine;


public class Platform_CheckCollider : MonoBehaviour
{
    [SerializeField] private Platform_Movement platform;
    public bool pressed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            pressed = true;
            switch (platform.movementType)
            {
                case Platform_Movement.MovementType.always:
                    break;

                case Platform_Movement.MovementType.pressed:
                    platform.playerList.Add(collision.gameObject);
                    if (!platform.isActivate)
                    {
                        platform.StartMovement();
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            switch (platform.movementType)
            {
                case Platform_Movement.MovementType.always:
                    break;

                case Platform_Movement.MovementType.pressed:
                    platform.playerList.Remove(collision.gameObject);
                    if (platform.playerList.Count == 0)
                    {
                        platform.StopMovement();
                        pressed = false;
                    }
                    break;
            }
        }
    }
}
