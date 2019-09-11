using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    private TaggingIdentifier[] m_playersIdentifiers;
    private List<TaggingIdentifier> m_playersIdentifiersList;

    // TODO Update UI Scoreboard
    private void Start() {
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>();
        m_playersIdentifiersList = m_playersIdentifiers.ToList();

        // Inject all ids into tagging identifiers
        for(int i = 0; i < m_playersIdentifiers.Length; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;
        }

        // TODO Select a Random one to start as tag
        GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>().SetAsTag();
    }

    // TODO Remove this
    private void Update() {
        if(Input.GetKeyDown(KeyCode.R)) {
            UpdateScoreboard();
        }
    }

    private void UpdateScoreboard() {
        m_playersIdentifiersList.Sort((leftHand, rightHand) => {
            return leftHand.TimeAsTag.CompareTo(rightHand.TimeAsTag);
        });
    }
}
