using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    private TaggingIdentifier[] m_playersIdentifiers;
    private List<TaggingIdentifier> m_playersIdentifiersList;

    public delegate void DelegateWithTaggingIdentifier(TaggingIdentifier identifier);
    public event DelegateWithTaggingIdentifier OnPlayerWasTagged;

    private int m_currentPlayerTaggingID;
    public int WhoIsTag {
        get {
            return m_currentPlayerTaggingID;
        }
    }

    // TODO Update UI Scoreboard
    private void Start() {
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>();
        m_playersIdentifiersList = m_playersIdentifiers.ToList();

        // Inject all ids into tagging identifiers
        for(int i = 0; i < m_playersIdentifiers.Length; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;

            // Every Player Subscribes their UpdateWhoIsTag function into the manager
            OnPlayerWasTagged += m_playersIdentifiers[i].UpdateWhoIsTag;
        }

        // TODO Select a Random one to start as tag
        TaggingIdentifier initialTagger = GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>();
        initialTagger.SetAsTagging();
        m_currentPlayerTaggingID = initialTagger.PlayerIdentifier;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
            UpdateScoreboard();
        }
    }

    private void UpdateScoreboard() {
        m_playersIdentifiersList.Sort((leftHandSide, rightHandSide) => {
            return leftHandSide.TimeAsTag.CompareTo(rightHandSide.TimeAsTag);
        });

        foreach(TaggingIdentifier identifier in m_playersIdentifiersList) {
            Debug.Log($"{identifier.gameObject.name}: {identifier.TimeAsTag} ");
        }
    }

    public void PlayerWasTagged(TaggingIdentifier _whoIsTag) {
        // TODO Have Visual Cue for the player who is cued
        m_currentPlayerTaggingID = _whoIsTag.PlayerIdentifier;
        OnPlayerWasTagged?.Invoke(_whoIsTag);
    }
}
