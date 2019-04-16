﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUnit
{
    public class EnemyUnit : GameUnit, IPointerDownHandler
    {
        private void Start()
        {
            hp = unitAttribute.HP;
        }

        /// <summary>
        /// 鼠标点击敌人Unit，掉血
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            hp -= 3;
            UnitManager.Instance.EnemyCurUnit = transform.GetComponentInParent<BattleMap.BattleMapBlock>().GetCoordinate();
            if (!IsDead())
            {
                Debug.Log(hp);
                float hpDivMaxHp = (float)hp / unitAttribute.MaxHp * 100;
                var textHp = transform.GetComponentInChildren<Text>();
                textHp.text = string.Format("Hp: {0}%", hpDivMaxHp);
            }
            else
            {
                Destroy(this.gameObject);            
                BattleMap.BattleMap.getInstance().upDateNeighbourBlock(UnitManager.Instance.EnemyCurUnit);
            }

        }
    }
}


