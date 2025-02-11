﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCollection;

//2019.4.25版
namespace MainMap
{
    /// <summary>角色的运动状态
    /// 
    /// </summary>
    public enum MoveState
    {
        Start,//准备移动阶段
        Moving,//移动中
        Stop,//移动结束
        MotionLess,//静止
    }
    public class Charactor : UnitySingleton<Charactor>
    {

        public Vector3 locate;
        /// <summary>开放给策划用于在Unity的Inspector里直接调整数值，代码内需要修改角色信息请修改CharacorData.
        /// 
        /// </summary>
        public int HP;//人物血量
        public int Step;//当前剩余步数，貌似不需要在外部修改，写在外面只是为了看见它。
        public int MaxStep;//最大步数
        public int MoveSpeed;//开放给策划用于调整人物角色移动速度
        /// <summary>储存角色信息的数据结构，考虑到未来可能有需求需要返回全部的数值，就写了这么个玩意。
        /// 
        /// </summary>
        public struct CharactorData
        {
            public int hp;
            public int maxstep;
            public int step;
            public object[,] bag;//背包
            public HexVector playerlocate;
            public GameObject underfeet;
            public MoveState charactorstate;
            public string VecToString(float i)
            {
                return (System.Convert.ToInt32(i)).ToString();
            }

        }
        /// <summary>传给战斗系统的相关数据
        /// 
        /// </summary>
        public struct BattleMapData
        {
            /// <summary>遭遇id
            /// 
            /// </summary>
            public string encounterid;
            /// <summary>最大专注
            /// 
            /// </summary>
            public int maxcost;
            /// <summary>手牌数最大值
            /// 
            /// </summary>
            public int numofhand;
            /// <summary>玩家卡牌收藏的引用
            /// 
            /// </summary>
            public List<string> playercollection;
            /// <summary>玩家战技的引用
            /// 
            /// </summary>
            public Dictionary<string, string> playerbattleskill;
        }
        /// <summary>储存角色周围地格信息的数据结构
        ///
        /// </summary>
        public Dictionary<string, MapUnit> aroundlist = new Dictionary<string, MapUnit>();
        public CharactorData charactordata;
        public BattleMapData battlemapdata;
        /// <summary>设定角色初始位置
        /// 
        /// </summary>
        /// <param name="locate"></param>
        /// <returns></returns>
        public Vector3 SetCharactorLocate(Vector3 locate)
        {
            Debug.Log("初始化角色位置");
            charactordata.playerlocate.ChangeToNormalVect(locate);
            return charactordata.playerlocate.Normal_vector;
        }
        /// <summary>初始化角色信息
        /// 
        /// </summary>
        public void CharactorInitalize()
        {
            GetComponent<Transform>().position = SetCharactorLocate(locate);
            this.SetMessage(HP, MaxStep);
            this.charactordata.charactorstate = MoveState.MotionLess;
            Vector3 vect = charactordata.playerlocate.Hex_vector;
            charactordata.underfeet = GameObject.Find("test" + charactordata.VecToString(vect.x) + charactordata.VecToString(vect.y));
            setaround(charactordata.underfeet);
            Debug.Log("角色初始化完成");
        }
        /// <summary>游戏运行开始时初始化battlemapdata
        /// 
        /// </summary>
        public void InitalizeBattleMapData()
        {
            Debug.Log("战斗信息初始化");
            battlemapdata.maxcost = 3;
            battlemapdata.numofhand = 5;
            battlemapdata.playercollection = CardCollection.Instance().mycollection;
            battlemapdata.playerbattleskill = CardCollection.Instance().battleskill;
        }

        /// <summary>设定初始步数和血量
        /// 
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="maxstep"></param>
        /// <returns></returns>
        public CharactorData SetMessage(int hp, int maxstep)
        {
            Debug.Log("初始化角色最大步数和血量");
            charactordata.hp = hp;
            charactordata.maxstep = maxstep;
            charactordata.step = charactordata.maxstep;
            Step = charactordata.step;
            return charactordata;
        }

