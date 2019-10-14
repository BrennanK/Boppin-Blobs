// Author: Yi Li
// Date: 09/12/2019
// Purpose: Functions for main menu
using UnityEngine;
using UnityEngine.SceneManagement;
using FancyScrollView.CustomizationMenu;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private string[] PossibleLevels;

    private AchievementManager achievementManager;

    private PausedMenuManager PausedMenu;

    private HowToManager tutCanvas;

    //**********
    private CustomizationManager customizationManager;
    //**********

    private bool enableMouseDetection = true;
    private AsyncOperation m_currentSceneBeingLoaded = null;

    // Start is called before the first frame update
    void Start()
    {
        achievementManager = AchievementManager._instance;
        PausedMenu = PausedMenuManager._instance;
        tutCanvas = FindObjectOfType<HowToManager>();
        tutCanvas.gameObject.SetActive(false);

        TaggingManager taggingManagerOnMainMenu = FindObjectOfType<TaggingManager>();
        if(taggingManagerOnMainMenu) {
            taggingManagerOnMainMenu.InitializeTaggingManager();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (achievementManager.achievementActivated || PausedMenu.IsOpen1) 
        {
            enableMouseDetection = false;
        } 
        else 
        {
            enableMouseDetection = true;
        }

        if (enableMouseDetection) {
            MouseDetection();
        }
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
                        if(m_currentSceneBeingLoaded == null) {
                            Debug.Log("Play the game!");
                            PausedMenuManager._instance.FadeIn(PausedMenuManager._instance.fadeTime);
                            m_currentSceneBeingLoaded = SceneManager.LoadSceneAsync(PossibleLevels[Random.Range(0, PossibleLevels.Length)]);
                        }
                        break;
                    case "Customization":
                        achievementManager.OpenCustomization();
                        Debug.Log("Open customization menu.");
                        break;
                    case "Achievements":
                        tutCanvas.gameObject.SetActive(true);
                        tutCanvas.Next();
                        break;
                    case "Options":
                        PausedMenu.EnablePausedMenu();
                        Debug.Log("Open option menu.");
                        break;
                    default:
                        
                        break;
                }
            }
        }
    }
}
