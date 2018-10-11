using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Patrol")]
public class State_Action_Patrol : State_Action {

    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        controller.destination.target = controller.StartDestination;
        controller.transform.parent = controller.EnemySpawnContainer;
    }
}
