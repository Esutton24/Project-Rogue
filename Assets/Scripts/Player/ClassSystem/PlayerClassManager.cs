
using UnityEngine;


public class PlayerClassManager : MonoBehaviour
{
    [Header("Testing")]
    [Tooltip("If true, always use the Fallback Class even if GameData has a selection.")]
    [SerializeField] private bool forceFallbackInEditor = false;

    #if UNITY_EDITOR
    [ContextMenu("Enable Fallback For Direct Play")]
    void EnableFallback() => forceFallbackInEditor = true;
#endif

    [Header("Player Setup")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerClassSO fallbackClass;
    [SerializeField] private Transform visualAnchor;

    [Header("Character Setup")]
    [SerializeField] private Transform characterParent;

    public PlayerClassSO CurrentClass { get; private set; }
    public System.Action<PlayerClassSO> OnClassChanged;

    private GameObject currentCharacter;

    void Start()
    {
        PlayerClassSO startClass = GameData.SelectedClass;

        // Fallback for testing when entering the game scene directly
        if (startClass == null)
        {
            if (fallbackClass != null)
            {
                startClass = fallbackClass;
                Debug.LogWarning("[PlayerClassManager] No class selected. Using fallback.");
            }
            else
            {
                Debug.LogError("[PlayerClassManager] No class assigned!");
                enabled = false;
                return;
            }
        }

        SetClass(startClass);
    }

    public void SetClass(PlayerClassSO newClass)
    {
        if (newClass == null)
        {
            Debug.LogError("[PlayerClassManager] Tried to set a null class.");
            return;
        }

        CurrentClass = newClass;

        LoadCharacter();
        ApplyStats();

        OnClassChanged?.Invoke(newClass);

        Debug.Log($"Class changed to: {newClass.className}");
    }

    void LoadCharacter()
    {
        if (currentCharacter != null)
            Destroy(currentCharacter);

        if (CurrentClass.characterPrefab == null)
        {
            Debug.LogError($"No character prefab assigned to {CurrentClass.className}!");
            return;
        }

        if (visualAnchor == null)
            visualAnchor = transform;

        // Spawn under the anchor
        currentCharacter = Instantiate(
            CurrentClass.characterPrefab,
            visualAnchor
        );

        // Apply per-class offset
        currentCharacter.transform.localPosition = CurrentClass.spawnPositionOffset;
        currentCharacter.transform.localRotation = Quaternion.Euler(CurrentClass.spawnRotationOffset);

        // Find animator
        Animator spawnedAnimator = currentCharacter.GetComponentInChildren<Animator>(true);

        movement?.SetAnimator(spawnedAnimator);
        if (TryGetComponent<PlayerCombat>(out var combat))
        {
            combat.SetAnimator(spawnedAnimator);
        }
    }

    void ApplyStats()
    {
        if (CurrentClass == null || movement == null) return;

        movement.ApplyClassStats(
            CurrentClass.moveSpeedMultiplier,
            CurrentClass.jumpForceMultiplier
        );
    }
}
