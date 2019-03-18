﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TriggerType
{
    BP,
    MPBegin,
    MPEnd,
    EP,
    CastCard,
    Summon,
    DrawCard,
    AddInHand,
    AnnounceAttack,
    ActiveAbility,
    BeAttacked,
    Damage,
    BeDamaged,
    Kill,
    BeKilled,
    Dead,
    ToBeKilled,
    Move
};

public class Info
{
    public Player RoundOwned; //回合所属
    public Player Caster; //施放者
    public Player CastingCard; //施放的牌
    public Player SummonersController; //召唤单位的控制者
    public List<GameUnit.GameUnit> SummonUnit; //召唤的单位
    public Player Drawer; //抓牌者
    public List<Card> CaughtCard; //抓的牌
    public Player HandAdder; //加手者
    public List<Card> AddingCard; //加手的牌
    public GameUnit.GameUnit Attacker; //宣言攻击者
    public GameUnit.GameUnit AttackedUnit; //被攻击者
    public GameUnit.GameUnit AbilitySpeller; //发动异能者
    public Ability SpellingAbility; //发动的异能
    public GameUnit.GameUnit Injurer; //伤害者
    public GameUnit.GameUnit InjuredUnit; //被伤害者
    public Damage damage; //伤害
    public GameUnit.GameUnit Killer; //击杀者
    public GameUnit.GameUnit KilledUnit; //被杀者
    public GameUnit.GameUnit Dead; //死者

    /// <summary>
    /// 被UI选定的Unit单位
    /// </summary>
    public GameUnit.GameUnit SelectingUnit;
}

public class Damage
{

}

public class DamageRequest { }

public class Gameplay
{
    public static Info Info;
    public RoundProcessController roundProcessController;

    public void HandleInput() { }
}