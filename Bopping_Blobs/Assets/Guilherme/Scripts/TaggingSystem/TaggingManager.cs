using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    private List<TaggingIdentifier> m_playersIdentifiers;

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
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>().ToList();

        // Inject all ids into tagging identifiers
        for(int i = 0; i < m_playersIdentifiers.Count; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;

            // Subscribing every player's UpdateWhoIsTag function into the manager
            OnPlayerWasTagged += m_playersIdentifiers[i].UpdateWhoIsTag;
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

    public void PlayerWasTagged(TaggingIdentifier _whoIsTag) {
        // TODO Have Visual Cue for the player who is cued
        m_currentPlayerTaggingID = _whoIsTag.PlayerIdentifier;
        _whoIsTag.SetAsTagging();
        OnPlayerWasTagged?.Invoke(_whoIsTag);
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
