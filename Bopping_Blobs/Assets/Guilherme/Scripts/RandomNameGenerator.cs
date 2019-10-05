using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomNameGenerator {
    private List<string> m_allPossibleNames = new List<string>() {
        "Aaron",
        "Aaryn",
        "Andrew",
        "Andy",
        "Alex",
        "Alexander",
        "Art",
        "Arty",
        "Ashleigh",
        "Asher",
        "Ash",
        "Alexei",
        "Arthur",
        "Al",
        "Avery",
        "Aiden",
        "Anthony"
    };

    public string GetRandomName() {
        return m_allPossibleNames[Random.Range(0, m_allPossibleNames.Count - 1)];
    }
}
