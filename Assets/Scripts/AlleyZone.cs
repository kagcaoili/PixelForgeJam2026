using UnityEngine;

public class AlleyZone : MonoBehaviour
{
    public bool playerInAlley { get; private set; }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInAlley = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInAlley = false;
        }
    }
}
