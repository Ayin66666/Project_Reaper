using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnim : MonoBehaviour
{
    public void End()
    {
        gameObject.SetActive(false);
    }

    public void EndDestory()
    {
        Destroy(gameObject);
    }
}
