// jave.lin : 2022/06/15
// ���Խ� RenderDoc ������ CSV���ٴε����� FBX
// Requirments : Unity FBX Export Packages
// Output Pipeline : Unity URP

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;

public class JaveLin_RDC_CSV2FBX : EditorWindow
{
    [MenuItem("Tools/JaveLin_RDC_CSV2FBX")]
    private static void _Show()
    {
        var win = EditorWindow.GetWindow<JaveLin_RDC_CSV2FBX>();
        win.titleContent = new GUIContent("JaveLin_RDC_CSV2FBX");
        win.Show();
    }

    // jave.lin : ����������Ϣ
    public class VertexIDInfo
    {
        public int IDX;                 // ����
        public VertexInfo vertexInfo;   // ������Ϣ
    }

    // jave.lin : ���������
    public enum SemanticType
    {
        VTX,

        IDX,

        POSITION_X,
        POSITION_Y,
        POSITION_Z,

        NORMAL_X,
        NORMAL_Y,
        NORMAL_Z,
        NORMAL_W,

        TANGENT_X,
        TANGENT_Y,
        TANGENT_Z,
        TANGENT_W,

        TEXCOORD0_X,
        TEXCOORD0_Y,
        TEXCOORD0_Z,
        TEXCOORD0_W,

        TEXCOORD1_X,
        TEXCOORD1_Y,
        TEXCOORD1_Z,
        TEXCOORD1_W,

        TEXCOORD2_X,
        TEXCOORD2_Y,
        TEXCOORD2_Z,
        TEXCOORD2_W,

        TEXCOORD3_X,
        TEXCOORD3_Y,
        TEXCOORD3_Z,
        TEXCOORD3_W,

        TEXCOORD4_X,
        TEXCOORD4_Y,
        TEXCOORD4_Z,
        TEXCOORD4_W,

        TEXCOORD5_X,
        TEXCOORD5_Y,
        TEXCOORD5_Z,
        TEXCOORD5_W,

        TEXCOORD6_X,
        TEXCOORD6_Y,
        TEXCOORD6_Z,
        TEXCOORD6_W,

        TEXCOORD7_X,
        TEXCOORD7_Y,
        TEXCOORD7_Z,
        TEXCOORD7_W,

        COLOR0_X,
        COLOR0_Y,
        COLOR0_Z,
        COLOR0_W,
    }

    // jave.lin : �������õķ�ʽ
    public enum MaterialSetType
    {
        CreateNew,
        UsingExsitMaterialAsset,
    }

    // jave.lin : application to vertex shader ��ͨ�����ͣ�����ת���ã�
    public class VertexInfo
    {
        public int VTX;
        public int IDX;

        public float POSITION_X;
        public float POSITION_Y;
        public float POSITION_Z;

        public float NORMAL_X;
        public float NORMAL_Y;
        public float NORMAL_Z;
        public float NORMAL_W;

        public float TANGENT_X;
        public float TANGENT_Y;
        public float TANGENT_Z;
        public float TANGENT_W;

        public float TEXCOORD0_X;
        public float TEXCOORD0_Y;
        public float TEXCOORD0_Z;
        public float TEXCOORD0_W;

        public float TEXCOORD1_X;
        public float TEXCOORD1_Y;
        public float TEXCOORD1_Z;
        public float TEXCOORD1_W;

        public float TEXCOORD2_X;
        public float TEXCOORD2_Y;
        public float TEXCOORD2_Z;
        public float TEXCOORD2_W;

        public float TEXCOORD3_X;
        public float TEXCOORD3_Y;
        public float TEXCOORD3_Z;
        public float TEXCOORD3_W;

        public float TEXCOORD4_X;
        public float TEXCOORD4_Y;
        public float TEXCOORD4_Z;
        public float TEXCOORD4_W;

        public float TEXCOORD5_X;
        public float TEXCOORD5_Y;
        public float TEXCOORD5_Z;
        public float TEXCOORD5_W;

        public float TEXCOORD6_X;
        public float TEXCOORD6_Y;
        public float TEXCOORD6_Z;
        public float TEXCOORD6_W;

        public float TEXCOORD7_X;
        public float TEXCOORD7_Y;
        public float TEXCOORD7_Z;
        public float TEXCOORD7_W;

        public float COLOR0_X;
        public float COLOR0_Y;
        public float COLOR0_Z;
        public float COLOR0_W;

        public Vector3 POSITION
        {
            get 
            { 
                return new Vector3(
                POSITION_X, 
                POSITION_Y, 
                POSITION_Z);
            }
        }

        // jave.lin : �������
        public Vector4 POSITION_H
        {
            get
            {
                return new Vector4(
                POSITION_X,
                POSITION_Y,
                POSITION_Z,
                1);
            }
        }

        public Vector4 NORMAL
        {
            get
            {
                return new Vector4(
                NORMAL_X,
                NORMAL_Y,
                NORMAL_Z,
                NORMAL_W);
            }
        }
        public Vector4 TANGENT
        {
            get
            {
                return new Vector4(
                TANGENT_X,
                TANGENT_Y,
                TANGENT_Z,
                TANGENT_W);
            }
        }

