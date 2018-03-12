using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

// TODO:
// - Handle cases where someone renames a local folder. Should be marked with a red
//   color to show it's not valid.
// - Add safety checks to prevent the user from setting the same folder as both
//   local and remote. Other safety checks?
// - Performance tuning. Seems like it's a bit slow the first time you open the
//   inspector. Could it be threaded? Show a progress bar per folder?

namespace Syng {

    [CustomEditor(typeof(FolderSyncConfig))]
    public class FolderSyncConfigInspector : Editor {

        private class FolderData {
            public string   icon;
            public float    offsetIconY;
            public float    offsetLabelY;
            public string   folderSelect;
        }

        private Color       lightGray = new Color(0.698f, 0.698f, 0.698f);
        private Color       lightRed = new Color(1f, 0.65f, 0.65f);

        private Vector2             scrollPos;
        private GUIStyle            labelBGStyle;
        private GUIStyle            labelLightGray;
        private GUIStyle            labelRed;
        private GUIStyle            labelOrange;
        private GUIStyle            labelGreen;
        private GUIStyle            toolbarStyle;
        private GUIStyle            rowStyle;
        private GUIStyle            folderBg;
        private GUIStyle            delBtn;
        private Texture2D           bgRed;
        private Texture2D           bgDarkRed;
        private Texture2D           bgDarkGray;
        private bool                deleteFolders;
        private List<int>           delFolders;
        private FolderData          folderLocalData;
        private FolderData          folderRemoteData;
        private FolderSyncData[]    syncData;

        void OnEnable() {
            folderLocalData = new FolderData {
                icon = "Folder Icon",
                offsetIconY = 2,
                offsetLabelY = 3,
                folderSelect = "Select a local folder"
            };
            folderRemoteData = new FolderData {
                icon = "d_CloudConnect",
                offsetIconY = 3,
                offsetLabelY = 3,
                folderSelect = "Select a remote folder"
            };

            delFolders = new List<int>();
            deleteFolders = false;

            bgRed = new Texture2D(1,1);
            bgRed.SetPixels(new Color[] {new Color(0.384f, 0f, 0f)});
            bgRed.Apply();

            bgDarkRed = new Texture2D(1,1);
            bgDarkRed.SetPixels(new Color[] {new Color(0.16f, 0f, 0f)});
            bgDarkRed.Apply();

            bgDarkGray = new Texture2D(1,1);
            bgDarkGray.SetPixels(new Color[] {new Color(0.16f, 0.16f, 0.16f)});
            bgDarkGray.Apply();

            UpdateSyncData();
        }

        private void UpdateSyncData() {
            syncData = FolderSyncHelper.GetSyncStatus(serializedObject);
        }

        private void AddSyncFolders() {
            FolderSyncHelper.AddSyncFolders(serializedObject);
            UpdateSyncData();
        }

        private void AddFolder(int i, string prop, string val) {
            FolderSyncHelper.AddFolder(serializedObject, i, prop, val);
            UpdateSyncData();
        }

        private void SyncAll() {
            FolderSyncHelper.Sync(serializedObject);
            UpdateSyncData();
        }

        private void DeleteFolders() {
            FolderSyncHelper.DeleteFolders(serializedObject, delFolders);
            delFolders = new List<int>();
            deleteFolders = false;
        }

        private void Preview() {
            FolderSyncHelper.PreviewSync(serializedObject);
        }

        private int numFolders {
            get {
                return FolderSyncHelper.NumFolders(serializedObject);
            }
        }

