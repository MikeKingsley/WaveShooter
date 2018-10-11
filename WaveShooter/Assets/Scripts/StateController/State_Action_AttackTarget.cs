using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackTarget")]
public class State_Action_AttackTarget: State_Action {

    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    void Attack(StateController controller)
    {
        if (controller.acquiredTarget != null && controller.acquiredTarget.GetComponent<StateController>() != null)
        {
            float dist = Vector3.Distance(controller.transform.position, controller.acquiredTarget.transform.position);
            Vector3 targetPostition = new Vector3(controller.acquiredTarget.transform.position.x, controller.transform.position.y, controller.acquiredTarget.transform.position.z);

            controller.destination.target = controller.acquiredTarget.transform;
            controller.transform.LookAt(targetPostition);

            controller.distanceFromTarget = dist;

            if (dist <= controller.enemyStats.attackRange)
            {
                StateController targetCon = controller.acquiredTarget.GetComponent<StateController>();

                if (targetCon != null)
                {
                    controller.attacking = true;
                    controller.SetAnimBool(controller.attackingAnim, true);

                    if (controller.CheckIfCountDownElapsed(controller.enemyStats.attackRate))
                    {
                        float dealDamage = controller.enemyStats.attackDamage * controller.enemyStats.attackForce;
                        targetCon.DoDamage(dealDamage);
                        controller.ResetTimeElapsed();
                    }
                }
            } else
            {
                controller.SetAnimBool(controller.attackingAnim, false);
                controller.attacking = false;
            }
        } else
        {
            controller.acquiredTarget = null;
            controller.destination.target = null;
            controller.SetAnimBool(controller.attackingAnim, false);
            controller.SetAnimBool(controller.combatIdleAnim, false);
            controller.attacking = false;
        }
    }

}
