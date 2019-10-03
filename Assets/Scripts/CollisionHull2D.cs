using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHull2D : MonoBehaviour
{
    /*
    public struct HullCollision
    {
        bool status;
        Vector2 overlap;
    }
    */
    private bool colliding;
    public enum hullType
    {
        AABB,
        OBB,
        CIRCLE
    }

    public hullType hull;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool/*HullCollision*/ CircleCircleCollision(CircleHull circleHull)
    {
        Vector3 range = circleHull.transform.position - this.transform.position;
        float overlap = range.magnitude - circleHull.radius * 2;
        //HullCollision returnCollision;
        if (overlap > 0)
        {
            colliding = false;
            return false;
        }
        else
        {
            colliding = true;
            return true;
        }
    }
}
