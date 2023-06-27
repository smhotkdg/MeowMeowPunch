using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using Sirenix.Utilities;
using System.Linq;

public class MonsterArrangement : OdinMenuEditorWindow
{
    public enum MapType
    {
        일반,
        기억,
        니은,
        세로
    }
    [Title("맵 타입 선택")]
    [EnumToggleButtons]
    [OnValueChanged("MapChange")]
    public MapType MapTypeField;

    [TableMatrix(HorizontalTitle = "Map", SquareCells = true)]
    public GameObject[,] MapMonster = new GameObject[6, 7];
    [Button]
    public void SetMap()
    {
        foreach(GameObject obj in MapMonster)
        {
            if(obj !=null)
            {
                Debug.Log(obj.name);
            }
            else
            {
                Debug.Log("## ");
            }
            
        }
    }
    public void MapChange()
    {
        switch(MapTypeField)
        {
            case MapType.기억:
                MapMonster = new GameObject[12, 14];
                break;
            case MapType.니은:
                MapMonster = new GameObject[12, 14];
                break;
            case MapType.세로:
                MapMonster = new GameObject[6, 14];
                break;
            case MapType.일반:
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
