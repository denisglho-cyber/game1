using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResourceManager : MonoBehaviour
{
    [Header("Статистика")]
    public int collectedCoins = 0;
    public int killedEnemies = 0;

    [Header("Настройки завершения")]
    public string winSceneName = "WinMenu"; 

    public void AddSonnet()
    {
        collectedCoins++;
        CheckWinCondition();
    }

    public void AddEnemyKill()
    {
        killedEnemies++;
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");


        if (enemies.Length == 0 && chests.Length == 0)
        {
            EndGame();
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void EndGame()
    {
        Debug.Log("Победа! Все враги повержены, все сундуки открыты.");
    }
}
