using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    [Header("King Mode Configuration")]
    public float baseSpeed = 10f;
    public float kingSpeedMultiplier = 0.85f;

    [Header("Knockback Configuration")]
    [Tooltip("When a player is tagged, everyone within the radius will be knocked back")]
    public float knockbackDelayTime = 2.0f;
    public float knockbackRadius = 10f;
    public float knockbackForce = 5f;

    private int m_currentPlayerTaggingID;
    public int WhoIsKing {
        get {
            return m_currentPlayerTaggingID;
        }
    }

    public Transform KingTransform {
        get {
            foreach (TaggingIdentifier identifier in m_playersIdentifiers) {
                if (identifier.PlayerIdentifier == m_currentPlayerTaggingID) {
                    return identifier.transform;
                }
            }

            return null;
        }
    }

    public delegate void DelegateWithTaggingIdentifier(TaggingIdentifier identifier);
    public event DelegateWithTaggingIdentifier OnPlayerWasTagged;

    private List<TaggingIdentifier> m_playersIdentifiers;
    public List<TaggingIdentifier> Players {
        get {
            m_playersIdentifiers.Sort((leftHandSide, rightHandSide) => {
                return rightHandSide.PlayerScore.CompareTo(leftHandSide.PlayerScore);
            });

            return m_playersIdentifiers;
        }
    }

    private UIManager m_UIManager;

    public void InitializeTaggingManager() {
        m_UIManager = FindObjectOfType<UIManager>();

        RandomNameGenerator randomNameGenerator = new RandomNameGenerator();

        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>().ToList();
        PlayerInfoUI[] playerInfoUI = FindObjectsOfType<PlayerInfoUI>();

        // Inject all PlayerInfoUI to Players
        if (playerInfoUI.Length == m_playersIdentifiers.Count) {
            for (int i = 0; i < m_playersIdentifiers.Count; i++) {
                m_playersIdentifiers[i].PlayerInfo = playerInfoUI[i];
                m_playersIdentifiers[i].PlayerName = randomNameGenerator.GetRandomName();
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
    }

    /// <summary>
    /// <para>Set the first player as tag, use this to initialize the game</para>
    /// </summary>
    public void StartTagging() {
        // TODO Select a Random one to start as tag
        TaggingIdentifier initialTagger = m_playersIdentifiers[Random.Range(0, m_playersIdentifiers.Count)];
        // TaggingIdentifier initialTagger = GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>();

        PlayerWasTagged(initialTagger);
        foreach (TaggingIdentifier notItPlayer in GetAllPlayersThatAreNotIt()) {
            notItPlayer.SetAsNotKing();
        }
    }

    /// <summary>
    /// <para>Communicates to the Tagging Manager a specific player was tagged.</para>
    /// </summary>
    /// <param name="_whoIsTag">Player that was tagged.</param>
    /// <param name="_knockbackEffect">Knockback players or not.</param>
    public void PlayerWasTagged(TaggingIdentifier _whoIsTag, bool _knockbackEffect = false) {
        m_currentPlayerTaggingID = _whoIsTag.PlayerIdentifier;
        _whoIsTag.SetAsKing();
        OnPlayerWasTagged?.Invoke(_whoIsTag);
        m_UIManager.ShowPlayerTaggedText(_whoIsTag.PlayerName, knockbackDelayTime);

        if (_knockbackEffect) {
            // TODO not have / 4.0f on the knockbackDelayTime
            StartCoroutine(KnockbackAllPlayerRoutine(knockbackDelayTime / 4.0f));
        }
    }

    private IEnumerator KnockbackAllPlayerRoutine(float _delayTime) {
        Transform whoIsTag = KingTransform;
        foreach (TaggingIdentifier player in m_playersIdentifiers) {
            if (player.PlayerIdentifier != m_currentPlayerTaggingID && Vector3.Distance(player.transform.position, whoIsTag.position) < knockbackRadius) {
                Vector3 knockbackDirection = (player.transform.position - KingTransform.position);

                // TODO fix knockbackforce magic number
                player.KnockbackPlayer(Color.magenta, knockbackDirection.normalized * knockbackForce * 3f, _delayTime);
            }
        }

        yield return new WaitForSecondsRealtime(_delayTime / 2.0f);
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

    /// <summary>
    /// <para>Deactivate all players controllers</para>
    /// </summary>
    public void FreezeAllPlayers() {
        foreach(TaggingIdentifier player in m_playersIdentifiers) {
            player.SetAsNotKing();
            player.DeactivatePlayer();
        }
    }

    /// <summary>
    /// <para>Activate all players controllers</para>
    /// </summary>
    public void EnableAllPlayers() {
        foreach(TaggingIdentifier player in m_playersIdentifiers) {
            player.ActivatePlayer();
        }
    }
}
