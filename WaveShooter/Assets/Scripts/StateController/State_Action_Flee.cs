using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Flee")]
public class State_Action_Flee : State_Action {

    public override void Act(StateController controller)
    {
        Flee(controller);
    }

    void Flee(StateController controller)
    {
        Transform pos = controller.startPosition;

        controller.destination.target = pos;
        controller.transform.parent = pos;
    }

}
