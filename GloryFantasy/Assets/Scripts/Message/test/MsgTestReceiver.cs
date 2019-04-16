﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using IMessage;

enum MsgTestType
{
    A,
    B,
    C,
    D,
    UnitMoving
};

public class MsgTestReceiver : MonoBehaviour, IMessage.MsgReceiver
{
    [FormerlySerializedAs("MapManager")] public BattleMap.BattleMap map;
    private Vector3 coordinate;

    public GameUnit.GameUnit GetGameUnit()
    {
        return null;
    }

    private void Awake()
    {
        //IMessage.MsgDispatcher.RegisterMsg(this, (int)MsgTestType.A, Condition, Action);
        //gameObject.SetActive(false);

        IMessage.MsgDispatcher.RegisterMsg(GetComponent<GameUnit.GameUnit>().GetMsgReceiver(), (int)MsgTestType.UnitMoving, Condition, Action);
        //IMessage.MsgDispatcher.RegisterMsg(this, (int)TriggerType.UpdateSource, Condition, UpdateSource);//初始or更新资源
        //IMessage.MsgDispatcher.RegisterMsg(this, (int)TriggerType.BP, Condition, BattlePresent);//战斗回合开始
    }

    private bool Condition()
    {
        return true;
    }

    private void Action()
    {
        //Debug.Log("action 激活");
        //gameObject.SetActive(true);
        //Gameplay.GetInstance().gamePlayInput.HandleConfirm(this.coordinate);
        // TODO :添加点击确定按钮事件
        //Debug.Log("Ok Cliked!");
        //Gameplay.GetInstance().gamePlayInput.HandleConfirm(this.coordinate);
    }

    private void UpdateSource()
    {
        Debug.Log("初始or更新资源");
        //TODO处理更新资源事件
        MsgTestButton.Instance.IsUpdateResourceOver = true;
        Debug.Log("初始or更新资源完毕");
        MsgTestButton.Instance.UnityEventUpdateResource.RemoveListener(UpdateSource);
    }

    private void BattlePresent()
    {
        Debug.Log("战斗开始，我的回合开始");
        MsgTestButton.Instance.IsBattlePresentOver = true;
        Debug.Log("回合开始阶准备阶段结束");
    }

}

//TODO:扩充这个Trigger
namespace IMessage
{

    class Trigger1 : Trigger
    {
        Trigger1()
        {
            msgName = (int)TriggerType.ActiveAbility;
            condition = Condition;
            action = Action;
        }

        private bool Condition()
        {
            if (this.GetAttacker().name == "fsaf")
                return false;
            else
                return true;
        }

        private void Action()
        {
            
        }
    }
}