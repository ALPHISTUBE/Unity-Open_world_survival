using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
    [Header("Settings")]
    public int size = 1536;
    public int subDivision = 3;
    public int maxChunkChild = 3;
    public Transform[] tarrains;
    public GameObject emt;

    [Header("ReadOnly")]
    public int calls;
    public int smallestChildRange;
    public Chunk rootChunk;
    private float positionOffset;
    Vector3[] points;

    public static QuadTree Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BakeQuadTree()
    {
        positionOffset = size / 6; //Origin Of Subdivided Chunk

        //Calculating Repeated Point range will {-(subdivision - 2) to +(subdivision - 2)}
        CalculateSubdivisionPoints();

        //Setting Root of all chunks
        rootChunk.self = transform;
        rootChunk.childChunk = new Chunk[subDivision * subDivision];

        //Accourding to Repeated point 
        SubdivideAccordingToPoints(positionOffset, true, transform, maxChunkChild - 1, rootChunk);

        calls++;
    }

    //Divide and distribute all Point of quadtree
    void SubdivideAccordingToPoints(float origin, bool doOffset, Transform parent, int subDivisionChild, Chunk parentChunk)
    {
        for (int i = 0; i < points.Length; i++)
        {
            //Spawn chunk empty
            GameObject g = Instantiate(emt, parent);

            //multiplying point with width and height, here width = height
            Vector3 _target = points[i] * origin * 2;
            if (doOffset)
            {
                //if it is root than apply the offset to make it center for chunk origin
                _target.x += origin;
                _target.z += origin;
            }


            //Apply position of chunk
            g.transform.localPosition = _target; //Must be apply to localposition
            string name = (doOffset ? $"Terrain({i})" : $"Child({subDivisionChild}),({i})");
            g.name = name + $"-{g.transform.position}";

            if (doOffset)
            {
                tarrains[i].transform.parent = g.transform;
            }

            parentChunk.childChunk[i].self = g.transform; //setting the spawn chunk as child of parent

            //if the chunk needs it own childs
            if (subDivisionChild > 0)
            {
                parentChunk.childChunk[i].childChunk = new Chunk[subDivision * subDivision];
                //orign should divide by 3 for childChunk and offset false
                SubdivideAccordingToPoints(origin/3, false, g.transform, subDivisionChild - 1, parentChunk.childChunk[i]);
            }

            calls++;
        }
    }

    //Calculate all repeated Point of quadtree
    void CalculateSubdivisionPoints()
    {
        points = new Vector3[subDivision*subDivision];
        int index = 0;
        int subdivisionOffset = subDivision - 2;
        for (int y = -subdivisionOffset; y <= subdivisionOffset; y++)
        {
            for (int x = -subdivisionOffset; x <= subdivisionOffset; x++)
            {
                points[index] = new Vector3(x, 0, y);
                index++;
            }
        }
    }

    //Get Random Quadtree Chunk point
    public Transform GetPoint()
    {
        return rootChunk.childChunk[UnityEngine.Random.Range(0, 9)].childChunk[UnityEngine.Random.Range(0, 9)].childChunk[UnityEngine.Random.Range(0, 9)].self;
    }

    //Chunk Information Holder
    [System.Serializable]
    public struct Chunk
    {
        public Transform self;
        public Chunk[] childChunk;
    }
}
