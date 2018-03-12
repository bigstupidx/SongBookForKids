using UnityEngine;
using System;

namespace Syng {

    [CreateAssetMenu(fileName = "FolderSyncConfig",
                     menuName = "Syng/Folder Sync Config",
                     order = 2)]
    [Serializable]
    public class FolderSyncConfig : ScriptableObject {
        [SerializeField]
        private string[]        localFolders;
        [SerializeField]
        private string[]        remoteFolders;
    }

}
