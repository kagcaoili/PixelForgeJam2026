using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public Cat catPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;

    List<Cat> activeCats = new List<Cat>();
    public float spawnTimer;

    // Update is called once per frame
    void Update()
    {

        // Only update if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnCat();
            spawnTimer = 0f;
        }
    }

    void SpawnCat()
    {
        // find first empty spawn point
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // spawn here if there isn't already a cat
            if (spawnPoints[i].GetComponentInChildren<Cat>() == null)
            {
                Cat newCat = Instantiate(catPrefab, spawnPoints[i].position, Quaternion.identity, spawnPoints[i]);
                newCat.transform.localPosition = Vector3.zero;
                activeCats.Add(newCat);
                break; 
            }
        }
    }
}
