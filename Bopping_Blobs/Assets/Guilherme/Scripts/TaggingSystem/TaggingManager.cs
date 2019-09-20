using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    private List<TaggingIdentifier> m_playersIdentifiers;

    [Header("Tagging Configuration")]
    public float isTagSpeed = 6f;
    public float isNotTagSpeed = 4f;
    [Tooltip("When a player is tagged, everyone within the radius will be knocked back")]
    public float knockbackRadius = 10f;
    public float knockbackForce = 10f;

    public delegate void DelegateWithTaggingIdentifier(TaggingIdentifier identifier);
    public event DelegateWithTaggingIdentifier OnPlayerWasTagged;

    private int m_currentPlayerTaggingID;
    public int WhoIsTag {
        get {
            return m_currentPlayerTaggingID;
        }
    }

    private SpawnPointManager m_spawnPointManager;

    private void Awake() {
        m_spawnPointManager = FindObjectOfType<SpawnPointManager>();
    }

    // TODO Update UI Scoreboard
    private void Start() {
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>().ToList();
        PlayerInfoUI[] playerInfoUI = FindObjectsOfType<PlayerInfoUI>();

        // Inject all ids into tagging identifiers
        for(int i = 0; i < m_playersIdentifiers.Count; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;

            // Subscribing every player's UpdateWhoIsTag function into the manager
            OnPlayerWasTagged += m_playersIdentifiers[i].UpdateWhoIsTag;
        }

        // Inject all PlayerInfoUI to Players
        if(playerInfoUI.Length == m_playersIdentifiers.Count) {
            for(int i = 0; i < m_playersIdentifiers.Count; i++) {
                m_playersIdentifiers[i].AssignPlayerInfo(playerInfoUI[i]);
            }
        }

        // TODO Select a Random one to start as tag
        TaggingIdentifier initialTagger = GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>();
        PlayerWasTagged(initialTagger);
    }

    private void UpdateScoreboard() {
        m_playersIdentifiers.Sort((leftHandSide, rightHandSide) => {
            return leftHandSide.TimeAsTag.CompareTo(rightHandSide.TimeAsTag);
        });
    }

    public void PlayerWasTagged(TaggingIdentifier _whoIsTag, bool _knockbackEffect = false) {
        m_currentPlayerTaggingID = _whoIsTag.PlayerIdentifier;
        _whoIsTag.SetAsTagging();
        OnPlayerWasTagged?.Invoke(_whoIsTag);

        // TODO knockback and changing speed could be on the same loop
        foreach(TaggingIdentifier player in m_playersIdentifiers) {
            if(player.PlayerIdentifier != m_currentPlayerTaggingID) {
                player.SetAsNotTag();
            }
        }

        // TODO improve this
        // Knockback all players that are not tag
        
        if(_knockbackEffect) {
            StartCoroutine(KnockbackAllPlayerRoutine());
        }
        
    }

    // TODO knockback only within a distance from the player who was tagged
    private IEnumerator KnockbackAllPlayerRoutine() {
        Time.timeScale = 0.25f;

        Transform whoIsTag = GetItTransform();
        foreach (TaggingIdentifier player in m_playersIdentifiers) {
            if (player.PlayerIdentifier != m_currentPlayerTaggingID && Vector3.Distance(player.transform.position, whoIsTag.position) < knockbackRadius) {
                Vector3 knockbackDirection = (player.transform.position - GetItTransform().position);
                player.KnockbackPlayer(Color.magenta, knockbackDirection.normalized * knockbackForce);
            }
        }

        yield return new WaitForSecondsRealtime(1.0f);
        m_spawnPointManager.RespawnAllPlayersWithDelay(0.5f);
    }

    public Transform GetItTransform() {
        foreach(TaggingIdentifier identifier in m_playersIdentifiers) {
            if(identifier.PlayerIdentifier == m_currentPlayerTaggingID) {
                return identifier.transform;
            }
        }

        return null;
    }

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
