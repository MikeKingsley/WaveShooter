using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/ChaseTarget")]
public class State_Action_ChaseTarget: State_Action {

    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    void Chase(StateController controller)
    {
        if (controller.acquiredTarget != null)
        {
            controller.destination.target = controller.acquiredTarget.transform;
            controller.SetAnimBool(controller.attackingAnim, false);
            //controller.aipathscript.maxSpeed = controller.enemyStats.runSpeed;
        }
    }

}
