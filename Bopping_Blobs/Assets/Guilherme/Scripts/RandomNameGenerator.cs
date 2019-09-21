using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomNameGenerator {
    private List<string> m_allPossibleNames;

    public RandomNameGenerator(string _path) {
        StreamReader reader = new StreamReader(_path);
        m_allPossibleNames = new List<string>();

        reader.ReadLine();
        while(!reader.EndOfStream) {
            string line = reader.ReadLine().Split(',')[1];
            string name = line.Substring(1, line.Length - 2);
            m_allPossibleNames.Add(name);
        }

        // Avoiding leaks
        reader.Close();
    }

    public string GetRandomName() {
        return m_allPossibleNames[Random.Range(0, m_allPossibleNames.Count - 1)];
    }
}
