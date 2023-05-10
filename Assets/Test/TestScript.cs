using DungeonMaker;
using DungeonMaker.Core;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Generator generator;
    public GameObject CmvCam;
    public GameObject Player;    

    // Start is called before the first frame update
    void Start()
    {
        InitSetting();
        AstarPath.active.Scan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitSetting()
    {
        Generator.OnGeneratorFinish += Generator_OnGeneratorFinish;
    }
    DungeonObject dungeonObject;
    IEnumerator MapMakeRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        int roomCount = 0;
        int connectCnt = 0;
        for (int i = 0; i < dungeonObject.Rooms.Count; i++)
        {
            bool [] ConnectList = new bool[4] { false, false, false, false };
            List<int> connectID = new List<int>();
            
            connectID.Clear();
            for (int connectCount = 0; connectCount < dungeonObject.Rooms[i].Connections.Count; connectCount++)
            {
                bool isConnect = false;
                if (dungeonObject.Rooms[i].transform.Find("Doors/TOP/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true && ConnectList[0] == false)
                {
                    roomCount = 0;
                    connectCnt = 0;
                    for (roomCount = 0; roomCount < dungeonObject.Rooms.Count; roomCount++)
                    {
                        if(dungeonObject.Rooms[roomCount].gameObject.transform.position.y > dungeonObject.Rooms[i].transform.position.y)
                        {
                            for (connectCnt = 0; connectCnt < connectID.Count; connectCnt++)
                            {
                                isConnect = false;
                                if (connectID[connectCnt] == dungeonObject.Rooms[roomCount].ID)
                                {
                                    isConnect = true;
                                }
                            }
                            if (dungeonObject.Rooms[i].IsConnectedTo(dungeonObject.Rooms[roomCount].ID) && isConnect == false)
                            {
                                if (dungeonObject.Rooms[roomCount].transform.Find("Doors/BOTTOM/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true)
                                {
                                    dungeonObject.Rooms[i].transform.Find("Doors/TOP/Open").gameObject.GetComponent<Rule>().NextPosition = dungeonObject.Rooms[roomCount].transform.Find("Doors/BOTTOM/Open").gameObject.GetComponent<Rule>().InitPos;
                                    dungeonObject.Rooms[i].transform.Find("Doors/TOP/Open").gameObject.GetComponent<Rule>().NextMap = dungeonObject.Rooms[roomCount].gameObject;
                                    ConnectList[0] = true;
                                    connectID.Add(dungeonObject.Rooms[roomCount].ID);
                                }
                            }
                        }
                        
                    }
                }
                else if (dungeonObject.Rooms[i].transform.Find("Doors/BOTTOM/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true && ConnectList[1] == false)
                {
                    roomCount = 0;
                    for (roomCount = 0; roomCount < dungeonObject.Rooms.Count; roomCount++)
                    {
                        if (dungeonObject.Rooms[roomCount].gameObject.transform.position.y < dungeonObject.Rooms[i].transform.position.y)
                        {
                            isConnect = false;
                            for (connectCnt = 0; connectCnt < connectID.Count; connectCnt++)
                            {

                                if (connectID[connectCnt] == dungeonObject.Rooms[roomCount].ID)
                                {
                                    isConnect = true;
                                }
                            }
                            if (dungeonObject.Rooms[i].IsConnectedTo(dungeonObject.Rooms[roomCount].ID) && isConnect == false)
                            {
                                if (dungeonObject.Rooms[roomCount].transform.Find("Doors/TOP/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true)
                                {
                                    dungeonObject.Rooms[i].transform.Find("Doors/BOTTOM/Open").gameObject.GetComponent<Rule>().NextPosition = dungeonObject.Rooms[roomCount].transform.Find("Doors/TOP/Open").gameObject.GetComponent<Rule>().InitPos;
                                    dungeonObject.Rooms[i].transform.Find("Doors/BOTTOM/Open").gameObject.GetComponent<Rule>().NextMap = dungeonObject.Rooms[roomCount].gameObject;
                                    ConnectList[1] = true;
                                    connectID.Add(dungeonObject.Rooms[roomCount].ID);
                                }
                            }
                        }
                        
                    }
                }
                else if (dungeonObject.Rooms[i].transform.Find("Doors/LEFT/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true && ConnectList[2] == false)
                {
                    roomCount = 0;
                    for (roomCount = 0; roomCount < dungeonObject.Rooms.Count; roomCount++)
                    {
                        if (dungeonObject.Rooms[roomCount].gameObject.transform.position.x < dungeonObject.Rooms[i].transform.position.x)
                        {
                            isConnect = false;
                            for (connectCnt = 0; connectCnt < connectID.Count; connectCnt++)
                            {
                                if (connectID[connectCnt] == dungeonObject.Rooms[roomCount].ID)
                                {
                                    isConnect = true;
                                }
                            }
                            if (dungeonObject.Rooms[i].IsConnectedTo(dungeonObject.Rooms[roomCount].ID) && isConnect == false)
                            {
                                if (dungeonObject.Rooms[roomCount].transform.Find("Doors/RIGHT/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true)
                                {
                                    dungeonObject.Rooms[i].transform.Find("Doors/LEFT/Open").gameObject.GetComponent<Rule>().NextPosition = dungeonObject.Rooms[roomCount].transform.Find("Doors/RIGHT/Open").gameObject.GetComponent<Rule>().InitPos;
                                    dungeonObject.Rooms[i].transform.Find("Doors/LEFT/Open").gameObject.GetComponent<Rule>().NextMap = dungeonObject.Rooms[roomCount].gameObject;
                                    ConnectList[2] = true;
                                    connectID.Add(dungeonObject.Rooms[roomCount].ID);
                                }
                            }
                        }
                          
                    }
                }
                else if (dungeonObject.Rooms[i].transform.Find("Doors/RIGHT/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true && ConnectList[3] ==false)
                {
                    roomCount = 0;
                    for (roomCount = 0; roomCount < dungeonObject.Rooms.Count; roomCount++)
                    {
                        if (dungeonObject.Rooms[roomCount].gameObject.transform.position.x > dungeonObject.Rooms[i].transform.position.x)
                        {
                            isConnect = false;
                            for (connectCnt = 0; connectCnt < connectID.Count; connectCnt++)
                            {
                                if (connectID[connectCnt] == dungeonObject.Rooms[roomCount].ID)
                                {
                                    isConnect = true;
                                }
                            }
                            if (dungeonObject.Rooms[i].IsConnectedTo(dungeonObject.Rooms[roomCount].ID) && isConnect == false)
                            {
                                if (dungeonObject.Rooms[roomCount].transform.Find("Doors/LEFT/Open").gameObject.transform.GetChild(0).gameObject.activeSelf == true)
                                {
                                    dungeonObject.Rooms[i].transform.Find("Doors/RIGHT/Open").gameObject.GetComponent<Rule>().NextPosition = dungeonObject.Rooms[roomCount].transform.Find("Doors/LEFT/Open").gameObject.GetComponent<Rule>().InitPos;
                                    dungeonObject.Rooms[i].transform.Find("Doors/RIGHT/Open").gameObject.GetComponent<Rule>().NextMap = dungeonObject.Rooms[roomCount].gameObject;
                                    ConnectList[3] = true;
                                    connectID.Add(dungeonObject.Rooms[roomCount].ID);
                                }
                            }
                        }
                          
                    }
                }
            }
        }
        AstarPath.active.Scan();
    }
    private void Generator_OnGeneratorFinish(DungeonObject d)
    {
        dungeonObject = d;
        GameObject temp = GameObject.Find(dungeonObject.name);
        if (temp == null)
            return;
        List<GameObject> MapList = new List<GameObject>();
        for (int i = 0; i < temp.transform.childCount; i++)
        {
            MapList.Add(temp.transform.GetChild(i).gameObject);
            if (temp.transform.GetChild(i).gameObject.name == "init")
            {
                CmvCam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = MapList[MapList.Count - 1].GetComponent<PolygonCollider2D>();
                Player.transform.position = MapList[MapList.Count - 1].transform.Find("InitPos").transform.position;


                AstarData data = AstarPath.active.data;
                GridGraph gg = data.graphs[0] as GridGraph;

                gg.center = MapList[MapList.Count - 1].transform.Find("InitPos").transform.position;                
                //gg.SetDimensions(15, 18, 0.2f);                            
            }
        }
        StartCoroutine(MapMakeRoutine());
        
    }
    public void ChangeMap(GameObject nextMap,GameObject Position)
    {
        //GameManager.Instance.AStar
        AstarData data = AstarPath.active.data;
        GridGraph gg = data.graphs[0] as GridGraph;

        gg.center = nextMap.transform.position;

        AstarPath.active.Scan();        
        CmvCam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = nextMap.GetComponent<PolygonCollider2D>();
        Player.transform.position = Position.transform.position;
    }

    public void MakeMap()
    {
        DungeonData dungeonData = generator.dungeons[0];
        generator.Generate(dungeonData);
        
        foreach (RoomNode node in generator.correlator.dungeon.nodes)        
        {
            if(node.room.name == "init")
            {                
                generator.correlator.dungeon.currentNode = node;
                
            }
        }
    }
}
