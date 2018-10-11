using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State_Transition
{
    public State_Decision decision;
    public State trueState;
    public State falseState;
}