using UnityEngine;

[CreateAssetMenu(fileName = "CreditsData", menuName = "Credits/Credits Data")]
public class CreditsData : ScriptableObject
{
    [System.Serializable]
    public struct CreditLine
    {
        public string name;
        public string role;
    }

    public CreditLine[] lines;
}
