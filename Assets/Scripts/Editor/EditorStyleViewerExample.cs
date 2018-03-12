﻿using UnityEditor;
using UnityEngine;

public class EditorStyleViewerExample : EditorWindow {

    private bool[] toggles = new bool[5];

    [MenuItem("Window/Style Viewer Example")]
    public static void OpenWindow() {
        EditorStyleViewerExample window = EditorWindow.GetWindow<EditorStyleViewerExample>();
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("  Options","LargeLabel");

        GUILayout.BeginVertical("HelpBox");
        GUILayout.BeginHorizontal();

        EditorGUILayout.TextField("Search","","ToolbarSeachTextField");
        GUILayout.Button("Close","ToolbarSeachCancelButton");

        GUILayout.Space(20);

        GUILayout.Button("minibuttonleft","minibuttonleft");
        GUILayout.Button("minibuttonright","minibuttonright");

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("Advanced","MiniToolbarButton")) {
            toggles[2] = !toggles[2];
        }

        if (toggles[2]) {

            GUILayout.BeginVertical("ProgressBarBack");
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("WhiteMiniLabel","WhiteMiniLabel");
            GUILayout.Label("ErrorLabel","ErrorLabel");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");

            GUILayout.BeginHorizontal();
            GUILayout.Button("OL Plus","OL Plus");
            GUILayout.Button("OL Minus","OL Minus");
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Button("OL Title","OL Title");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Button("OL Titleleft","OL Titleleft");
            GUILayout.Button("OL Titlemid","OL Titlemid");
            GUILayout.Button("OL Titleright","OL Titleright");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal("GroupBox");
            GUILayout.Label("CN EntryInfo","CN EntryInfo");
            GUILayout.Label("CN EntryWarn","CN EntryWarn");
            GUILayout.Label("CN EntryError","CN EntryError");
            GUILayout.EndHorizontal();

            GUILayout.Button("flow node 1", "flow node 1");
            GUILayout.Button("flow node 1 on", "flow node 1 on");

            EditorGUILayout.Popup(0, new string[] {"abc", "def", "ghi"});
            EditorGUILayout.Popup(0, new string[] {"abc", "def", "ghi"}, "ToolbarPopup");
            EditorGUILayout.Popup(0, new string[] {"abc", "def", "ghi"}, "Popup");
        }

        GUILayout.EndVertical();
    }
}