        public Vector4 TEXCOORD0
        {
            get
            {
                return new Vector4(
                TEXCOORD0_X,
                TEXCOORD0_Y,
                TEXCOORD0_Z,
                TEXCOORD0_W);
            }
        }

        public Vector4 TEXCOORD1
        {
            get
            {
                return new Vector4(
                TEXCOORD1_X,
                TEXCOORD1_Y,
                TEXCOORD1_Z,
                TEXCOORD1_W);
            }
        }

        public Vector4 TEXCOORD2
        {
            get
            {
                return new Vector4(
                TEXCOORD2_X,
                TEXCOORD2_Y,
                TEXCOORD2_Z,
                TEXCOORD2_W);
            }
        }

        public Vector4 TEXCOORD3
        {
            get
            {
                return new Vector4(
                TEXCOORD3_X,
                TEXCOORD3_Y,
                TEXCOORD3_Z,
                TEXCOORD3_W);
            }
        }

        public Vector4 TEXCOORD4
        {
            get
            {
                return new Vector4(
                TEXCOORD4_X,
                TEXCOORD4_Y,
                TEXCOORD4_Z,
                TEXCOORD4_W);
            }
        }

        public Vector4 TEXCOORD5
        {
            get
            {
                return new Vector4(
                TEXCOORD5_X,
                TEXCOORD5_Y,
                TEXCOORD5_Z,
                TEXCOORD5_W);
            }
        }

        public Vector4 TEXCOORD6
        {
            get
            {
                return new Vector4(
                TEXCOORD6_X,
                TEXCOORD6_Y,
                TEXCOORD6_Z,
                TEXCOORD6_W);
            }
        }

        public Vector4 TEXCOORD7
        {
            get
            {
                return new Vector4(
                TEXCOORD7_X,
                TEXCOORD7_Y,
                TEXCOORD7_Z,
                TEXCOORD7_W);
            }
        }

        public Color COLOR0
        {
            get
            {
                return new Color(
                COLOR0_X,
                COLOR0_Y,
                COLOR0_Z,
                COLOR0_W);
            }
        }
    }

    private const string GO_Parent_Name = "Models_From_CSV";

    // jave.lin : on_gui ����ʾ�Ķ���
    private TextAsset RDC_Text_Asset;
    private string fbxName;
    private string outputDir;
    private string outputFullName;

    // jave.lin : on_gui - options
    private Vector2 optionsScrollPos;
    private bool options_show = true;
    private bool is_from_DX_CSV = true;
    private Vector3 vertexOffset = Vector3.zero;
    private Vector3 vertexRotation = Vector3.zero;
    private Vector3 vertexScale = Vector3.one;
    private bool is_recalculate_bound = true;
    private bool has_uv0 = true;
    private bool has_uv1 = false;
    private bool has_uv2 = false;
    private bool has_uv3 = false;
    private bool has_uv4 = false;
    private bool has_uv5 = false;
    private bool has_uv6 = false;
    private bool has_uv7 = false;
    private bool has_color0 = false;
    private ModelImporterNormals normalImportType = ModelImporterNormals.Import;
    private ModelImporterTangents tangentImportType = ModelImporterTangents.Import;
    private bool show_mat_toggle = true;
    private MaterialSetType materialSetType = MaterialSetType.CreateNew;
    private Shader shader;
    private Texture texture;
    private Material material;

    // jave.lin : helper ����
    private Dictionary<string, SemanticType> semanticTypeDict_key_name_helper;
    private SemanticType[] semanticsIDX_helper;

