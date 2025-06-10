using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private KeyCode interactKey;
    [SerializeField] private Transform lid;
    [SerializeField] private float openAngle;
    [SerializeField] private float openDuration;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float spewOffset;
    [SerializeField] private ChestItem chestItem;

    [Header("Loot Arc Settings")]
    [SerializeField] private float lootArcHeight;
    [SerializeField] private float lootArcForwardDistance;
    [SerializeField] private float lootArcDuration;

    private bool playerInRange, isOpen;
    private Quaternion closedRot, openRot;

    void Awake()
    {
        interactUI?.SetActive(false);
        closedRot = lid.localRotation;
        openRot = Quaternion.Euler(lid.localEulerAngles + Vector3.up * openAngle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = true;
            interactUI?.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = false;
            interactUI?.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(interactKey))
            StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isOpen = true;
        interactUI?.SetActive(false);

        if (openSound != null)
        {
            float pitch = openSound.length / openDuration;
            soundFXmanager.instance.PlayPitched3DClip(openSound, transform, 1f, pitch, 1f, 10f);
        }

        float elapsedTime = 0f;
        while (elapsedTime < openDuration)
        {
            float normalized = elapsedTime / openDuration;
            lid.localRotation = Quaternion.Slerp(closedRot, openRot, normalized);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lid.localRotation = openRot;

        if (chestItem != null)
        {
            GameObject loot = chestItem.GetRandomLoot();
            if (loot != null)
                StartCoroutine(AnimateArc(loot));
            if (chestItem.ShouldDoubleLoot())
                StartCoroutine(AnimateArc(loot));

            GameObject money = chestItem.GetRandomCurrency();
            if (money != null)
                StartCoroutine(AnimateArc(money));
            if (chestItem.ShouldDoubleCurrency())
                StartCoroutine(AnimateArc(money));
        }
    }

    private IEnumerator AnimateArc(GameObject prefab)
    {
        Vector3 startPos = transform.position + transform.forward * spewOffset + Vector3.up * 0.5f;
        GameObject spawned = Instantiate(prefab, startPos, transform.rotation);
        Vector3 endPos = startPos + spawned.transform.forward * lootArcForwardDistance;

        float elapsedTime = 0f;
        while (elapsedTime < lootArcDuration)
        {
            float t = elapsedTime / lootArcDuration;
            Vector3 horizontal = Vector3.Lerp(startPos, endPos, t);
            float vertical = Mathf.Sin(t * Mathf.PI) * lootArcHeight;
            spawned.transform.position = horizontal + Vector3.up * vertical;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spawned.transform.position = endPos;
    }
}
