using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ach : MonoBehaviour
{
    public enum UIState
    {
        INCOMPLETE = 0,
        COMPLETED_UNCLAIMED,
        COMPLETED_CLAIMED,
    }

    public UIState state;

    public abstract void ChangeState(int num);

}
