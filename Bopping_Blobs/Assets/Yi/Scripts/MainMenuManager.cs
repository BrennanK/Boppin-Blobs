// Author: Yi Li
// Date: 09/12/2019
// Purpose: Functions for main menu
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private string[] PossibleLevels;

    private AchievementManager achievementManager;

    private PausedMenuManager PausedMenu;

    private bool enableMouseDetection = true;

    // Start is called before the first frame update
    void Start()
    {
        achievementManager = AchievementManager._instance;
        PausedMenu = PausedMenuManager._instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Add a bool to disable mouse detection when enable Customization Menu
        if (enableMouseDetection)
            MouseDetection();
        if (achievementManager.achievementActivated || PausedMenu.IsOpen1)
            enableMouseDetection = false;
        else
            enableMouseDetection = true;
    }

    private void MouseDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "Play":
                        Debug.Log("Play the game!");
                        SceneManager.LoadSceneAsync(PossibleLevels[Random.Range(0, PossibleLevels.Length)]);
                        break;
                    case "Customization":
                        Debug.Log("Open customization menu.");
                        break;
                    case "Achievements":
                        achievementManager.OpenAchievement();
                        break;
                    case "Options":
                        PausedMenu.EnablePausedMenu();
                        Debug.Log("Open option menu.");
                        break;
                    default:
                        Debug.Log("Nothing Selected!");
                        break;
                }
            }
        }
    }
}
