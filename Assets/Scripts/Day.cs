using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Day", menuName = "Scriptable Objects/Day")]
public class Day : ScriptableObject
{
    [Tooltip("Display name for Day")]
    public string dayName;

    [Tooltip("List of all possible recipes that can be ordered")]
    public Recipe[] recipePool;

    [Header("Order Settings")]

    [Tooltip("Number of orders to generate at the start of the day")]
    public int initialOrderCount = 1; // TODO: Oops this isn't working lol

    [Tooltip("Time interval (in seconds) between spawning new orders")]
    public float orderSpawnInterval = 10f;

    [Tooltip("Maximum number of active orders at a time")]
    public int maxActiveOrders = 3;

    [Tooltip("How much patience the manager has for missed orders or cats on the counter before the day ends in failure")]
    public float patienceMeterStartAmount = 30f; // TODO

    [Header("Objectives")]
    [Tooltip("Objectives to complete for this day")]
    public Objective[] objectives;
}