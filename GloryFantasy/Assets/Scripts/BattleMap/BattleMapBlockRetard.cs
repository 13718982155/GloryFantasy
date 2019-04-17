﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit = GameUnit.GameUnit;
using BattleMapBlock = BattleMap.BattleMapBlock;
/// <summary>
/// 粘滞地图块
/// </summary>
public class MapBlockRetard : BattleMapBlock
{

    private Unit unit;
    private Vector3 vector;
    private bool hasUnit;//判断地图块上是否有单位
    private bool hasRetard;

    private int tempMov;

    // Use this for initialization
    void Start()
    {
        unit = null;
        vector = GetSelfPosition();
        hasUnit = false;
        hasRetard = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleMap.BattleMap.getInstance().CheckIfHasUnits(vector) && hasRetard == false)
        {
            hasUnit = true;
            //units = MapManager.MapManager.getInstance().GetUnitsOnMapBlock(vector);
            if (transform.GetComponentInChildren<Unit>() != null)
            {
                unit = transform.GetComponentInChildren<Unit>();
                tempMov = unit.unitAttribute.Mov;
                Debug.Log(unit.unitAttribute.Mov);
                Retard(unit);
                Debug.Log(unit.unitAttribute.Mov);
            }
        }
        else
        {
            hasUnit = false;
            hasRetard = false;
            unit.unitAttribute.Mov = tempMov;
        }
    }

    //处理滞泻
    private void Retard(Unit unit)
    {
        if (hasUnit == true)
        {
            unit.unitAttribute.Mov = 0;
            hasRetard = true;
        }
    }
}
