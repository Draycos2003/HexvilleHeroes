using UnityEngine;
using System.Collections;

public class MagicArr : MonoBehaviour
{
    //Dont need a start, instead using an OnTriggerEnter to determine if the player is within range of the caster
    private void Start()
    {
        string[] spells = { "Fireball", "Call Lightning", "Summon Skeleton" };
    }
}
