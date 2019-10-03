using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHull2D : MonoBehaviour
{
    private Vector2 min0, max0, min1, max1;
    
    public class HullCollision
    {
        public struct Contact
        {
            Vector2 point;
            Vector2 normal;
            float restitution;
        }

        public Vector2 closingVelocity;
        public Vector2 penetration;
        public CollisionHull2D a;
        public CollisionHull2D b;
        public Contact[] contacts = new Contact[4];
        bool status;
    }
    
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
        colliding = false;
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

    public bool CircleAABBCollision(AABBHull boxHull)
    {
        Vector2 closestPoint = new Vector2(0.0f, 0.0f);
        Vector2 distance = boxHull.gameObject.transform.position - this.gameObject.transform.position;
        distance += this.GetComponent<CircleHull>().offset;

        closestPoint = new Vector2(Mathf.Clamp(distance.x, -boxHull.halfX, boxHull.halfX), Mathf.Clamp(distance.y, -boxHull.halfY, boxHull.halfY));

        if ((distance - closestPoint).magnitude - this.GetComponent<CircleHull>().radius < 0)
            return true;
        else return false;
    }
    /*
    public bool CircleOBBCollision(OBBHull bBHull)
    {

    }
    */
    public bool AABBAABBCollision(AABBHull boxHull)
    {
        AABBHull thisHull = this.GetComponent<AABBHull>();
        min0 = thisHull.transform.position - new Vector3(thisHull.halfX, thisHull.halfY) + thisHull.offset;
        max0 = thisHull.transform.position + new Vector3(thisHull.halfX, thisHull.halfY) + thisHull.offset;
        min1 = boxHull.transform.position - new Vector3(thisHull.halfX, thisHull.halfY) + boxHull.offset;
        max1 = boxHull.transform.position + new Vector3(thisHull.halfX, thisHull.halfY) + boxHull.offset;

        if (max0.x >= min1.x && max1.x >= min0.x)
            if (max0.y >= min1.y && max1.y >= min0.y)
                return true;
        return false;
    }
    /*
    public bool AABBOBBCollision(OBBHull boxHull)
    {

    }

    public bool OBBOBBCollision(OBBHull boxHull)
    {

    }
    */
}
