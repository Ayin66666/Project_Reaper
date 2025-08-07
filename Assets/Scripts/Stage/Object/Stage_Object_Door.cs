using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Object_Door : MonoBehaviour
{
    [SerializeField] private bool isActivate;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject targetObj;

    public void Object_Setting()
    {
        StartCoroutine(Object_Check());
    }

    public IEnumerator Object_Check()
    {
        isActivate = true;
        while (targetObj != null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Door Off
        door.SetActive(false);
    }
}
