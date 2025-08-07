using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_WallSetting : MonoBehaviour
{
    public enum WallType { None, Jump }
    public WallType wallType;
    public Transform jumpPos;
}
