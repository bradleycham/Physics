using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHull2D : MonoBehaviour
{
    
    public Vector2[] shape1Corners;
    public Vector2[] shape2Corners;
    public float restitution;

    public class HullCollision
    {
        public struct Contact
        {
            public Vector2 point;
            public Vector2 normal;
            public float restitution;
        }

        public Vector2 closingVelocity;
        public Vector2 penetration;
        public CollisionHull2D a;
        public CollisionHull2D b;
        public Contact[] contacts = new Contact[4];
        public bool status;
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

    public static HullCollision CircleCircleCollision(CircleHull circleHull1, CircleHull circleHull2)
    {
        Vector3 c1Offset = circleHull1.offset;
        Vector3 c2Offset = circleHull2.offset;

        Vector2 range = (circleHull2.transform.position + c2Offset) - (circleHull1.transform.position + c1Offset); // make sure offsets arent screwing things up
        float overlap = (circleHull2.radius + circleHull1.radius) - range.magnitude;

        HullCollision col = new HullCollision();
        col.a = circleHull1;
        col.b = circleHull2;
        col.penetration = range * overlap;

        HullCollision.Contact con0 = new HullCollision.Contact();
        con0.point = range.normalized * circleHull1.radius;
        con0.normal = range.normalized;
        con0.restitution = Mathf.Min(circleHull1.restitution, circleHull2.restitution);

        col.contacts[0] = con0;

        Particle2D c1 = circleHull1.GetComponentInParent<Particle2D>();
        Particle2D c2 = circleHull2.GetComponentInParent<Particle2D>();

        //this section may need tweaking
        /*
        Vector3 c1Closing = new Vector3(Mathf.Clamp(c1.velocity.x, 0, range.normalized.x), Mathf.Clamp(c1.velocity.y, 0, range.normalized.y));
        Vector3 c2Closing = new Vector3(Mathf.Clamp(c2.velocity.x, 0, -range.normalized.x), Mathf.Clamp(c2.velocity.y, 0, -range.normalized.y));
        Vector3 closingVel = c1Closing - c2Closing;
        col.closingVelocity = closingVel;
        */
        Vector3 closingVel = c2.velocity - c1.velocity; // started as c1 -c2
        col.closingVelocity = closingVel;

        if (overlap > 0)
        {
            col.status = true;
            //ResolveCollision(col);
            return col;
        }
        else
        {
            col.status = false;
            return col;
        }
    }

    public static void ResolveCollision(HullCollision col)
    {
        Particle2D A = col.a.GetComponent<Particle2D>();
        Particle2D B = col.b.GetComponent<Particle2D>();
        float invAMass;
        float invBMass;
        if (A.mass == 0) invAMass = 0;
        else invAMass = 1 / A.mass;
        if (B.mass == 0) invBMass = 0;
        else invBMass = 1 / B.mass;


        float velAlongNormal = Vector3.Dot(col.closingVelocity, col.contacts[0].normal);
        Debug.Log("velAlongNormal " + velAlongNormal);

        if (velAlongNormal > 0) return; // > makes square work properly
        //Debug.Log(velAlongNormal);
        // restitustion
        float e = col.contacts[0].restitution;
        // impulse scalar
        float j = -(1 + e) * velAlongNormal;
        j /= invAMass + invBMass;
        //Debug.Log(j);

        Vector2 impulse = j * col.contacts[0].normal;
        Debug.Log(impulse);

        //A.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //B.velocity = new Vector3(0.0f, 0.0f, 0.0f);

        A.velocity += invAMass * impulse;
        B.velocity -= invBMass * impulse;

        // Positional Correction
        
        float percent = 0.2f;
        float slop = 0.01f;
        Vector2 correction = Mathf.Max(velAlongNormal - slop, 0) / (invAMass + invBMass) * percent * col.contacts[0].normal;
        A.position -= invAMass * correction;
        B.position += invBMass * correction;
        
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
    public static HullCollision AABBAABBCollision(AABBHull boxHull1, AABBHull boxHull2)
    {
        Vector2 min0, max0, min1, max1;
        Vector3 b1Offset = boxHull1.offset;
        Vector3 b2Offset = boxHull2.offset;
        Particle2D A = boxHull1.GetComponent<Particle2D>();
        Particle2D B = boxHull2.GetComponent<Particle2D>();

        min0 = boxHull1.transform.position - new Vector3(boxHull1.halfX, boxHull1.halfY) + b1Offset;
        max0 = boxHull1.transform.position + new Vector3(boxHull1.halfX, boxHull1.halfY) + b1Offset;
        min1 = boxHull2.transform.position - new Vector3(boxHull2.halfX, boxHull2.halfY) + b2Offset;
        max1 = boxHull2.transform.position + new Vector3(boxHull2.halfX, boxHull2.halfY) + b2Offset;

        Vector2 range = (boxHull2.transform.position + b2Offset) - (boxHull1.transform.position + b1Offset); // make sure offsets arent screwing things up

        HullCollision col = new HullCollision();
        col.a = boxHull1;
        col.b = boxHull2;

        float xOverlap = boxHull1.halfX + boxHull2.halfX - Mathf.Abs(range.x);
        float yOverlap = boxHull1.halfY + boxHull2.halfY - Mathf.Abs(range.y);

        //TRANSPORTATION
        // TRANSPORTATION
        

        if (max0.x >= min1.x && max1.x >= min0.x)
            if (max0.y >= min1.y && max1.y >= min0.y)
            {
                col.status = true;
                Vector2 collisionNormal = new Vector2();
                if (Mathf.Abs(xOverlap) > Mathf.Abs(yOverlap))//added mathf
                    collisionNormal = boxHull1.getRightNormal(180) * (range.x / Mathf.Abs(range.x));
                if (Mathf.Abs(yOverlap) > Mathf.Abs(xOverlap))//added mathf
                    collisionNormal = boxHull1.getUpNormal(180) * (range.y / Mathf.Abs(range.y));
                Debug.Log(collisionNormal);
                col.penetration = new Vector2(xOverlap, yOverlap);

                Vector3 closingVel = A.velocity - B.velocity;
                //Vector3 closingVel = B.velocity - A.velocity;
                col.closingVelocity = closingVel;

                HullCollision.Contact con0 = new HullCollision.Contact();
                con0.point = new Vector2(Mathf.Clamp(range.x, -boxHull1.halfX, boxHull1.halfX), Mathf.Clamp(range.y, -boxHull1.halfY, boxHull1.halfY));
                con0.normal = collisionNormal;
                con0.restitution = Mathf.Min(boxHull1.restitution, boxHull2.restitution);
                col.contacts[0] = con0;
                //ResolveCollision(col);
            }
            else col.status = false;
        //ResolveCollision(col);
        return col;
        // end of or      
    }
    
    public bool AABBOBBCollision(OBBHull boxHull)
    {
        shape1Corners = new Vector2[4];
        shape2Corners = new Vector2[4];
        Vector2[] normals = new Vector2[4];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        AABBHull staticHull = this.GetComponent<AABBHull>();

        shape1Corners[0] = staticHull.transform.position + new Vector3(-staticHull.halfX, -staticHull.halfY);
        shape1Corners[1] = staticHull.transform.position + new Vector3(-staticHull.halfX, staticHull.halfY);
        shape1Corners[2] = staticHull.transform.position + new Vector3(staticHull.halfX, -staticHull.halfY);
        shape1Corners[3] = staticHull.transform.position + new Vector3(staticHull.halfX, staticHull.halfY);
        shape2Corners = getRotatedCorners(boxHull);

        normals[0] = getUpNormal(-staticHull.transform.eulerAngles.z);
        normals[1] = getRightNormal(-staticHull.transform.eulerAngles.z);
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
    
    public bool OBBOBBCollision(OBBHull boxHull)
    {
        shape1Corners = new Vector2[4];
        shape2Corners = new Vector2[4];
        Vector2[] normals = new Vector2[4];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        shape1Corners = getRotatedCorners(this.GetComponent<OBBHull>());
        shape2Corners = getRotatedCorners(boxHull);

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
