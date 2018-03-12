using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Syng {

    public class FolderSyncData {
        public List<string> modified;
        public List<string> added;
        public List<string> deleted;

        public FolderSyncData() {
            modified = new List<string>();
            added = new List<string>();
            deleted = new List<string>();
        }
    }

    static public class FolderSyncHelper {

        static public void Sync(SerializedObject so) {
            FolderSyncData[] sync = GetSyncStatus(so);
            SerializedProperty local = GetLocalFolders(so);
            SerializedProperty remote = GetRemoteFolders(so);

            int folders = sync.Length;

            for (int i=0; i<folders; i++) {
                FolderSyncData data = sync[i];
                string remoteFolder = remote.GetArrayElementAtIndex(i).stringValue;
                string localFolder = local.GetArrayElementAtIndex(i).stringValue;
                int totalFiles = data.added.Count + data.modified.Count + data.deleted.Count;

                int al = data.added.Count;
                for (int a=0; a<al; a++) {
                    string target = localFolder + data.added[a].Replace(remoteFolder, "");
                    File.Copy(data.added[a], target, true);
                    Progressbar(remoteFolder, i, folders, a / totalFiles);
                }

                int ml = data.modified.Count;
                for (int m=0; m<ml; m++) {
                    string target = localFolder + data.modified[m].Replace(remoteFolder, "");
                    File.Copy(data.modified[m], target, true);
                    Progressbar(remoteFolder, i, folders, (m + al) / totalFiles);
                }

                int dl = data.deleted.Count;
                for (int d=0; d<dl; d++) {
                    File.Delete(data.deleted[d]);
                    Progressbar(localFolder, i, folders, (d + al + ml) / totalFiles);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        static private void Progressbar(string folder, int f, int ftot, float progress) {
            EditorUtility.DisplayProgressBar("Synchronizing",
                                             "Processing " + folder +
                                             " (" + f + "/" + ftot + ")",
                                             progress);
        }

        static public void PreviewSync(SerializedObject so) {
            FolderSyncData[] sync = GetSyncStatus(so);

            foreach (FolderSyncData data in sync) {

                // Added
                int al = data.added.Count;
                D.Log(D.Green("ADD ("+al+")"));
                for (int a=0; a<al; a++) {
                    D.Log(D.Green("[ADD]"), data.added[a]);
                }

                // Modified
                int ml = data.modified.Count;
                D.Log(D.Orange("MODIFY ("+ml+")"));
                for (int m=0; m<ml; m++) {
                    D.Log(D.Orange("[MOD]"), data.modified[m]);
                }

                // Deleted
                int dl = data.deleted.Count;
                D.Log(D.Red("DELETE ("+dl+")"));
                for (int d=0; d<dl; d++) {
                    D.Log(D.Red("[DEL]"), data.deleted[d]);
                }

            }
        }

        static public void AddSyncFolders(SerializedObject so) {
            SerializedProperty local = GetLocalFolders(so);
            int end = local.arraySize;
            local.InsertArrayElementAtIndex(end);
            local.GetArrayElementAtIndex(end).stringValue = "Undefined";

            SerializedProperty remote = GetRemoteFolders(so);
            remote.InsertArrayElementAtIndex(end);
            remote.GetArrayElementAtIndex(end).stringValue = "Undefined";

            so.ApplyModifiedProperties();
        }

        static public void AddFolder(SerializedObject so, int i, string prop, string val) {
            SerializedProperty folder = so
                .FindProperty(prop)
                .GetArrayElementAtIndex(i);
            folder.stringValue = val;

            so.ApplyModifiedProperties();
        }

        static public void DeleteFolders(SerializedObject so, List<int> folders) {
            for (int i=folders.Count-1; i>=0; i--) {
                GetLocalFolders(so).DeleteArrayElementAtIndex(i);
                GetRemoteFolders(so).DeleteArrayElementAtIndex(i);
            }
            so.ApplyModifiedProperties();
        }

        static public FolderSyncData[] GetSyncStatus(SerializedObject so) {
            List<FolderSyncData> dataList = new List<FolderSyncData>();
            int numFolders = NumFolders(so);
            SerializedProperty locals = GetLocalFolders(so);
            SerializedProperty remotes = GetRemoteFolders(so);

            for (int i=0; i<numFolders; i++) {
                SerializedProperty localProp = locals.GetArrayElementAtIndex(i);
                bool localUndef = localProp.stringValue == "Undefined";
                List<string> localFiles = localUndef
                    ? new List<string>()
                    : GetFiles(localProp.stringValue);

                SerializedProperty remoteProp = remotes.GetArrayElementAtIndex(i);
                bool remoteUndef = remoteProp.stringValue == "Undefined";
                List<string> remoteFiles = remoteUndef
                    ? new List<string>()
                    : GetFiles(remoteProp.stringValue);

                FolderSyncData data = new FolderSyncData();

                for (int n=0; n<remoteFiles.Count; n++) {
                    string remoteFile = remoteFiles[n];
                    string curFile = localProp.stringValue +
                        remoteFile.Replace(remoteProp.stringValue, "");

                    int fi = localFiles.IndexOf(curFile);
                    if (fi >= 0) {
                        string localFile = localFiles[fi];
                        DateTime localDate = File.GetLastWriteTimeUtc(localFile);
                        DateTime remoteDate = File.GetLastWriteTimeUtc(remoteFile);
                        // dateDiff < 0 : remote is older than local
                        // dateDiff = 0 : remote is same as local
                        // dateDiff > 0 : remote is newer than local
                        int dateDiff = DateTime.Compare(remoteDate, localDate);
                        if (dateDiff > 0) {
                            data.modified.Add(remoteFile);
                        }
                    } else {
                        if (!localUndef) {
                            data.added.Add(remoteFile);
                        }
                    }
                }

                for (int j=0; j<localFiles.Count; j++) {
                    string localFile = localFiles[j];
                    string curFile = remoteProp.stringValue +
                        localFile.Replace(localProp.stringValue, "");
                    if (!remoteFiles.Contains(curFile)) {
                        data.deleted.Add(localFile);
                    }
                }

                dataList.Add(data);
            }
            return dataList.ToArray();
        }

        static private string RemoteToLocal(SerializedObject so, int i, string filePath) {
            SerializedProperty locals = GetLocalFolders(so);
            SerializedProperty localProp = locals.GetArrayElementAtIndex(i);

            SerializedProperty remotes = GetRemoteFolders(so);
            SerializedProperty remoteProp = remotes.GetArrayElementAtIndex(i);

            return localProp.stringValue + filePath.Replace(remoteProp.stringValue, "");
        }

        static public int NumFolders(SerializedObject so) {
            return so.FindProperty("localFolders").arraySize;
        }

        static SerializedProperty GetFiles(SerializedObject so) {
            return so.FindProperty("files");
        }

        static SerializedProperty GetLocalFolders(SerializedObject so) {
            return so.FindProperty("localFolders");
        }

        static SerializedProperty GetRemoteFolders(SerializedObject so) {
            return so.FindProperty("remoteFolders");
        }

        static private List<string> GetFiles(string folder) {
            List<string> fileNames = new List<string>();
            DirSearch(folder, fileNames);
            return fileNames;
        }

        static private void DirSearch(string dir, List<string> files) {
            if (!Directory.Exists(dir)) {
                return;
            }

            foreach (string d in Directory.GetDirectories(dir)) {
                foreach (string f in Directory.GetFiles(d)) {
                    if (f.Substring(f.Length - 5) == ".meta") {
                        continue;
                    }
                    if (f.Substring(f.Length - 6) == ".asset") {
                        continue;
                    }
                    if (f.Substring(f.Length - 9) == ".DS_Store") {
                        continue;
                    }
                    files.Add(f);
                }
                DirSearch(d, files);
            }
        }
    }
}
