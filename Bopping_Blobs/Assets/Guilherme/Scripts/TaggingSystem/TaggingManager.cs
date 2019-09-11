using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggingManager : MonoBehaviour {
    private TaggingIdentifier[] m_playersIdentifiers;

    private void Start() {
        m_playersIdentifiers = FindObjectsOfType<TaggingIdentifier>();

        // Inject all ids into tagging identifiers
        for(int i = 0; i < m_playersIdentifiers.Length; i++) {
            m_playersIdentifiers[i].PlayerIdentifier = i;
            m_playersIdentifiers[i].taggingManager = this;
        }

        // TODO Select a Random one to start as tag
        GameObject.FindGameObjectWithTag("Player").GetComponent<TaggingIdentifier>().SetTag();
    }
}
