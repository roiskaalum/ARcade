using UnityEngine;
using System.Collections.Generic;

public class PhysicsManager : MonoBehaviour
{
    public static PhysicsManager Instance { get; private set; }

    [SerializeField] private List<Rigidbody> cans; // Tildel dine dåser i Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int CheckCansHit()
    {
        int hitCount = 0;

        foreach (var can in cans)
        {
            if (can != null && can.transform.position.y < 0.2f) // fx. væltet
            {
                hitCount++;
            }
        }

        return hitCount;
    }

    public bool AllCansKnocked()
    {
        foreach (var can in cans)
        {
            if (can != null && can.transform.up.y > 0.5f) // stadig nogenlunde oprejst
            {
                return false;
            }
        }

        return true;
    }
}
