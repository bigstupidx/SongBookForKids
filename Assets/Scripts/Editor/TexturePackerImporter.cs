using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class TPImporterWin : EditorWindow {

    private const string prefGenMesh = "TPI_GenMesh";
    private const string prefGenMaterial = "TPI_GenMaterial";
    private const string prefGenOBJ = "TPI_GenOBJ";
    private const string prefGenPrefab = "TPI_GenPrefab";

    private struct GenState {
        public bool genMesh;
        public bool genMat;
        public bool genOBJ;
        public bool genPrefab;
    }

    [MenuItem("Window/Texture Packer Importer")]
    public static void OpenTPImporter() {
        TPImporterWin window = EditorWindow.GetWindow<TPImporterWin>();
        window.titleContent = new GUIContent("Texture Packer Importer");
        window.Show();
    }

    void OnGUI() {
        UnityEngine.Object[] objects = Selection.objects;
        bool[] valids = TexturePackerImporter.ValidateAssets(objects);

        int numValids = 0;

        EditorGUI.indentLevel++;
        for (int i=0; i<valids.Length; i++) {
            if (valids[i]) {
                numValids++;
                EditorGUILayout.LabelField(objects[i].name);
            }
        }
        EditorGUI.indentLevel--;

        if (numValids == 0) {
            return;
        }

        GenState gen = new GenState();

        gen.genMesh = Toggle("Generate Meshes", prefGenMesh);

        if (!gen.genMesh && EditorPrefs.GetBool(prefGenOBJ)) {
            EditorPrefs.SetBool(prefGenOBJ, false);
        }

        if (!gen.genMesh && EditorPrefs.GetBool(prefGenPrefab)) {
            EditorPrefs.SetBool(prefGenPrefab, false);
        }

        gen.genMat = Toggle("Generate Materials", prefGenMaterial);
        gen.genOBJ = Toggle("Generate OBJs", prefGenOBJ);
        gen.genPrefab = Toggle("Generate Prefabs", prefGenPrefab);

        if (gen.genOBJ && !gen.genMesh) {
            EditorPrefs.SetBool(prefGenMesh, true);
        }

        if (gen.genPrefab && !gen.genMesh) {
            EditorPrefs.SetBool(prefGenMesh, true);
        }

        if (gen.genPrefab && !gen.genMat) {
            EditorPrefs.SetBool(prefGenMaterial, true);
        }

        if (GUILayout.Button("Generate")) {
            Generate(objects, valids, gen);
        }
    }

    private void Generate(UnityEngine.Object[]  objects,
                          bool[]                valids,
                          GenState              gen) {
        for (int i=0; i<valids.Length; i++) {
            if (!valids[i]) {
                continue;
            }

            TextAsset atlas = objects[i] as TextAsset;

            if (gen.genOBJ) {
                Mesh[] meshes = TexturePackerImporter.GenerateMeshes(atlas);
                if (meshes == null) {
                    return;
                }
                TexturePackerImporter.GenerateOBJs(atlas, meshes);
            }
            if (gen.genPrefab) {
                Mesh[] meshes = TexturePackerImporter.GenerateMeshes(atlas);
                if (meshes == null) {
                    return;
                }
                Material mat = TexturePackerImporter.GenerateMaterial(atlas);
                if (mat == null) {
                    return;
                }
                TexturePackerImporter.GeneratePrefabs(atlas, meshes, mat);
            }
            if (gen.genMesh) {
                TexturePackerImporter.GenerateMeshes(atlas);
            }
            if (gen.genMat) {
                TexturePackerImporter.GenerateMaterial(atlas);
            }
        }
    }

    private bool Toggle(string label, string pref) {
        bool b = EditorPrefs.GetBool(pref);
        b = EditorGUILayout.Toggle(label, b);
        EditorPrefs.SetBool(pref, b);
        return b;
    }
}

static public class TexturePackerImporter {

