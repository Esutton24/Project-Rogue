using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "Player/Class")]
public class PlayerClassSO : ScriptableObject
{
    public string className = "Class";
    public Sprite classIcon;

    [Header("Stat Multipliers")]
    public float moveSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1f;

    [Header("Attacks")]
    public List<AttackDataSO> attacks = new List<AttackDataSO>();
}