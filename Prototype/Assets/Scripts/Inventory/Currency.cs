using UnityEngine;

public class Currency : MonoBehaviour
{
    private GameObject Player;

    void Start()
    {
        Player = gamemanager.instance?.Player;

        if (Player == null)
        {
            Debug.LogError("[Currency] Player not found from GameManager.");
        }
    }

    public void AddGold(int amount)
    {
        if (Player == null) return;

        playerController pc = Player.GetComponent<playerController>();
        pc?.AddGold(amount);
    }

    public void RemoveGold(int amount)
    {
        if (Player == null) return;

        playerController pc = Player.GetComponent<playerController>();
        pc?.RemoveGold(amount);
    }
}
