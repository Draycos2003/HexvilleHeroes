using UnityEngine;

[CreateAssetMenu(menuName = "StatModifier/ shieldMod")]
public class ShieldModifierSO : CharactersStatModSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        playerController player = character.GetComponent<playerController>();
        if (player != null)
        {
            player.gainShield((int)val);
        }
    }
}
