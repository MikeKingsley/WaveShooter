using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/AcquireTarget")]
public class State_Decision_AcquireTarget : State_Decision {

    public override bool Decide(StateController controller)
    {
        return CheckForObstacles(controller);
    }

    bool CheckForObstacles(StateController controller)
    {
        if (controller.acquiredTarget == null)
            return false;

        float distToTarget = Vector3.Distance(controller.transform.position, controller.acquiredTarget.transform.position);

        if (distToTarget <= controller.enemyStats.attackRange * 2)
        {
            RaycastHit hit;

            if (Physics.Raycast(controller.eyes.position, controller.eyes.forward, out hit, controller.enemyStats.attackRange))
            {
                if (controller.DrawGizmos)
                {
                    Debug.DrawLine(controller.eyes.position, hit.point, Color.red);
                }

                StateController hitCon = hit.transform.GetComponent<StateController>();

                if (hitCon == null || controller.gate != null)
                {
                    return false; //something is in the way
                }
            }
        }

        return true; //continue to attack
    }

}
