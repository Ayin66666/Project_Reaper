using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Status",menuName = "Scriptable Object/Enemy_Status",order = int.MaxValue)]
public class Enemy_StatusDataSO : ScriptableObject
{
    [SerializeField] private string objectName;
    public string ObjectName { get { return objectName; } }

    [Header("---Attack Status---")]
    [SerializeField] private int damage;
    public int Damage { get { return damage; } }

    [SerializeField] private int criticalChance;
    public int CriticalChance { get { return criticalChance; } }

    [SerializeField] private float criticalMultiplier;
    public float CriticalMultiplier { get { return criticalMultiplier; } }


    [Header("---Defense Status---")]
    [SerializeField] private int hp;
    public int Hp { get { return hp; } }


    [Header("---Utillity Status---")]
    [SerializeField] private int moveSpeed;
    public int MoveSpeed { get { return moveSpeed; } }

    [SerializeField] private int attackSpeed;
    public int AttackSpeed { get { return attackSpeed; } }

    [SerializeField] private float delay;
    public float Delay { get { return delay; } }
    
    [SerializeField] private int score;
    public int Score { get { return score; } }
}
