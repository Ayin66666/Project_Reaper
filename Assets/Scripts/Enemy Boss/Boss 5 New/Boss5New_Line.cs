using UnityEngine;


public class Boss5New_Line : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    public GameObject target;


    public void LineOn(GameObject target)
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.transform.position);

    }

    public void LineOff()
    {
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
        line.enabled = false;
    }
}
