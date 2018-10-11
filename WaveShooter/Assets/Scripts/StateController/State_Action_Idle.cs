using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Idle")]
public class State_Action_Idle : State_Action {

    public override void Act(StateController controller)
    {
        controller.SetAnimBool(controller.attackingAnim, false);
        controller.SetAnimBool(controller.combatIdleAnim, false);
    }

}
