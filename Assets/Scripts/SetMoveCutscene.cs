using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Networking", "Set Move", "Set Move")]

public class SetMoveCutscene : Command
{
    public bool setMove;
    public MoveMode moveMode;
    public enum MoveMode
    {
        move, turn
    }
    public override void OnEnter()
    {
        switch (moveMode)
        {
            case MoveMode.move:
                GameData.setCanMove.Invoke(setMove);
                break;
            case MoveMode.turn:
                GameData.setCanTurn.Invoke(setMove);
                break;
        }
        Continue();
    }
}
