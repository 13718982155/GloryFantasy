using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCard
{
    /*
     * 暂时仅用单位牌，且使用超级生物的信息进行默认初始化
     */
    public class BaseCard : MonoBehaviour, IMessage.MsgReceiver
    {

        public string id;

        T IMessage.MsgReceiver.GetUnit<T>()
        {
            return this as T;
        }

        // 记录卡牌冷却回合数
        public int cooldownRounds { get; set; }

    }
    
}