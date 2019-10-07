// Author: Yi Li
// Date: 09/17/2019
// Purpose: Functions for Paused Menu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PausedMenuManager : MonoBehaviour
{
	[Header("Audio Controllers")]
	[SerializeField] AudioSource BGMPlayer;
	[SerializeField] AudioSource SFXPlayer;
	[SerializeField] AudioMixer AudioController;

	[Header("Option Sliders")]
	[SerializeField] Slider BGMSlider;
	[SerializeField] Slider SFXSlider;

	[Header("Other References")]
	[SerializeField] GameObject OptionButton;
	[SerializeField] Animator Paused;
    [SerializeField] Animator Credits;
    [SerializeField] GameObject ReturnToMainMenuButton;
    [SerializeField] GameObject CreditsButton;

	[Header("BGM List")]
	[SerializeField] AudioClip MainMenu;
	[SerializeField] AudioClip GardenLevel;
    [SerializeField] AudioClip[] LevelsMusic;

	[Header("SFX List")]
	[SerializeField] AudioClip[] SFXGroup;

	// Check if cannot open
	private bool CanNotOpen = false;
	// Check if is open
	private bool IsOpen = false;

	// Singleton
	public static PausedMenuManager _instance;

    public bool IsOpen1 { get => IsOpen; set => IsOpen = value; }

    #region Initilize & Enable/Disable UI
    private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_instance = this;
		}

		DontDestroyOnLoad(gameObject);
	}
	// Start is called before the first frame update
	void Start()
	{
		BGMSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
		SFXSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);

		AudioController.SetFloat("BGMMixer", Mathf.Log10(BGMSlider.value) * 20);
		AudioController.SetFloat("SFXMixer", Mathf.Log10(SFXSlider.value) * 20);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !CanNotOpen)
		{
			StartCoroutine(CallPauseMenu(0.2f));
		}
	}

	// Enable Paused Menu UI with a delay
	private IEnumerator CallPauseMenu(float waitTime)
	{
        IsOpen1 = !IsOpen1;

        if (IsOpen1)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;

		Paused.SetTrigger("TriggerPauseMenu");
		PlaySFX(0);
		yield return new WaitForSeconds(waitTime);
	}

    private IEnumerator CallCreditsMenu(float waitTime)
    {
        Credits.SetTrigger("TriggerCredits");
        PlaySFX(0);
        yield return new WaitForSeconds(waitTime);
    }

    // Enable Paused Menu By Button
    public void EnablePausedMenu()
	{
        StartCoroutine(CallPauseMenu(0.2f));
	}

    public void EnablePausedMenu(bool _pauseStatusToSet) {
        if(IsOpen == _pauseStatusToSet) {
            return;
        }

        StartCoroutine(CallPauseMenu(0.2f));
    }

	// Return to Main Menu By Button
	public void ReturnToMainMenu()
	{
		StartCoroutine(CallPauseMenu(0.2f));
		SceneManager.LoadSceneAsync("MainMenu");
	}

    // Enable Credits UI
    public void EnableCreditsMenu()
    {
        StartCoroutine(CallCreditsMenu(0.2f));
    }
	#endregion

	#region Load Scene Event & Audio Controller

	public void SetMusic()
	{
		AudioController.SetFloat("BGMMixer", Mathf.Log10(BGMSlider.value) * 20);
		PlayerPrefs.SetFloat("MusicVolume", BGMSlider.value);
	}

	public void SetSound()
	{
		AudioController.SetFloat("SFXMixer", Mathf.Log10(SFXSlider.value) * 20);
		PlayerPrefs.SetFloat("SoundVolume", SFXSlider.value);
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	// Play Background Music when loaded a scene
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		switch (scene.name)
		{
			case "MainMenu":
				BGMPlayer.Stop();
				BGMPlayer.clip = MainMenu;
				BGMPlayer.Play();
                OptionButton.SetActive(false);
                ReturnToMainMenuButton.SetActive(false);
                CreditsButton.SetActive(true);
				break;
            /*
            case "Garden_v2":
                BGMPlayer.Stop();
                BGMPlayer.clip = GardenLevel;
                BGMPlayer.Play();
                OptionButton.SetActive(true);
                ReturnToMainMenuButton.SetActive(true);
                CreditsButton.SetActive(false);
                break;
            */
            default:
                BGMPlayer.Stop();
                BGMPlayer.clip = LevelsMusic[Random.Range(0, LevelsMusic.Length)];
                BGMPlayer.Play();
                OptionButton.SetActive(true);
                ReturnToMainMenuButton.SetActive(true);
                CreditsButton.SetActive(false);
				break;
		}
	}

	/// <summary>
	/// 0 = Button Click, 1 = ?
	/// </summary>
	/// <param name="clipnumber">Choose a SFX to play</param>
	public void PlaySFX(int clipnumber)
	{
		SFXPlayer.Stop();
		SFXPlayer.PlayOneShot(SFXGroup[clipnumber]);
	}

    /// <summary>
    /// Play a Sound Effect from a Sound Clip
    /// </summary>
    /// <param name="_clip">Clip to be played</param>
    public void PlaySFX(AudioClip _clip) {
        SFXPlayer.PlayOneShot(_clip);
    }

	#endregion
}
