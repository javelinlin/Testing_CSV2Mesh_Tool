// jave.lin : 2022/06/15
// 尝试将 RenderDoc 导出的 CSV，再次导出成 FBX
// Requirments : Unity FBX Export Packages
// Output Pipeline : Unity URP

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using static JaveLin_RDC_CSV2FBX;

public class JaveLin_RDC_CSV2FBX : EditorWindow
{
    [MenuItem("Tools/JaveLin_RDC_CSV2FBX")]
    private static void _Show()
    {
        var win = EditorWindow.GetWindow<JaveLin_RDC_CSV2FBX>();
        win.titleContent = new GUIContent("JaveLin_RDC_CSV2FBX");
        win.Show();
    }

    // jave.lin : 顶点索引信息
    public class VertexIDInfo
    {
        public int IDX;                 // 索引
        public VertexInfo vertexInfo;   // 顶点信息
    }

    // jave.lin : 语义的类型
    public enum SemanticType
    {
        Unknow,

        VTX,

        IDX,

        POSITION_X,
        POSITION_Y,
        POSITION_Z,
        POSITION_W,

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

    // jave.lin : Semantic 映射类型
    public enum SemanticMappingType
    {
        Default,            // jave.lin : 使用默认的
        ManuallyMapping,    // jave.lin : 使用手动设置映射的
    }

    // jave.lin : 材质设置的方式
    public enum MaterialSetType
    {
        CreateNew,
        UsingExsitMaterialAsset,
    }

    // jave.lin : 导出文件类型
    public enum ExportFileType
    {
        FBX,            // 注意如果使用这种方式的话，会有 uv 不能保存超过 vector2 分量的数据，也就是说，uv无法保存 vector3 或 vector4 的数据
        UnityMesh,      // 如果抓帧的模型发现 shaderlab 里面有使用到 vertex input TEXCOORD[n] 是超过2个以上的分量的，就不能使用 FBX 了，使用 UnityMesh 保存的网格可以保存下这些 uv 数据
    }

    private bool model_readable = false;

    private ExportFileType exportFileType;

    // jave.lin : application to vertex shader 的通用类型（辅助转换用）
    public class VertexInfo
    {
        public int VTX;
        public int IDX;

        public float POSITION_X;
        public float POSITION_Y;
        public float POSITION_Z;
        public float POSITION_W;

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

        // jave.lin : 齐次坐标
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

    // jave.lin : on_gui 上显示的对象
    private TextAsset RDC_Text_Asset;
    private string modelName;
    private string outputDir;
    private string outputModelPrefabFullName;
    private string outputUnityMeshFullName;

    // jave.lin : on_gui - options
    private Vector2 optionsScrollPos;
    private bool options_show = true;
    private bool is_from_DX_CSV = true;
    private Vector3 vertexOffset = Vector3.zero;
    private Vector3 vertexRotation = Vector3.zero;
    private Vector3 vertexScale = Vector3.one;
    private bool is_reverse_vertex_order = true; // jave.lin : for reverse normal
    private bool is_recalculate_bound = true;
    private SemanticMappingType semanticMappingType = SemanticMappingType.Default;

    private bool include_normal = true;
    private bool include_tangent = true;
    private bool include_uv0 = true;
    private bool include_uv1 = false;
    private bool include_uv2 = false;
    private bool include_uv3 = false;
    private bool include_uv4 = false;
    private bool include_uv5 = false;
    private bool include_uv6 = false;
    private bool include_uv7 = false;
    private bool include_color0 = false;
    private ModelImporterNormals normalImportType = ModelImporterNormals.Import;
    private ModelImporterTangents tangentImportType = ModelImporterTangents.Import;
    private bool show_mat_toggle = true;
    private MaterialSetType materialSetType = MaterialSetType.CreateNew;
    private Shader shader;
    private Texture texture;
    private Material material;

    // jave.lin : helper obj
    private Dictionary<string, SemanticType> semanticTypeDict_key_name_helper;
    private Dictionary<string, SemanticType> semanticManullyMappingTypeDict_key_name_helper;
    private SemanticType[] semanticsIDX_helper;
    private int[] semantics_check_duplicated_helper;
    private List<string> stringListHelper;

    private int[] GetSemantics_check_duplicated_helper()
    {
        if (semantics_check_duplicated_helper == null || semantics_check_duplicated_helper.Length == 0)
        {
            var vals = Enum.GetValues(typeof(SemanticType));
            semantics_check_duplicated_helper = new int[vals.Length];
            for (int i = 0; i < vals.Length; i++)
            {
                semantics_check_duplicated_helper[i] = 0;
            }
        }
        return semantics_check_duplicated_helper;
    }

    private void ClearSemantics_check_duplicated_helper(int[] arr)
    {
        if (arr != null)
        {
            Array.Clear(arr, 0, arr.Length);
        }
    }

    private List<string> GetStringListHelper()
    {
        if (stringListHelper == null)
        {
            stringListHelper = new List<string>();
        }
        return stringListHelper;
    }

