using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine.Sprites;
using UnityEditor.AddressableAssets;
using System.Reflection;


public class GenericSpriteAtlasGenerator : Editor
{
    public static string[] storySelected = new string[] { "story1", "story2", "story3", "story4", "story5" } ;
    public static int actualSelectedStory;

    static MethodInfo _clearConsoleMethod;
    static MethodInfo clearConsoleMethod
    {
        get
        {
            if (_clearConsoleMethod == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                _clearConsoleMethod = logEntries.GetMethod("Clear");
            }
            return _clearConsoleMethod;
        }
    }
    public static void ClearLogConsole()
    {
        clearConsoleMethod.Invoke(new object(), null);
    }
    [MenuItem("Assets/Check if File is on SpriteAtlas")]
    [MenuItem("Generate Files/Check if File is on SpriteAtlas")]
    static bool CheckIfIsOnSpriteAtlas()
    {
        ClearLogConsole();
        Object[] ob = Selection.objects;
        string[] spriteAtlasPath = AssetDatabase.FindAssets("t:spriteatlas");
        List<SpriteAtlas> spritesAtlas = new List<SpriteAtlas>();
        int found = 0;
        Dictionary<Object, List<string>> archivesPath = new Dictionary<Object, List<string>>();
        for (int i = 0; i < ob.Length; i++)
        {
            string[] paths = AssetDatabase.GetAssetPath(ob[i]).Split('/', '\\');
            archivesPath.Add(ob[i], paths.ToList());
        }
        for (int i = 0; i < spriteAtlasPath.Length; i++)
        {
            SpriteAtlas s = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(spriteAtlasPath[i]));
            spritesAtlas.Add(s);
        }
        for (int i = 0; i < spritesAtlas.Count; i++)
        {
            for (int j = 0; j < spritesAtlas[i].GetPackables().Length; j++)
            {
                for (int h = 0; h < ob.Length; h++)
                    if (ob[h].GetInstanceID() == spritesAtlas[i].GetPackables()[j].GetInstanceID())
                    {
                        Debug.Log("O arquivo " + ob[h].name + " foi achado no SpriteAtlas " + spritesAtlas[i].name);
                        found++;
                    }
                for (int k = 0; k < archivesPath.Count; k++)
                    for (int l = 0; l < archivesPath.ElementAt(k).Value.Count; l++)
                        if (archivesPath.ElementAt(k).Value[l] == spritesAtlas[i].GetPackables()[j].name)
                        {
                            Debug.Log("O arquivo " + archivesPath.ElementAt(k).Key.name + " foi achado no SpriteAtlas " + spritesAtlas[i].name + " com a pasta " + archivesPath.ElementAt(k).Value[l]);
                            found++;
                        }
            }
        }
        if (found >= 1)
            return true;
        else
        {
            Debug.Log("Nenhum arquivo selecionado está em um Sprite Atlas");
            return false;
        }
    }
    //[MenuItem("Assets/Generate Files/SpriteAtlas/Create Scene SpriteAtlas")]
    //[MenuItem("Generate Files/SpriteAtlas/Create Scene SpriteAtlas")]
    //static void GenerateSceneSpriteAtlas()
    //{
    //    actualSelectedStory = EditorPrefs.GetInt("actualSelectedStory");

    //    foreach (var item in FindObjectsOfType<SpriteRenderer>())
    //    {

    //    }
    //    //    Object[] ob = { Selection.activeObject };
    //    //    SpriteAtlas spriteAtlas = new SpriteAtlas();
    //    //    spriteAtlas.SetIncludeInBuild(false);
    //    //    spriteAtlas.Add(Selection.objects);
    //    //    if (!System.IO.Directory.Exists(System.IO.Path.Combine("Assets", "Codezone", storySelected[actualSelectedStory])))
    //    //    {
    //    //        System.IO.Directory.CreateDirectory(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
    //    //    }
    //    //    AssetDatabase.CreateAsset(spriteAtlas, System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas");
    //    //    var addressable = AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas"));
    //    //    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetLabel(storySelected[actualSelectedStory], true);
    //    //    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetAddress(Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas", true);
    //    //    Debug.Log("Sprite Atlas " + Selection.activeObject.name.Replace('.', '_') + " foi gerado na pasta " + System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
    //    //}

    //}
    [MenuItem("Assets/Generate Files/SpriteAtlas/Create Single SpriteAtlas")]
    [MenuItem("Generate Files/SpriteAtlas/Create Single SpriteAtlas")]
    static void GenerateSpriteAtlas()
    {
        actualSelectedStory = EditorPrefs.GetInt("actualSelectedStory");
        if (Selection.activeObject == null)
        {
            Debug.LogError("Precisa ter um Objeto Selecionado");
            return;
        }
        if (CheckIfIsOnSpriteAtlas())
        {
            Debug.LogError("O Arquivo "+ Selection.activeObject.name +  " já está em um SpriteAtlas");
            return;
        }
        Object[] ob = { Selection.activeObject };
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        spriteAtlas.SetIncludeInBuild(false);
        spriteAtlas.Add(Selection.objects);
        if(!System.IO.Directory.Exists(System.IO.Path.Combine("Assets", "Codezone", storySelected[actualSelectedStory])))
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine("Assets", "Codezone","SpriteAtlas", storySelected[actualSelectedStory]));
        }
        AssetDatabase.CreateAsset(spriteAtlas, System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas");
        var addressable = AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas"));
        AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetLabel(storySelected[actualSelectedStory], true);
        AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetAddress(Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas", true);
        Debug.Log("Sprite Atlas " + Selection.activeObject.name.Replace('.', '_') + " foi gerado na pasta " + System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
    }
    [MenuItem("Assets/Generate Files/SpriteAtlas/UI Create Single SpriteAtlas")]
    [MenuItem("Generate Files/SpriteAtlas/UI Create Single SpriteAtlas")]
    static void GenerateSpriteAtlasUI()
    {
        actualSelectedStory = EditorPrefs.GetInt("actualSelectedStory");
        if (Selection.activeObject == null)
        {
            Debug.LogError("Precisa ter um Objeto Selecionado");
            return;
        }
        if (CheckIfIsOnSpriteAtlas())
        {
            Debug.LogError("O Arquivo "+ Selection.activeObject.name +  " já está em um SpriteAtlas");
            return;
        }
        Object[] ob = { Selection.activeObject };
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        spriteAtlas.SetIncludeInBuild(false);
        var spConf = spriteAtlas.GetPackingSettings();
        spConf.enableRotation = false;
        spConf.enableTightPacking = false;
        spriteAtlas.SetPackingSettings(spConf);
        spriteAtlas.Add(Selection.objects);
        if (!System.IO.Directory.Exists(System.IO.Path.Combine("Assets", "Codezone", storySelected[actualSelectedStory])))
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
        }
        AssetDatabase.CreateAsset(spriteAtlas, System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas");
        var addressable = AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas"));
        AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetLabel(storySelected[actualSelectedStory], true);
        AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetAddress(Selection.activeObject.name.Replace('.', '_') + "_" + Selection.activeObject.GetInstanceID() + "_SpriteAtlas", true);
        Debug.Log("Sprite Atlas " + Selection.activeObject.name.Replace('.', '_') + " foi gerado na pasta " + System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
    }
    [MenuItem("Assets/Generate Files/SpriteAtlas/Create Multiples SpriteAtlas")]
    [MenuItem("Generate Files/SpriteAtlas/Create Multiples SpriteAtlas")]
    static void GenerateMultiplesSpriteAtlas()
    {
        actualSelectedStory = EditorPrefs.GetInt("actualSelectedStory");
        if (Selection.activeObject == null)
        {
            Debug.LogError("Precisa ter um Objeto Selecionado");
            return;
        }
        if (CheckIfIsOnSpriteAtlas())
        {
            Debug.LogError("Algum arquivo já está em um SpriteAtlas, checar log acima");
            return;
        }
        Object[] ob = Selection.objects;
        for (int i = 0; i < ob.Length; i++)
        {
            SpriteAtlas spriteAtlas = new SpriteAtlas();
            spriteAtlas.SetIncludeInBuild(false);
            spriteAtlas.Add(ob);
            if (!System.IO.Directory.Exists(System.IO.Path.Combine("Assets", "Codezone", storySelected[actualSelectedStory])))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
            }
            AssetDatabase.CreateAsset(spriteAtlas, System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.objects[i].name.Replace('.', '_') + "_" +  Selection.objects[i].GetInstanceID() + "_SpriteAtlas.spriteatlas");
            var addressable = AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.objects[i].name.Replace('.', '_') + "_" + Selection.objects[i].GetInstanceID() + "_SpriteAtlas.spriteatlas"));
            AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.objects[i].name.Replace('.', '_') + "_" + Selection.objects[i].GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetLabel(storySelected[actualSelectedStory], true);
            AddressableAssetSettingsDefaultObject.Settings.DefaultGroup.GetAssetEntry(AssetDatabase.AssetPathToGUID(System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]) + "/" + Selection.objects[i].name.Replace('.', '_') + "_" + Selection.objects[i].GetInstanceID() + "_SpriteAtlas.spriteatlas")).SetAddress(Selection.objects[i].name.Replace('.', '_') + "_" + Selection.objects[i].GetInstanceID() + "_SpriteAtlas", true);
            Debug.Log("Sprite Atlas " + Selection.objects[i].name.Replace('.', '_') + " foi gerado na pasta " + System.IO.Path.Combine("Assets", "Codezone", "SpriteAtlas", storySelected[actualSelectedStory]));
        }
    }
    //[MenuItem("Generate Files/Anim/Create Multiples Anim")]
    //static void GenerateMultiplesAnim()
    //{
    //    if (Selection.activeObject == null)
    //    {
    //        Debug.LogError("Precisa ter um Objeto Selecionado");
    //        return;
    //    }
    //    for (int i = 0; i < Selection.objects.Length; i++)
    //    {
    //        Object[] ob = { Selection.objects[i] };
    //        AnimationClip anim = new AnimationClip();
    //        AnimationEvent evn = new AnimationEvent();
    //        AnimationClipSettings s = new AnimationClipSettings();
    //        s.loopTime = false;

    //        //Animation a = new Animation();

    //        //evn.data = ;
    //        //Animation anim = new Animation();
    //        //anim.
    //        //SpriteAtlas spriteAtlas = new SpriteAtlas();
    //        //spriteAtlas.SetIncludeInBuild(false);
    //        //spriteAtlas.Add(ob);
    //        //AssetDatabase.CreateAsset(spriteAtlas, AssetDatabase.GetAssetPath(Selection.objects[i].GetInstanceID()) + "_SpriteAtlas.spriteatlas");
    //    }
    //}
    [MenuItem("Assets/Generate Files/Change Story")]
    [MenuItem("Generate Files/Change Story")]
    static void ChangeStory()
    {
        actualSelectedStory++;
        if (actualSelectedStory >= storySelected.Length)
            actualSelectedStory = 0;

        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
    [MenuItem("Assets/Generate Files/Select Story/Story 1")]
    [MenuItem("Generate Files/Select Story/Story 1")]
    static void ChangeToStoryOne()
    {
        actualSelectedStory = 0;
        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
    [MenuItem("Assets/Generate Files/Select Story/Story 2")]
    [MenuItem("Generate Files/Select Story/Story 2")]
    static void ChangeToStoryTwo()
    {
        actualSelectedStory = 1;
        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
    [MenuItem("Assets/Generate Files/Select Story/Story 3")]
    [MenuItem("Generate Files/Select Story/Story 3")]
    static void ChangeToStoryThree()
    {
        actualSelectedStory = 2;
        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
    [MenuItem("Assets/Generate Files/Select Story/Story 4")]
    [MenuItem("Generate Files/Select Story/Story 4")]
    static void ChangeToStoryFour()
    {
        actualSelectedStory = 3;
        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
    [MenuItem("Assets/Generate Files/Select Story/Story 5")]
    [MenuItem("Generate Files/Select Story/Story 5")]
    static void ChangeToStoryFive()
    {
        actualSelectedStory = 4;
        EditorPrefs.SetInt("actualSelectedStory", actualSelectedStory);
        Debug.Log(storySelected[actualSelectedStory]);
    }
}