        public override void OnInspectorGUI() {
            // Recreate the style object on each GUI call. Some of the
            // GUI styles depend on internally defined GUI styles which are
            // not available in OnEnable
            CreateStyles();

            EditorGUILayout.BeginHorizontal(toolbarStyle);
            {
                if (numFolders > 0) {
                    if (ToolbarButton("Sync all", 64f)) {
                        SyncAll();
                    }

                    if (RefreshButton()) {
                        UpdateSyncData();
                    }

                    // if (GUILayout.Button("Preview", toolbarStyle)) {
                    //     Preview();
                    // }
                }

                GUILayout.FlexibleSpace();
                if (ToolbarButton("Delete folders", 80f, lightRed)) {
                    deleteFolders = !deleteFolders;
                    delFolders = new List<int>();
                }
                if (ToolbarButton("Add folder", 72f)) {
                    AddSyncFolders();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                for (int i=0; i<numFolders; i++) {
                    FolderItem(i);
                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (delFolders.Count > 0) {
                if (GUILayout.Button("Delete marked folders", GUILayout.Height(32f))) {
                    DeleteFolders();
                }
            }

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void FolderItem(int index) {
            if (delFolders.Contains(index)) {
                EditorGUILayout.BeginVertical(folderBg);
            } else {
                EditorGUILayout.BeginVertical();
            }
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        FolderRow(index, "localFolders", folderLocalData);
                        FolderRow(index, "remoteFolders", folderRemoteData);
                    }
                    EditorGUILayout.EndVertical();

                    DeleteButton(index);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                SyncStatus(index);
            }
            EditorGUILayout.EndVertical();
        }

        private void FolderRow(int i, string propName, FolderData data) {
            SerializedProperty folder = serializedObject
                .FindProperty(propName)
                .GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = !deleteFolders;
                string newFolder = "";
                if (FileBrowseButton(data.folderSelect, out newFolder)) {
                    AddFolder(i, propName, newFolder);
                }
                GUI.enabled = true;
                GUIContent iconLocal = new GUIContent(EditorGUIUtility.FindTexture(data.icon));

                rowStyle.contentOffset = new Vector2(0f, data.offsetIconY);
                EditorGUILayout.LabelField(iconLocal, rowStyle, GUILayout.Width(20f));

                rowStyle.contentOffset = new Vector2(0f, data.offsetLabelY);
                EditorGUILayout.LabelField(FolderDisplay(folder.stringValue), rowStyle);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SyncStatus(int index) {
            labelBGStyle.normal.background = delFolders.Contains(index)
                ? bgDarkRed
                : bgDarkGray;

            EditorGUILayout.BeginHorizontal(labelBGStyle);
            {
                FolderSyncData data = syncData[index];
                Label(data.added.Count      + " added",
                      deleteFolders ? labelLightGray : labelGreen);
                Label(data.modified.Count   + " changed",
                      deleteFolders ? labelLightGray : labelOrange);
                Label(data.deleted.Count    + " deleted",
                      deleteFolders ? labelLightGray : labelRed);
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool FileBrowseButton(string title, out string folder) {
            if (GUILayout.Button("...", GUILayout.Width(32f))) {
                folder = EditorUtility.OpenFolderPanel(title, Application.dataPath, "");
                return true;
            }
            folder = "";
            return false;
        }

        private void DeleteButton(int folder) {
            if (deleteFolders) {
                bool del = delFolders.Contains(folder);
                string icon = del ? "Collab" : "CollabNew";
                if (GUILayout.Button(EditorGUIUtility.FindTexture(icon), delBtn)) {
                    if (del) {
                        delFolders.Remove(folder);
                    } else {
                        delFolders.Add(folder);
                    }
                }
            }
        }

        private bool RefreshButton() {
            Texture2D t2d = EditorGUIUtility.FindTexture("d_preAudioLoopOff");
            GUIContent content = new GUIContent(t2d);
            return GUILayout.Button(content,
                                    toolbarStyle,
                                    GUILayout.Height(20f));
        }

        private bool ToolbarButton(string label, float width) {
            return ToolbarButton(label, width, GUI.color);
        }

        private bool ToolbarButton(string label, float width, Color col) {
            Color c = GUI.color;
            GUI.color = col;
            bool val = GUILayout.Button(label, toolbarStyle, GUILayout.Width(width));
            GUI.color = c;
            return val;
        }

        private void CreateStyles() {
            labelBGStyle = new GUIStyle("box");
            labelBGStyle.stretchWidth = true;
            labelBGStyle.normal.background = bgDarkGray;

            labelLightGray = LabelStyle(lightGray);
            labelRed = LabelStyle(Color.red);
            labelOrange = LabelStyle(new Color(1f, 0.56f, 0f));
            labelGreen = LabelStyle(Color.green);

            toolbarStyle = new GUIStyle(EditorStyles.toolbarButton);
            toolbarStyle.fixedHeight = 24f;
            toolbarStyle.fontStyle = FontStyle.Bold;

            rowStyle = new GUIStyle();
            rowStyle.margin = new RectOffset();
            rowStyle.normal.textColor = lightGray;

            folderBg = new GUIStyle();
            folderBg.normal.background = bgRed;

            delBtn = new GUIStyle("button");
            delBtn.fixedWidth = 32f;
            delBtn.fixedHeight = 24f;
        }

        private GUIStyle LabelStyle(Color col) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = col;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }

        private void Label(string label, GUIStyle style) {
            GUIContent content = new GUIContent(label);
            Vector2 size = style.CalcSize(content);
            EditorGUILayout.LabelField(content,
                                       style,
                                       GUILayout.MaxWidth(size.x + 10f));
        }

        // TODO: Put in helper class
        // Would be cool to be able to autselect the right pref panel
        // Take a look at the source code. Can use reflection to get the
        // section list and iterate it until we find the right one, and then
        // set the current section int.
        private void OpenUnityPreferences() {
            Assembly asm = Assembly.GetAssembly(typeof(EditorWindow));
            Type prefType = asm.GetType("UnityEditor.PreferencesWindow");
            MethodInfo method = prefType.GetMethod("ShowPreferencesWindow",
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Static);
            method.Invoke(null, null);
        }

        static private string Capitalize(string str) {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        static private string FolderDisplay(string folder) {
            string[] parts = folder.Split('/');
            StringBuilder sb = new StringBuilder();
            for (int i=parts.Length-1; i>=0; i--) {
                if (parts[i] == "Assets") {
                    sb.Insert(0, "Assets/");
                    break;
                }
                if (parts[i] == "Dropbox") {
                    sb.Insert(0, "Dropbox/");
                    break;
                }
                sb.Insert(0, parts[i] + "/");
            }
            return sb.ToString();
        }
    }
}
