using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetImporterWindow : EditorWindow
{
    private AssetType _selectedAssetType;
    private Vector2 _rect = new Vector2(200f, 200f);
    
    private List<AssetInfo> _assetInfos = new();
    private Vector2 _scrollPosition;

    private TextureImporterCompression _targetTextureCompression = TextureImporterCompression.Compressed;
    private bool _targetCrunchCompression;
    private ModelImporterMeshCompression _targetMeshCompression = ModelImporterMeshCompression.Off; 

    private class AssetInfo
    {
        public string Path;
        public string DisplayName;
        public AssetImporter Importer;
        public Object AssetObject;
    }
    
   public void OnGUI()
    {
        GUILayout.Label("Asset importer global settings", EditorStyles.boldLabel);
        
        _selectedAssetType = (AssetType)EditorGUILayout.EnumPopup("Asset type :", _selectedAssetType);
        
        if (GUILayout.Button("Refresh assets"))
        {
            RefreshAssetList();
        }
        EditorGUILayout.Space();
        
        if (_selectedAssetType != AssetType.None)
        {
            EditorGUILayout.LabelField("Assets settings for all of the assets below", EditorStyles.boldLabel);
            DrawBulkSettingsControls();
            EditorGUILayout.Space();
            
            if (_assetInfos.Count > 0)
            {
                if (GUILayout.Button($"Apply settings to {_assetInfos.Count} {_selectedAssetType}(s)"))
                {
                    if (EditorUtility.DisplayDialog("Are you sure to apply changes ?",
                        $"Are you sure to apply changes to {_assetInfos.Count} assets of type '{_selectedAssetType}'?\n",
                        "Yes", "Cancel"))
                    {
                        ApplyBulkSettings();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("There are no any assets here, try to refresh it", MessageType.Info);
            }
        }
        else
        {
             EditorGUILayout.HelpBox("Choose asset type and refresh the list", MessageType.Info);
        }


        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Found assets", EditorStyles.boldLabel);
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        if (_assetInfos.Count > 0)
        {
            foreach (var info in _assetInfos)
            {
                DrawAssetInfo(info);
                EditorGUILayout.Separator();
            }
        }
        else
        {
            GUILayout.Label("There are no assets to display");
        }
        EditorGUILayout.EndScrollView();
    }

   
    void DrawBulkSettingsControls()
    {
        switch (_selectedAssetType)
        {
            case AssetType.Texture:
            case AssetType.Sprite:
                _targetTextureCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", _targetTextureCompression);
                _targetCrunchCompression = EditorGUILayout.Toggle("Use crunch", _targetCrunchCompression);
                break;
            case AssetType.Mesh:
                _targetMeshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh compression", _targetMeshCompression);
                break;
        }
    }
    
    void DrawAssetInfo(AssetInfo info)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(info.DisplayName, EditorStyles.label, GUILayout.Width(position.width * 0.5f)))
        {
            Selection.activeObject = info.AssetObject;
            EditorGUIUtility.PingObject(info.AssetObject);
        }
        
        switch (info.Importer)
        {
            case TextureImporter ti:
                EditorGUILayout.LabelField($"Comp: {ti.textureCompression}", GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField($"Crunch: {(ti.crunchedCompression ? "On" : "Off")}", GUILayout.ExpandWidth(false));
                break;
            case ModelImporter mi:
                EditorGUILayout.LabelField($"Mesh Comp: {mi.meshCompression}", GUILayout.ExpandWidth(false));
                break;
        }
        EditorGUILayout.EndHorizontal();
    }
    
    void RefreshAssetList()
    {
        _assetInfos.Clear();
        if (_selectedAssetType == AssetType.None) return;

        var searchFilter = "";
        
        switch (_selectedAssetType)
        {
            case AssetType.Texture: 
                searchFilter = "t:Texture";
                break;
            case AssetType.Sprite: 
                searchFilter = "t:Sprite"; 
                break;
            case AssetType.Mesh:
                searchFilter = "t:Model";
                break;
        }

        var guids = AssetDatabase.FindAssets(searchFilter);
        Debug.Log($"Found {guids.Length} assets for filter '{searchFilter}'.");

        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith("Packages/")) continue;

                var importer = AssetImporter.GetAtPath(path);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

                if (importer != null && asset != null)
                {
                    bool typeMatch = false;
                    
                    switch (_selectedAssetType)
                    {
                        case AssetType.Texture:
                            typeMatch = importer is TextureImporter texImp && texImp.textureType != TextureImporterType.Sprite;
                            break;
                        case AssetType.Sprite:
                            typeMatch = importer is TextureImporter { textureType: TextureImporterType.Sprite };
                            break;
                        case AssetType.Mesh:
                            typeMatch = importer is ModelImporter;
                            break;
                    }

                    if (typeMatch)
                    {
                        _assetInfos.Add(new AssetInfo
                        {
                            Path = path,
                            DisplayName = System.IO.Path.GetFileName(path),
                            Importer = importer,
                            AssetObject = asset
                        });
                    }
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
        
        _assetInfos = _assetInfos.OrderBy(a => a.DisplayName).ToList();
    }
    
    void ApplyBulkSettings()
    {
        if (_assetInfos.Count == 0) return;

        try
        {
            AssetDatabase.StartAssetEditing();
            var total = _assetInfos.Count;

            for (var i = 0; i < total; i++)
            {
                var info = _assetInfos[i];
                var progress = (float)i / total;
                if (EditorUtility.DisplayCancelableProgressBar($"Applying settings to {_selectedAssetType}(s)", $"Completed : {info.DisplayName} ({i + 1}/{total})", progress))
                {
                    break;
                }

                var changed = false;
                switch (info.Importer)
                {
                    case TextureImporter ti:
                        if ((_selectedAssetType == AssetType.Texture && ti.textureType != TextureImporterType.Sprite) ||
                            (_selectedAssetType == AssetType.Sprite && ti.textureType == TextureImporterType.Sprite))
                        {
                            if (ti.textureCompression != _targetTextureCompression) { ti.textureCompression = _targetTextureCompression; changed = true; }
                            if (ti.crunchedCompression != _targetCrunchCompression) { ti.crunchedCompression = _targetCrunchCompression; changed = true; }
                        }
                        break;
                    case ModelImporter mi:
                        if (_selectedAssetType == AssetType.Mesh)
                        {
                            if (mi.meshCompression != _targetMeshCompression) { mi.meshCompression = _targetMeshCompression; changed = true; }
                        }
                        break;
                }

                if (!changed)
                    continue;
                
                EditorUtility.SetDirty(info.Importer);
                AssetDatabase.WriteImportSettingsIfDirty(info.Path);
                AssetDatabase.ImportAsset(info.Path, ImportAssetOptions.DontDownloadFromCacheServer);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
            RefreshAssetList();
        }
    }
}

public enum AssetType
{
    None,
    Texture,
    Sprite,
    Mesh
}