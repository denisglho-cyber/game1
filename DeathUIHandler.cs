using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUIHandler : MonoBehaviour
{
    public static DeathUIHandler Instance { get; private set; }

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton; // Опционально: кнопка выхода в главное меню
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Имя сцены главного меню

    [Header("Settings")]
    [SerializeField] private bool pauseGameOnDeath = true;
    [SerializeField] private bool showCursorOnDeath = true;

    private void Awake()
    {
        // Исправляем проблему с синглтоном
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (deathScreen != null)
            deathScreen.SetActive(false);

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ShowDeathScreen()
    {
        if (deathScreen == null)
        {
            Debug.LogWarning("Death screen is not assigned in DeathUIHandler!");
            return;
        }

        deathScreen.SetActive(true);

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }

        if (showCursorOnDeath)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Опционально: воспроизвести звук смерти
        // AudioManager.Instance?.PlayDeathSound();

        Debug.Log("Death screen shown");
    }

    public void RestartGame()
    {
        // Сбрасываем время перед загрузкой
        Time.timeScale = 1f;

        // Опционально: сбросить курсор
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Загружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Метод для сброса времени (например, при выходе из игры)
    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    // Метод для скрытия экрана смерти (если нужно)
    public void HideDeathScreen()
    {
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }
}