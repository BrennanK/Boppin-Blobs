using UnityEngine;

public class SaveGameManager : MonoBehaviour {
    public static SaveGameManager instance;

    private const string HAS_SAVED_GAME = "HAS_SAVED_GAME";
    private SaveData m_currentLoadedSaveData;
    public SaveData SaveDataInfo {
        get {
            return m_currentLoadedSaveData;
        }
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        m_currentLoadedSaveData = new SaveData();
        if (PlayerPrefs.HasKey(HAS_SAVED_GAME)) {
            LoadSavedGame();
        } else {
            CreateSaveGame();
        }
    }

    public void IncrementSavedData(PlayerProfile _matchPlayerProfile, int _moneyGainedOnMatch) {
        m_currentLoadedSaveData.MoneyInfo += _moneyGainedOnMatch;
        m_currentLoadedSaveData.playerProfile.IncrementProfileData(_matchPlayerProfile);
        SaveGame();
    }

    private void CreateSaveGame() {
        PlayerPrefs.SetString(HAS_SAVED_GAME, JsonUtility.ToJson(m_currentLoadedSaveData));
    }

    private void SaveGame() {
        Debug.Log("<color=red>Game Saved...</color>");
        PlayerPrefs.SetString(HAS_SAVED_GAME, JsonUtility.ToJson(m_currentLoadedSaveData));
    }

    private void LoadSavedGame() {
        Debug.Log("<color=red>Loading Game...</color>");
        Debug.Log($"Loaded Game: {PlayerPrefs.GetString(HAS_SAVED_GAME)}");
        m_currentLoadedSaveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(HAS_SAVED_GAME));
    }
}
