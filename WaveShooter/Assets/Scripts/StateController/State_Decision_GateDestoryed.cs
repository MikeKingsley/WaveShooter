using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/CheckGateHealth")]
public class State_Decision_GateDestoryed : State_Decision
{
    public override bool Decide(StateController controller)
    {
        if (controller.gate == null)
            return true;

        return controller.gate.GateDestoryed;
    }


}
