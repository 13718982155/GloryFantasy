using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using LitJson;
using System.IO;
using Unit = GameUnit.GameUnit;
using UnityEngine.UI;

using GameUnit;

namespace BattleMap
{
    public class BattleMap : UnitySingleton<BattleMap>
    {

        private void Awake()
        {
            _unitsList = new List<Unit>();
            _instance = this;
            IsColor = false;
            MapNavigator = new MapNavigator();
            battleArea = new BattleArea();
        }

        private void Start()
        {
            InitMap();
        }

        public void InitMap()
        {
            //初始化地图
            InitAndInstantiateMapBlocks();
        }

        //初始化地图的地址
        //更改地图数据位置则需修改此处路径
        public string InitialMapDataPath = "/Scripts/BattleMap/eg1.json";
        // 获取战斗地图上的所有单位
        private List<Unit> _unitsList;//TODO考虑后面是否毁用到，暂留
        public List<Unit> UnitsList{get{return _unitsList;}}              
        private int columns;                 // 地图方块每列的数量
        private int rows;                    // 地图方块每行的数量
        public int Columns{get{return columns;}}
        public int Rows{get{return rows;}}                    
        public int BlockCount{get{return columns * rows;}}
        public bool IsColor { get; set; }//控制是否高亮战区
        public Vector3 curMapPos;//TODO
        private BattleMapBlock[,] _mapBlocks;         //普通的地图方块
        public GameObject normalMapBlocks;//实例地图块的prefab
        public Transform _tilesHolder;          // 存储所有地图单位引用的变量
        public GameObject[] enemys;             // 存储敌方单位素材的数组
        public GameObject[] enemy_sets;         //存储敌方群体单位素材的数组
        public GameObject player_assete;       // 存放玩家单位素材的引用
        public GameObject obstacle;
        public MapNavigator MapNavigator;//寻路类
        public BattleArea battleArea;//战区类
               
        private void InitAndInstantiateMapBlocks()
        { 
            JsonData mapData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + InitialMapDataPath));
            int mapDataCount = mapData.Count;
            this.columns = (int)mapData[mapDataCount - 1]["y"] + 1;
            this.rows = (int)mapData[mapDataCount - 1]["x"] + 1;
            _mapBlocks = new BattleMapBlock[rows, columns];
            GameObject instance = new GameObject();
            int x = 0;
            int y = 0;
            int area = 0;

            battleArea.GetAreas(mapData);

            for (int i = 0; i < mapDataCount; i++)
            {
                x = (int)mapData[i]["x"];
                y = (int)mapData[i]["y"];
                area = (int)mapData[i]["area"];
                Vector2 mapPos = new Vector2(x, y);
                battleArea.StoreBattleArea(area, mapPos);

                //实例化地图块
                instance = GameObject.Instantiate(normalMapBlocks, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(_tilesHolder);
                instance.gameObject.AddComponent<BattleMapBlock>();
                //初始化mapBlock成员
                _mapBlocks[x, y] = instance.gameObject.GetComponent<BattleMapBlock>();
                _mapBlocks[x, y].area = area;
                _mapBlocks[x, y].x = x;
                _mapBlocks[x, y].y = y;
                _mapBlocks[x, y].blockType = EMapBlockType.normal;


                int tokenCount = mapData[i]["token"].Count;
                if (tokenCount == 1)
                {
                    Unit unit = InitAndInstantiateGameUnit(mapData[i]["token"][0], x, y);
                    unit.mapBlockBelow = _mapBlocks[x, y];
                    unit.gameObject.GetComponent<GameUnit.GameUnit>().owner = GameUnit.OwnerEnum.Enemy;

                    _unitsList.Add(unit);
                    _mapBlocks[x, y].AddUnit(unit);
                }
            }
        }

