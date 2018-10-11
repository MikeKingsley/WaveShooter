using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/DestinationReached")]
public class State_Decision_DestinationReached : State_Decision
{
    public override bool Decide(StateController controller)
    {        
        return controller.aipathscript.reachedEndOfPath;
    }


}