        /// <summary>如果移动合法，调用这个函数改变角色坐标。
        /// 
        /// </summary>
        /// <param name="newtransform"></param>
        /// <returns></returns>
        public void Move(Vector3 newtransform, int step)
        {
            if (ChangeStep(step))
            {
                charactordata.charactorstate = MoveState.Start;
                StartCoroutine(MoveAction(newtransform, MoveSpeed));
            }
            else
            {
                HasDead();
                Debug.Log("角色因死亡返回至：" + charactordata.playerlocate.Hex_vector.x.ToString() + "," + charactordata.playerlocate.Hex_vector.z.ToString());
            }
        }
        /// <summary>重设角色周围地形格写入AroundList
        /// 
        /// </summary>
        /// <param name="onclk"></param>
        /// <returns></returns>
        public Dictionary<string, MapUnit> setaround(GameObject onclk)
        {
            aroundlist["0,1"] = SetAround(onclk, 0, 1);
            aroundlist["0,-1"] = SetAround(onclk, 0, -1);
            aroundlist["1,0"] = SetAround(onclk, 1, 0);
            aroundlist["-1,0"] = SetAround(onclk, -1, 0);
            aroundlist["-1,1"] = SetAround(onclk, -1, 1);
            aroundlist["1,-1"] = SetAround(onclk, 1, -1);
            return aroundlist;
        }
        /// <summary>重设地形格字典值的具体逻辑
        /// 
        /// </summary>
        /// <param name="onclk"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public MapUnit SetAround(GameObject onclk, int a, int b)
        {
            MapUnit playeraround;
            int x = (int)onclk.GetComponent<MapUnit>().hexVector.Hex_vector.x + a;
            int y = (int)onclk.GetComponent<MapUnit>().hexVector.Hex_vector.y + b;
            if (GameObject.Find("test" + x.ToString() + y.ToString()) != null)
            {
                playeraround = GameObject.Find("test" + x.ToString() + y.ToString()).GetComponent<MapUnit>();
            }
            else
            {
                Debug.Log("角色无此相邻地格");
                playeraround = null;
            }
            return playeraround;
        }
        /// <summary>改变角色步数时调用， 并返回剩余步数。
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ChangeStep(int value)
        {
            Debug.Log("角色减少步数" + value);
            if (charactordata.step + value <= charactordata.maxstep)
            {

                charactordata.step = charactordata.step + value;
                Step = charactordata.step;
                if (charactordata.step < 0)
                {
                    return false;
                }
            }
            else
            {
                Debug.Log("超出最大步数");
                charactordata.step = charactordata.maxstep;
                Step = charactordata.step;
            }

            return true;
        }
        /// <summary>角色死亡时调用。
        /// 
        /// </summary>
        public void HasDead()
        {

            Debug.Log("步数为负，角色累死了，返回起点");
            CharactorInitalize();

        }
        public void Onclick()
        {
            //todo：这个方法是干嘛的来着？貌似没什么用 2019.5.29
            Debug.Log("onclick");
        }

        /// <summary>实现移动的携程
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="movespeed"></param>
        /// <returns></returns>
        IEnumerator MoveAction(Vector3 target, float movespeed)
        {
            if (charactordata.charactorstate == MoveState.Start)
            {
                charactordata.charactorstate = MoveState.Moving;
                Debug.Log("移动开始");
                while (GetComponent<Transform>().position != target)
                {
                    Debug.Log("移动中");
                    this.GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, target, movespeed * Time.deltaTime);
                    yield return 0;
                }
                charactordata.charactorstate = MoveState.Stop;
                Debug.Log("移动结束");
                Vector3 vect = charactordata.playerlocate.ChangeToHexVect(target);
                setaround(GameObject.Find("test" + charactordata.VecToString(vect.x) + charactordata.VecToString(vect.y)));
                charactordata.underfeet = GameObject.Find("test" + charactordata.VecToString(vect.x) + charactordata.VecToString(vect.y));
                Debug.Log("角色移动至：" + charactordata.VecToString(vect.x) + charactordata.VecToString(vect.y));
                charactordata.charactorstate = MoveState.MotionLess;
                charactordata.underfeet.GetComponent<MapUnit>().ChangePositionOver();

            }

        }

    }
}