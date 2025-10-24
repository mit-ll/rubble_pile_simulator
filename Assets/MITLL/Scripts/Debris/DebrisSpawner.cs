// DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
//  
// This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
//  
// © 2024 Massachusetts Institute of Technology.
// Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
//  
// The software/firmware is provided to you on an As-Is basis
//  
// Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    private RandomManager random;
    public GameManager gameManager;
    public WeightedItemCollectionSO debrisCollection;
    public WeightedItemCollectionSO smallDebrisCollection;
    public GameObject SpawnVolume;
    public int numToSpawn;
    public float spawnDelay;
    public int numPiles;
    private Bounds spawnBounds;
    private List<GameObject> objList = new List<GameObject>();
    private CustomArgs customArgs;
    private bool exportSTL;
    
    
    // Start is called before the first frame update
    void Start()
    {
        customArgs = CustomArgs.Instance;

        numToSpawn = (int)CustomArgs.GetWithDefault("numobjs", 300);
        numPiles = (int)CustomArgs.GetWithDefault("numlayers", 3);

        exportSTL = CustomArgs.FloatToBool(CustomArgs.GetWithDefault("exportstl", 0));
        
        Vector3 SPAWN_START_POS = new Vector3(
            CustomArgs.GetWithDefault("spawnposx", 0),
            CustomArgs.GetWithDefault("spawnposy", 15),
            CustomArgs.GetWithDefault("spawnposz", 0)
        );
            
            
        Vector3 SPAWN_BOUNDS_SIZE =  new Vector3(
            CustomArgs.GetWithDefault("spawnboundx", 10),
            CustomArgs.GetWithDefault("spawnboundy", 10),
            CustomArgs.GetWithDefault("spawnboundz", 10)
            );

        random = RandomManager.Instance;
        if (SpawnVolume == null)
        {
            SpawnVolume = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SpawnVolume.name = "SpawnVolume";
            SpawnVolume.transform.position = SPAWN_START_POS;

            var meshbounds = SpawnVolume.GetComponent<MeshRenderer>().bounds;
            meshbounds.size = SPAWN_BOUNDS_SIZE;
            spawnBounds = meshbounds;
            Destroy(SpawnVolume.GetComponent<BoxCollider>());
            SpawnVolume.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    private void OnEnable()
    {
        GameManager.doReset += Reset;
    }

    private void OnDisable()
    {
        GameManager.doReset -= Reset;
    }

    
    public void Reset()
    {
        StopAllCoroutines();
        if (objList.Count > 0)
        {
            foreach (GameObject go in objList)
            {
                GameObject.Destroy(go.gameObject);
            }
            objList.Clear();
        }

        StartCoroutine(SetUpScene());


    }
    IEnumerator SetUpScene()
    {
        Time.timeScale = 10f;
        GeneratePile(smallDebrisCollection);
        yield return new WaitForSeconds(spawnDelay);
        for (int i = 0; i < numPiles; i++)
        {
            GeneratePile(debrisCollection);
            yield return new WaitForSeconds(spawnDelay);
        }
        
        FreezeDebris();
        Time.timeScale = 1f;
        gameManager.Initialize();
    }

    public void GenerateAdditional()
    {
        GeneratePile(debrisCollection);
    }
    private void GeneratePile(WeightedItemCollectionSO itemCollection){
    
        // -----------------------------------------------------------
        // Setup spawn volume 
        // -----------------------------------------------------------
        //spawnBounds = SpawnCollider.bounds;

        float widthMax = spawnBounds.max.x;  // x is left-right
        float widthMin = spawnBounds.min.x;  // x is left-right
        float lengthMax = spawnBounds.max.z; 
        float lengthMin = spawnBounds.min.z; 
        float heightMin = spawnBounds.min.y; // y is up
        float heightMax = spawnBounds.max.y; // y is up

        for(int i = 0; i < numToSpawn; i++)
        {
            // -----------------------------------------------------------
            // Adding Debris Piece
            // -----------------------------------------------------------
            // Position
            float x = random.GetStaticFloat(widthMin, widthMax);
            float y = random.GetStaticFloat(heightMin, heightMax);
            float z = random.GetStaticFloat(lengthMin, lengthMax);
            
            // Uniform Scale
            float scale = random.GetStaticFloat(1, 5);
            
            // Rotation
            Vector3 randRot = Vector3.zero;

            randRot.x = random.GetStaticFloat(-180, 180);
            randRot.y = random.GetStaticFloat(-180, 180);
            randRot.z = random.GetStaticFloat(-180, 180);
            
            Quaternion randQuat = Quaternion.Euler(randRot);
            
            // Creating the gameObject, positioning, and scaling it in scene
            GameObject debris = Spawn(itemCollection, new Vector3(x, y, z), randQuat);
            
            debris.transform.localScale = new Vector3(scale, scale, scale);
            debris.name = "debris" + i;

            ValidateDebrisColliders(debris);
            objList.Add(debris);
        } 
    }

    private void ValidateDebrisColliders(GameObject debris)
    {
        // -----------------------------------------------------------
        // Catch missing colliders and rigidbodies 
        // ----------------------------------------------------------
        if (!debris.GetComponent<Rigidbody>())
        {
            Rigidbody rb;

            if (!debris.GetComponent<Collider>())
            {
                //Add mesh collider
                MeshCollider mc;
                mc = debris.AddComponent<MeshCollider>();
                mc.convex = true;
            }

            //Configure rigidbody
            rb = debris.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 20;

        }
    }
    public GameObject Spawn(WeightedItemCollectionSO weightedList, Vector3 position, Quaternion rotation)
       {
           float weightMax = 0f;
           int curSpawned = 0;
           int breakout = 99;
           foreach (var item in weightedList.weightedItems)
           {
               weightMax += item.weight;
           }
           
           // Instantiate weighted objects
           while (curSpawned < 1)
           {
               if (breakout == 0)
               {
                   //Catch endless loops
                   break;
               }
               foreach (var item in weightedList.weightedItems)
               {
                   if (item.weight > random.GetStaticFloat(0,weightMax))
                   {
                       GameObject go = Instantiate(item.gameObject, position, rotation);
                       return go;
                   }
               }

               breakout--;
           }

           return weightedList.weightedItems[0].gameObject;
       }

    public void FreezeDebris()
    {
        List<GameObject> outOfBounds = new List<GameObject>();
        foreach (GameObject go in objList)
        {
            if (go.transform.position.y < -0.5f)
            {
                outOfBounds.Add(go);
            }
            
            Rigidbody rb = go.GetComponent<Rigidbody>();
            Destroy(rb);
            go.isStatic = true;
        }

        GameObject root = new GameObject();
        root.name = "root";
        root.gameObject.AddComponent<MeshFilter>();

        objList.RemoveAll(go => outOfBounds.Contains(go));
        for (int i = 0; i < outOfBounds.Count; i++)
        {
            Destroy(outOfBounds[i]);
        }

        if (exportSTL)
        {
            SceneToSTLExporter.ExportSceneToSTL();
        }

        foreach (var go in objList)
        {
            go.transform.parent = root.transform;
        }
        
        StaticBatchingUtility.Combine(objList.ToArray(), root);
        
    }

    public List<GameObject> GetDebrisObj()
    {
        return objList;
    }
}
