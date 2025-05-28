using UnityEngine;

[CreateAssetMenu(menuName = "StatModifier/ speedMod")]
public class SpeedModifierSO : CharactersStatModSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        playerController player = character.GetComponent<playerController>();
        if (player != null)
        {
            player.gainSpeed((int)val);
        }
    }
}
