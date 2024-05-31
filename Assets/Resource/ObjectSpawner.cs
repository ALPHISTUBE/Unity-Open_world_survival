using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public int rockAmount;
    public int rockSmallAmount;
    public int treeAmount;
    public int size;
    public float treeSpawnMinHeight = 12f;
    public float minSnowHeight = 12f;
    public Texture2D[] landscapeTexture;
    public GameObject[] rocks;
    public GameObject[] smallRocks;
    public GameObject[] trees;
    public LayerMask layerMask;

    public void CastRayOnTerrain()
    {
        Vector3 pos = transform.position;
        size /= 2;
        for (int i = 0; i < rockAmount; i++)
        {
            Transform t = QuadTree.Instance.GetPoint();
            RaycastHit hit = CalculateRaycast(t.position);
            if (hit.transform)
            {
                SpawnObjectOnPoint(rocks, hit.point, hit.normal, t);
            }
        }

        for (int i = 0; i < rockSmallAmount; i++)
        {
            Transform t = QuadTree.Instance.GetPoint();
            RaycastHit hit = CalculateRaycast(t.position);
            if (hit.transform)
            {
                SpawnObjectOnPoint(smallRocks, hit.point, hit.normal, t);
            }
        }

        for (int i = 0; i < treeAmount; i++)
        {
            Transform t = QuadTree.Instance.GetPoint();
            RaycastHit hit = CalculateRaycast(t.position);
            if (hit.transform)
            {
                SpawnObjectOnPoint(trees, hit.point, hit.normal, t);
            }
        }
    }

    RaycastHit CalculateRaycast(Vector3 pos)
    {
        int quadTreeSmalletChildRange = QuadTree.Instance.smallestChildRange/2;
        int spawnRangeX = Random.Range(-quadTreeSmalletChildRange, quadTreeSmalletChildRange);
        int spawnRangeY = Random.Range(-quadTreeSmalletChildRange, quadTreeSmalletChildRange);
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(new Vector3(spawnRangeX + pos.x, pos.y + 155, spawnRangeY + pos.z), -transform.up);
        
        if (Physics.Raycast(ray, out hit, 200, layerMask))
        {
            return hit;
        }
        else
        {
            return new RaycastHit();
        }
    }

    void SpawnObjectOnPoint(GameObject[] obj, Vector3 position, Vector3 normal, Transform parent)
    {
        if(position.y > treeSpawnMinHeight)
        {
            int objIndex = Random.Range(0, obj.Length);
            GameObject o = Instantiate(obj[objIndex], position, Quaternion.FromToRotation(Vector3.up, normal), parent);
            if(o.tag == "Rocks")
            {
                if(o.transform.position.y > minSnowHeight)
                {
                    Renderer[] m = o.GetComponentsInChildren<Renderer>();
                    foreach(Renderer mat in m)
                    {
                        mat.material.SetTexture("_Land", landscapeTexture[0]);
                    }
                }
            }
        }
    }
}
