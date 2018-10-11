using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/RangeOfGate")]
public class State_Decision_Gate : State_Decision {

    public override bool Decide(StateController controller)
    {
        return InRangeSphere(controller);
    }

    bool InRangeSphere(StateController controller)
    {
        Collider[] gatesInRange = Physics.OverlapSphere(controller.transform.position, controller.enemyStats.lookSphereCastRadius);

        if (gatesInRange.Length > 0)
        {
            foreach (Collider gate in gatesInRange)
            {
                if (gate.CompareTag("GateTrigger"))
                {
                    controller.gate = gate.GetComponent<GateController>();
                    if (controller.DrawGizmos)
                    {
                        Debug.DrawLine(controller.eyes.position, controller.gate.transform.position, Color.red);
                    }
                    if (!controller.gate.GateDestoryed && controller.gate.TeamID != controller.enemyStats.teamID)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Old version
    bool InRange(StateController controller)
    {
        RaycastHit hit;

        if (Physics.Raycast(controller.eyes.position, controller.eyes.forward, out hit, controller.enemyStats.lookRange))
        {
            if (hit.transform.GetComponent<GateController>() != null && !hit.transform.GetComponent<GateController>().GateDestoryed)
            {
                if (controller.DrawGizmos)
                {
                    Debug.DrawLine(controller.eyes.position, hit.point, Color.red);
                }
                controller.gate = hit.transform.GetComponent<GateController>();
                return true;
            }
        } else
        {
            controller.gate = null;
        }

        return false;
    }
}
