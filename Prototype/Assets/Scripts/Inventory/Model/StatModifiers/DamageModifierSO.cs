using UnityEngine;

[CreateAssetMenu (menuName = "StatModifier/ damageMod")]
public class DamageModifierSO : CharactersStatModSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        playerController player = character.GetComponent<playerController>();
        if (player != null)
        {
            player.gainDamage((int)val);
        }
    }
}
