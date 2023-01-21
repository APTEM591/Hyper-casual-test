using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private Food[] foodPrefabs;

    [Tooltip("Delay between every spawned food (in seconds)")]
    [SerializeField] private float spawnFrequency = 1;

    private float timer = 0;
    
    /// <summary>
    /// Bound of belt mesh
    /// </summary>
    private Bounds meshBounds;

    private void Awake()
    {
        if(foodPrefabs.Length == 0)
            throw new System.NullReferenceException("No food found in array. Add at least 1 object.");

        meshBounds = GetComponent<MeshRenderer>().bounds;
    }

    private void Update()
    {
        if (timer <= 0)
        {
            SpawnRandomFood();
            timer = spawnFrequency;
        }
        else timer -= Time.deltaTime;

    }

    private void SpawnRandomFood()
    {
        int foodIndex = Random.Range(0, foodPrefabs.Length);
        
        var food = Instantiate(foodPrefabs[foodIndex]);
        food.transform.SetParent(transform);
        food.transform.localPosition = CalculateStartPosition(food.transform);
    }

    /// <summary>
    /// Calculate food spawn position related to belt size
    /// </summary>
    /// <returns>Position where to spawn food</returns>
    private Vector3 CalculateStartPosition(Transform objectToSpawn)
    {
        Vector3 startPosition;
        
        startPosition.x = meshBounds.extents.x;
        startPosition.y = objectToSpawn.GetComponent<MeshRenderer>().bounds.extents.y;
        startPosition.z = 0;

        return startPosition;
    }
}
