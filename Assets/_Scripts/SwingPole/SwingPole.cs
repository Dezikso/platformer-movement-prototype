using UnityEngine;

public class SwingPole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.StartSwinging(transform);
        }
    }
}
