using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
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
    private AudioSource src;

    void Awake()
    {
        interactUI?.SetActive(false);
        closedRot = lid.localRotation;
        openRot = Quaternion.Euler(lid.localEulerAngles + Vector3.up * openAngle);
        src = GetComponent<AudioSource>();
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
        // mark the chest open so it cant be reopened
        isOpen = true;

        // hide the "Press E" UI prompt
        if (interactUI != null)
            interactUI.SetActive(false);

        // play the open sound stretched to match how long the lid takes
        if (openSound != null)
        {
            src.clip = openSound;
            src.pitch = openSound.length / openDuration;
            src.Play();
        }

        // animate the lid from closedRot to openRot over openDuration
        float elapsedTime = 0f;
        while (elapsedTime < openDuration)
        {
            float normalized = elapsedTime / openDuration;
            lid.localRotation = Quaternion.Slerp(closedRot, openRot, normalized);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lid.localRotation = openRot;  // wait for it to finish opening

        // if we have a loot table, spawn loot and currency
        if (chestItem != null)
        {
            // primary loot
            GameObject loot = chestItem.GetRandomLoot();
            if (loot != null)
                StartCoroutine(AnimateArc(loot));
            // bonus loot
            if (chestItem.ShouldDoubleLoot())
                StartCoroutine(AnimateArc(loot));

            // primary currency
            GameObject money = chestItem.GetRandomCurrency();
            if (money != null)
                StartCoroutine(AnimateArc(money));
            // bonus currency
            if (chestItem.ShouldDoubleCurrency())
                StartCoroutine(AnimateArc(money));
        }
    }

    private IEnumerator AnimateArc(GameObject prefab)
    {
        // calculate the spawn position in front of the chest
        Vector3 startPos = transform.position
                         + transform.forward * spewOffset
                         + Vector3.up * 0.5f;

        // spawn the item with the chests rotation so its forward is local
        GameObject spawned = Instantiate(prefab, startPos, transform.rotation);

        // calc the end position along the spawned objects forward
        Vector3 endPos = startPos + spawned.transform.forward * lootArcForwardDistance;

        float elapsedTime = 0f;
        while (elapsedTime < lootArcDuration)
        {
            float t = elapsedTime / lootArcDuration;

            // move horizontally from start to end
            Vector3 horizontal = Vector3.Lerp(startPos, endPos, t);

            // apply a vertical arc that peaks mid-flight
            float vertical = Mathf.Sin(t * Mathf.PI) * lootArcHeight;

            spawned.transform.position = horizontal + Vector3.up * vertical;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ensure it lands exactly at endPos
        spawned.transform.position = endPos;
    }
}
