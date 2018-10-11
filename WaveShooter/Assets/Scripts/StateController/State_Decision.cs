using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State_Decision : ScriptableObject
{
    public abstract bool Decide(StateController controller);
}