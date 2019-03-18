﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoundInput
{

};

public class RoundProcessController
{
    public RoundProcessController()
    {
        _state = RoundState.crystalPhase;
    }

    public RoundState _state;
}

public class RoundState
{
    virtual public void HandleInput(RoundProcessController roundProcessController, RoundInput input) { }
    virtual public void Update(RoundProcessController roundProcessController) { }
    virtual public void Enter(RoundProcessController roundProcessController) { }
    virtual public void Exit(RoundProcessController roundProcessController) { }
    virtual public void NextState(RoundProcessController roundProcessController)
    {
        Exit(roundProcessController);
    }

    public static CrystalPhase crystalPhase;
    public static StartPhase startPhase;
    public static DrawPhase drawPhase;
    public static ReadyPhase readyPhase;
    public static MainPhase mainPhase;
    public static DiscardPhase discardPhase;
    public static EndPhase endPhase;
}

public class CrystalPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.startPhase;
    }
}

public class StartPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.drawPhase;
    }
}

public class DrawPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.readyPhase;
    }
}

public class ReadyPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.mainPhase;
    }
}

public class MainPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.discardPhase;
    }
}

public class DiscardPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.endPhase;
    }
}

public class EndPhase : RoundState
{
    public override void NextState(RoundProcessController roundProcessController)
    {
        base.NextState(roundProcessController);
        roundProcessController._state = RoundState.crystalPhase;
    }
}