    // jave.lin : 删除指定目录+目录下的所有文件
    private void DelectDir(string dir)
    {
        try
        {
            if (!Directory.Exists(outputDir))
                return;

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            // 返回目录中所有文件和子目录
            FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();
            foreach (FileSystemInfo fileInfo in fileInfos)
            {
                if (fileInfo is DirectoryInfo)
                {
                    // 判断是否文件夹
                    DirectoryInfo subDir = new DirectoryInfo(fileInfo.FullName);
                    subDir.Delete(true);            // 删除子目录和文件
                }
                else
                {
                    File.Delete(fileInfo.FullName);      // 删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    // jave.lin : 根据全路径名 转换为 assets 目录下的名字
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

    // jave.lin : 获取Shader
    private Shader GetShader(string custom_shader_name = null)
    {
        Shader ret = null;
        if (string.IsNullOrEmpty(custom_shader_name))
            ret = Shader.Find(custom_shader_name); // jave.lin : custom special
        if (ret == null) // jave.lin : BRP standard
            ret = Shader.Find("Standard");
        if (ret == null)
            ret = Shader.Find("Universal Render Pipeline/Lit"); // jave.lin : URP Lit
        if (ret == null)
            Debug.LogError($"找不到 BRP Standard shader 或是 URP Lit shader");

        return ret;
    }

    private void Output_RDC_CSV_Handle()
    {
        // jave.lin : RenderDoc 的 CSV 的 .txt 文件
        var new_textAsset = EditorGUILayout.ObjectField("RDC_CSV", RDC_Text_Asset, typeof(TextAsset), false) as TextAsset;

        var refresh_csv = false;
        // jave.lin : 如果 CSV 切换了
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

        if (refresh_csv)
        {
            material = null;
            semanticManullyMappingTypeDict_key_name_helper = null;
            ClearSemantics_check_duplicated_helper(semantics_check_duplicated_helper);
        }

        // jave.lin : FBX 模型名字
        modelName = EditorGUILayout.TextField("FBX Name", modelName);
        if (RDC_Text_Asset != null && (refresh_csv || string.IsNullOrEmpty(modelName)))
        {
            modelName = GenerateGOName(RDC_Text_Asset);
        }

        // jave.lin : output path
        EditorGUILayout.BeginHorizontal();
        outputDir = EditorGUILayout.TextField("Output Path(Dir)", outputDir);
        if (refresh_csv || string.IsNullOrEmpty(outputDir))
        {
            // jave.lin : 拼接生成路径
            outputDir = Path.Combine(Application.dataPath, $"Models_From_CSV/{modelName}");
            outputDir = outputDir.Replace("\\", "/");
        }
        if (GUILayout.Button("Browser...", GUILayout.Width(100)))
        {
            outputDir = EditorUtility.OpenFolderPanel("Select an output path", outputDir, "");
        }
        if (GUILayout.Button("Pin", GUILayout.Width(100)))
        {
            var folderPath = "";
            if (outputDir.Contains(Application.dataPath))
            {
                folderPath = "Assets" + outputDir.Replace(Application.dataPath, "");
            }
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folderPath);
            if (obj != null) EditorGUIUtility.PingObject(obj);
        }
        EditorGUILayout.EndHorizontal();
        // jave.lin : 显示导出的 full name
        GUI.enabled = false;
        if (exportFileType == ExportFileType.FBX)
        {
            outputModelPrefabFullName = Path.Combine(outputDir, modelName + ".fbx");
            outputModelPrefabFullName = outputModelPrefabFullName.Replace("\\", "/");
            EditorGUILayout.TextField("Output Full Name", outputModelPrefabFullName);
        }
        else
        {
            outputUnityMeshFullName = Path.Combine(outputDir, modelName + "_mesh.asset");
            outputUnityMeshFullName = outputUnityMeshFullName.Replace("\\", "/");
            outputModelPrefabFullName = Path.Combine(outputDir, modelName + ".prefab");
            outputModelPrefabFullName = outputModelPrefabFullName.Replace("\\", "/");
            EditorGUILayout.TextField("Output Unity Mesh Full Name", outputUnityMeshFullName);
            EditorGUILayout.TextField("Output Model Prefab Full Name", outputModelPrefabFullName);
        }
        GUI.enabled = true;

        // 显示注意选项
        {
            var src_color = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.Label("！！！注意注意！！！如果vertex input 里面的 TEXCOORD[n] 有使用到超过2个分量的数据类型，那么需要使用 UnityMesh 的导出类型，否则FBX中无法存储");

            GUI.contentColor = Color.green;
            if (GUILayout.Button("点击我，了解苦逼测试历程"))
            {
                Application.OpenURL("https://blog.csdn.net/linjf520/article/details/133993603");
            }

            GUI.contentColor = Color.yellow;
            exportFileType = (ExportFileType)EditorGUILayout.EnumPopup("导出文件类型", exportFileType);
            GUI.contentColor = src_color;
        }

        if (exportFileType == ExportFileType.FBX)
        {
            // jave.lin : 导出 CSV 对应的 FBX
            if (GUILayout.Button("Export FBX"))
            {
                ExportFBXHandle();
            }
        }
        else
        {
            // jave.lin : 导出 CSV 对应的 UnityMesh
            if (GUILayout.Button("Export UnityMesh"))
            {
                ExportUnityMeshHandle();
            }
        }

        // jave.lin : 显示 scroll view
        optionsScrollPos = EditorGUILayout.BeginScrollView(optionsScrollPos);

        // jave.lin : options 内容
        EditorGUILayout.Space(10);
        options_show = EditorGUILayout.BeginFoldoutHeaderGroup(options_show, "Model Options");
        if (options_show)
        {
            EditorGUI.indentLevel++;
            model_readable = EditorGUILayout.Toggle("Model Readable", model_readable);
            // jave.lin : 是否从 dx 的 Graphics API 导出而来的 CSV
            is_from_DX_CSV = EditorGUILayout.Toggle("Is From DirectX CSV", is_from_DX_CSV);
            // jave.lin : 是否反转法线 : 通过反转 indices 的顺序即可达到效果
            is_reverse_vertex_order = EditorGUILayout.Toggle("Is Reverse Normal", is_reverse_vertex_order);
            // jave.lin : 是否重新计算 AABB
            is_recalculate_bound = EditorGUILayout.Toggle("Is Recalculate AABB", is_recalculate_bound);
            // jave.lin : 顶点平移
            vertexOffset = EditorGUILayout.Vector3Field("Vertex Offset", vertexOffset);
            // jave.lin : 顶点旋转
            vertexRotation = EditorGUILayout.Vector3Field("Vertex Rotation", vertexRotation);
            // jave.lin : 顶点缩放
            vertexScale = EditorGUILayout.Vector3Field("Vertex Scale", vertexScale);
            // jave.lin : auto toggle includes by csv titles
            if (GUILayout.Button("Auto Toggle Includes by CSV titles"))
            {
                AutoToggleIncludeByCSVTitles();
            }
            // jave.lin : include normal, tangent
            //include_normal, include_tangent
            include_normal = EditorGUILayout.Toggle("Includes normal", include_normal);
            include_tangent = EditorGUILayout.Toggle("Includes tangent", include_tangent);
            // jave.lin : include_uv0,1,2,3,4,5,6,7
            include_uv0 = EditorGUILayout.Toggle("Includes UV0", include_uv0);
            include_uv1 = EditorGUILayout.Toggle("Includes UV1", include_uv1);
            include_uv2 = EditorGUILayout.Toggle("Includes UV2", include_uv2);
            include_uv3 = EditorGUILayout.Toggle("Includes UV3", include_uv3);
            include_uv4 = EditorGUILayout.Toggle("Includes UV4", include_uv4);
            include_uv5 = EditorGUILayout.Toggle("Includes UV5", include_uv5);
            include_uv6 = EditorGUILayout.Toggle("Includes UV6", include_uv6);
            include_uv7 = EditorGUILayout.Toggle("Includes UV7", include_uv7);
            // jave.lin : include_color0
            include_color0 = EditorGUILayout.Toggle("Includes Color0", include_color0);

            // jave.lin : 法线导入方式
            if (include_normal)
            {
                normalImportType = (ModelImporterNormals)EditorGUILayout.EnumPopup("Normal Import Type", normalImportType);
            }
            // jave.lin : 切线导入方式
            if (include_tangent)
            {
                tangentImportType = (ModelImporterTangents)EditorGUILayout.EnumPopup("Tangent Import Type", tangentImportType);
            }
            // jave.lin : semantic 映射类型
            semanticMappingType = (SemanticMappingType)EditorGUILayout.EnumPopup("Semantic Mapping Type", semanticMappingType);
            if (semanticMappingType == SemanticMappingType.ManuallyMapping)
            {
                var refreshCSVSemanticTitle = false;
                if (GUILayout.Button("Refresh Analysis CSV Semantic Title"))
                {
                    refreshCSVSemanticTitle = true;
                }

                if (semanticManullyMappingTypeDict_key_name_helper == null)
                {
                    refreshCSVSemanticTitle = true;
                }

                if (refreshCSVSemanticTitle)
                {
                    Analysis_CSV_SemanticTitle();
                }

                // jave.lin : semantic 的手动设置数据
                var keys = semanticManullyMappingTypeDict_key_name_helper.Keys;
                var stringList = GetStringListHelper();
                stringList.Clear();
                stringList.AddRange(keys);

                // jave.lin : 根据名字排序一下
                stringList.Sort();

                var check_duplicated_helper = GetSemantics_check_duplicated_helper();
                for (int i = 0; i < stringList.Count; i++)
                {
                    if (semanticManullyMappingTypeDict_key_name_helper.TryGetValue(stringList[i], out SemanticType mappedST))
                    {
                        var idx = (int)mappedST;
                        check_duplicated_helper[idx]++;
                    }
                }

                // jave.lin : 显示 semantic manually mapping data 的控件 的 title
                EditorGUILayout.BeginHorizontal();
                {
                    var src_col = GUI.contentColor;
                    GUI.contentColor = Color.yellow;
                    EditorGUILayout.LabelField("CSV Seman Name", GUILayout.Width(120));
                    EditorGUILayout.LabelField("Map To", GUILayout.Width(120));
                    GUI.contentColor = src_col;
                }
                EditorGUILayout.EndHorizontal();

                // jave.lin : 显示 semantic manually mapping data 的控件
                for (int i = 0; i < stringList.Count; i++)
                {
                    var semantic_name = stringList[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(semantic_name, GUILayout.Width(120));
                    
                    if (!semanticManullyMappingTypeDict_key_name_helper.TryGetValue(semantic_name, out SemanticType mappedST))
                    {
                        Debug.LogError($"un mapped semantic name : {semantic_name}");
                        continue;
                    }

                    // jave.lin : 控件显示，修改
                    mappedST = (SemanticType)EditorGUILayout.EnumPopup(mappedST, GUILayout.Width(400));

                    // jave.lin : 重新映射
                    semanticManullyMappingTypeDict_key_name_helper[semantic_name] = mappedST;
                    if (check_duplicated_helper[(int)mappedST] > 1)
                    {
                        var src_col = GUI.contentColor;
                        GUI.contentColor = Color.red;
                        EditorGUILayout.LabelField("Duplicated Options");
                        GUI.contentColor = src_col;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }

                ClearSemantics_check_duplicated_helper(check_duplicated_helper);
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // jave.lin : 是否自动创建材质
        EditorGUILayout.Space(10);
        show_mat_toggle = EditorGUILayout.BeginFoldoutHeaderGroup(show_mat_toggle, "Material Options");
        if (show_mat_toggle)
        {
            EditorGUI.indentLevel++;
            var newMaterialSetType = (MaterialSetType)EditorGUILayout.EnumPopup("Material Set Type", materialSetType);
            if (material == null || materialSetType != newMaterialSetType)
            {
                materialSetType = newMaterialSetType;
                // jave.lin : 创建
                if (materialSetType == MaterialSetType.CreateNew)
                {
                    // jave.lin : shader 不能为空
                    if (shader == null)
                    {
                        shader = GetShader();
                    }
                    material = new Material(shader);
                }
                else
                {
                    // jave.lin : 默认使用 导出目录下的 mat 材质
                    var mat_path = Path.Combine(outputDir, modelName + ".mat").Replace("\\", "/");
                    mat_path = GetAssetPathByFullName(mat_path);
                    var mat_asset = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
                    if (mat_asset != null) material = mat_asset;
                }
            }

            if (materialSetType == MaterialSetType.CreateNew)
            {
                // jave.lin : 使用的 shader
                shader = EditorGUILayout.ObjectField("Shader", shader, typeof(Shader), false) as Shader;
                // jave.lin : 使用的 主纹理
                texture = EditorGUILayout.ObjectField("Main Texture", texture, typeof(Texture), false) as Texture;
            }
            // jave.lin : 设置
            else // MaterialSetType.UseExsitMaterialAsset
            {
                material = EditorGUILayout.ObjectField("Material Asset", material, typeof(Material), false) as Material;
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.EndScrollView();
    }

    private void AutoToggleIncludeByCSVTitles()
    {
        // jave.lin : auto-toggle include
        for (int i = 0; i < semanticsIDX_helper.Length; i++)
        {
            switch (semanticsIDX_helper[i])
            {
                case SemanticType.NORMAL_X:
                case SemanticType.NORMAL_Y:
                case SemanticType.NORMAL_Z:
                case SemanticType.NORMAL_W:
                    include_normal = true;
                    break;

                case SemanticType.TANGENT_X:
                case SemanticType.TANGENT_Y:
                case SemanticType.TANGENT_Z:
                case SemanticType.TANGENT_W:
                    include_tangent = true;
                    break;

                case SemanticType.TEXCOORD0_X:
                case SemanticType.TEXCOORD0_Y:
                case SemanticType.TEXCOORD0_Z:
                case SemanticType.TEXCOORD0_W:
                    include_uv0 = true;
                    break;

                case SemanticType.TEXCOORD1_X:
                case SemanticType.TEXCOORD1_Y:
                case SemanticType.TEXCOORD1_Z:
                case SemanticType.TEXCOORD1_W:
                    include_uv1 = true;
                    break;

                case SemanticType.TEXCOORD2_X:
                case SemanticType.TEXCOORD2_Y:
                case SemanticType.TEXCOORD2_Z:
                case SemanticType.TEXCOORD2_W:
                    include_uv2 = true;
                    break;

                case SemanticType.TEXCOORD3_X:
                case SemanticType.TEXCOORD3_Y:
                case SemanticType.TEXCOORD3_Z:
                case SemanticType.TEXCOORD3_W:
                    include_uv3 = true;
                    break;

                case SemanticType.TEXCOORD4_X:
                case SemanticType.TEXCOORD4_Y:
                case SemanticType.TEXCOORD4_Z:
                case SemanticType.TEXCOORD4_W:
                    include_uv4 = true;
                    break;

                case SemanticType.TEXCOORD5_X:
                case SemanticType.TEXCOORD5_Y:
                case SemanticType.TEXCOORD5_Z:
                case SemanticType.TEXCOORD5_W:
                    include_uv5 = true;
                    break;

                case SemanticType.TEXCOORD6_X:
                case SemanticType.TEXCOORD6_Y:
                case SemanticType.TEXCOORD6_Z:
                case SemanticType.TEXCOORD6_W:
                    include_uv6 = true;
                    break;

                case SemanticType.TEXCOORD7_X:
                case SemanticType.TEXCOORD7_Y:
                case SemanticType.TEXCOORD7_Z:
                case SemanticType.TEXCOORD7_W:
                    include_uv7 = true;
                    break;

                case SemanticType.COLOR0_X:
                case SemanticType.COLOR0_Y:
                case SemanticType.COLOR0_Z:
                case SemanticType.COLOR0_W:
                    include_color0 = true;
                    break;
            }
        }
    }

    private void Analysis_CSV_SemanticTitle()
    {
        if (semanticManullyMappingTypeDict_key_name_helper != null)
        {
            semanticManullyMappingTypeDict_key_name_helper.Clear();
        }
        else
        {
            semanticManullyMappingTypeDict_key_name_helper = new Dictionary<string, SemanticType>();
        }
        var text = RDC_Text_Asset.text;
        var firstLine = text.Substring(0, text.IndexOf("\n")).Trim();
        var line_element_splitor = new string[] { "," };
        // jave.lin : 构建 vertex buffer format 的 semantics 和 idx 的对应关系
        var semanticTitles = firstLine.Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

        // jave.lin : 先加载 semanticTypeDict_key_name_helper 的映射
        MappingSemanticsTypeByNames(ref semanticTypeDict_key_name_helper);

        for (int i = 0; i < semanticTitles.Length; i++)
        {
            var semantics = semanticTitles[i].Trim().ToUpper();
            if (semanticTypeDict_key_name_helper.TryGetValue(semantics, out SemanticType semanticType))
            {
                //Debug.Log($"semantics : {semantics.Trim()}, type : {semanticType}");
                semanticManullyMappingTypeDict_key_name_helper[semantics] = semanticType;
            }
            else
            {
                //Debug.LogError($"Cannot find the semantic mapping data : {semantics}");
                semanticManullyMappingTypeDict_key_name_helper[semantics] = SemanticType.Unknow;
            }
        }
    }

    // jave.lin : 导出FBX处理
    private void ExportFBXHandle()
    {
        if (RDC_Text_Asset != null)
        {
            try
            {
                // jave.lin : 先映射好 semantics 名字和类型
                MappingSemanticsTypeByNames(ref semanticTypeDict_key_name_helper);
                // jave.lin : 清理之前的 GO
                var parent = GetParentTrans();
                // jave.lin : 将 CSV 的内容转为 MeshRenderer 的 GO
                var outputGO = GameObject.Find($"{GO_Parent_Name }/{modelName}");
                if (outputGO != null)
                {
                    GameObject.DestroyImmediate(outputGO);
                }
                outputGO = GenerateGOWithMeshRendererFromCSV(RDC_Text_Asset.text, is_from_DX_CSV);
                outputGO.transform.SetParent(parent);
                outputGO.name = modelName;

                //// jave.lin : 先清理目录下的内容
                //DelectDir(outputDir);
                // jave.lin : 然后重新创建新的目录
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (materialSetType == MaterialSetType.CreateNew)
                {
                    // jave.lin : 自动创建材质
                    var create_mat = outputGO.GetComponent<MeshRenderer>().sharedMaterial;
                    // jave.lin : 创建前，先设置主纹理
                    create_mat.mainTexture = texture;

                    var mat_created_path = Path.Combine(outputDir, modelName + ".mat").Replace("\\", "/");
                    mat_created_path = GetAssetPathByFullName(mat_created_path);
                    Debug.Log($"mat_created_path : {mat_created_path}");
                    // jave.lin : 先删除原来的
                    var src_mat = AssetDatabase.LoadAssetAtPath<Material>(mat_created_path);
                    if (src_mat == create_mat)
                    {
                        // nop
                    }
                    else
                    {
                        AssetDatabase.DeleteAsset(mat_created_path);
                        AssetDatabase.CreateAsset(create_mat, mat_created_path);
                    }
                }

                // jave.lin : 使用 FBX Exporter 插件导出 FBX
                ModelExporter.ExportObject(outputModelPrefabFullName, outputGO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // jave.lin : 重新设置 MI，并且重新导入
                string mi_path = GetAssetPathByFullName(outputModelPrefabFullName);
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
                mi.isReadable = model_readable;
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
                outputGO.name = modelName;
                // jave.lin : set material
                var mat_path = Path.Combine(outputDir, modelName + ".mat").Replace("\\", "/");
                mat_path = GetAssetPathByFullName(mat_path);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
                outputGO.GetComponent<MeshRenderer>().sharedMaterial = mat;
                // jave.lin : new real prefab
                var prefab_created_path = Path.Combine(outputDir, modelName + ".prefab").Replace("\\", "/");
                prefab_created_path = GetAssetPathByFullName(prefab_created_path);
                Debug.Log($"prefab_created_path : {prefab_created_path}");
                PrefabUtility.SaveAsPrefabAssetAndConnect(outputGO, prefab_created_path, InteractionMode.AutomatedAction);

                // jave.lin : 打印打出成功的信息
                Debug.Log($"Export FBX Successfully! outputPath : {outputModelPrefabFullName}");
            }
            catch (Exception er)
            {
                Debug.LogError($"Export FBX Failed! er: {er}");
            }
        }
    }

    // jave.lin : 导出UnityMesh处理
    private void ExportUnityMeshHandle()
    {
        if (RDC_Text_Asset != null)
        {
            try
            {
                // jave.lin : 先映射好 semantics 名字和类型
                MappingSemanticsTypeByNames(ref semanticTypeDict_key_name_helper);
                // jave.lin : 清理之前的 GO
                var parent = GetParentTrans();
                // jave.lin : 将 CSV 的内容转为 MeshRenderer 的 GO
                var outputGO = GameObject.Find($"{GO_Parent_Name}/{modelName}");
                if (outputGO != null)
                {
                    GameObject.DestroyImmediate(outputGO);
                }

                outputGO = new GameObject(modelName);
                var mesh = GenerateMeshFromCSV(RDC_Text_Asset.text, is_from_DX_CSV);

                var meshFilter = outputGO.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;

                var meshRenderer = outputGO.AddComponent<MeshRenderer>();

                // jave.lin : 默认使用 URP 的 PBR Shader
                meshRenderer.sharedMaterial = material;

                outputGO.transform.localPosition = Vector3.zero;
                outputGO.transform.localRotation = Quaternion.identity;
                outputGO.transform.localScale = Vector3.one;

                outputGO.transform.SetParent(parent);
                outputGO.name = modelName;

                //// jave.lin : 先清理目录下的内容
                //DelectDir(outputDir);
                // jave.lin : 然后重新创建新的目录
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (materialSetType == MaterialSetType.CreateNew)
                {
                    // jave.lin : 自动创建材质
                    var create_mat = outputGO.GetComponent<MeshRenderer>().sharedMaterial;
                    // jave.lin : 创建前，先设置主纹理
                    create_mat.mainTexture = texture;

                    var mat_created_path = Path.Combine(outputDir, modelName + ".mat").Replace("\\", "/");
                    mat_created_path = GetAssetPathByFullName(mat_created_path);
                    Debug.Log($"mat_created_path : {mat_created_path}");
                    // jave.lin : 先删除原来的
                    var src_mat = AssetDatabase.LoadAssetAtPath<Material>(mat_created_path);
                    if (src_mat == create_mat)
                    {
                        // nop
                    }
                    else
                    {
                        AssetDatabase.DeleteAsset(mat_created_path);
                        AssetDatabase.CreateAsset(create_mat, mat_created_path);
                    }
                }

                // jave.lin : 导出 unity mesh 保留 uv 超过 2 个分量以上的数据
                MeshUtility.Optimize(mesh);
                var so = new SerializedObject(mesh);
                var sp = so.FindProperty("m_IsReadable");
                sp.boolValue = model_readable;
                so.ApplyModifiedPropertiesWithoutUndo();
                var outputMeshPath = GetAssetPathByFullName(outputUnityMeshFullName);
                AssetDatabase.CreateAsset(mesh, outputMeshPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // jave.lin : 如果 readable == false，那么 将 主存那份内存删除
                if (!model_readable)
                {
                    mesh.UploadMeshData(true);
                }

                // jave.lin : 重新设置 mesh
                mesh = AssetDatabase.LoadAssetAtPath<Mesh>(outputMeshPath);
                outputGO.GetComponent<MeshFilter>().sharedMesh = mesh;

                // jave.lin : 导出 prefab
                //AssetDatabase.CreateAsset(outputGO, outputModelPrefabFullName);
                var outputPrefabPath = GetAssetPathByFullName(outputModelPrefabFullName);
                PrefabUtility.SaveAsPrefabAssetAndConnect(outputGO, outputPrefabPath, InteractionMode.AutomatedAction);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // jave.lin : replace outputGO from model prefab
                var src_parent = outputGO.transform.parent;
                var src_local_pos = outputGO.transform.localPosition;
                var src_local_rot = outputGO.transform.localRotation;
                var src_local_scl = outputGO.transform.localScale;
                //DestroyImmediate(outputGO);
                // jave.lin : new model prefab
                //var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(outputModelPrefabFullName);
                //outputGO = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                outputGO.transform.SetParent(src_parent);
                outputGO.transform.localPosition = src_local_pos;
                outputGO.transform.localRotation = src_local_rot;
                outputGO.transform.localScale = src_local_scl;
                outputGO.name = modelName;
                // jave.lin : set material
                if (materialSetType == MaterialSetType.CreateNew)
                {
                    var mat_path = Path.Combine(outputDir, modelName + ".mat").Replace("\\", "/");
                    mat_path = GetAssetPathByFullName(mat_path);
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
                    outputGO.GetComponent<MeshRenderer>().sharedMaterial = mat;
                }
                else
                {
                    outputGO.GetComponent<MeshRenderer>().sharedMaterial = material;
                }
                // jave.lin : new real regular prefab
                var prefab_created_path = Path.Combine(outputDir, modelName + ".prefab").Replace("\\", "/");
                prefab_created_path = GetAssetPathByFullName(prefab_created_path);
                Debug.Log($"prefab_created_path : {prefab_created_path}");
                PrefabUtility.SaveAsPrefabAssetAndConnect(outputGO, prefab_created_path, InteractionMode.AutomatedAction);

                // jave.lin : 打印打出成功的信息
                Debug.Log($"Export Prefab & UnityMesh Successfully! outputPath : {outputModelPrefabFullName}");
            }
            catch (Exception er)
            {
                Debug.LogError($"Export FBX Failed! er: {er}");
            }
        }
    }

    // jave.lin : 映射 semantics 的 name 和 type
    private void MappingSemanticsTypeByNames(ref Dictionary<string, SemanticType> container)
    {
        if (container == null)
        {
            container = new Dictionary<string, SemanticType>();
        }
        else
        {
            container.Clear();
        }
        container["VTX"] = SemanticType.VTX;
        container["IDX"] = SemanticType.IDX;

        container["SV_POSITION.X"] = SemanticType.POSITION_X;
        container["SV_POSITION.Y"] = SemanticType.POSITION_Y;
        container["SV_POSITION.Z"] = SemanticType.POSITION_Z;
        container["SV_POSITION.W"] = SemanticType.POSITION_W;
        container["POSITION.X"] = SemanticType.POSITION_X;
        container["POSITION.Y"] = SemanticType.POSITION_Y;
        container["POSITION.Z"] = SemanticType.POSITION_Z;
        container["POSITION.W"] = SemanticType.POSITION_W;
        container["POSITION0.X"] = SemanticType.POSITION_X;
        container["POSITION0.Y"] = SemanticType.POSITION_Y;
        container["POSITION0.Z"] = SemanticType.POSITION_Z;
        container["POSITION0.W"] = SemanticType.POSITION_W;
        container["IN_POSITION0.X"] = SemanticType.POSITION_X;
        container["IN_POSITION0.Y"] = SemanticType.POSITION_Y;
        container["IN_POSITION0.Z"] = SemanticType.POSITION_Z;
        container["IN_POSITION0.W"] = SemanticType.POSITION_W;

        container["NORMAL.X"] = SemanticType.NORMAL_X;
        container["NORMAL.Y"] = SemanticType.NORMAL_Y;
        container["NORMAL.Z"] = SemanticType.NORMAL_Z;
        container["NORMAL.W"] = SemanticType.NORMAL_W;
        container["NORMAL0.X"] = SemanticType.NORMAL_X;
        container["NORMAL0.Y"] = SemanticType.NORMAL_Y;
        container["NORMAL0.Z"] = SemanticType.NORMAL_Z;
        container["NORMAL0.W"] = SemanticType.NORMAL_W;
        container["IN_NORMAL0.X"] = SemanticType.NORMAL_X;
        container["IN_NORMAL0.Y"] = SemanticType.NORMAL_Y;
        container["IN_NORMAL0.Z"] = SemanticType.NORMAL_Z;
        container["IN_NORMAL0.W"] = SemanticType.NORMAL_W;

        container["TANGENT.X"] = SemanticType.TANGENT_X;
        container["TANGENT.Y"] = SemanticType.TANGENT_Y;
        container["TANGENT.Z"] = SemanticType.TANGENT_Z;
        container["TANGENT.W"] = SemanticType.TANGENT_W;
        container["TANGENT0.X"] = SemanticType.TANGENT_X;
        container["TANGENT0.Y"] = SemanticType.TANGENT_Y;
        container["TANGENT0.Z"] = SemanticType.TANGENT_Z;
        container["TANGENT0.W"] = SemanticType.TANGENT_W;
        container["IN_TANGENT0.X"] = SemanticType.TANGENT_X;
        container["IN_TANGENT0.Y"] = SemanticType.TANGENT_Y;
        container["IN_TANGENT0.Z"] = SemanticType.TANGENT_Z;
        container["IN_TANGENT0.W"] = SemanticType.TANGENT_W;

        container["TEXCOORD.X"] = SemanticType.TEXCOORD0_X;
        container["TEXCOORD.Y"] = SemanticType.TEXCOORD0_Y;
        container["TEXCOORD.Z"] = SemanticType.TEXCOORD0_Z;
        container["TEXCOORD.W"] = SemanticType.TEXCOORD0_W;
        container["TEXCOORD0.X"] = SemanticType.TEXCOORD0_X;
        container["TEXCOORD0.Y"] = SemanticType.TEXCOORD0_Y;
        container["TEXCOORD0.Z"] = SemanticType.TEXCOORD0_Z;
        container["TEXCOORD0.W"] = SemanticType.TEXCOORD0_W;
        container["IN_TEXCOORD0.X"] = SemanticType.TEXCOORD0_X;
        container["IN_TEXCOORD0.Y"] = SemanticType.TEXCOORD0_Y;
        container["IN_TEXCOORD0.Z"] = SemanticType.TEXCOORD0_Z;
        container["IN_TEXCOORD0.W"] = SemanticType.TEXCOORD0_W;

        container["TEXCOORD1.X"] = SemanticType.TEXCOORD1_X;
        container["TEXCOORD1.Y"] = SemanticType.TEXCOORD1_Y;
        container["TEXCOORD1.Z"] = SemanticType.TEXCOORD1_Z;
        container["TEXCOORD1.W"] = SemanticType.TEXCOORD1_W;
        container["IN_TEXCOORD1.X"] = SemanticType.TEXCOORD1_X;
        container["IN_TEXCOORD1.Y"] = SemanticType.TEXCOORD1_Y;
        container["IN_TEXCOORD1.Z"] = SemanticType.TEXCOORD1_Z;
        container["IN_TEXCOORD1.W"] = SemanticType.TEXCOORD1_W;

        container["TEXCOORD2.X"] = SemanticType.TEXCOORD2_X;
        container["TEXCOORD2.Y"] = SemanticType.TEXCOORD2_Y;
        container["TEXCOORD2.Z"] = SemanticType.TEXCOORD2_Z;
        container["TEXCOORD2.W"] = SemanticType.TEXCOORD2_W;
        container["IN_TEXCOORD2.X"] = SemanticType.TEXCOORD2_X;
        container["IN_TEXCOORD2.Y"] = SemanticType.TEXCOORD2_Y;
        container["IN_TEXCOORD2.Z"] = SemanticType.TEXCOORD2_Z;
        container["IN_TEXCOORD2.W"] = SemanticType.TEXCOORD2_W;

        container["TEXCOORD3.X"] = SemanticType.TEXCOORD3_X;
        container["TEXCOORD3.Y"] = SemanticType.TEXCOORD3_Y;
        container["TEXCOORD3.Z"] = SemanticType.TEXCOORD3_Z;
        container["TEXCOORD3.W"] = SemanticType.TEXCOORD3_W;
        container["IN_TEXCOORD3.X"] = SemanticType.TEXCOORD3_X;
        container["IN_TEXCOORD3.Y"] = SemanticType.TEXCOORD3_Y;
        container["IN_TEXCOORD3.Z"] = SemanticType.TEXCOORD3_Z;
        container["IN_TEXCOORD3.W"] = SemanticType.TEXCOORD3_W;

        container["TEXCOORD4.X"] = SemanticType.TEXCOORD4_X;
        container["TEXCOORD4.Y"] = SemanticType.TEXCOORD4_Y;
        container["TEXCOORD4.Z"] = SemanticType.TEXCOORD4_Z;
        container["TEXCOORD4.W"] = SemanticType.TEXCOORD4_W;
        container["IN_TEXCOORD4.X"] = SemanticType.TEXCOORD4_X;
        container["IN_TEXCOORD4.Y"] = SemanticType.TEXCOORD4_Y;
        container["IN_TEXCOORD4.Z"] = SemanticType.TEXCOORD4_Z;
        container["IN_TEXCOORD4.W"] = SemanticType.TEXCOORD4_W;

        container["TEXCOORD5.X"] = SemanticType.TEXCOORD5_X;
        container["TEXCOORD5.Y"] = SemanticType.TEXCOORD5_Y;
        container["TEXCOORD5.Z"] = SemanticType.TEXCOORD5_Z;
        container["TEXCOORD5.W"] = SemanticType.TEXCOORD5_W;
        container["IN_TEXCOORD5.X"] = SemanticType.TEXCOORD5_X;
        container["IN_TEXCOORD5.Y"] = SemanticType.TEXCOORD5_Y;
        container["IN_TEXCOORD5.Z"] = SemanticType.TEXCOORD5_Z;
        container["IN_TEXCOORD5.W"] = SemanticType.TEXCOORD5_W;

        container["TEXCOORD6.X"] = SemanticType.TEXCOORD6_X;
        container["TEXCOORD6.Y"] = SemanticType.TEXCOORD6_Y;
        container["TEXCOORD6.Z"] = SemanticType.TEXCOORD6_Z;
        container["TEXCOORD6.W"] = SemanticType.TEXCOORD6_W;
        container["IN_TEXCOORD6.X"] = SemanticType.TEXCOORD6_X;
        container["IN_TEXCOORD6.Y"] = SemanticType.TEXCOORD6_Y;
        container["IN_TEXCOORD6.Z"] = SemanticType.TEXCOORD6_Z;
        container["IN_TEXCOORD6.W"] = SemanticType.TEXCOORD6_W;

        container["TEXCOORD7.X"] = SemanticType.TEXCOORD7_X;
        container["TEXCOORD7.Y"] = SemanticType.TEXCOORD7_Y;
        container["TEXCOORD7.Z"] = SemanticType.TEXCOORD7_Z;
        container["TEXCOORD7.W"] = SemanticType.TEXCOORD7_W;
        container["IN_TEXCOORD7.X"] = SemanticType.TEXCOORD7_X;
        container["IN_TEXCOORD7.Y"] = SemanticType.TEXCOORD7_Y;
        container["IN_TEXCOORD7.Z"] = SemanticType.TEXCOORD7_Z;
        container["IN_TEXCOORD7.W"] = SemanticType.TEXCOORD7_W;

        container["COLOR0.X"] = SemanticType.COLOR0_X;
        container["COLOR0.Y"] = SemanticType.COLOR0_Y;
        container["COLOR0.Z"] = SemanticType.COLOR0_Z;
        container["COLOR0.W"] = SemanticType.COLOR0_W;
        container["COLOR.X"] = SemanticType.COLOR0_X;
        container["COLOR.Y"] = SemanticType.COLOR0_Y;
        container["COLOR.Z"] = SemanticType.COLOR0_Z;
        container["COLOR.W"] = SemanticType.COLOR0_W;
        container["IN_COLOR0.X"] = SemanticType.COLOR0_X;
        container["IN_COLOR0.Y"] = SemanticType.COLOR0_Y;
        container["IN_COLOR0.Z"] = SemanticType.COLOR0_Z;
        container["IN_COLOR0.W"] = SemanticType.COLOR0_W;
    }

    // jave.lin : 获取 parent transform 对象
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

    // jave.lin : 根据名字生成 GO Name
    private string GenerateGOName(TextAsset ta)
    {
        //return $"From_CSV_{ta.text.GetHashCode()}";
        //return $"From_CSV_{ta.name}";
        return ta.name;
    }

    // jave.lin : 根据 CSV 内容生成 MeshRenderer 对应的 GO
    private GameObject GenerateGOWithMeshRendererFromCSV(string csv, bool is_from_DX_CSV)
    {
        var ret = new GameObject();

        var mesh = new Mesh();

        // jave.lin : 根据 csv 来填充 mesh 信息
        FillMeshFromCSV(mesh, csv, is_from_DX_CSV);

        var meshFilter = ret.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = ret.AddComponent<MeshRenderer>();

        // jave.lin : 默认使用 URP 的 PBR Shader
        meshRenderer.sharedMaterial = material;

        ret.transform.position = Vector3.zero;
        ret.transform.localRotation = Quaternion.identity;
        ret.transform.localScale = Vector3.one;

        return ret;
    }

    // jave.lin : 根据 CSV 内容生成 Mesh
    private Mesh GenerateMeshFromCSV(string csv, bool is_from_DX_CSV)
    {
        var mesh = new Mesh();

        // jave.lin : 根据 csv 来填充 mesh 信息
        FillMeshFromCSV(mesh, csv, is_from_DX_CSV);

        return mesh;
    }

    // jave.lin : 根据 semantic type 和 data 来填充到 数据字段
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
            case SemanticType.POSITION_W:
                info.POSITION_W = float.Parse(data);
                Debug.LogWarning("WARNING: unity mesh cannot transfer position.w to shader program.");
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
            case SemanticType.Unknow:
                // jave.lin : nop
                break;
            // jave.lin : un-implements
            default:
                Debug.LogError($"Fill_A2V_Common_Type_Data un-implements SemanticType : {semanticType}");
                break;
        }
    }

    // jave.lin : 根据 csv 来填充 mesh 信息
    private void FillMeshFromCSV(Mesh mesh, string csv, bool is_from_DX_CSV)
    {
        var line_splitor = new string[] { "\n" };
        var line_element_splitor = new string[] { "," };

        var lines = csv.Split(line_splitor, StringSplitOptions.RemoveEmptyEntries);

        // jave.lin : lines[0] == "VTX, IDX, POSITION.x, POSITION.y, POSITION.z, NORMAL.x, NORMAL.y, NORMAL.z, NORMAL.w, TANGENT.x, TANGENT.y, TANGENT.z, TANGENT.w, TEXCOORD0.x, TEXCOORD0.y"

        // jave.lin : 构建 vertex buffer format 的 semantics 和 idx 的对应关系
        var semanticTitles = lines[0].Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, SemanticType> semantic_type_map_key_name;
        if (semanticMappingType == SemanticMappingType.Default)
        {
            semantic_type_map_key_name = semanticTypeDict_key_name_helper;
        }
        else
        {
            semantic_type_map_key_name = semanticManullyMappingTypeDict_key_name_helper;
        }

        semanticsIDX_helper = new SemanticType[semanticTitles.Length];
        Debug.Log($"semanticTitles : {lines[0]}");
        for (int i = 0; i < semanticTitles.Length; i++)
        {
            var semantics = semanticTitles[i].Trim().ToUpper();
            if (semantic_type_map_key_name.TryGetValue(semantics, out SemanticType semanticType))
            {
                semanticsIDX_helper[i] = semanticType;
                //Debug.Log($"semantics : {semantics}, type : {semanticType}");
            }
            else
            {
                Debug.LogWarning($"un-implements semantic : {semantics}");
            }
        }

        // jave.lin : 先根据 IDX 来排序还原 vertex buffer 的内容
        // lines[1~count-1] : 比如： 0, 0,  0.0402, -1.57095E-17,  0.12606, -0.97949,  0.00, -0.20056,  0.00,  0.1098,  0.83691, -0.53613,  1.00, -0.06058,  0.81738

        Dictionary<int, VertexInfo> vertex_dict_key_idx = new Dictionary<int, VertexInfo>();

        var indices = new List<int>();

        var min_idx = int.MaxValue;
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var linesElements = line.Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

            // jave.lin : 第几个顶点索引（0~count-1)
            var idx = int.Parse(linesElements[1]);
            if (min_idx > idx)
            {
                min_idx = idx;
            }
        }

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var linesElements = line.Split(line_element_splitor, StringSplitOptions.RemoveEmptyEntries);

            // jave.lin : 第几个顶点索引（0~count-1)
            var idx = int.Parse(linesElements[1]) - min_idx;

            // jave.lin : indices 缓存索引数据的添加
            indices.Add(idx);

            // jave.lin : 如果该 vertex 没有处理过，那么才去处理
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

        // jave.lin : 缩放、旋转、平移
        var rotation = Quaternion.Euler(vertexRotation);
        var TRS_mat = Matrix4x4.TRS(vertexOffset, rotation, vertexScale);
        // jave.lin : 法线变换矩阵需要特殊处理，针对 vertex scale 为非 uniform scale 的情况
        // ref : LearnGL - 11.5 - 矩阵04 - 法线从对象空间变换到世界空间
        // https://blog.csdn.net/linjf520/article/details/107501215
        var M_IT_mat = Matrix4x4.TRS(Vector3.zero, rotation, vertexScale).inverse.transpose;

        // jave.lin : composite the data （最后就是我们要组合数据，统一赋值给 mesh）
        var vertices = new Vector3[vertex_dict_key_idx.Count];
        var normals = new Vector3[vertex_dict_key_idx.Count];
        var tangents = new Vector4[vertex_dict_key_idx.Count];
        var uv = new Vector2[vertex_dict_key_idx.Count];
        var uv2 = new Vector2[vertex_dict_key_idx.Count];
        var uv3 = new Vector2[vertex_dict_key_idx.Count];
        var uv4 = new Vector2[vertex_dict_key_idx.Count];
        var uv5 = new Vector2[vertex_dict_key_idx.Count];
        var uv6 = new Vector2[vertex_dict_key_idx.Count];
        var uv7 = new Vector2[vertex_dict_key_idx.Count];
        var uv8 = new Vector2[vertex_dict_key_idx.Count];
        var color0 = new Color[vertex_dict_key_idx.Count];

        // jave.lin : 根据 0~count 的索引顺序来组织相关的 vertex 数据
        for (int idx = 0; idx < vertices.Length; idx++)
        {
            var info = vertex_dict_key_idx[idx];
            vertices[idx] = TRS_mat * info.POSITION_H;
            normals[idx] = M_IT_mat * info.NORMAL;
            tangents[idx] = info.TANGENT;
            uv[idx] = info.TEXCOORD0;
            uv2[idx] = info.TEXCOORD1;
            uv3[idx] = info.TEXCOORD2;
            uv4[idx] = info.TEXCOORD3;
            uv5[idx] = info.TEXCOORD4;
            uv6[idx] = info.TEXCOORD5;
            uv7[idx] = info.TEXCOORD6;
            uv8[idx] = info.TEXCOORD7;
            color0[idx] = info.COLOR0;
        }

        // jave.lin : 设置 mesh 信息
        mesh.vertices = vertices;

        // jave.lin : 是否 reverse idx
        if (is_reverse_vertex_order) indices.Reverse();
        mesh.triangles = indices.ToArray();

        // jave.lin : unity 不能超过 uv[0~7]
        mesh.uv = include_uv0 ? uv : null;
        mesh.uv2 = include_uv1 ? uv2 : null;
        mesh.uv3 = include_uv2 ? uv3 : null;
        mesh.uv4 = include_uv3 ? uv4 : null;
        mesh.uv5 = include_uv4 ? uv5 : null;
        mesh.uv6 = include_uv5 ? uv6 : null;
        mesh.uv7 = include_uv6 ? uv7 : null;
        mesh.uv8 = include_uv7 ? uv8 : null;

        mesh.colors = include_color0 ? color0 : null;

        // jave.lin : AABB
        if (is_recalculate_bound)
        {
            mesh.RecalculateBounds();
        }

        if (include_normal)
        {
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
        }

        if (include_tangent)
        {
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
        }

        //// jave.lin : 打印一下
        //Debug.Log("FillMeshFromCSV done!");
    }
}
