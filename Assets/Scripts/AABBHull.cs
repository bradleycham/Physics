using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBHull : CollisionHull2D
{
    // Start is called before the first frame update
    public Vector3 offset;
    public float halfX;
    public float halfY;
    public float startingRotation;

    private void Start()
    {
        hull = hullType.AABB;
    }
    private void Update()
    {
        
    }
    Vector2 getMinCorner()
    {
        return this.transform.position + (offset - new Vector3(halfX, halfY));
    }

    Vector2 getMaxCorner()
    {
        return this.transform.position + (offset + new Vector3(halfX, halfY));
    }
}
