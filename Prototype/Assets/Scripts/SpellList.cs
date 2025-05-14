using UnityEngine;

//Required in order to use lists in C#
using System.Collections.Generic;


public class SpellList : MonoBehaviour
{
    //Created a spell class
    class Spell
    {
    //Field to set the name of the spells
        [SerializeField] string name;
    }
    //The spell objects
    Spell fireball;
    Spell callLightning;
    Spell summonSkeleton;

    //A list containing the spells
    List<Spell> spellList = new List<Spell>();

    void Start()
    {
    //Adding spells to the list once the game initializes 
        spellList.Add(fireball);
        spellList.Add(callLightning);
        spellList.Add(summonSkeleton);
    }
}