    static public void GeneratePrefabs(TextAsset    atlas,
                                       Mesh[]       meshes,
                                       Material     material) {
        string parentDir = GetParentDir(atlas);
        string prefabDirName = "Prefabs";
        string prefabDir = parentDir + "/" + prefabDirName + "/";

        // Create directory for the prefabs
        if (GetNumShapes(atlas) > 0 &&
            !Directory.Exists(prefabDir)) {
            AssetDatabase.CreateFolder(parentDir, prefabDirName);
        }

        GameObject parent = new GameObject("TMP_Prefab_Import");

        for (int i=0; i<meshes.Length; i++) {
            Mesh mesh = meshes[i];
            string prefabPath = prefabDir + mesh.name + ".prefab";
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (go == null) {
                go = new GameObject(mesh.name);
                go.transform.parent = parent.transform;
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterial = material;
                PrefabUtility.CreatePrefab(prefabPath, go, ReplacePrefabOptions.Default);
            }

            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        foreach (Transform child in parent.transform) {
            GameObject.DestroyImmediate(child.gameObject);
        }

        GameObject.DestroyImmediate(parent);
    }

    static public Mesh[] GenerateMeshes(TextAsset atlas) {
        if (!ValidateAsset(atlas)) {
            D.Err(atlas.name, "is not a valid Texture Packer atlas");
            return null;
        }
        return CreateMeshes(atlas);
    }

    static public Material GenerateMaterial(TextAsset atlas) {
        if (!ValidateAsset(atlas)) {
            D.Err(atlas.name, "is not a valid Texture Packer atlas");
            return null;
        }
        return CreateMaterial(atlas);
    }

    static public void GenerateOBJs(TextAsset atlas, Mesh[] meshes) {
        if (!ValidateAsset(atlas)) {
            D.Err(atlas.name, "is not a valid Texture Packer atlas");
            return;
        }

        string parentDir = GetParentDir(atlas);
        string objDirName = "OBJs";
        string objDir = parentDir + "/" + objDirName + "/";

        if (GetNumShapes(atlas) > 0 &&
            !Directory.Exists(objDir)) {
            AssetDatabase.CreateFolder(parentDir, objDirName);
        }

        foreach (Mesh mesh in meshes) {
            using (StreamWriter sw = new StreamWriter(objDir + mesh.name + ".obj")) {
                sw.Write(MeshToString(mesh));
            }
        }
    }

    static public bool[] ValidateAssets(UnityEngine.Object[] objects) {
        bool[] valids = new bool[objects.Length];
        for (int i=0; i<objects.Length; i++) {
            valids[i] = ValidateAsset(objects[i] as TextAsset);
        }
        return valids;
    }

    static private bool ValidateAsset(TextAsset asset) {
        if (asset == null) {
            return false;
        }
        string[] lines = GetLines(asset);
        if (lines == null) {
            return false;
        }
        return lines[0] == "{\"frames\": {";
    }

    static private string GetParentDir(TextAsset atlas) {
        string jsonPath = AssetDatabase.GetAssetPath(atlas);
        return Path.GetDirectoryName(jsonPath);
    }

    static private Material CreateMaterial(TextAsset atlas) {
        string parentDir = GetParentDir(atlas);
        Material mat = GetSelectedMaterial();

        if (mat == null) {
            string textureName = GetMetaData("image", atlas);
            if (String.IsNullOrEmpty(textureName)) {
                D.Err("Cannot find file name for texture in meta data.");
                return null;
            }

            textureName = textureName.Replace("\"", "");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(parentDir + "/" + textureName);

            string matName = Path.GetFileNameWithoutExtension(textureName);
            string matFileName = matName + ".mat";
            mat = AssetDatabase.LoadAssetAtPath<Material>(parentDir + "/" + matFileName);
            if (mat == null) {
                mat = new Material(Shader.Find("Standard"));
                mat.name = matName;
                AssetDatabase.CreateAsset(mat, parentDir + "/" + matFileName);
            }
            mat.SetTexture("_MainTex", texture);
        }

        return mat;
    }

    static private Mesh[] CreateMeshes(TextAsset atlas, Action<string, Mesh> cb = null) {
        string parentDir = GetParentDir(atlas);
        string destinationDir = "Meshes";
        string meshDir = parentDir + "/" + destinationDir + "/";
        List<TPFrame> shapes = GetShapes(atlas);
        List<Mesh> meshes = new List<Mesh>();

        // Create directory for the meshes
        if (shapes.Count > 0 &&
            !Directory.Exists(meshDir)) {
            AssetDatabase.CreateFolder(parentDir, destinationDir);
        }

        AssetDatabase.StartAssetEditing();

        foreach (TPFrame shape in shapes) {
            string meshPath = meshDir + shape.name + ".asset";
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            bool write = false;
            if (mesh == null) {
                mesh = new Mesh();
                write = true;
            }
            mesh.vertices = shape.vertices;
            mesh.uv = shape.verticesUV;
            mesh.triangles = shape.triangles;

            int numVerts = shape.vertices.Length;

            mesh.RecalculateNormals();
            ;

            Vector3[] normals = mesh.normals;
            for (int i=0; i<numVerts; i++) {
                normals[i] = Vector3.back;
            }
            mesh.normals = normals;

            Color[] colors = new Color[numVerts];
            for (int i=0; i<numVerts; i++) {
                colors[i] = Color.white;
            }
            mesh.colors = colors;

            if (write) {
                mesh.name = shape.name;
                AssetDatabase.CreateAsset(mesh, meshPath);
            }

            if (cb != null) {
                cb(shape.name, mesh);
            }

            meshes.Add(mesh);
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return meshes.ToArray();
    }

    // Doesn't handle multiple selected materials
    static private Material GetSelectedMaterial() {
        foreach (UnityEngine.Object obj in Selection.objects) {
            if (obj is Material) {
                return obj as Material;
            }
        }
        return null;
    }

    // Doesn't handle multiple selected atlas
    static private TextAsset GetSelectedAtlas() {
        foreach (UnityEngine.Object obj in Selection.objects) {
            if (obj is TextAsset) {
                return obj as TextAsset;
            }
        }
        return null;
    }

    static private int GetNumShapes(TextAsset atlas) {
        return GetLines(atlas).Length;
    }

    static private string[] GetLines(TextAsset obj) {
        string path = AssetDatabase.GetAssetPath(obj);
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (asset == null) {
            return null;
        }
        return asset.text.Split('\n');
    }

    static private List<TPFrame> GetShapes(TextAsset atlas) {
        string[] lines = GetLines(atlas);
        List<TPFrame> shapes = new List<TPFrame>();
        List<string> lineBuffer = new List<string>();
        List<int[]> arrayBuffer = new List<int[]>();
        bool inFrame = false;
        string lnVertices = "";
        string lnVerticesUV = "";
        string lnTriangles = "";
        Vector2 vertLinePadding = new Vector2(14, 2);
        Vector2 vertUVLinePadding = new Vector2(16, 2);
        Vector2 triLinePadding = new Vector2(15, 1);
        float pxToUnits = 2048f / 21f;
        string shapeName = "";

        // first: search for meta and get the size
        TPSize size = GetSize(lines);

        for (int i=2; i<lines.Length; i++) {
            string line = lines[i];
            if (line.EndsWith(":", StringComparison.Ordinal)) {
                shapeName = line.Substring(1, line.Length - 3);
                shapeName = shapeName.Replace(".png", "");
                inFrame = true;
                continue;
            }
            if (inFrame) {
                if (line == "}," || line == "}},") {
                    inFrame = false;
                    string lastLine = lineBuffer[lineBuffer.Count-1];
                    lastLine = lastLine.Substring(0, lastLine.Length-1);
                    lineBuffer.RemoveAt(lineBuffer.Count-1);
                    lineBuffer.Add(lastLine);
                    lineBuffer.Add("}");
                    TPFrame tpframe = JsonUtility.FromJson<TPFrame>(Join(lineBuffer));
                    tpframe.name = shapeName;

                    // vertices
                    ParseArray(line:      lnVertices,
                               padding:   vertLinePadding,
                               buffer:    arrayBuffer);
                    List<Vector3> vertices = new List<Vector3>();
                    for (int j=0; j<arrayBuffer.Count; j++) {
                        int[] vraw = arrayBuffer[j];
                        float x = vraw[0] - tpframe.pivot.x * tpframe.sourceSize.w;
                        float y = vraw[1] - tpframe.pivot.y * tpframe.sourceSize.h;
                        Vector3 v = new Vector3(x / pxToUnits,
                                                -y / pxToUnits,
                                                0f);
                        vertices.Add(v);
                    }
                    tpframe.vertices = vertices.ToArray();

                    // vertices UV
                    ParseArray(line:      lnVerticesUV,
                               padding:   vertUVLinePadding,
                               buffer:    arrayBuffer);
                    List<Vector2> uvs = new List<Vector2>();
                    for (int j=0; j<arrayBuffer.Count; j++) {
                        int[] uvRaw = arrayBuffer[j];
                        uvs.Add(new Vector2((float)uvRaw[0] / (float)size.w,
                                            1f - ((float)uvRaw[1] / (float)size.h)));
                    }
                    tpframe.verticesUV = uvs.ToArray();

                    // triangles
                    ParseArray(line:      lnTriangles,
                               padding:   triLinePadding,
                               buffer:    arrayBuffer);
                    List<int> triangles = new List<int>();
                    for (int j=0; j<arrayBuffer.Count; j++) {
                        int[] traw = arrayBuffer[j];
                        for (int k=0; k<traw.Length; k++) {
                            triangles.Add(traw[k]);
                        }
                    }
                    tpframe.triangles = triangles.ToArray();

                    shapes.Add(tpframe);
                    lineBuffer.Clear();
                } else {

                    bool addLine = true;
                    if (line.StartsWith("\t\"vertices\": ", StringComparison.Ordinal)) {
                        lnVertices = line;
                        addLine = false;
                    }
                    if (line.StartsWith("\t\"verticesUV\": ", StringComparison.Ordinal)) {
                        lnVerticesUV = line;
                        addLine = false;
                    }
                    if (line.StartsWith("\t\"triangles\": ", StringComparison.Ordinal)) {
                        lnTriangles = line;
                        addLine = false;
                    }

                    if (addLine) {
                        lineBuffer.Add(line);
                    }
                }
            }
        }

        return shapes;
    }

    static private void ParseArray(string       line,
                                   Vector3      padding,
                                   List<int[]>  buffer) {
        buffer.Clear();
        // strip beginning
        line = line.Remove(0, (int)padding.x);
        // strip end
        line = line.Remove(line.Length - (int)padding.y);
        line = line.Replace(", ", ";");

        string[] coordsRaw = line.Split(';');
        for (int c=0; c<coordsRaw.Length; c++) {
            string coordRaw = coordsRaw[c];
            coordRaw = coordRaw.Replace("[", "");
            coordRaw = coordRaw.Replace("]", "");
            string[] coordStr = coordRaw.Split(',');
            int[] coord = new int[coordStr.Length];
            for (int n=0; n<coord.Length; n++) {
                coord[n] = Int32.Parse(coordStr[n]);
            }
            buffer.Add(coord);
        }
    }

    static private string Join(List<string> strings) {
        string str = "";
        for (int i=0; i<strings.Count; i++) {
            str += strings[i];
        }
        return str;
    }

    static private TPSize GetSize(string[] lines) {
        string size = GetMetaData("size", lines);
        return String.IsNullOrEmpty(size)
            ? null
            : JsonUtility.FromJson<TPSize>(size);
    }

    static private string GetMetaData(string label, TextAsset atlas) {
        return GetMetaData(label, GetLines(atlas));
    }

    static private string GetMetaData(string label, string[] lines) {
        int metaLine = -1;
        for (int i=lines.Length-1; i>=0; i--) {
            if (lines[i] == "\"meta\": {") {
                metaLine = i;
                break;
            }
        }

        string labelStart = "\t\""+label+"\": ";
        for (int i=metaLine; i<lines.Length; i++) {
            if (lines[i].StartsWith(labelStart, StringComparison.Ordinal)) {
                string line = lines[i].Replace(labelStart, "");
                return line.Substring(0, line.Length-1);
            }
        }
        return null;
    }

    // private static string MeshToString(Mesh mesh, Material mat)  {
    private static string MeshToString(Mesh mesh)  {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mesh.name).Append("\n");
        foreach (Vector3 v in mesh.vertices)  {
            // For some reason, the x coordinates needs to be inverted.
            // Probably has to do with OBJs coordinate system
            sb.Append(string.Format("v {0} {1} {2}\n", -v.x, v.y, v.z));
        }
        sb.Append("\n");

        foreach (Vector3 n in mesh.normals) {
            // Invert x here too, just like with the vertices
            sb.Append(string.Format("vn {0} {1} {2}\n", -n.x, n.y, n.z));
        }
        sb.Append("\n");

        foreach (Vector3 v in mesh.uv)  {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        for (int subMesh=0; subMesh < mesh.subMeshCount; subMesh++) {
            sb.Append("\n");
            // sb.Append("usemtl ").Append(mat.name).Append("\n");
            // sb.Append("usemap ").Append(mat.name).Append("\n");

            int[] triangles = mesh.GetTriangles(subMesh);
            for (int i=0; i<triangles.Length; i+=3) {
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                    triangles[i] + 1,
                    triangles[i + 1] + 1,
                    triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }
}

[Serializable]
public class TPFrame {
    public string name;
    public TPRect frame;
    public bool rotated;
    public bool trimmed;
    public TPRect spriteSourceSize;
    public TPSize sourceSize;
    public Vector2 pivot;
    [NonSerialized]
    public Vector3[] vertices;
    [NonSerialized]
    public Vector2[] verticesUV;
    [NonSerialized]
    public int[] triangles;
}

[Serializable]
public class TPRect {
    public int x;
    public int y;
    public int w;
    public int h;

    override public string ToString() {
        return "TPRect {x: "+x+", y: "+y+", w: "+w+", h: "+h+"}";
    }
}

[Serializable]
public class TPSize {
    public int w;
    public int h;

    override public string ToString() {
        return "TPSize {w: "+w+", h: "+h+"}";
    }
}
