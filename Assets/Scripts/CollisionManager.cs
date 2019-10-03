using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public struct potentialCollision
    {
        public potentialCollision(CollisionHull2D newHull1, CollisionHull2D newHull2)
        {
            hull1 = newHull1;
            hull2 = newHull2;
        }
        CollisionHull2D hull1;
        CollisionHull2D hull2;
    }

    public List<CollisionHull2D> allColliders;
    public List<potentialCollision> potentialCollisions;
    public float distanceCheckRadius;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < allColliders.Count; i ++)
        {
            for (int j = 0; j < allColliders.Count; j++)
            {
                if(allColliders[i] != allColliders[j])
                {
                    Vector3 range = allColliders[j].transform.position - allColliders[i].transform.position;
                    if (range.magnitude - distanceCheckRadius * 2 < 0)
                    {
                        potentialCollision potCol = new potentialCollision(allColliders[i], allColliders[j]);
                        if (potentialCollisions.Contains(potCol))
                        {
                            potentialCollisions.Add(potCol);
                        }
                        
                        //potentialCollisions.Add(allColliders[i]);
                        //potentialCollisions.Add(allColliders[j]);
                    }
                }
                
            }
        }
    }

    public void AddCollisionHull(CollisionHull2D hull)
    {
        allColliders.Add(hull);
    }
}
