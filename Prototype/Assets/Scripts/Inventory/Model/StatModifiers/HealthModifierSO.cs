using UnityEngine;

[CreateAssetMenu(menuName = "StatModifier/ healthMod")]
public class HealthModifierSO : CharactersStatModSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        playerController player = character.GetComponent<playerController>();
        if (player != null)
        {
            player.gainHealth((int)val);
        }
    }
}