        /// <summary>
        /// 获取传入寻路结点相邻的方块列表
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<BattleMapBlock> GetNeighbourBlock(Node node)
        {
            List<BattleMapBlock> neighbour = new List<BattleMapBlock>();
            int x = (int)node.position.x;
            int y = (int)node.position.y;
            if (GetSpecificMapBlock(x - 1, y) != null && GetSpecificMapBlock(x - 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x - 1, y));
            }
            if (GetSpecificMapBlock(x + 1, y) != null && GetSpecificMapBlock(x + 1, y).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x + 1, y));
            }
            if (GetSpecificMapBlock(x, y - 1) != null && GetSpecificMapBlock(x, y - 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y - 1));
            }
            if (GetSpecificMapBlock(x, y + 1) != null && GetSpecificMapBlock(x, y + 1).units_on_me.Count == 0)
            {
                neighbour.Add(GetSpecificMapBlock(x, y + 1));
            }
            return neighbour;
        }

        //初始地图单位
        private Unit InitAndInstantiateGameUnit(JsonData data, int x, int y)
        {
            Unit newUnit;
            GameObject _object;
            //TODO:怎么没有根据所有者分别做处理
            OwnerEnum owner;
            switch (data["Controler - Enemy, Friendly or Self"].ToString())
            {
                case ("Enemy"):
                    owner = OwnerEnum.Enemy; break;
                case ("Friendly"):
                    owner = OwnerEnum.neutrality; break;
                case ("Self"):
                    owner = OwnerEnum.Player; break;
                default:
                    owner = OwnerEnum.Enemy;break;
            }
            //从对象池获取单位
            _object = GameUnitPool.Instance().GetInst(data["name"].ToString(), owner, (int)data["Damaged"]);     
            //修改单位对象的父级为地图方块
            _object.transform.SetParent(_mapBlocks[x, y].transform);
            _object.transform.localPosition = Vector3.zero;


            //TODO 血量显示 test版本, 此后用slider显示
            var TextHp = _object.transform.GetComponentInChildren<Text>();
            var gameUnit = _object.GetComponent<GameUnit.GameUnit>();
            float hp = gameUnit.hp/* - Random.Range(2, 6)*/;
            float maxHp = gameUnit.MaxHP;
            float hpDivMaxHp = hp / maxHp * 100;
            TextHp.text = string.Format("Hp: {0}%", hpDivMaxHp);              
            
            newUnit = _object.GetComponent<Unit>();
            return newUnit;
        }


        //TODO 根据坐标返回地图块儿 --> 在对应返回的地图块儿上抓取池内的对象，"投递上去"
        //TODO 相当于是召唤技能，可以与郑大佬的技能脚本产生联系
        //TODO 类似做一个召唤技能，通过UGUI的按钮实现

        //TODO 如何实现
        //1. 首先我们输入一个坐标 -> 传递给某个函数，此函数能够根据坐标获得地图块儿 -> 获取到地图块儿后便可以通过地图块儿，从池子中取出Unit “投递”到该地图块儿上
        //2. 完成
        //3. 转移成一个skill

        /// <summary>
        /// 将单位设置在MapBlock下
        /// </summary>
        public void SetUnitToMapBlock()
        {
            //TODO：完善一下
        }

        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(Vector2 pos)
        {
            return GetSpecificMapBlock((int)pos.x, (int)pos.y);
        }
        /// <summary>
        /// 传入坐标，获取对应的MapBlock。坐标不合法会返回null
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BattleMapBlock GetSpecificMapBlock(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < columns && y < rows)
                return this._mapBlocks[x, y];
            return null;
        }
        /// <summary>
        /// 传入MapBlock，返回该MapBlock的坐标
        /// </summary>
        /// <param name="mapBlock"></param>
        /// <returns></returns>
        public Vector3 GetCoordinate(BattleMapBlock mapBlock)
        {
            for (int i = columns - 1; i > 0; i--)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (_mapBlocks[i, j] == mapBlock)
                    {
                        return new Vector3(i, j, 0f);
                    }
                }
            }
            return new Vector3(-1, -1, 0f);
        }
        /// <summary>
        /// 确定给定坐标上是否含有单位，坐标不合法会返回false，其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Boolean CheckIfHasUnits(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0
                && this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>() != null &&
                this._mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>().id != "Obstacle"/*units_on_me.Count != 0*/)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 返回给定坐标上单位list，坐标不合法会返回null, 其他依据实际情况返回值
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Unit GetUnitsOnMapBlock(Vector3 vector)
        {
            if (this._mapBlocks[(int)vector.x, (int)vector.y] != null && this._mapBlocks[(int)vector.x, (int)vector.y].transform.childCount != 0)
            {
                return _mapBlocks[(int)vector.x, (int)vector.y].GetComponentInChildren<Unit>();
            }
            return null;
        }
        /// <summary>
        /// 传入坐标，返回该坐标的MapBlock的Type
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public EMapBlockType GetMapBlockType(Vector3 coordinate)
        {
            int x = (int)coordinate.x;
            int y = (int)coordinate.y;
            if (x < 0 || y < 0 || x >= columns || y >= rows)
            {
                // TODO: 添加异常坐标处理
            }

            return _mapBlocks[x, y].blockType;
        }
        /// <summary>
        /// 根据给定unit寻找其所处坐标，若找不到则会返回不合法坐标
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Vector3 GetUnitCoordinate(Unit unit)
        {

            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    return gameUnit.mapBlockBelow.GetCoordinate();
                }
            }

            return new Vector3(-1, -1, -1);
        }
        /// <summary>
        /// 传入unit和坐标，将Unit瞬间移动到该坐标（仅做坐标变更，不做其他处理）
        /// <param name="unit">移动的目标单位</param>
        /// <param name="gameobjectCoordinate">地图块儿自身的物体坐标</param>
        /// <returns></returns>
        /// </summary>
        public bool MoveUnitToCoordinate(Unit unit,  Vector2 gameobjectCoordinate)
        {
            foreach (Unit gameUnit in _unitsList)
            {
                if (gameUnit == unit)
                {
                    unit.mapBlockBelow.RemoveUnit(unit);
                    if (_mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y] != null)
                    {
                        unit.mapBlockBelow = _mapBlocks[(int)gameobjectCoordinate.x, (int)gameobjectCoordinate.y];
                    }
                    unit.mapBlockBelow.AddUnit(unit);
                    //unit.transform.position = _destination;
                    unit.transform.localPosition = Vector3.zero;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 地图方块染色接口
        /// </summary>
        /// <param name="positions">染色的方块坐标</param>
        /// <param name="color">方块要被染的颜色</param>
        public void ColorMapBlocks(List<Vector2> positions, Color color)
        {
            foreach (Vector3 position in positions)
            {
                if (position.x < columns && position.y < rows && position.x >= 0 && position.y >= 0)
                {
                    _mapBlocks[(int)position.x, (int)position.y].gameObject.GetComponent<Image>().color = color;
                }
            }
        }

        //判断该战区能不能召唤（所属权）
        public bool WarZoneBelong(Vector3 position)
        {
            return battleArea.WarZoneBelong(position, _mapBlocks);
        }

        //显示战区
        public void ShowBattleZooe(Vector3 position)
        {
            battleArea.ShowBattleZooe(position, _mapBlocks);
        }

        //隐藏战区
        public void HideBattleZooe(Vector2 position)
        {
            battleArea.HideBattleZooe(position, _mapBlocks);
        }

        /// <summary>
        /// 移除BattleBlock下的 unit
        /// </summary>
        public void RemoveUnitOnBlock(Unit deadUnit)
        {
            //获取死亡单位的Pos
            Vector2 unitPos = GetUnitCoordinate(deadUnit);
            //通过unitPos的坐标获取对应的地图块儿
            BattleMapBlock battleMap = GetSpecificMapBlock(unitPos);
            //移除对应地图块儿下的deadUnit
            battleMap.units_on_me.Remove(deadUnit);
        }
        /// <summary>
        /// 移除BattleBlock下的 units_on_me
        /// </summary>
        public void RemoveUnitsOnBlock()
        {

        }
    }
}