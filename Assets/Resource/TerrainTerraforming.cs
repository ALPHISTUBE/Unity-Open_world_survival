using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using Tayx.Graphy.Utils.NumString;

public class TerrainTerraforming : MonoBehaviour
{
    [Header("Settings")]
    public bool on = true;
    public Transform debuggingObject;

    [Header("Terrian Data")]
    private float[,,] alphaMaps;
    private float[,] heightMap;
    private int[,] detailMap;
    private TerrainData tData;

    private const int path = 0; //the textures are loaded onto the terrain
    private const int grass = 1; //These numbers depend on the order in which
    private const int snow = 2; //the textures are loaded onto the terrain

    [Header("Variable")]
    public float noiseScale = 00.5f;
    public float noiseScale2 = 5f;
    public float heightMultiplier = 3f;
    public int seed = 0;

    [Header("Output")]
    public RawImage noiseTexture;


    //Private
    private int heightHM;
    private int widthHM;
    private int heightAM;
    private int widthAM;
    private int resulationDM;
    CalculateHeights calculateHeights;
    JobHandle handle;
    NativeArray<float> height;

    void Start()
    {
        // Set up the texture and a Color array to hold pixels during processing.
        tData = GetComponent<Terrain>().terrainData;

        heightHM = tData.heightmapResolution;
        widthHM = tData.heightmapResolution;
        heightAM = tData.alphamapHeight;
        widthAM = tData.alphamapWidth;
        resulationDM = tData.detailResolution;

        heightMap = tData.GetHeights(0, 0, widthHM, heightHM);
        alphaMaps = tData.GetAlphamaps(0, 0, widthAM, heightAM);

        if(seed == 0)
        {
            seed = UnityEngine.Random.Range(-100000, 100000);
        }

        UpdateTerrain();
        on = true;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //UpdateTerrain();
        }
    }

    private void UpdateTerrain()
    {
        CalculateTerrianHeight();
        OnTerrainHeightCalculateJob();
        SetTerrainHeight();
        //UpdateNoiseTexture();
        SetColorMap();
        //SetGrassCullingMap();
    }


    public void CalculateTerrianHeight()
    {
        height = new NativeArray<float>(heightHM * widthHM, Allocator.TempJob);

        calculateHeights = new CalculateHeights
        {
            _height = height,
            _size = heightHM,
            _seed = seed,
            _offsetX = transform.position.x,
            _offsetY = transform.position.z,
            _heightMultiplier = heightMultiplier,
            _scale = noiseScale,
            _scale2 = noiseScale2
        };

        handle = calculateHeights.Schedule();
    }

    public void UpdateNoiseTexture()
    {
        Color[] pixels = new Color[heightHM * widthHM];
        for (int y = 0; y < heightHM; y++)
        {
            for (int x = 0; x < widthHM; x++)
            {
                pixels[x + widthHM * y] = Color.Lerp(Color.black, Color.white, heightMap[y, x]);
            }
        }

        Texture2D texture = new Texture2D(widthHM, heightHM);
        texture.SetPixels(pixels);
        texture.Apply();
        noiseTexture.texture = texture;
    }


    public void OnTerrainHeightCalculateJob()
    {
        handle.Complete();


        for (int y = 0; y < heightHM; y++)
        {
            for (int x = 0; x < widthHM; x++)
            {
                heightMap[y, x] = height[x + widthHM * y];
            }
        }

        height.Dispose();
    }

    public void SetTerrainHeight()
    {
        tData.SetHeights(0, 0, heightMap);
    }

    [BurstCompile]
    struct CalculateHeights : IJob
    {
        [WriteOnly]public NativeArray<float> _height;
        [ReadOnly]public int _size;
        [ReadOnly]public int _seed;
        [ReadOnly]public float _offsetX;
        [ReadOnly]public float _offsetY;
        [ReadOnly]public float _heightMultiplier;
        [ReadOnly]public float _scale;
        [ReadOnly]public float _scale2;
        public void Execute()
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    float h = 0;
                    float posX = 0;
                    float posY = 0;

                    posX = ((x.ToFloat() + _offsetX + _seed.ToFloat()) / _size.ToFloat()) * 300f;
                    posY = ((y.ToFloat() + _offsetY + _seed.ToFloat()) / _size.ToFloat()) * 300f;
                    //h = noise.cnoise(new float2(posX, posY)) * .05f;
                    h = Mathf.PerlinNoise(posX, posY) * .05f;
                    
                    posX = ((x.ToFloat() + _offsetX + _seed.ToFloat() + 5000f) / _size.ToFloat()) * _scale;
                    posY = ((y.ToFloat() + _offsetY + _seed.ToFloat() + 5000f) / _size.ToFloat()) * _scale;
                    h += Mathf.PerlinNoise(posX, posY);

                    
                    posX = ((x.ToFloat() + _offsetX + _seed.ToFloat() + 2000f) / _size.ToFloat()) * _scale2;
                    posY = ((y.ToFloat() + _offsetY + _seed.ToFloat() + 2000f) / _size.ToFloat()) * _scale2;
                    h += Mathf.PerlinNoise(posX, posY);
                    h /= 3;
                    _height[x + _size * y] = h * _heightMultiplier;
                }
            }
        }
    }

    
    public float CalculatePerlin(float x, float y, float scale, float heightMultiplier, float offsetX, float offsetY, int size)
    {
        float posX = (((float)x + offsetX) / (float)size) * scale;
        float posY = (((float)y + offsetY) / (float)size) * scale;
        return Mathf.PerlinNoise(posX, posY) * heightMultiplier;
    }


    //Terrain color map
    public void SetColorMap()
    {
        for (int y = 0; y < heightAM; y++)
        {
            for (int x = 0; x < widthAM; x++)
            {
                float a0 = alphaMaps[x, y, path];
                float a1 = alphaMaps[x, y, grass];
                float a2 = alphaMaps[x, y, snow];

                float height = Mathf.InverseLerp(0, 35, tData.GetHeight(x,y));
                if (height <= 0.3f)
                {
                    a0 = 0.9f;
                    a1 = 0.05f;
                    a2 = 0.05f;
                }
                else if (height > 0.3f && height < 0.5f)
                {
                    a0 = 0.05f;
                    a1 = 0.9f;
                    a2 = 0.05f;
                }
                else if (height > 0.5f)
                {
                    a0 = 0.05f;
                    a1 = 0.05f;
                    a2 = 0.9f;
                }

                float t = a0 + a1 + a2;

                alphaMaps[y, x, path] = a0;
                alphaMaps[y, x, grass] = a1;
                alphaMaps[y, x, snow] = a2;
            }
        }
        tData.SetAlphamaps(0, 0, alphaMaps);

        on = false;

        TerrainManager.instance.terrainCalculationDoneCount++;
        TerrainManager.instance.StartSpawning();
    }

    //Terrain grass alpha map
    public void SetGrassCullingMap()
    {
        Vector3 rectCenter = debuggingObject.transform.position;
        float rectSizeX = debuggingObject.transform.localScale.x;
        float rectSizeY = debuggingObject.transform.localScale.z;
        int Tw = tData.heightmapResolution;
        int Th = tData.heightmapResolution;
        detailMap = tData.GetDetailLayer(0, 0, Tw, Th, 0);

        // Determine the world coordinates
        float scalex = tData.heightmapResolution / tData.size.x;
        float scaley = tData.heightmapResolution / tData.size.z;
        Vector2 rectCenterMap = new Vector2((int)(rectCenter.x * scalex), (int)(rectCenter.z * scaley));

        int rectSizeXMap = (int)((rectSizeX * scalex) / 2f);
        int rectSizeYMap = (int)((rectSizeY * scaley) / 2f);

        for (int Ty = 0; Ty < Th; Ty++)
        {
            for (int Tx = 0; Tx < Tw; Tx++)
            {
                PunchOutRectangle(Ty, Tx, rectCenterMap, rectSizeXMap, rectSizeYMap);
            }
        }

        tData.SetDetailLayer(0, 0, 0, detailMap);

        on = false;
    }

    public void PunchOutRectangle(int xrow, int yrow, Vector2 rectCenterMap, int rectSizeXMap, int rectSizeYMap)
    {
        if ((xrow <= (rectCenterMap.x + rectSizeXMap)) && (xrow >= (rectCenterMap.x - rectSizeXMap)) &&
           (yrow <= (rectCenterMap.y + rectSizeYMap)) && (yrow >= (rectCenterMap.y - rectSizeYMap)))
        {
            // Raise the terrain to center of the cube
            detailMap[yrow, xrow] = 0;
        }
    }
}
