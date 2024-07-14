using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : CharacterController
{
    public void TakePunch()
    {
        EnableRagdoll(true);
    }
}
