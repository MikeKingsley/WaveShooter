using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Chase")]
public class State_Decision_Chase : State_Decision {

    public override bool Decide(StateController controller)
    {
        return AnyTargetsInRange(controller);
    }

    bool AnyTargetsInRange(StateController controller)
    {
        if (controller.gate != null)
            return false;

        Collider[] targetsInRange = Physics.OverlapSphere(controller.transform.position, controller.enemyStats.lookSphereCastRadius);

        if (targetsInRange.Length > 0)
        {
            controller.acquiredTarget = FindClosestEnemy(controller, targetsInRange);

            if (controller.acquiredTarget != null)
            {
                if (controller.DrawGizmos)
                {
                    Debug.DrawLine(controller.transform.position, controller.acquiredTarget.transform.position, controller.randomColor);
                }
                return true;
            }
        }

        return false;
    }

    GameObject FindClosestEnemy(StateController controller, Collider[] targets)
    {
        GameObject closest = null;
        float distance = controller.enemyStats.lookRange;

        foreach (Collider target in targets)
        {
            if (target.CompareTag(controller.tag))
            {
                GameObject go = target.transform.gameObject;

                if (go != null && go.GetComponent<StateController>() != null)
                {
                    StateController targetRef = go.GetComponent<StateController>();
                    if (targetRef.enemyStats.teamID != controller.enemyStats.teamID)// && !targetRef.flee
                    {
                        Vector3 diff = go.transform.position - controller.transform.position;
                        float curDistance = diff.sqrMagnitude;

                        if (curDistance < distance)
                        {
                            closest = go;
                            distance = curDistance;
                        }
                    }
                }
            }
        }
        return closest;
    }

}
