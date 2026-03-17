
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // reference to player controller to access the health.
    public PlayerController player;
    // Gather a list of all enemies so we know when all robots are fixed.
    EnemyController[] enemies;
    // reference so that we know which ui to display.
    public UIHandler uiHandler;
    public string npcMessage = "Press C to shoot cogs and help me fix these broken robots! Come speak to me when you're finished!"; 

    int enemiesFixed = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach(var enemy in enemies)
        {
            enemy.OnFixed += HandleEnemyFixed;
        }
        uiHandler.SetCounter(0, enemies.Length);
        player.OnTalkedToNPC += HandlePlayerTalkedToNPC;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.health <=0)
        {
            uiHandler.DisplayLoseScreen();
            // Invoke function allows you to execude a function after a delay.
            Invoke(nameof(ReloadScene), 3f);
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool AllEnemiesFixed()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isBroken) return false;
        }
        return true;
    }

    void HandleEnemyFixed()
    {
        enemiesFixed++;
        uiHandler.SetCounter(enemiesFixed, enemies.Length);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void HandlePlayerTalkedToNPC()
    {
        if (AllEnemiesFixed())
        {
            uiHandler.DisplayWinScreen();
            Invoke(nameof(ReturnToMainMenu), 3f);
        }else
        {
            string message = player.currentInteractingNPC.npcMessage;
            UIHandler.instance.DisplayDialogue(message); 
        }
    }
}
