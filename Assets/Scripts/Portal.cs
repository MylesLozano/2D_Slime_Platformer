using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    [SerializeField] private Transform destination;

    // Add this for the portal sound effect
    [SerializeField] private AudioClip portalSFX;
    private AudioSource audioSource;

    private void Awake()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (portalObjects.Contains(collision.gameObject))
        {
            return;
        }

        // Play the portal sound effect
        PlayPortalSFX();

        if (destination.TryGetComponent(out Portal destinationPortal))
        {
            destinationPortal.portalObjects.Add(collision.gameObject);
        }

        collision.transform.position = destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        portalObjects.Remove(collision.gameObject);
    }

    private void PlayPortalSFX()
    {
        if (portalSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(portalSFX);
        }
    }
}
