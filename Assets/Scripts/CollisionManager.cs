using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    
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
    
    PotentialCollision potCol = new PotentialCollision(null, null);
    public List<CollisionHull2D> allColliders = new List<CollisionHull2D>();
    public List<PotentialCollision> potentialCollisions = new List<PotentialCollision>();
    public float distanceCheckRadius;
    void Start()
    {
    
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

                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.CIRCLE)
                            if(allColliders[i].CircleCircleCollision(allColliders[j].GetComponentInParent<CircleHull>()))
                            {
                                //collision occured
                                allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                                allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                            }
                        
                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                            if (allColliders[i].CircleAABBCollision(allColliders[j].GetComponentInParent<AABBHull>()))
                            {
                                allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                                allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                            }
                        /*
                        if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if(allColliders[i].CircleOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {

                            }
                        */
                        if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                            if (allColliders[i].AABBAABBCollision(allColliders[j].GetComponentInParent<AABBHull>()))
                            {
                                allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                                allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                            }
                            /*
                        if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if (allColliders[i].AABBOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {

                            }
                            */
                        if (allColliders[i].hull == CollisionHull2D.hullType.OBB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            if (allColliders[i].OBBOBBCollision(allColliders[j].GetComponentInParent<OBBHull>()))
                            {
                                allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                                allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;
                            }
                            


                        //potCol = new PotentialCollision(allColliders[i], allColliders[j]);

                        //if (!potentialCollisions.)
                        //{
                        //   potentialCollisions.Add(potCol);
                        //} 

                    }
                }  
            }
        }
        /*
        for (int k = 0; k < potentialCollisions.Count; k++)
        {
           potentialCollisions[k].
        }
        */
        //Debug.Log(potentialCollisions.Count);
    }

    public void AddCollisionHull(CollisionHull2D hull)
    {
        allColliders.Add(hull);
    }
}
