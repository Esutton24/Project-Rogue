using UnityEngine;

public enum AttackType { Melee, Ranged, Area }

[CreateAssetMenu(fileName = "NewAttack", menuName = "Player/Attack")]
public class AttackDataSO : ScriptableObject
{
    public string attackName = "Attack";
    public AttackType attackType = AttackType.Melee;

    [Tooltip("Damage applied on hit")]
    public float damage = 10f;
    public float cooldown = 0.5f;
    public float range = 2f;

    [Header("Ranged Only")]
    public GameObject projectilePrefab;

    [Header("Effects")]
    public ParticleSystem attackEffect;
    public AudioClip attackSound;

    [TextArea] public string description;
}