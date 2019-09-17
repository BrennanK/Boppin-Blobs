using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TaggingManager))]
public class TagManagerInspector : Editor {
    public override void OnInspectorGUI() {
        TaggingIdentifier[] identifiers = FindObjectsOfType<TaggingIdentifier>();
        TaggingManager taggingManager = FindObjectOfType<TaggingManager>();
        base.DrawDefaultInspector();

        GUILayout.Space(10);
        GUIStyle gStyle = new GUIStyle();
        gStyle.normal.background = MakeTex(200, 50, Color.gray);
        GUILayout.BeginHorizontal(gStyle);
        GUILayout.Label("Player Name", GUILayout.Width(200));
        GUILayout.Label("Player ID", GUILayout.Width(75));
        GUILayout.Label("Is It?", GUILayout.Width(50));
        GUILayout.Label("Time as It", GUILayout.Width(75));
        GUILayout.EndHorizontal();

        foreach (TaggingIdentifier identifier in identifiers) {
            bool isIt = identifier.PlayerIdentifier == taggingManager.WhoIsTag;

            if(isIt) {
                gStyle.normal.background = MakeTex(200, 50, Color.red);
            } else {
                gStyle.normal.background = MakeTex(200, 50, Color.green);
            }

            GUILayout.BeginHorizontal(gStyle);
            GUILayout.Label(identifier.gameObject.name, GUILayout.Width(200));
            GUILayout.Label(identifier.PlayerIdentifier.ToString(), GUILayout.Width(75));
            GUILayout.Label(isIt.ToString(), GUILayout.Width(50));
            GUILayout.Label(identifier.TimeAsTag.ToString(), GUILayout.Width(75));
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
    }

    private Texture2D MakeTex(int _width, int _height, Color _color) {
        Color[] pixels = new Color[_width * _height];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = _color;
        }

        Texture2D result = new Texture2D(_width, _height);
        result.SetPixels(pixels);
        result.Apply();

        return result;
    }
}
