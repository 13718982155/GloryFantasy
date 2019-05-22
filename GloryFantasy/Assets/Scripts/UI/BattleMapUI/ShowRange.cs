using UnityEngine;
using System.Collections.Generic;
using Unit = GameUnit.GameUnit;

using GameUnit;

namespace GameGUI
{

    public class ShowRange : UnitySingleton<ShowRange>
    { 
        private bool unitMove;
        private List<Vector2> cannotArrivePoss;
        public Unit BeforeUnit { get; set; }
       
        /// <summary>
        /// ���ص�λ�������ƶ���Χ�ڵ�ͼ�����������
        /// </summary>
        /// <param name="position">��λ����</param>
        /// <param name="ManhattanDistance">��Χ</param>
        /// <returns></returns>
        private List<Vector2> GetPositionsWithinCertainMd(Vector2 position, int ManhattanDistance)
        {
            List<Vector2> reslist = new List<Vector2>();
            if (unitMove)
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);
                RemoveMapBlokHasUnit(reslist);
            }
            else
            {
                RecrusiveBody((int)position.x, (int)position.y, ManhattanDistance, reslist);   
            }
            List<Vector2> temList = reslist;
            GetMapBlockCannontArrive(position, temList);
            return reslist;
        }

        //for��λ���ƶ��򹥻���Χ
        private void RecrusiveBody(int x, int y, int leftManhattanDistance, List<Vector2> reslist)
        {
            int columns = BattleMap.BattleMap.Instance().Columns;
            int rows = BattleMap.BattleMap.Instance().Rows;
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (leftManhattanDistance == 0)
                return;
            RecrusiveBody(x + 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x - 1, y, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y + 1, leftManhattanDistance - 1, reslist);
            RecrusiveBody(x, y - 1, leftManhattanDistance - 1, reslist);
        }

        //for��λ�ļ��ܷ�Χ
        private void RecrusiveBody2(int x, int y, int range, List<Vector2> reslist)
        {
            int columns = BattleMap.BattleMap.Instance().Columns;
            int rows = BattleMap.BattleMap.Instance().Rows;
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            Vector2 centPosition = new Vector2(x, y);
            int tempRange = range%2 == 0 ? range / 2 - 1 : (range - 1) / 2 ;
            int starPosition_x =(int)centPosition.x - tempRange;
            int starPosition_y = (int)centPosition.y - tempRange;
            for(int i = 0;i < range; i++)
            {
                for(int j = 0; j < range; j++)
                {
                    reslist.Add(new Vector2(starPosition_x + j, starPosition_y + i));
                }
            }
        }


        /// <summary>
        /// �Ƴ���ͼ�����е�λ�ĵ�ͼ��
        /// </summary>
        /// <param name="reslist"></param>
        private void RemoveMapBlokHasUnit(List<Vector2> reslist)
        {
            //�Ƴ��ظ���Ԫ��
            for (int i = 0; i < reslist.Count; i++)
            {
                for (int j = reslist.Count - 1; j > i; j--)
                {

                    if (reslist[i] == reslist[j])
                    {
                        reslist.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < reslist.Count;)
            {
                if (BattleMap.BattleMap.Instance().CheckIfHasUnits(reslist[i]))
                {
                    reslist.Remove(reslist[i]);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// ��ȡ��λ���ܵ���ĵ�ͼ�������
        /// </summary>
        /// <param name="starPos"></param>
        /// <param name="vector2s"></param>
        /// <returns></returns>
        private void GetMapBlockCannontArrive(Vector2 starPos,List<Vector2> vector2s)
        {
            cannotArrivePoss = new List<Vector2>();
            foreach(Vector2 endPos in vector2s)
            {
                if(!BattleMap.BattleMap.Instance().MapNavigator.PathSearch(starPos, endPos))
                {
                    cannotArrivePoss.Add(endPos);
                }
            }
            RemoveMapBlokHasUnit(cannotArrivePoss);
        }

        /// <summary>
        /// ��ȡ��λ���ܷ�Χ�ڵ����е�ͼ������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="reslist"></param>
        private void RecrusiveBodyForSkill(int x,int y,int range,List<Vector2> reslist)
        {
            int columns = BattleMap.BattleMap.Instance().Columns;
            int rows = BattleMap.BattleMap.Instance().Rows;
            if (x < 0 || y < 0 || x >= columns || y >= rows) return;
            reslist.Add(new Vector2(x, y));
            if (range == 0) return;
            if(range == 2 || range == 4)
            {
                if(range == 2)
                {
                    range = range - 1;
                }
                else if(range == 4)
                {
                    range = range - 2;
                }
                RecrusiveBody(x, y, range, reslist);
            }
            if(range == 3 || range == 6)
            {
                if (range == 6) range = range - 1;
                RecrusiveBody2(x, y, range, reslist);
            }
            if(range == 5)
            {
                //TODO
                RecrusiveBody2(x, y, range, reslist);
            }
        }

        /// <summary>
        /// �õ�֮ǰȾɫ�ĵ�λ���Է���λ����ı��������λ��ָ��
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private Unit GetBeforeUnit()
        {
            return BeforeUnit;
        }

        private void SetBeforeUnit(Unit unit)
        {
            BeforeUnit = unit;
        }

        /// <summary>
        /// ���ؼ��ܷ�Χ�ڵ����е�ͼ���������б�
        /// </summary>
        /// <param name="position">��������</param>
        /// <param name="range">��Χ��1-6��</param>
        /// <returns></returns>
        public List<Vector2> GetSkillRnage(Vector2 position, int range)
        {
            List<Vector2> reslist = new List<Vector2>();
            RecrusiveBodyForSkill((int)position.x, (int)position.y,range, reslist);
            return reslist;
        }

        /// <summary>
        /// ������λ�ƶ���Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        public void MarkMoveRange(Vector2 target, Unit unit)
        {
            unitMove = true;
            SetBeforeUnit(unit);
            BattleMap.BattleMap.Instance().ColorMapBlocks(
               GetPositionsWithinCertainMd(target, unit.mov), Color.green);
            if(cannotArrivePoss.Count != 0)
            {
                BattleMap.BattleMap.Instance().ColorMapBlocks(cannotArrivePoss, Color.red);
            }   
        }

        /// <summary>
        /// ������λ������Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        public void MarkAttackRange(Vector2 target,Unit unit)
        {
            SetBeforeUnit(unit);
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, unit.rng), Color.red);
        }

        /// <summary>
        /// ȡ����λ�ƶ���Χ����
        /// </summary>
        /// <param name="target"></param>
        public void CancleMoveRangeMark(Vector2 target)
        {
            unitMove = false;
            Unit unit = GetBeforeUnit();
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.mov), Color.white);   
        }

        /// <summary>
        /// ȡ����λ������Χ����
        /// </summary>
        /// <param name="target"></param>
        public void CancleAttackRangeMark(Vector2 target)
        {
            Unit unit = GetBeforeUnit();
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                 GetPositionsWithinCertainMd(target, unit.rng), Color.white);
        }

        /// <summary>
        /// �������ܷ�Χ
        /// </summary>
        /// <param name="target">��λ����</param>
        /// <param name="range">���ܷ�Χ����Χ�ȼ���1-6����</param>
        public void MarkSkillRange(Vector2 target, int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetSkillRnage(target, range), Color.red);
        }

        /// <summary>
        /// ȡ�����ܷ�Χ����
        /// </summary>
        /// <param name="target">��λ����</param>
        /// <param name="range">���ܷ�Χ����Χ�ȼ���1-6����</param>
        public void CancleSkillRangeMark(Vector2 target,int range)
        {
            BattleMap.BattleMap.Instance().ColorMapBlocks(
                GetPositionsWithinCertainMd(target, range), Color.white);
        }
    }
}