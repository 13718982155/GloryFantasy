using UnityEngine;
using System;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;
using UnityEngine.EventSystems;




//TODO ͨ�� ���� this.transform.position���һ�����ͼ�������(298.8)�Ĳ�ļ�����ϵ�õ�����Ϊ(0, 0) -> (7, 7)����������

namespace BattleMap
{
    public enum AStarState
    {
        free,
        isOpenList,
        isCloseList
    }


    public enum EMapBlockType
    {
        normal,   //��ͨ��ͼ���
        burnning, //���տ��
        Retire,   //�������
        aStarPath   //A��·��
    }


    public class BattleMapBlock : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        
        
        //public MapBlock(int area, string[] data)
        //{
        //    this.area = area;
        //    this.data = data;
        //}

        //public MapBlock()
        //{
        //    this.area = 0;
        //    this.data = null;
        //}

        public BattleMapBlock() { }
        public BattleMapBlock(int x,int y)
        {
            this.x = x;
            this.y = y;
        }


        //public List<Unit> GetGameUnits()
        //{
        //    Debug.Log("getunitList");
        //    this.units_on_me = MapManager.getInstance().UnitsList;
        //    return this.units_on_me;
        //}

        public void AddUnit(Unit unit)
        {
            Debug.Log("MapBlocks--Added unit:" + unit.ToString());
            units_on_me.Add(unit);
        }

        public void AddUnits(Unit[] units)
        {
            Debug.Log("MapBlocks--Adding Units");
            foreach (Unit gameUnit in units)
            {
                units_on_me.Add(gameUnit);
            }
        }

        public void RemoveUnit(Unit unit)
        {
            Debug.Log("MapBlocks--Removed unit:" + unit.ToString());
            units_on_me.Remove(unit);
        }

        public Vector3 GetCoordinate()
        {
            return new Vector3(this.x, this.y, 0f);
        }

        //��ȡ�õ�ͼ�������λ��
        public Vector3 GetSelfPosition()
        {
            //return this.transform.position;
            return coordinate;
        }       


        //�����ͼ�����¼�
        public void OnPointerDown(PointerEventData eventData)
        {
            if (UnitManager.Instance.IsPickedUnit)
            {
                //��⵽��ͼ����ʵ��������
                if (!BattleMap.getInstance().WarZoneBelong(GetSelfPosition())) return;
                UnitManager.Instance.CouldInstantiation(true,this.transform);

            }

            /*GameObject go = */
            //MapManager.getInstance().OnClickInstantiateUnit();
            //go.transform.SetParent(this.transform);
            //go.transform.localScale = GetCoordinate();
            //go.transform.localPosition = Vector3.zero;

            BattleMap.getInstance().curMapPos = GetSelfPosition();
            Debug.Log(GetSelfPosition());
        }

        private void Awake()
        {
            setMapBlackPosition();
        }

        //�����ͼ�������
        private void setMapBlackPosition()
        {

            coordinate = new Vector3((int)transform.position.x, (int)transform.position.y, 0.0f);
            //Debug.Log(coordinate);
            //var minusValue = Mathf.Abs(this.transform.position.x - coordinateSub);
            //Debug.Log("Position: " + this.transform.position + "/(0, " + minusValue / coordinateDivisor + ")");

        }

        public int Distance(BattleMapBlock target)
        {
            //TODO ���������ľ���

           
            return 0;
        }

        //��ʾս��
        public void OnPointerEnter(PointerEventData eventData)
        {
            BattleMap.getInstance().ShowBattleZooe(GetSelfPosition());
        }
        //����ս��
        public void OnPointerExit(PointerEventData eventData)
        {
            BattleMap.getInstance().HideBattleZooe(GetSelfPosition());
        }




        private Vector3 coordinate;

        public int area { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string[] data { get; set; }
        public string type { get; set; }
        public int tokenCount;
        public List<Unit> units_on_me = new List<Unit>();
        public BattleMapBlock[] neighbourBlocke { get; set; }
        public AStarState aStarState { get; set; }
        public EMapBlockType blockType { get; set; }

        //ָ��obj���͵�ͨ����ʱʹ�õ�ָ��
        public System.Object tempRef;
    }
}

