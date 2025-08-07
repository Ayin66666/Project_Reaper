using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_Cam : MonoBehaviour
{
    [SerializeField] Transform target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        if (target == null)
        {
            return;
        }

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
