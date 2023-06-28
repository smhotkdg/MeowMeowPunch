using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using Sirenix.Utilities;
using System.Linq;
using Leguar.TotalJSON;
using System.IO;

public class MonsterArrangement : OdinMenuEditorWindow
{
    public enum MapType
    {
        Normal,
        Right,
        Left,
        Vertical
    }
    
    [Title("∏  ≈∏¿‘ º±≈√")]
    [EnumToggleButtons]
    [OnValueChanged("MapChange")]
    public MapType MapTypeField;
    

    [SerializeField]
    string FILE_PATH = "Assets/Resources/Test.json";
    [TableMatrix(HorizontalTitle = "Map", SquareCells = true)]
    public GameObject[,] MapMonster = new GameObject[6, 7];
       
    GameObject xObject;
    struct MapStruct
    {
        int xPos;
        int yPos;
        string name;
    }
    private void Awake()
    {
        xObject = Resources.Load<GameObject>("X");
    }
    struct SaveMapData
    {
        public int mapType;
        public string name;
        public int xPos;
        public int yPos;        
    }
    [Button]
    public void SetMap()
    {
        JSON json = new JSON();     
        for (int i =0; i< MapMonster.GetLength(0); i++)
        {
            for(int j =0; j < MapMonster.GetLength(1); j++)
            {
                if(MapMonster[i,j]!=null && MapMonster[i,j].name != "X")
                {
                    SaveMapData data = new SaveMapData();
                    data.mapType = (int)MapTypeField;
                    data.xPos = i;
                    data.yPos = j;
                    data.name = MapMonster[i, j].name;
                    json = JSON.Serialize(data);
                    saveJsonObjectToTextFile(json);
                }

            }
        } 
    }
    private void saveJsonObjectToTextFile(JSON jsonObject)
    {
        string jsonAsString = jsonObject.CreateString(); // Could also use "CreatePrettyString()" to make more human readable result, it is still valid JSON to read and parse by computer
        StreamWriter writer = new StreamWriter(FILE_PATH,true);
        writer.WriteLine(jsonAsString);        
        writer.Close();
    }

    private JSON loadTextFileToJsonObject()
    {
        StreamReader reader = new StreamReader(FILE_PATH);
        string jsonAsString = reader.ReadToEnd();
        reader.Close();
        JSON jsonObject = JSON.ParseString(jsonAsString);
        return jsonObject;
    }
    public void MapChange()
    {
        
        switch (MapTypeField)
        {
            case MapType.Right:
                MapMonster = new GameObject[12, 14];
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 7; j < 14; j++)
                    {
                        MapMonster[i, j] = xObject;
                    }
                }                
                break;
            case MapType.Left:
                MapMonster = new GameObject[12, 14];
                for(int i =6; i< 12; i++)
                {
                    for(int j =0; j< 7; j++)
                    {
                        MapMonster[i, j] = xObject;
                    }
                }
                break;
            case MapType.Vertical:
                MapMonster = new GameObject[6, 14];              
                break;                
            case MapType.Normal:
                MapMonster = new GameObject[6, 7];             
                break;
        }
        
    }
    [MenuItem("MonsterArrangement/New Arrangement", priority = 1)]
    private static void OpenWindow()
    {
        var window  = GetWindow<MonsterArrangement>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);

    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "Home",                           this,                           EditorIcons.House                       }
                
            };
        //var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // Adds the character overview table.
        //CharacterOverview.Instance.UpdateCharacterOverview();
        //tree.Add("Characters", new CharacterTable(CharacterOverview.Instance.AllCharacters));

        tree.AddAllAssetsAtPath("Monsters", "Assets/Prefabs/Monster", typeof(GameObject),true,true).ForEach(this.AddDragHandles);
        

        tree.EnumerateTree().AddIcons<GameObject>(x => x.GetComponent<ArrangementUnit>().Icon);
        // Add drag handles to items, so they can be easily dragged into the inventory if characters etc...

        return tree;
    }
    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }
}
