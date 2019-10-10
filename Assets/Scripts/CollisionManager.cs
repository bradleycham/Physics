using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    /*
    public class PotentialCollision
    {
        CollisionHull2D hull1;
        CollisionHull2D hull2;
        public PotentialCollision(CollisionHull2D newHull1, CollisionHull2D newHull2)
        {
            hull1 = newHull1;
            hull2 = newHull2;
        }
    }
    */
    //PotentialCollision potCol = new PotentialCollision(null, null);
    public List<CollisionHull2D> allColliders = new List<CollisionHull2D>();
    public List<CollisionHull2D.HullCollision> Collisions = new List<CollisionHull2D.HullCollision>();
    public List<string> currentCollisions;
    public float distanceCheckRadius = 2;
    bool collisionHappened;
    void Start()
    {
        collisionHappened = false;
    }

// Update is called once per frame
void Update()
    {
        for (int i = 0; i < allColliders.Count; i ++)
        {
            allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.red;

            for (int j = 0; j < allColliders.Count; j++)
            {
                if (i != j && allColliders[i].gameObject != allColliders[j].gameObject)
                {
                    Vector3 range = allColliders[j].transform.position - allColliders[i].transform.position;
                    if (range.magnitude - (distanceCheckRadius * 2) < 0)
                    {
                        // alternatively you could just add the collisions to a list and operate on them in another loop
                        CollisionHull2D.HullCollision newCollision = new CollisionHull2D.HullCollision();

                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.CIRCLE)
                        {
                            newCollision = CollisionHull2D.CircleCircleCollision(allColliders[i].GetComponent<CircleHull>(), allColliders[j].GetComponent<CircleHull>());
                            collisionHappened = newCollision.status;
                            //if (newCollision.status)
                                //CollisionHull2D.ResolveCollision(newCollision);

                        }

                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                            if (allColliders[i].CircleAABBCollision(allColliders[j].GetComponentInParent<AABBHull>()))
                            {
                                collisionHappened = true;
                            }
                        /*
                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if(allColliders[i].CircleOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {

                            }
                        */
                        if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                        {
                            newCollision = CollisionHull2D.AABBAABBCollision(allColliders[i].GetComponent<AABBHull>(), allColliders[j].GetComponent<AABBHull>());
                            collisionHappened = newCollision.status;
                            //Debug.Log(newCollision.status);
                            //if(newCollision.status)
                                //CollisionHull2D.ResolveCollision(newCollision);

                        }

                        if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if (allColliders[i].AABBOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {
                                collisionHappened = true;
                            }
                            
                        if (allColliders[i].hull == CollisionHull2D.hullType.OBB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if (allColliders[i].OBBOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {
                                collisionHappened = true;
                            }


                        if (collisionHappened)
                        {
                            if(newCollision.status)
                            {
                                //currentCollisions.Add();
                                bool duplicate = false;
                                for(int h = 0; h < Collisions.Count; h++)
                                {
                                    if ((newCollision.a == Collisions[h].a || newCollision.a == Collisions[h].b) && (newCollision.b == Collisions[h].a || newCollision.b == Collisions[h].b))
                                    {
                                        Debug.Log("duplicate");
                                        duplicate = true;
                                    }
                                }
                                if(!duplicate)
                                    Collisions.Add(newCollision);
                            }
                            allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                            allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                        }
                        collisionHappened = false;
                    }
                }  
            }
        }
        
        for (int k = 0; k < Collisions.Count; k++)
        {
            CollisionHull2D.ResolveCollision(Collisions[k]);
        }
        
        Collisions.Clear();
    }

    public void AddCollisionHull(CollisionHull2D hull)
    {
        allColliders.Add(hull);
    }
}