    // jave.lin : ɾ��ָ��Ŀ¼+Ŀ¼�µ������ļ�
    private void DelectDir(string dir)
    {
        try
        {
            if (!Directory.Exists(outputDir))
                return;

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            // ����Ŀ¼�������ļ�����Ŀ¼
            FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fileInfo in fileInfos)
            {
                if (fileInfo is DirectoryInfo)
                {
                    // �ж��Ƿ��ļ���
                    DirectoryInfo subDir = new DirectoryInfo(fileInfo.FullName);
                    subDir.Delete(true);            // ɾ����Ŀ¼���ļ�
                }
                else
                {
                    File.Delete(fileInfo.FullName);      // ɾ��ָ���ļ�
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    // jave.lin : ����ȫ·���� ת��Ϊ assets Ŀ¼�µ�����
    private string GetAssetPathByFullName(string fullName)
    {
        fullName = fullName.Replace("\\", "/");
        var dataPath_prefix = Application.dataPath.Replace("Assets", "");
        dataPath_prefix = dataPath_prefix.Replace(dataPath_prefix + "/", "");
        var mi_path = fullName.Replace(dataPath_prefix, "");
        return mi_path;
    }

    private void OnGUI()
    {
        Output_RDC_CSV_Handle();
    }

    private void Output_RDC_CSV_Handle()
    {
        // jave.lin : RenderDoc �� CSV �� .txt �ļ�
        var new_textAsset = EditorGUILayout.ObjectField("RDC_CSV", RDC_Text_Asset, typeof(TextAsset), false) as TextAsset;

        var refresh_csv = false;
        // jave.lin : ��� CSV �л���
        if (RDC_Text_Asset != new_textAsset)
        {
            refresh_csv = true;
            RDC_Text_Asset = new_textAsset;
        }

        if (new_textAsset == null)
        {
            var srcCol = GUI.contentColor;
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("Have no setting the RDC_CSV yet!");
            GUI.contentColor = srcCol;
            return;
        }

        // jave.lin : FBX ģ������
        fbxName = EditorGUILayout.TextField("FBX Name", fbxName);
        if (RDC_Text_Asset != null && (refresh_csv || string.IsNullOrEmpty(fbxName)))
        {
            fbxName = GenerateGOName(RDC_Text_Asset);
        }

        // jave.lin : output path
        EditorGUILayout.BeginHorizontal();
        outputDir = EditorGUILayout.TextField("Output Path(Dir)", outputDir);
        if (refresh_csv || string.IsNullOrEmpty(outputDir))
        {
            // jave.lin : ƴ������·��
            outputDir = Path.Combine(Application.dataPath, $"Models_From_CSV/{fbxName}");
            outputDir = outputDir.Replace("\\", "/");
        }
        if (GUILayout.Button("Browser...", GUILayout.Width(100)))
        {
            outputDir = EditorUtility.OpenFolderPanel("Select an output path", outputDir, "");
        }
        EditorGUILayout.EndHorizontal();
        // jave.lin : ��ʾ������ full name
        GUI.enabled = false;
        outputFullName = Path.Combine(outputDir, fbxName + ".fbx");
        outputFullName = outputFullName.Replace("\\", "/");
        EditorGUILayout.TextField("Output Full Name", outputFullName);
        GUI.enabled = true;

        // jave.lin : ���� CSV ��Ӧ�� FBX
        if (GUILayout.Button("Export FBX"))
        {
            ExportHandle();
        }

        // jave.lin : ��ʾ scroll view
        optionsScrollPos = EditorGUILayout.BeginScrollView(optionsScrollPos);

        // jave.lin : options ����
        EditorGUILayout.Space(10);
        options_show = EditorGUILayout.BeginFoldoutHeaderGroup(options_show, "Model Options");
        if (options_show)
        {
            EditorGUI.indentLevel++;
            // jave.lin : �Ƿ�� dx �� Graphics API ���������� CSV
            is_from_DX_CSV = EditorGUILayout.Toggle("Is From DirectX CSV", is_from_DX_CSV);
            // jave.lin : �Ƿ����¼��� AABB
            is_recalculate_bound = EditorGUILayout.Toggle("Is Recalculate AABB", is_recalculate_bound);
            // jave.lin : ����ƽ��
            vertexOffset = EditorGUILayout.Vector3Field("Vertex Offset", vertexOffset);
            // jave.lin : ������ת
            vertexRotation = EditorGUILayout.Vector3Field("Vertex Rotation", vertexRotation);
            // jave.lin : ��������
            vertexScale = EditorGUILayout.Vector3Field("Vertex Scale", vertexScale);
            // jave.lin : has_uv0,1,2,3,4,5,6,7
            has_uv0 = EditorGUILayout.Toggle("Has UV0", has_uv0);
            has_uv1 = EditorGUILayout.Toggle("Has UV1", has_uv1);
            has_uv2 = EditorGUILayout.Toggle("Has UV2", has_uv2);
            has_uv3 = EditorGUILayout.Toggle("Has UV3", has_uv3);
            has_uv4 = EditorGUILayout.Toggle("Has UV4", has_uv4);
            has_uv5 = EditorGUILayout.Toggle("Has UV5", has_uv5);
            has_uv6 = EditorGUILayout.Toggle("Has UV6", has_uv6);
            has_uv7 = EditorGUILayout.Toggle("Has UV7", has_uv7);
            // jave.lin : has_color0
            has_color0 = EditorGUILayout.Toggle("Has Color0", has_color0);
            // jave.lin : ���ߵ��뷽ʽ
            normalImportType = (ModelImporterNormals)EditorGUILayout.EnumPopup("Normal Import Type", normalImportType);
            // jave.lin : ���ߵ��뷽ʽ
            tangentImportType = (ModelImporterTangents)EditorGUILayout.EnumPopup("Tangent Import Type", tangentImportType);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // jave.lin : �Ƿ��Զ���������
        EditorGUILayout.Space(10);
        show_mat_toggle = EditorGUILayout.BeginFoldoutHeaderGroup(show_mat_toggle, "Material Options");
        if (show_mat_toggle)
        {
            EditorGUI.indentLevel++;
            var newMaterialSetType = (MaterialSetType)EditorGUILayout.EnumPopup("Material Set Type", materialSetType);
            if (material == null || materialSetType != newMaterialSetType)
            {
                materialSetType = newMaterialSetType;
                // jave.lin : ����
                if (materialSetType == MaterialSetType.CreateNew)
                {
                    // jave.lin : shader ����Ϊ��
                    if (shader == null)
                    {
                        shader = Shader.Find("Universal Render Pipeline/Lit");
                    }
                    material = new Material(shader);
                }
                else
                {
                    // jave.lin : Ĭ��ʹ�� ����Ŀ¼�µ� mat ����
                    var mat_path = Path.Combine(outputDir, fbxName + ".mat").Replace("\\", "/");
                    mat_path = GetAssetPathByFullName(mat_path);
                    var mat_asset = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
                    if (mat_asset != null) material = mat_asset;
                }
            }

            if (materialSetType == MaterialSetType.CreateNew)
            {
                // jave.lin : ʹ�õ� shader
                shader = EditorGUILayout.ObjectField("Shader", shader, typeof(Shader), false) as Shader;
                // jave.lin : ʹ�õ� ������
                texture = EditorGUILayout.ObjectField("Main Texture", texture, typeof(Texture), false) as Texture;
            }
            // jave.lin : ����
            else // MaterialSetType.UseExsitMaterialAsset
            {
                material = EditorGUILayout.ObjectField("Material Asset", material, typeof(Material), false) as Material;
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.EndScrollView();
    }

    // jave.lin : ��������
    private void ExportHandle()
    {
        if (RDC_Text_Asset != null)
        {
            try
            {
                // jave.lin : ��ӳ��� semantics ���ֺ�����
                MappingSemanticsTypeByNames();
                // jave.lin : ����֮ǰ�� GO
                var parent = GetParentTrans();
                // jave.lin : �� CSV ������תΪ MeshRenderer �� GO
                var outputGO = GameObject.Find($"{GO_Parent_Name }/{fbxName}");
                if (outputGO != null)
                {
                    GameObject.DestroyImmediate(outputGO);
                }
                outputGO = GenerateGOWithMeshRendererFromCSV(RDC_Text_Asset.text, is_from_DX_CSV);
                outputGO.transform.SetParent(parent);
                outputGO.name = fbxName;

                //// jave.lin : ������Ŀ¼�µ�����
                //DelectDir(outputDir);
                // jave.lin : Ȼ�����´����µ�Ŀ¼
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (materialSetType == MaterialSetType.CreateNew)
                {
                    // jave.lin : �Զ���������
                    var create_mat = outputGO.GetComponent<MeshRenderer>().sharedMaterial;
                    // jave.lin : ����ǰ��������������
                    create_mat.mainTexture = texture;

                    var mat_created_path = Path.Combine(outputDir, fbxName + ".mat").Replace("\\", "/");
                    mat_created_path = GetAssetPathByFullName(mat_created_path);
                    Debug.Log($"mat_created_path : {mat_created_path}");
                    AssetDatabase.CreateAsset(create_mat, mat_created_path);
                }

                // jave.lin : ʹ�� FBX Exporter ������� FBX
                ModelExporter.ExportObject(outputFullName, outputGO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // jave.lin : �������� MI���������µ���
                string mi_path = GetAssetPathByFullName(outputFullName);
                ModelImporter mi = ModelImporter.GetAtPath(mi_path) as ModelImporter;
                mi.importNormals = normalImportType;
                mi.importTangents = tangentImportType;
                mi.importAnimation = false;
                mi.importAnimatedCustomProperties = false;
                mi.importBlendShapeNormals = ModelImporterNormals.None;
                mi.importBlendShapes = false;
                mi.importCameras = false;
                mi.importConstraints = false;
                mi.importLights = false;
                mi.importVisibility = false;
                mi.animationType = ModelImporterAnimationType.None;
                mi.materialImportMode = ModelImporterMaterialImportMode.None;
                mi.SaveAndReimport();

                // jave.lin : replace outputGO from model prefab
                var src_parent = outputGO.transform.parent;
                var src_local_pos = outputGO.transform.localPosition;
                var src_local_rot = outputGO.transform.localRotation;
                var src_local_scl = outputGO.transform.localScale;
                DestroyImmediate(outputGO);
                // jave.lin : new model prefab
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(mi_path);
                outputGO = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                outputGO.transform.SetParent(src_parent);
                outputGO.transform.localPosition = src_local_pos;
                outputGO.transform.localRotation = src_local_rot;
                outputGO.transform.localScale = src_local_scl;
                outputGO.name = fbxName;
                // jave.lin : set material
                var mat_path = Path.Combine(outputDir, fbxName + ".mat").Replace("\\", "/");
                mat_path = GetAssetPathByFullName(mat_path);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
                outputGO.GetComponent<MeshRenderer>().sharedMaterial = mat;
                // jave.lin : new real prefab
                var prefab_created_path = Path.Combine(outputDir, fbxName + ".prefab").Replace("\\", "/");
                prefab_created_path = GetAssetPathByFullName(prefab_created_path);
                Debug.Log($"prefab_created_path : {prefab_created_path}");
                PrefabUtility.SaveAsPrefabAssetAndConnect(outputGO, prefab_created_path, InteractionMode.AutomatedAction);

                // jave.lin : ��ӡ����ɹ�����Ϣ
                Debug.Log($"Export FBX Successfully! outputPath : {outputFullName}");
            }
            catch (Exception er)
            {
                Debug.LogError($"Export FBX Failed! er: {er}");
            }
        }
    }

    // jave.lin : ӳ�� semantics �� name �� type
    private void MappingSemanticsTypeByNames()
    {
        if (semanticTypeDict_key_name_helper == null)
        {
            semanticTypeDict_key_name_helper = new Dictionary<string, SemanticType>();
        }
        else
        {
            semanticTypeDict_key_name_helper.Clear();
        }
        semanticTypeDict_key_name_helper["VTX"]            = SemanticType.VTX;
        semanticTypeDict_key_name_helper["IDX"]            = SemanticType.IDX;
        semanticTypeDict_key_name_helper["POSITION.x"]     = SemanticType.POSITION_X;
        semanticTypeDict_key_name_helper["POSITION.y"]     = SemanticType.POSITION_Y;
        semanticTypeDict_key_name_helper["POSITION.z"]     = SemanticType.POSITION_Z;
        semanticTypeDict_key_name_helper["NORMAL.x"]       = SemanticType.NORMAL_X;
        semanticTypeDict_key_name_helper["NORMAL.y"]       = SemanticType.NORMAL_Y;
        semanticTypeDict_key_name_helper["NORMAL.z"]       = SemanticType.NORMAL_Z;
        semanticTypeDict_key_name_helper["NORMAL.w"]       = SemanticType.NORMAL_W;
        semanticTypeDict_key_name_helper["TANGENT.x"]      = SemanticType.TANGENT_X;
        semanticTypeDict_key_name_helper["TANGENT.y"]      = SemanticType.TANGENT_Y;
        semanticTypeDict_key_name_helper["TANGENT.z"]      = SemanticType.TANGENT_Z;
        semanticTypeDict_key_name_helper["TANGENT.w"]      = SemanticType.TANGENT_W;
        semanticTypeDict_key_name_helper["TEXCOORD0.x"]    = SemanticType.TEXCOORD0_X;
        semanticTypeDict_key_name_helper["TEXCOORD0.y"]    = SemanticType.TEXCOORD0_Y;
        semanticTypeDict_key_name_helper["TEXCOORD0.z"]    = SemanticType.TEXCOORD0_Z;
        semanticTypeDict_key_name_helper["TEXCOORD0.w"]    = SemanticType.TEXCOORD0_W;
        semanticTypeDict_key_name_helper["TEXCOORD1.x"]    = SemanticType.TEXCOORD1_X;
        semanticTypeDict_key_name_helper["TEXCOORD1.y"]    = SemanticType.TEXCOORD1_Y;
        semanticTypeDict_key_name_helper["TEXCOORD1.z"]    = SemanticType.TEXCOORD1_Z;
        semanticTypeDict_key_name_helper["TEXCOORD1.w"]    = SemanticType.TEXCOORD1_W;
        semanticTypeDict_key_name_helper["TEXCOORD2.x"]    = SemanticType.TEXCOORD2_X;
        semanticTypeDict_key_name_helper["TEXCOORD2.y"]    = SemanticType.TEXCOORD2_Y;
        semanticTypeDict_key_name_helper["TEXCOORD2.z"]    = SemanticType.TEXCOORD2_Z;
        semanticTypeDict_key_name_helper["TEXCOORD2.w"]    = SemanticType.TEXCOORD2_W;
        semanticTypeDict_key_name_helper["TEXCOORD3.x"]    = SemanticType.TEXCOORD3_X;
        semanticTypeDict_key_name_helper["TEXCOORD3.y"]    = SemanticType.TEXCOORD3_Y;
        semanticTypeDict_key_name_helper["TEXCOORD3.z"]    = SemanticType.TEXCOORD3_Z;
        semanticTypeDict_key_name_helper["TEXCOORD3.w"]    = SemanticType.TEXCOORD3_W;
        semanticTypeDict_key_name_helper["TEXCOORD4.x"]    = SemanticType.TEXCOORD4_X;
        semanticTypeDict_key_name_helper["TEXCOORD4.y"]    = SemanticType.TEXCOORD4_Y;
        semanticTypeDict_key_name_helper["TEXCOORD4.z"]    = SemanticType.TEXCOORD4_Z;
        semanticTypeDict_key_name_helper["TEXCOORD4.w"]    = SemanticType.TEXCOORD4_W;
        semanticTypeDict_key_name_helper["TEXCOORD5.x"]    = SemanticType.TEXCOORD5_X;
        semanticTypeDict_key_name_helper["TEXCOORD5.y"]    = SemanticType.TEXCOORD5_Y;
        semanticTypeDict_key_name_helper["TEXCOORD5.z"]    = SemanticType.TEXCOORD5_Z;
        semanticTypeDict_key_name_helper["TEXCOORD5.w"]    = SemanticType.TEXCOORD5_W;
        semanticTypeDict_key_name_helper["TEXCOORD6.x"]    = SemanticType.TEXCOORD6_X;
        semanticTypeDict_key_name_helper["TEXCOORD6.y"]    = SemanticType.TEXCOORD6_Y;
        semanticTypeDict_key_name_helper["TEXCOORD6.z"]    = SemanticType.TEXCOORD6_Z;
        semanticTypeDict_key_name_helper["TEXCOORD6.w"]    = SemanticType.TEXCOORD6_W;
        semanticTypeDict_key_name_helper["TEXCOORD7.x"]    = SemanticType.TEXCOORD7_X;
        semanticTypeDict_key_name_helper["TEXCOORD7.y"]    = SemanticType.TEXCOORD7_Y;
        semanticTypeDict_key_name_helper["TEXCOORD7.z"]    = SemanticType.TEXCOORD7_Z;
        semanticTypeDict_key_name_helper["TEXCOORD7.w"]    = SemanticType.TEXCOORD7_W;
        semanticTypeDict_key_name_helper["COLOR0.x"]       = SemanticType.COLOR0_X;
        semanticTypeDict_key_name_helper["COLOR0.y"]       = SemanticType.COLOR0_Y;
        semanticTypeDict_key_name_helper["COLOR0.z"]       = SemanticType.COLOR0_Z;
        semanticTypeDict_key_name_helper["COLOR0.w"]       = SemanticType.COLOR0_W;
    }

    // jave.lin : ��ȡ parent transform ����
    private Transform GetParentTrans()
    {
        var parentGO = GameObject.Find(GO_Parent_Name);
        if (parentGO == null)
        {
            parentGO = new GameObject(GO_Parent_Name);
            parentGO.transform.position = Vector3.zero;
            parentGO.transform.localRotation = Quaternion.identity;
            parentGO.transform.localScale = Vector3.one;
        }
        return parentGO.transform;
    }

    // jave.lin : ������������ GO Name
    private string GenerateGOName(TextAsset ta)
    {
        //return $"From_CSV_{ta.text.GetHashCode()}";
        //return $"From_CSV_{ta.name}";
        return ta.name;
    }

    // jave.lin : ���� CSV �������� MeshRenderer ��Ӧ�� GO
    private GameObject GenerateGOWithMeshRendererFromCSV(string csv, bool is_from_DX_CSV)
    {
        var ret = new GameObject();

        var mesh = new Mesh();

        // jave.lin : ���� csv ����� mesh ��Ϣ
        FillMeshFromCSV(mesh, csv, is_from_DX_CSV);

        var meshFilter = ret.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = ret.AddComponent<MeshRenderer>();

        // jave.lin : Ĭ��ʹ�� URP �� PBR Shader
        meshRenderer.sharedMaterial = material;

        ret.transform.position = Vector3.zero;
        ret.transform.localRotation = Quaternion.identity;
        ret.transform.localScale = Vector3.one;

        return ret;
    }

    // jave.lin : ���� semantic type �� data ����䵽 �����ֶ�
    private void FillVertexFieldInfo(VertexInfo info, SemanticType semanticType, string data, bool is_from_DX_CSV)
    {
        switch (semanticType)
        {
            // jave.lin : VTX
            case SemanticType.VTX:
                info.VTX = int.Parse(data);
                break;

            // jave.lin : IDX
            case SemanticType.IDX:
                info.IDX = int.Parse(data);
                break;

            // jave.lin : position
            case SemanticType.POSITION_X:
                info.POSITION_X = float.Parse(data);
                break;
            case SemanticType.POSITION_Y:
                info.POSITION_Y = float.Parse(data);
                break;
            case SemanticType.POSITION_Z:
                info.POSITION_Z = float.Parse(data);
                break;

            // jave.lin : normal
            case SemanticType.NORMAL_X:
                info.NORMAL_X = float.Parse(data);
                break;
            case SemanticType.NORMAL_Y:
                info.NORMAL_Y = float.Parse(data);
                break;
            case SemanticType.NORMAL_Z:
                info.NORMAL_Z = float.Parse(data);
                break;
            case SemanticType.NORMAL_W:
                info.NORMAL_W = float.Parse(data);
                break;

            // jave.lin : tangent
            case SemanticType.TANGENT_X:
                info.TANGENT_X = float.Parse(data);
                break;
            case SemanticType.TANGENT_Y:
                info.TANGENT_Y = float.Parse(data);
                break;
            case SemanticType.TANGENT_Z:
                info.TANGENT_Z = float.Parse(data);
                break;
            case SemanticType.TANGENT_W:
                info.TANGENT_W = float.Parse(data);
                break;

            // jave.lin : texcoord0
            case SemanticType.TEXCOORD0_X:
                info.TEXCOORD0_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD0_Y:
                info.TEXCOORD0_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD0_Y = 1 - info.TEXCOORD0_Y;
                break;
            case SemanticType.TEXCOORD0_Z:
                info.TEXCOORD0_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD0_W:
                info.TEXCOORD0_W = float.Parse(data);
                break;

            // jave.lin : texcoord1
            case SemanticType.TEXCOORD1_X:
                info.TEXCOORD1_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD1_Y:
                info.TEXCOORD1_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD1_Y = 1 - info.TEXCOORD1_Y;
                break;
            case SemanticType.TEXCOORD1_Z:
                info.TEXCOORD1_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD1_W:
                info.TEXCOORD1_W = float.Parse(data);
                break;

            // jave.lin : texcoord2
            case SemanticType.TEXCOORD2_X:
                info.TEXCOORD2_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD2_Y:
                info.TEXCOORD2_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD2_Y = 1 - info.TEXCOORD2_Y;
                break;
            case SemanticType.TEXCOORD2_Z:
                info.TEXCOORD2_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD2_W:
                info.TEXCOORD2_W = float.Parse(data);
                break;

            // jave.lin : texcoord3
            case SemanticType.TEXCOORD3_X:
                info.TEXCOORD3_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD3_Y:
                info.TEXCOORD3_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD3_Y = 1 - info.TEXCOORD3_Y;
                break;
            case SemanticType.TEXCOORD3_Z:
                info.TEXCOORD3_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD3_W:
                info.TEXCOORD3_W = float.Parse(data);
                break;

            // jave.lin : texcoord4
            case SemanticType.TEXCOORD4_X:
                info.TEXCOORD4_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD4_Y:
                info.TEXCOORD4_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD4_Y = 1 - info.TEXCOORD4_Y;
                break;
            case SemanticType.TEXCOORD4_Z:
                info.TEXCOORD4_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD4_W:
                info.TEXCOORD4_W = float.Parse(data);
                break;

            // jave.lin : texcoord5
            case SemanticType.TEXCOORD5_X:
                info.TEXCOORD5_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD5_Y:
                info.TEXCOORD5_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD5_Y = 1 - info.TEXCOORD5_Y;
                break;
            case SemanticType.TEXCOORD5_Z:
                info.TEXCOORD5_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD5_W:
                info.TEXCOORD5_W = float.Parse(data);
                break;

            // jave.lin : texcoord6
            case SemanticType.TEXCOORD6_X:
                info.TEXCOORD6_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD6_Y:
                info.TEXCOORD6_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD6_Y = 1 - info.TEXCOORD6_Y;
                break;
            case SemanticType.TEXCOORD6_Z:
                info.TEXCOORD6_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD6_W:
                info.TEXCOORD6_W = float.Parse(data);
                break;

            // jave.lin : texcoord7
            case SemanticType.TEXCOORD7_X:
                info.TEXCOORD7_X = float.Parse(data);
                break;
            case SemanticType.TEXCOORD7_Y:
                info.TEXCOORD7_Y = float.Parse(data);
                if (is_from_DX_CSV) info.TEXCOORD7_Y = 1 - info.TEXCOORD7_Y;
                break;
            case SemanticType.TEXCOORD7_Z:
                info.TEXCOORD7_Z = float.Parse(data);
                break;
            case SemanticType.TEXCOORD7_W:
                info.TEXCOORD7_W = float.Parse(data);
                break;

            // jave.lin : color0
            case SemanticType.COLOR0_X:
                info.COLOR0_X = float.Parse(data);
                break;
            case SemanticType.COLOR0_Y:
                info.COLOR0_Y = float.Parse(data);
                break;
            case SemanticType.COLOR0_Z:
                info.COLOR0_Z = float.Parse(data);
                break;
            case SemanticType.COLOR0_W:
                info.COLOR0_W = float.Parse(data);
                break;
            // jave.lin : un-implements
            default:
                Debug.LogError($"Fill_A2V_Common_Type_Data un-implements SemanticType : {semanticType}");
                break;
        }
    }

    // jave.lin : ���� csv ����� mesh ��Ϣ
    private void FillMeshFromCSV(Mesh mesh, string csv, bool is_from_DX_CSV)
    {
        var line_splitor = new string[] { "\n" };
        var line_element_splitor = new string[] { "," };

        var lines = csv.Split(line_splitor, StringSplitOptions.RemoveEmptyEntries);

        // jave.lin : lines[0] == "VTX, IDX, POSITION.x, POSITION.y, POSITION.z, NORMAL.x, NORMAL.y, NORMAL.z, NORMAL.w, TANGENT.x, TANGENT.y, TANGENT.z, TANGENT.w, TEXCOORD0.x, TEXCOORD0.y"

        // jave.lin : ���� vertex buffer format �� semantics �� idx �Ķ�Ӧ��ϵ
        var semanticTitles = lines[0].Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

        semanticsIDX_helper = new SemanticType[semanticTitles.Length];
        Debug.Log($"semanticTitles : {lines[0]}");
        for (int i = 0; i < semanticTitles.Length; i++)
        {
            var title = semanticTitles[i];
            var semantics = title.Trim();
            if (semanticTypeDict_key_name_helper.TryGetValue(semantics, out SemanticType semanticType))
            {
                semanticsIDX_helper[i] = semanticType;
                //Debug.Log($"semantics : {title.Trim()}, type : {semanticType}");
            }
            else
            {
                Debug.LogError($"un-implements semantic : {semantics}");
            }
        }

        // jave.lin : �ȸ��� IDX ������ԭ vertex buffer ������
        // lines[1~count-1] : ���磺 0, 0,  0.0402, -1.57095E-17,  0.12606, -0.97949,  0.00, -0.20056,  0.00,  0.1098,  0.83691, -0.53613,  1.00, -0.06058,  0.81738

        Dictionary<int, VertexInfo> vertex_dict_key_idx = new Dictionary<int, VertexInfo>();

        var indices = new List<int>();

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var linesElements = line.Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

            // jave.lin : �ڼ�������������0~count-1)
            var idx = int.Parse(linesElements[1]);

            // jave.lin : indices �����������ݵ�����
            indices.Add(idx);

            // jave.lin : ����� vertex û�д���������ô��ȥ����
            if (!vertex_dict_key_idx.TryGetValue(idx, out VertexInfo info))
            {
                info = new VertexInfo();
                vertex_dict_key_idx[idx] = info;

                // jave.lin : loop to fill the a2v field
                for (int j = 0; j < linesElements.Length; j++)
                {
                    var semanticType = semanticsIDX_helper[j];
                    FillVertexFieldInfo(info, semanticType, linesElements[j], is_from_DX_CSV);
                }
            }
        }

        // jave.lin : ���š���ת��ƽ��
        var rotation    = Quaternion.Euler(vertexRotation);
        var TRS_mat     = Matrix4x4.TRS(vertexOffset, rotation, vertexScale);

        // jave.lin : composite the data ������������Ҫ������ݣ�ͬ�⸳ֵ�� mesh��
        var vertices    = new Vector3[vertex_dict_key_idx.Count];
        var normals     = new Vector3[vertex_dict_key_idx.Count];
        var tangents    = new Vector4[vertex_dict_key_idx.Count];
        var uv          = new Vector2[vertex_dict_key_idx.Count];
        var uv2         = new Vector2[vertex_dict_key_idx.Count];
        var uv3         = new Vector2[vertex_dict_key_idx.Count];
        var uv4         = new Vector2[vertex_dict_key_idx.Count];
        var uv5         = new Vector2[vertex_dict_key_idx.Count];
        var uv6         = new Vector2[vertex_dict_key_idx.Count];
        var uv7         = new Vector2[vertex_dict_key_idx.Count];
        var uv8         = new Vector2[vertex_dict_key_idx.Count];
        var color0      = new Color[vertex_dict_key_idx.Count];

        // jave.lin : ���� 0~count ������˳������֯��ص� vertex ����
        for (int idx = 0; idx < vertices.Length; idx++)
        {
            var info        = vertex_dict_key_idx[idx];
            vertices[idx]   = TRS_mat * info.POSITION_H;
            normals[idx]    = rotation * info.NORMAL;
            tangents[idx]   = info.TANGENT;
            uv[idx]         = info.TEXCOORD0;
            uv2[idx]        = info.TEXCOORD1;
            uv3[idx]        = info.TEXCOORD2;
            uv4[idx]        = info.TEXCOORD3;
            uv5[idx]        = info.TEXCOORD4;
            uv6[idx]        = info.TEXCOORD5;
            uv7[idx]        = info.TEXCOORD6;
            uv8[idx]        = info.TEXCOORD7;
            color0[idx]     = info.COLOR0;
        }

        // jave.lin : ���� mesh ��Ϣ
        mesh.vertices   = vertices;
        mesh.triangles  = indices.ToArray();
        mesh.uv         = has_uv0 ? uv : null;
        mesh.uv2        = has_uv1 ? uv2 : null;
        mesh.uv3        = has_uv2 ? uv3 : null;
        mesh.uv4        = has_uv3 ? uv4 : null;
        mesh.uv5        = has_uv4 ? uv5 : null;
        mesh.uv6        = has_uv5 ? uv6 : null;
        mesh.uv7        = has_uv6 ? uv7 : null;
        mesh.uv8        = has_uv7 ? uv8 : null;
        mesh.colors     = has_color0 ? color0 : null;

        // jave.lin : AABB
        if (is_recalculate_bound)
        {
            mesh.RecalculateBounds();
        }

        // jave.lin : NORMAL
        switch (normalImportType)
        {
            case ModelImporterNormals.None:
                // nop
                break;
            case ModelImporterNormals.Import:
                mesh.normals = normals;
                break;
            case ModelImporterNormals.Calculate:
                mesh.RecalculateNormals();
                break;
            default:
                break;
        }

        // jave.lin : TANGENT
        switch (tangentImportType)
        {
            case ModelImporterTangents.None:
                // nop
                break;
            case ModelImporterTangents.Import:
                mesh.tangents = tangents;
                break;
            case ModelImporterTangents.CalculateLegacy:
            case ModelImporterTangents.CalculateLegacyWithSplitTangents:
            case ModelImporterTangents.CalculateMikk:
                mesh.RecalculateTangents();
                break;
            default:
                break;
        }

        //// jave.lin : ��ӡһ��
        //Debug.Log("FillMeshFromCSV done!");
    }
}