using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    [Header("Tagging Configuration")]
    public float isTagSpeed = 6f;
    public float isNotTagSpeed = 4f;
    [Tooltip("When a player is tagged, everyone within the radius will be knocked back")]
    public float knockbackRadius = 10f;
    public float knockbackForce = 5f;

    [Header("Knockback Configuration")]
    public float knockbackDelayTime = 0.5f;

    private int m_currentPlayerTaggingID;
    public int WhoIsTag {
        get {
            return m_currentPlayerTaggingID;
        }
    }

    public delegate void DelegateWithTaggingIdentifier(TaggingIdentifier identifier);
    public event DelegateWithTaggingIdentifier OnPlayerWasTagged;

    private List<TaggingIdentifier> m_playersIdentifiers;
    private SpawnPointManager m_spawnPointManager;
    private UIManager m_UIManager;

    private void Awake() {
        m_spawnPointManager = FindObjectOfType<SpawnPointManager>();
        m_UIManager = FindObjectOfType<UIManager>();

        if(m_spawnPointManager == null) {
            Debug.LogError($"There is no spawn point manager in the scene!");
        }
    }

    private void Start() {
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>().ToList();
        PlayerInfoUI[] playerInfoUI = FindObjectsOfType<PlayerInfoUI>();

        // Inject all PlayerInfoUI to Players
        if (playerInfoUI.Length == m_playersIdentifiers.Count) {
            for (int i = 0; i < m_playersIdentifiers.Count; i++) {
                m_playersIdentifiers[i].PlayerInfo = playerInfoUI[i];
                m_playersIdentifiers[i].PlayerName = "Jerry";
            }
        } else {
            Debug.LogWarning($"There are more or less PlayerInfo scripts than Players in the scene!! You have {m_playersIdentifiers.Count} players and {playerInfoUI.Length} info scripts!");
            foreach (PlayerInfoUI infoUI in playerInfoUI) {
                infoUI.gameObject.SetActive(false);
            }
        }

        // Inject all ids into tagging identifiers
        for (int i = 0; i < m_playersIdentifiers.Count; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;

            // Subscribing every player's UpdateWhoIsTag function into the manager
            OnPlayerWasTagged += m_playersIdentifiers[i].UpdateWhoIsTag;
        }

        // TODO Select a Random one to start as tag
        TaggingIdentifier initialTagger = GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>();
        PlayerWasTagged(initialTagger);
        foreach(TaggingIdentifier notItPlayer in GetAllPlayersThatAreNotIt()) {
            notItPlayer.SetAsNotTag();
        }
    }

    private void UpdateScoreboard() {
        m_playersIdentifiers.Sort((leftHandSide, rightHandSide) => {
            return leftHandSide.TimeAsTag.CompareTo(rightHandSide.TimeAsTag);
        });
    }

    /// <summary>
    /// <para>Communicates to the Tagging Manager a specific player was tagged.</para>
    /// </summary>
    /// <param name="_whoIsTag">Player that was tagged.</param>
    /// <param name="_knockbackEffect">Knockback players or not.</param>
    public void PlayerWasTagged(TaggingIdentifier _whoIsTag, bool _knockbackEffect = false) {
        m_currentPlayerTaggingID = _whoIsTag.PlayerIdentifier;
        _whoIsTag.SetAsTagging();
        OnPlayerWasTagged?.Invoke(_whoIsTag);
        m_UIManager.ShowPlayerTaggedText(_whoIsTag.PlayerName, knockbackDelayTime * 2f);

        if (_knockbackEffect) {
            StartCoroutine(KnockbackAllPlayerRoutine());
        }
    }

    private IEnumerator KnockbackAllPlayerRoutine() {
        Time.timeScale = 0.25f;

        Transform whoIsTag = GetItTransform();
        foreach (TaggingIdentifier player in m_playersIdentifiers) {
            if (player.PlayerIdentifier != m_currentPlayerTaggingID && Vector3.Distance(player.transform.position, whoIsTag.position) < knockbackRadius) {
                Vector3 knockbackDirection = (player.transform.position - GetItTransform().position);
                player.KnockbackPlayer(Color.magenta, knockbackDirection.normalized * knockbackForce);
            }
        }

        yield return new WaitForSecondsRealtime(knockbackDelayTime * 2f);
        m_spawnPointManager.RespawnAllPlayers();
    }

    /// <summary>
    /// <para>Get the transform for the player that is currently TAG.</para>
    /// </summary>
    /// <returns>TAG player transform.</returns>
    public Transform GetItTransform() {
        foreach(TaggingIdentifier identifier in m_playersIdentifiers) {
            if(identifier.PlayerIdentifier == m_currentPlayerTaggingID) {
                return identifier.transform;
            }
        }

        return null;
    }

    /// <summary>
    /// <para>Returns all players that are not TAG.</para>
    /// </summary>
    /// <returns>List of all players that are no TAG</returns>
    public TaggingIdentifier[] GetAllPlayersThatAreNotIt() {
        List<TaggingIdentifier> playersThatAreNotIt = new List<TaggingIdentifier>();

        foreach(TaggingIdentifier player in m_playersIdentifiers) {
            if(player.PlayerIdentifier != m_currentPlayerTaggingID) {
                playersThatAreNotIt.Add(player);
            }
        }

        return playersThatAreNotIt.ToArray();
    }
}
