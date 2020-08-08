using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveNode : Node
{
    public virtual bool Setup()
    {
        return false;
    }

    public virtual void EnterRange()
    {

    }

    public virtual void ExitRange()
    {

    }
}
