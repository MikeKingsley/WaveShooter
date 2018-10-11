using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Flee")]
public class State_Decision_Flee : State_Decision
{
    public override bool Decide(StateController controller)
    {
        if (controller.enemyStats.canFlee && controller.CheckIfCountDownElapsed(controller.enemyStats.bravery) && !controller.flee)
        {
            controller.flee = controller.ChanceToFlee();
            controller.ResetTimeElapsed();
        }
        return controller.flee;
    }


}
