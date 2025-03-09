using UnityEngine;
public class Level_Generated : MonoBehaviour
{
    public Vector2 size;
    public Vector2 offset;
    public GameObject[] BrickPrefabs;

    private void Awake()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                int randomIndex = Random.Range(0, BrickPrefabs.Length);
                GameObject randomPrefab = BrickPrefabs[randomIndex];

                GameObject newBrick = Instantiate(randomPrefab, transform);
                newBrick.transform.position = transform.position + new Vector3((i - (size.x - 1) * 0.5f) * offset.x, j * offset.y, 0);
            }
        }
    }
}