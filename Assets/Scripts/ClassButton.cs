using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class ClassButton : MonoBehaviour
{
    [SerializeField] private PlayerClassSO playerClass;
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        if (playerClass == null) return;

        GameData.SelectedClass = playerClass;
        SceneManager.LoadScene(gameSceneName);
    }
}