using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationNodeType
{
    PLATFORM = 0,
}

public class GenerationNode {
    public bool isLevelStart = false;
    public bool isLevelEnd = false;
    public GenerationNodeType nodeType = GenerationNodeType.PLATFORM;
    public GameObject nodeRoot = null;
    public List<GameObject> nodeParts = new List<GameObject>();
    public List<GenerationNode> nextNodes = new List<GenerationNode>();
}

public class Platform
{
    public int yPosition;
    public int xStart;
    public int xEnd;
}

public class GenerationController : MonoBehaviour
{
    public PlayerController player;
    public GameObject platformPrefab;

    public int levelLength = 64;

    public int minPlatformLength = 1;
    public int maxPlatformLength = 8;

    public int minPlatformDistanceX = 1;
    public int maxPlatformDistanceX = 4;

    public int minVerticalPlatformDistance = 2;
    public int maxConcurrentPlatforms = 3;

    public int levelMinY = 0;
    public int levelMaxY = 24;
    public int LevelHeight { get => levelMaxY - levelMinY; }

    public bool[,] generatedPlatforms = new bool[0, 0];
    public List<GameObject> spawnedObjects = new List<GameObject>();
    public List<Platform> platforms = new List<Platform>();

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate() {
        Initialize();
        int levelMid = Mathf.RoundToInt(LevelHeight / 2f);
        Platform startPlatform = new Platform() {
            yPosition = levelMid,
            xStart = 0,
            xEnd = Random.Range(minPlatformLength, maxPlatformLength),
        };

        for (int i = 0; i < startPlatform.xEnd; i++) {
            generatedPlatforms[i, startPlatform.yPosition] = true;
            GameObject spawnedObject = Instantiate(platformPrefab);
            spawnedObject.transform.position = new Vector3(i, startPlatform.yPosition, 0f);
            spawnedObjects.Add(spawnedObject);
        }

        player.transform.position = new Vector3(startPlatform.xStart, startPlatform.yPosition + 1.5f, 0f);

        Platform lastPlatform = startPlatform;
        platforms.Add(lastPlatform);

        int levelPlatformMax = levelMaxY - minVerticalPlatformDistance;
        int levelPlatformMin = levelMinY + minVerticalPlatformDistance;
        int levelPlatformEndXCancel = levelLength - minPlatformDistanceX;

        while (lastPlatform.xEnd < levelPlatformEndXCancel) {
            int gap = Random.Range(minPlatformDistanceX, maxPlatformDistanceX);
            int platformStartX = lastPlatform.xEnd + gap;
            int platformEndX = Mathf.Min(levelLength, platformStartX + Random.Range(minPlatformLength, maxPlatformLength));
            int platformY = lastPlatform.yPosition;

            int platformBoundsMax = Mathf.Min(lastPlatform.yPosition + minVerticalPlatformDistance, levelPlatformMax);
            int platformBoundsMin = Mathf.Max(lastPlatform.yPosition - minVerticalPlatformDistance, levelPlatformMin);
            if (lastPlatform.yPosition >= (levelMaxY - minVerticalPlatformDistance)) {
                // no up adds
                platformY = Random.Range(platformBoundsMin, lastPlatform.yPosition);
            } else if (lastPlatform.yPosition <= (levelMinY + minVerticalPlatformDistance)) {
                // no down adds
                platformY = Random.Range(platformBoundsMax, lastPlatform.yPosition);
            } else {
                // any adds
                platformY = Random.Range(platformBoundsMin, platformBoundsMax);
            }

            Platform newPlatform = new Platform() {
                xStart = platformStartX,
                xEnd = platformEndX,
                yPosition = platformY,
            };

            GameObject o = SpawnPlatformObject(newPlatform);
            spawnedObjects.Add(o);

            lastPlatform = newPlatform;
            platforms.Add(lastPlatform);
        }
    }

    public GameObject SpawnPlatformObject(Platform platform) {
        GameObject root = new GameObject();
        root.transform.position = new Vector3(platform.xStart, platform.yPosition, 0f);

        int size = platform.xEnd - platform.xStart;
        for (int i = 0; i < size; i++) {
            Vector3 worldPos = new Vector3(platform.xStart + i, platform.yPosition, 0f);
            GameObject o = Instantiate(platformPrefab, worldPos, Quaternion.identity);
            o.transform.parent = root.transform;
        }

        return root;
    }

    //public GenerationNode CreatePlatformNode(int x, int y, int size) {
    //    Vector2 position = new Vector2(x, y);

    //    GameObject nodeRoot = new GameObject();
    //    nodeRoot.transform.position = position;

    //    List<GameObject> nodeObjects = new List<GameObject>();
    //    for (int i = 0; i < size; i++) {
    //        GameObject newObject = Instantiate(platformPrefab, nodeRoot.transform);
    //        newObject.transform.position = position;
    //        nodeObjects.Add(newObject);
    //        position.x += 1f;
    //    }

    //    return new GenerationNode() {
    //        nodeRoot = nodeRoot,
    //        nodeParts = nodeObjects,
    //        nodeType = GenerationNodeType.PLATFORM,
    //    };
    //}

    //public void SpawnPlatforms() {
    //    int halfLevelHeight = Mathf.FloorToInt(LevelHeight / 2);
    //    int firstNodeLength = Random.Range(minPlatformLength, maxPlatformLength);
    //    GenerationNode firstNode = CreatePlatformNode(0, halfLevelHeight, firstNodeLength);
    //    firstNode.isLevelStart = true;

    //    GenerationNode lastNode = firstNode;
    //    for (int x = firstNodeLength; x < levelLength; x++) {
    //        int jumpGap = Random.Range(minPlatformDistanceX, maxPlatformDistanceX);
    //        x += jumpGap;

    //        int platformSize = Random.Range(minPlatformLength, maxPlatformLength);
    //        GenerationNode nextNode = CreatePlatformNode(x, halfLevelHeight, platformSize);
    //        lastNode.nextNodes.Add(nextNode);
    //        lastNode = nextNode;
    //    }


    //    player.transform.position = firstNode.nodeParts[0].transform.position = Vector3.up * 2f;
    //}

    public void Initialize() {
        for (int i = 0; i < spawnedObjects.Count; i++) {
            DestroyImmediate(spawnedObjects[i]);
        }

        generatedPlatforms = new bool[levelLength, LevelHeight];
    }
}
