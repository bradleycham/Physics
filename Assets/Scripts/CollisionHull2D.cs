using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHull2D : MonoBehaviour
{
    private Vector2 min0, max0, min1, max1;
    public Vector2[] shape1Corners;
    public Vector2[] shape2Corners;


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
        Vector2 range = circleHull.transform.position - this.transform.position;
        range -= this.GetComponent<CircleHull>().offset;
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
        distance -= this.GetComponent<CircleHull>().offset;

        closestPoint = new Vector2(Mathf.Clamp(distance.x, -boxHull.halfX, boxHull.halfX), Mathf.Clamp(distance.y, -boxHull.halfY, boxHull.halfY));

        if ((distance - closestPoint).magnitude - this.GetComponent<CircleHull>().radius < 0)
            return true;
        else return false;
    }
    /*
    public bool CircleOBBCollision(OBBHull OBBHull)
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
    */
    public bool OBBOBBCollision(OBBHull boxHull)
    {
        shape1Corners = new Vector2[4];
        shape2Corners = new Vector2[4];
        Vector2[] normals = new Vector2[4];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        shape1Corners = getRotatedCorners(this.GetComponent<OBBHull>());
        shape2Corners = getRotatedCorners(boxHull);

        //normals[0] = getUpNormal(this.GetComponent<OBBHull>().currentRotation);
        //normals[1] = getRightNormal(this.GetComponent<OBBHull>().currentRotation);
        //normals[2] = getUpNormal(boxHull.currentRotation);
        //normals[3] = getRightNormal(boxHull.currentRotation);
        normals[0] = getUpNormal(-this.GetComponent<OBBHull>().currentRotation);
        normals[1] = getRightNormal(-this.GetComponent<OBBHull>().currentRotation);
        normals[2] = getUpNormal(-boxHull.currentRotation);
        normals[3] = getRightNormal(-boxHull.currentRotation);

        for (int i = 0; i < normals.Length; i ++ )
        {
            //Debug.Log("testing corner" + i);

            shape1MinMax = SatTest(normals[i], shape1Corners);
            shape2MinMax = SatTest(normals[i], shape2Corners);
            if (!Overlap(shape1MinMax[0], shape1MinMax[1], shape2MinMax[0], shape2MinMax[1]))
            {
                //Debug.Log("falure");

                return false;

            }
        }

        return true;
    }
    
    
    Vector2 getUpNormal(float theta)
    {
        float rad = theta * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), -Mathf.Sin(rad));
    }

    Vector2 getRightNormal(float theta)
    {
        float rad = theta * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
    }

    float[] SatTest(Vector2 axis, Vector2[] points)
    {
        float[] minMax = new float[2];
        float minAlong = 1000000; float maxAlong = -1000000;
        for(int i = 0; i < points.Length; i++)
        {
            float dotValue = Vector2.Dot(points[i], axis);
            if (dotValue < minAlong) minAlong = dotValue;
            if (dotValue > maxAlong) maxAlong = dotValue;
        }
        minMax[0] = minAlong;
        minMax[1] = maxAlong;
        //Debug.Log(minMax[0] + " " + minMax[1]);
        return minMax;
    }

    Vector2[] getRotatedCorners(OBBHull newHull)
    {
        Vector2[] returnPoints = new Vector2[4];
        returnPoints[0] = getRotatedPoint(new Vector2(newHull.transform.position.x - newHull.halfX, newHull.transform.position.y - newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[1] = getRotatedPoint(new Vector2(newHull.transform.position.x - newHull.halfX, newHull.transform.position.y + newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[2] = getRotatedPoint(new Vector2(newHull.transform.position.x + newHull.halfX, newHull.transform.position.y - newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[3] = getRotatedPoint(new Vector2(newHull.transform.position.x + newHull.halfX, newHull.transform.position.y + newHull.halfY), newHull.transform.position, newHull.currentRotation);

        return returnPoints;
    }

    Vector2 getRotatedPoint(Vector2 cornerPos, Vector2 centerPos, float theta)
    {
        float rad = theta * Mathf.Deg2Rad;

        float xPos = cornerPos.x - centerPos.x;
        float yPos = cornerPos.y - centerPos.y;
        float xRot = (xPos * Mathf.Cos(rad)) - (yPos * Mathf.Sin(rad));
        float yRot = (xPos * Mathf.Sin(rad)) + (yPos * Mathf.Cos(rad));
     
        Vector2 returnVector = new Vector2(xRot, yRot);

        returnVector += centerPos;
       
        return returnVector;
    }

    bool Overlap(float min1, float max1, float min2, float max2)
    {
        return IsBetweenOrdered(min2, min1, max1) || IsBetweenOrdered(min1, min2, max2);
    }
    bool IsBetweenOrdered(float val, float lowerBound, float upperBound)
    {
        return lowerBound <= val && val <= upperBound;
    }
}
