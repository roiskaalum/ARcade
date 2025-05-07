using UnityEngine;

public class CanStacker : MonoBehaviour
{
    Vector3 basePosition = new Vector3(0f, -1f, 24f);

    public GameObject canPrefab;
    public int layers = 5;

    [SerializeField] private float horizontalSpacing = 1.2f;
    [SerializeField] private float canHeight = 1.5f;

    private void Start()
    {
        if (canPrefab != null)
        {
            GameObject temp = Instantiate(canPrefab);
            Renderer rend = temp.GetComponent<Renderer>();
            if (rend != null)
            {
                canHeight = rend.bounds.size.y;
            }
            Destroy(temp);
        }

        BuildStack();
    }

    public enum StackType
    {
        Pyramid,
        Wall,
        Tower
    }

    public StackType stackType = StackType.Pyramid;

    public void BuildStack()
    {
        switch (stackType)
        {
            case StackType.Pyramid:
                BuildPyramid();
                break;
            case StackType.Wall:
                BuildWall();
                break;
            case StackType.Tower:
                BuildTower();
                break;
        }
    }

    private void BuildPyramid()
    {
        for (int y = 0; y < layers; y++)
        {
            int cansInLayer = layers - y;
            float startX = -cansInLayer * horizontalSpacing * 0.5f + horizontalSpacing * 0.5f;

            float verticalOffset = 0.0f;

            for (int x = 0; x < cansInLayer; x++)
            {
                Vector3 position = basePosition + new Vector3(startX + x * horizontalSpacing, y * (canHeight + verticalOffset), 0f);
                //rotation - så den står ligesom ved Patricks
                Quaternion rotation = Quaternion.Euler(0f, 45f, 0f); 
                Instantiate(canPrefab, position, rotation, transform);
            }
        }
    }

    private void BuildWall()
    {
        int width = 5;
        int height = 5;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * horizontalSpacing, y * canHeight, 0f);
                Instantiate(canPrefab, position, Quaternion.identity, transform);
            }
        }
    }

    private void BuildTower()
    {
        for (int y = 0; y < layers; y++)
        {
            Vector3 position = new Vector3(0f, y * canHeight, 0f);
            Instantiate(canPrefab, position, Quaternion.identity, transform);
        }
    }

    public void ResetStack()
    {
        // Fjern alle eksisterende dåser (children)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Byg en ny stack
        BuildStack();
    }
}
