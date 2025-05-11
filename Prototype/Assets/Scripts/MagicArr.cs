using UnityEngine;
using System.Collections;

public class MagicArr : MonoBehaviour
{
    //Dont need a start, instead using an OnTriggerEnter to determine if the player is within range of the caster
    void OnTriggerEnter(Collider other)
    {
        //Compares if the object is tagged "Player"
        if (other.CompareTag("Player"))
        {
            //Exicuting RandomMagic() if the object is tagged "Player"
            //This is our immediate cast that will happen when the enemy is close enough
            RandomMagic();
            //The start of a coroutine that makes the attacker randomly cast a spell after 2 seconds.
            StartCoroutine(DelaySpell(2f));

        }
    }
    //IEnumerator that is used to control the speed that RandomMagic() is cast in a row
    IEnumerator DelaySpell(float spellTime)
    {
        //defining spellTime
        spellTime = 2f;
        //Wait for the spellTime before using RandomMagic() again.
        yield return new WaitForSeconds(spellTime);
        RandomMagic();
    }
    void RandomMagic()
    {
        //Array for Random Magic
        string[] spells = { "Fireball", "Call Lightning", "Summon Skeleton" };
        //Randomly selects spell
        string RandomSpell = spells[Random.Range(0, spells.Length)];
        //Print to the console that you casted a spell
        Debug.Log("You cast: " + RandomSpell);
    }
 
}
