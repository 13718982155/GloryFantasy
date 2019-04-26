﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleMapBlock = BattleMap.BattleMapBlock;
using Unit = GameUnit.GameUnit;

//懒得吐槽了，去看BattleMap我写的注释，改下

/// <summary>
/// 灼烧地图块
/// </summary>
public class BattleMapBlockBurning : BattleMapBlock
{
    private Vector3 vector;
    private Unit unit;
    private bool hasUnit;//判断地图块上是否有单位
    private bool hasBurined;//保证单位进入灼烧块只受到一次伤害

    private void Start()
    {
        vector = GetSelfPosition();
        unit = null;
        hasUnit = false;
        hasBurined = false;
    }

    private void Update()
    {
        Debug.Log(1);
        //检测地图块上是否存在单位
        if (BattleMap.BattleMap.Instance().CheckIfHasUnits(vector))
        {
            hasUnit = true;
            //units = MapManager.MapManager.getInstance().GetUnitsOnMapBlock(vector);
            if (transform.GetComponentInChildren<Unit>() != null)
            {
                unit = transform.GetComponentInChildren<Unit>();
                //Debug.Log(unit.hp);
                Burning(unit);
                //Debug.Log(unit.hp);
            }
        }
        else
        {
            hasUnit = false;
            hasBurined = false;
        }
    }

    //处理灼烧
    private void Burning(Unit unit)
    {
        if (hasUnit == true && hasBurined == false)
        {
            unit.hp -= 1;
            hasBurined = true;
        }
    }

    //
}
