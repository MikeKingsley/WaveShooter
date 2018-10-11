using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement Settings")]
    [Range(0f, 3f)] public float walkSpeed = 1.5f;
    [Range(3f, 6f)] public float runSpeed = 3f;
    public float lookRange = 40f;
    public float lookSphereCastRadius = 1f;

    [Header("Ability Settings")]
    [Range(0f, 100f)] public float Health = 100f;
    public float attackRange = 1f;
    public float attackRate = 1f;
    public float attackForce = 15f;
    public float attackDamage = 50f;

    [Header("Personality Settings")]
    public bool canFlee = true;
    [Range(0f, 20f)] public float bravery = 10f;
    public int teamID = 1;

    [Header("Effects")]
    public ParticleEffect impactEffect;
    public ParticleEffect deathEffect;
}