using UnityEngine;

public class AlleyZone : MonoBehaviour
{
    public bool playerInAlley;
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
