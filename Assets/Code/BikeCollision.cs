using UnityEngine;

public class BikeCollision : MonoBehaviour
{
    public FlappyCheckers manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bike hit: " + other.name);
        if (manager != null)
            manager.GameOver(manager.Score);
    }
}
