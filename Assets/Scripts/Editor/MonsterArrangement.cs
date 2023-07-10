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

    [Title("¸Ê Å¸ÀÔ ¼±ÅÃ")]
    [EnumToggleButtons]
    [OnValueChanged("MapChange")]
    public MapType MapTypeField;

    private class MyCustomMenuItem : OdinMenuItem
    {
        private readonly SomeCustomClass instance;
        public MyCustomMenuItem(OdinMenuTree tree, SomeCustomClass instance) : base(tree, instance.Name, instance)
        {
            this.instance = instance;
        }        

        //protected override void OnDrawMenuItem(Rect rect, Rect labelRect)
        //{
           
        //    labelRect.x -= 16;
        //    //this.instance.Enabled = GUI.Toggle(labelRect.AlignMiddle(18).AlignLeft(16), this.instance.Enabled, GUIContent.none);

        //    // Toggle selection when pressing space.
        //    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        //    {
        //        var selection = this.MenuTree.Selection
        //            .Select(x => x.Value)
        //            .OfType<SomeCustomClass>();

        //        if (selection.Any())
        //        {
        //            //var enabled = !selection.FirstOrDefault().Enabled;
        //            //selection.ForEach(x => x.Enabled = enabled);
        //            Event.current.Use();
        //        }
        //    }
        //}
        
        public override string SmartName { get { return this.instance.Name; } }
    }

    private class SomeCustomClass : SerializedScriptableObject
    {        
        [ReadOnly]
        public string Name;
    
        [TableMatrix(HorizontalTitle = "Map_", SquareCells = true)]        
        public GameObject[,] MapMonster = new GameObject[6, 7];
    }


    [SerializeField]
    string FILE_PATH = "Assets/Resources/Map/";
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
        CheckFolder();
    }
    public class SaveMapData
    {
        public string name = "";
        public int xPos;
        public int yPos;
    }
    public class MapData
    {
        public float MapHash;
        public int MapType;
        public List<SaveMapData> saveMapDatas = new List<SaveMapData>();
    }
    void GetMaxIndex()
    {
        //MapData data= loadTextFileToJsonObject();
        //Debug.Log(data);
    }    
    [Button(ButtonSizes.Gigantic),GUIColor(0,1,0)]
    public void SaveData()
    {
        JSON json = new JSON();
        MapData mapData = new MapData();
        mapData.MapHash = Random.Range(0f, 10000000f);
        mapData.MapType = (int)MapTypeField; 
        for (int i =0; i< MapMonster.GetLength(0); i++)
        {
            for(int j =0; j < MapMonster.GetLength(1); j++)
            {                
                if (MapMonster[i,j]!=null && MapMonster[i,j].name != "X")
                {
                    SaveMapData data = new SaveMapData();                     
                    data.name = MapMonster[i, j].name;
                    data.xPos = i;
                    data.yPos = j;
                    
                    mapData.saveMapDatas.Add(data);
                }                
            }
        } 
        json = JSON.Serialize(mapData);
        saveJsonObjectToTextFile(json);
        GetMaxIndex();
        MapChange();
        ForceMenuTreeRebuild();        
    }

    private void saveJsonObjectToTextFile(JSON jsonObject)
    {
        string jsonAsString = jsonObject.CreateString(); // Could also use "CreatePrettyString()" to make more human readable result, it is still valid JSON to read and parse by computer
        StreamWriter writer = new StreamWriter(FILE_PATH);
        writer.WriteLine(jsonAsString);        
        writer.Close();
    }
    //[Button]
    public void MergeJson()
    {
        JSON aa = new JSON();
        for (int i = 1; i <= 3; i++)
        {
            string path = "Assets/Resources/Map/Test";
            string index = i.ToString();
            path = path + index + ".json";
            StreamReader reader = new StreamReader(path);
            string jsonAsString = reader.ReadToEnd();
            reader.Close();
            JSON jsonObject = JSON.ParseString(jsonAsString);
            aa.Add(index, jsonObject);
        }
        saveJsonObjectToTextFile(aa);
    }
    FileInfo[] info;
    //[Button]
    public void CheckFolder()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Map");
        info = dir.GetFiles("*.json");
        int count = 1;
        foreach(FileInfo f in info)
        {
            count++;            
        }
        string path = "Assets/Resources/Map/";
        path += count + ".json";
        FILE_PATH = path;
        
    }
    
    //[Button]
    public void LoadTest()
    {
        JSON data = loadTextFileToJsonObject();
        MapData map =  data.GetJSON("1").Deserialize<MapData>();
        //Debug.Log(data.Get("1")["saveMapDatas"][0]["name"]);
        Debug.Log(map.saveMapDatas[0].name);
    }
    //[Button]
    public MapData Load_Single(string path)
    {
        JSON data = loadTextFileToJsonObject_path(path);
        //Debug.Log(data.GetJArray("saveMapDatas")[0]["xPos"]);
        MapData m = data.Deserialize<MapData>();

        return m;
    }

    public void LoadJsonByPath(string path)
    {
        JSON data = loadTextFileToJsonObject_path(path);
        MapData map = data.GetJSON("1").Deserialize<MapData>();        
        //Debug.Log(data.Get("1")["saveMapDatas"][0]["name"]);
        Debug.Log(map.saveMapDatas[0].name);
    }

    private JSON loadTextFileToJsonObject_path(string path)
    {
        string fullPath = "Assets/Resources/Map/" + path;
        StreamReader reader = new StreamReader(fullPath);
        string jsonAsString = reader.ReadToEnd();
        reader.Close();
        JSON jsonObject = JSON.ParseString(jsonAsString);
        return jsonObject;
    }
    private JSON loadTextFileToJsonObject()
    {
        StreamReader reader = new StreamReader(FILE_PATH);
        string jsonAsString = reader.ReadToEnd();
        reader.Close();
        JSON jsonObject = JSON.ParseString(jsonAsString);
        return jsonObject;
    }
    static MonsterArrangement window;
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
        window = GetWindow<MonsterArrangement>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 800);
        
    }
    protected override void OnGUI()
    {        
        base.OnGUI();      
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        CheckFolder();
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
        for (int i = 0; i < info.Length; i++)
        {
            GameObject[,] TempList = new GameObject[0,0];
            MapData m = Load_Single(info[i].Name);
            switch (m.MapType)
            {
                case 1:
                    TempList = new GameObject[12, 14];
                    for (int k = 0; k < 6; k++)
                    {
                        for (int p = 7; p < 14; p++)
                        {
                            TempList[k, p] = xObject;
                        }
                    }
                    break;
                case 2:
                    TempList = new GameObject[12, 14];
                    for (int k = 6; k < 12; k++)
                    {
                        for (int p = 0; p < 7; p++)
                        {
                            TempList[k, p] = xObject;
                        }
                    }
                    break;
                case 3:
                    TempList = new GameObject[6, 14];
                    break;
                case 0:
                    TempList = new GameObject[6, 7];
                    break;
            }
            var customObject = new SomeCustomClass() { Name = info[i].Name, MapMonster = TempList};
            
            for (int j = 0; j < m.saveMapDatas.Count; j++)
            {
                for(int k =0; k < tree.MenuItems[1].ChildMenuItems.Count; k++)
                {
                    if(tree.MenuItems[1].ChildMenuItems[k].Name == m.saveMapDatas[j].name)
                    {
                        customObject.MapMonster[m.saveMapDatas[j].xPos, m.saveMapDatas[j].yPos] =
                    (GameObject)tree.MenuItems[1].ChildMenuItems[k].Value;
                    }
                }                
            }            
            
            var customMenuItem = new MyCustomMenuItem(tree, customObject);
            tree.AddMenuItemAtPath("Maps", customMenuItem);
        }


        tree.EnumerateTree().AddIcons<GameObject>(x => x.GetComponent<ArrangementUnit>().Icon);
        
        // Add drag handles to items, so they can be easily dragged into the inventory if characters etc...

        return tree;
    }
    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }
}

