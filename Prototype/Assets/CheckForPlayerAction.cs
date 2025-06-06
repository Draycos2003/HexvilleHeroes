using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckForPlayer", story: "Check if [targetDector] has a target", category: "Action", id: "a925eda0e04244275a7d8af395ea20f6")]
public partial class CheckForPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetDector;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

