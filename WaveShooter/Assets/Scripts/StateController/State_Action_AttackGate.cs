using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackGate")]
public class State_Action_AttackGate : State_Action {

    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    void Attack(StateController controller)
    {

        if (controller.gate != null && !controller.gate.GateDestoryed)
        {
            float dist = Vector3.Distance(controller.transform.position, controller.gate.transform.position);

            controller.destination.target = controller.gate.transform;

            if (dist <= controller.enemyStats.attackRange)
            {
                if (controller.enemyStats.teamID != controller.gate.TeamID && controller.CheckIfCountDownElapsed(controller.enemyStats.attackRate))
                {
                    float dealDamage = controller.enemyStats.attackDamage * controller.enemyStats.attackForce;
                    controller.gate.DamageGate(dealDamage);
                    controller.SetAnimBool(controller.attackingAnim, true);
                    controller.ResetTimeElapsed();
                } else
                {
                    controller.SetAnimBool(controller.combatIdleAnim, true);
                }
           }
        }

        if (controller.gate != null && controller.gate.GateDestoryed)
        {
            controller.gate = null;
            controller.SetAnimBool(controller.attackingAnim, false);
            controller.SetAnimBool(controller.combatIdleAnim, false);
        }
    }

}
