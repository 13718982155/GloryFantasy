using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCard
{
    public class ESSlot : MonoBehaviour
    {
        private List<string> _exSkillCardsList;

        public List<string> ExSkillCards { get { return _exSkillCardsList; } }


        public void Start()
        {
            _exSkillCardsList = new List<string>();
            
            // 将映射关系全部导入当前战技槽
            foreach (string exSkillCardId in CardManager.Instance().unitsExSkillCardDataBase[
                gameObject.GetComponentInChildren<GameUnit.GameUnit>().name])
            {
                InsertESCard(exSkillCardId);
            }

            // TODO： 这只是DEMO中使用，直接把所有的战技牌放入手牌中
            for (int i = _exSkillCardsList.Count - 1; i >= 0; i--)
            {
                SettleESCard(i, true);
            }
        }

        /// <summary>
        /// 向槽中插入新的战技牌
        /// </summary>
        /// <param name="exSkillCardId">战技牌的id</param>
        public void InsertESCard(string exSkillCardId)
        {
            _exSkillCardsList.Add(exSkillCardId);
        }

        
        /// <summary>
        /// 面向UI的接口，用于玩家操作选择把哪个战技牌放到哪里
        /// </summary>
        /// <param name="pos">战技牌在战技槽中的位置</param>
        /// <param name="intoHandCard">是否放入手牌，为true则进入手牌，false进入牌库</param>
        /// <param name="controlRemove">是否控制移除，若为true，则不在本函数内移除，默认false，移除</param>
        public void SettleESCard(int pos, bool intoHandCard, bool controlRemove = false)
        {
            CardManager.Instance().ArrangeExSkillCard(_exSkillCardsList[pos], gameObject.GetInstanceID(), intoHandCard);
            if(!controlRemove)
                _exSkillCardsList.RemoveAt(pos);
        }

        /// <summary>
        /// 获取当前挂载物体实例的id
        /// </summary>
        /// <returns>当前挂载的单位的实例的id</returns>
        public int GetAttatchedObjectId()
        {
            return gameObject.GetInstanceID();
        }
        
        
        /// <summary>
        /// 清空当前战技牌槽
        /// </summary>
        public void ClearESCard()
        {
            _exSkillCardsList.Clear();
        }
        
    }
}