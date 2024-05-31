using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAddjustment : MonoBehaviour
{

    public Renderer[] treeMesh;
    public Renderer imposterMesh;
    public ParticleSystem fallingLeavesPS;
    public string leavesColorTag = "_LeafColor";
    public Color[] leafColor;

    // Start is called before the first frame update
    void Start()
    {
        Color c = leafColor[Random.Range(0, leafColor.Length)];
        foreach (Renderer r in treeMesh)
        {
            r.material.SetColor(leavesColorTag, c);
        }

        imposterMesh.material.SetColor(leavesColorTag, c);
        fallingLeavesPS.startColor = c;
        fallingLeavesPS.Play();
    }
}
