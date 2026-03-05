using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyStateMachine : MonoBehaviour
{
    public enum enemyState
    {
        idle,
        patrol,
        chase,
        attack,
        flee,
        stagger,
        stalk,
        death
    }
}
