using UnityEngine;

public class PlayerClassManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerClassSO fallbackClass; // debug fallback if you skip the menu

    public PlayerClassSO CurrentClass { get; private set; }
    public System.Action<PlayerClassSO> OnClassChanged;

    void Start()
    {
        PlayerClassSO startClass = GameData.SelectedClass;

        // Fallback for testing: pressing Play directly inside the game scene
        if (startClass == null)
        {
            if (fallbackClass != null)
            {
                startClass = fallbackClass;
                Debug.LogWarning("[PlayerClassManager] No class selected from menu. Using fallback.");
            }
            else
            {
                Debug.LogError("[PlayerClassManager] No class assigned! Set a fallback or select from menu.");
                enabled = false;
                return;
            }
        }

        SetClass(startClass);
    }

    public void SetClass(PlayerClassSO newClass)
    {
        CurrentClass = newClass;
        ApplyStats();
        OnClassChanged?.Invoke(newClass);
        Debug.Log($"Class changed to: {newClass.className}");
    }

    void ApplyStats()
    {
        if (CurrentClass == null || movement == null) return;
        movement.ApplyClassStats(CurrentClass.moveSpeedMultiplier, CurrentClass.jumpForceMultiplier);
    }
}