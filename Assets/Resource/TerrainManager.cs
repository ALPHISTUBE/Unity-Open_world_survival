using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Setting")]
    public float noiseScale = 00.5f;
    public float noiseScale2 = 5f;
    public float heightMultiplier = 3f;
    public int seed = 0;

    [Header("Classes")]
    public TerrainTerraforming[] tt;
    public ObjectSpawner os;

    bool done = false;
    public int terrainCalculationDoneCount;

    public static TerrainManager instance;

    private void Awake()
    {
        instance = this;
        QualitySettings.vSyncCount = 0;

        if(seed == 0)
        {
            seed = Random.Range(-100000, 100000);
        }

        for (int i = 0; i < tt.Length; i++)
        {
            tt[i].noiseScale = noiseScale;
            tt[i].noiseScale2 = noiseScale2;
            tt[i].heightMultiplier = heightMultiplier;
            tt[i].seed = seed;
        }
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void StartSpawning()
    {
        if (terrainCalculationDoneCount == tt.Length)
        {
            QuadTree.Instance.BakeQuadTree();
            os.CastRayOnTerrain();
        }
    }
}
