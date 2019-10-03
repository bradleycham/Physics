using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBHull : CollisionHull2D
{
    // Start is called before the first frame update
    void Start()
    {
        hull = hullType.OBB;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
