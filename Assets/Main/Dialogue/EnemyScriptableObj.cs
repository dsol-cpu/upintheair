using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Enemy")]
public class EnemyScriptableObj : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private string enemyClass;
    [SerializeField] private float damage;
    [SerializeField] private float health;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
}