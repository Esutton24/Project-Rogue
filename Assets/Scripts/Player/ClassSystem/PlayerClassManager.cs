
using UnityEngine;

public class PlayerClassManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerClassSO fallbackClass;

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
        {
            Destroy(currentCharacter);
        }

        if (CurrentClass.characterPrefab == null)
        {
            Debug.LogError($"No character prefab assigned to {CurrentClass.className}!");
            return;
        }

        Transform parent = characterParent != null ? characterParent : transform;

        currentCharacter = Instantiate(
            CurrentClass.characterPrefab,
            parent
        );

        Animator spawnedAnimator = currentCharacter.GetComponentInChildren<Animator>(true);

        if (movement != null)
        {
            movement.SetAnimator(spawnedAnimator);
        }

        PlayerCombat combat = GetComponent<PlayerCombat>();

        if (combat != null)
        {
            combat.SetAnimator(spawnedAnimator);
        }

        currentCharacter.transform.localPosition = Vector3.zero;
        currentCharacter.transform.localRotation = Quaternion.identity;
        currentCharacter.transform.localScale = Vector3.one;
        
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
