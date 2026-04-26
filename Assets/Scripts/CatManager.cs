using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public Cat catPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 10f;
    public Transform exitPoint; // where cats go when manager shoos them away
    public float shooSpeed = 5f;

    List<Cat> activeCats = new List<Cat>();
    public float spawnTimer;

    // Update is called once per frame
    void Update()
    {

        // Only update if the day has begun
        if (!GameManager.Instance.dayManager.isDayActive) return;

        if (activeCats.Count >= spawnPoints.Length) return; // no more space

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

    public void RemoveCat(Cat cat)
    {
        activeCats.Remove(cat);
        Destroy(cat.gameObject);
    }

    public void ShooCats()
    {
        Debug.Log("Shooing cats away!");
        for(int i = activeCats.Count - 1; i >= 0; i--)
        {
            Cat cat = activeCats[i];
            cat.ClearNeeds();

            // move cat to exit point and then remove it
            cat.transform.position = Vector3.MoveTowards(cat.transform.position, exitPoint.position, shooSpeed * Time.deltaTime);
            Vector3 dir = (exitPoint.position - cat.transform.position).normalized;
            if (dir.sqrMagnitude > 0f)
            {
                cat.transform.rotation = Quaternion.LookRotation(dir);
            }
            if (Vector3.Distance(cat.transform.position, exitPoint.position) < 0.1f)
            {
                RemoveCat(cat);
            }
        }
    }
}
