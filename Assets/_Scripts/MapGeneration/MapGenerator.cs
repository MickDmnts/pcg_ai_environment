using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: These variables must be assigned from the inspector for the script to work correctly.
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The GenerateMap() method gets called when the scene loads to start the map generation.
* 
* [Must know]
* 1. The OnMapGenerationFinished event gets called when the grass generation finishes.
*/

[Serializable]
public struct GridCell
{
    public bool Visited;
    public int PosX;
    public int PosZ;
    public int Index;
}

[DefaultExecutionOrder(500)]
public class MapGenerator : MonoBehaviour
{
    #region INPSECTOR_VARIABLES
    [Header("\tPlayer Prefab")]
    [SerializeField] GameObject player;

    [Header("\tMap Generation Settings")]
    [SerializeField] int mapSize;
    [SerializeField] GameObject grassTile;
    [SerializeField] GameObject wallTile;
    [SerializeField] GameObject pathTilePrefab;
    [SerializeField] int pathLength;

    [Header("\tTree Generation")]
    [SerializeField] GameObject treePrefab;
    [SerializeField, Range(1, 100)] int treeDensity;

    [Header("\tHouse Generation")]
    [SerializeField] List<GameObject> housePrefabs;

    [Header("\tFireworks Platform generation")]
    [SerializeField] GameObject fireworksPlatformPrefab;

    [Header("\tPressure Plate Generation")]
    [SerializeField] GameObject PressurePlatePrefab;
    #endregion

    #region PRIVATE_VARIABLES
    //Map Generation Speeds
    float terrainGenerationSpeed = 0.005f;
    float pathGenerationSpeed = 0.01f;
    float treeGenerationSpeed = 0.01f;

    //Variables for spawned objs anchors
    GameObject root;
    GameObject houseRoot;
    GameObject treeParent;
    GameObject playerInstance;
    Vector3Int[] tilePositions;
    List<GridCell> visitedTiles = new List<GridCell>();

    //The first tile to be spawned
    NavMeshSurface rootNavMeshSurface;

    int iterations = 4;
    int lastVisitedTileIndex;
    Vector3 tilePosition;

    //Legend
    // * 0 = not path tile
    // * 1 = path tile
    int[] path;

    //Calculated based on mapSize
    int Width;
    int Height;
    int size;

    //Tile size for correct tile placement
    int tileSize;

    #endregion

    private void Start()
    {
        if (GameManager.S != null)
        {
            GameManager.S.GameEventsHandler.onMapGenerationFinish += SpawnPlayer;
        }

        GenerateMap();
    }

    /// <summary>
    /// Call to start the map generation sequence.
    /// </summary>
    public void GenerateMap()
    {
        ClearVisitedRooms();
        ClearMap();
        InitializeMapRoot();

        //Set the needed map variables
        Width = mapSize;
        Height = mapSize;
        size = Width * Height;

        tileSize = (int)grassTile.transform.localScale.x;

        //Initialize the path array based on the calculated map size.
        path = new int[size];

        //Mark each path as 0 (not path tile for now)
        for (int i = 0; i < size; i++)
        { path[i] = 0; }

        //Initialize the tilePositions array based on the calculated map size.
        tilePositions = new Vector3Int[size];

        //Populate the tilePositions array with the calculated path tile positions based on their sizes.
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + z * Height;
                tilePositions[i] = new Vector3Int(x * tileSize, 0, z * tileSize);
            }
        }

        lastVisitedTileIndex = 0;

        GameManager.S.UIManager.ChangeInfoText("Generating Map...");

        //Start the map visualization process
        StartCoroutine(FillDungeon());

        IterateThroughPossibleTiles();
    }

    /// <summary>
    /// Call to create a new List(GridCell) and assign it to the visitedTiles list.
    /// </summary>
    void ClearVisitedRooms()
    {
        visitedTiles = new List<GridCell>();
    }

    /// <summary>
    /// Call to destroy each map generated parent gameobject, along with its children.
    /// </summary>
    public void ClearMap()
    {
        if (root != null) DestroyImmediate(root);
        if (houseRoot != null) DestroyImmediate(houseRoot);
        if (playerInstance != null) DestroyImmediate(playerInstance);
        if (treeParent != null) DestroyImmediate(treeParent);
    }

    /// <summary>
    /// Call to create the map root gameObject that will hold all the the generated map tiles.
    /// </summary>
    void InitializeMapRoot()
    {
        root = new GameObject();
        root.name = "Map";
        root.AddComponent<Grid>();
    }

    /// <summary>
    /// Invoke to start the map tile filling sequence, this coroutine creates the Grass and Wall tiles of the map.
    /// </summary>
    /// <returns></returns>
    IEnumerator FillDungeon()
    {
        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                tilePosition = new Vector3(x * tileSize, 0, z * tileSize);

                if (z == 0 && x == 0)
                {
                    Instantiate(wallTile, new Vector3(-(tileSize / 2), (tileSize / 2), 0f), Quaternion.Euler(0f, 0f, 90f), root.transform);
                    Instantiate(wallTile, new Vector3(0f, (tileSize / 2), -(tileSize / 2)), wallTile.transform.rotation, root.transform);

                    GameObject firstTile = Instantiate(grassTile, tilePosition, Quaternion.identity, root.transform);
                    rootNavMeshSurface = firstTile.GetComponent<NavMeshSurface>();
                }
                else
                {
                    Instantiate(grassTile, tilePosition, Quaternion.identity, root.transform);
                }

                //Generate the left side walls (from top view, z == forward)
                if (x == 0)
                {
                    Instantiate(wallTile, new Vector3(tilePosition.x - (tileSize / 2), (tileSize / 2), tilePosition.z), Quaternion.Euler(0f, 0f, 90f), root.transform);
                }

                //Generate the bottom side wall (from top view, z == forward)
                if (x - Width <= 0)
                {
                    Instantiate(wallTile, new Vector3(tilePosition.x, (tileSize / 2), -(tileSize / 2)), wallTile.transform.rotation, root.transform);
                }

                //Generate the right side wall, (from top view, z == forward)
                if (x == mapSize - 1)
                {
                    Instantiate(wallTile, new Vector3(tilePosition.x + tileSize / 2, (tileSize / 2), tilePosition.z), Quaternion.Euler(0f, 0f, 90f), root.transform);
                }

                //Generate the top side wall, (from top view, z == forward)
                if (z == mapSize - 1)
                {
                    Instantiate(wallTile, new Vector3(tilePosition.x, (tileSize / 2), tilePosition.z + tileSize / 2), wallTile.transform.rotation, root.transform);
                }

                yield return new WaitForSecondsRealtime(terrainGenerationSpeed);
            }

            yield return new WaitForSecondsRealtime(terrainGenerationSpeed);
        }

        //Call the OnMapGenerationFinish() event to notify the AI Handler to spawn the enemy.
        GameManager.S.GameEventsHandler.OnMapGenerationFinish();

        yield return null;
    }

    void IterateThroughPossibleTiles()
    {
        for (int i = 0; i < iterations; i++)
        {
            FindNextRoom();
        }

        StartCoroutine(PlaceCorrectTiles());
    }

    /// <summary>
    /// Call to gradually create a random path. 
    /// </summary>
    void FindNextRoom()
    {
        for (int i = 0; i < pathLength; i++)
        {
            // Pick a random direction
            int randDirection = UnityEngine.Random.Range(0, 4);
            int index = 0;

            switch (randDirection)
            {
                //FORWARD
                case 0:
                    // Find the cell above
                    index = lastVisitedTileIndex + Width;

                    //If the cell is within our map and has not been visited yet
                    if (index < size && !HasCellBeenVisited(index))
                    {
                        MarkCellVisited(index);
                    }
                    break;

                //LEFT
                case 1:
                    //Find the left cell
                    index = lastVisitedTileIndex + 1;

                    //If the cell is within our map and has not been visited yet
                    if (index < size && tilePositions[index].z == tilePositions[lastVisitedTileIndex].z && !HasCellBeenVisited(index))
                    {
                        MarkCellVisited(index);
                    }
                    break;

                //BACKWARDS
                case 2:
                    //Find the cell below
                    index = lastVisitedTileIndex - Width;

                    //If the cell is within our map and has not been visited yet
                    if (index >= 0 && !HasCellBeenVisited(index))
                    {
                        MarkCellVisited(index);
                        break;
                    }
                    break;

                //RIGHT
                case 3:
                    //Find the right cell
                    index = lastVisitedTileIndex - 1;

                    //If the cell is within our map and has not been visited yet
                    if (index >= 0 && tilePositions[index].x == tilePositions[lastVisitedTileIndex].x && !HasCellBeenVisited(index))
                    {
                        MarkCellVisited(index);
                        break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Call to check if the passed cell index is not visited.
    /// </summary>
    /// <param name="index">The cell index</param>
    /// <returns>False if the path index is 0, True otherwise</returns>
    bool HasCellBeenVisited(int index)
    {
        if (path[index] == 0) return false;
        else return true;
    }

    /// <summary>
    /// Call to mark the passed path element index as visited
    /// </summary>
    /// <param name="index">The element to mark as visited in the path array.</param>
    void MarkCellVisited(int index)
    {
        // Mark it as visited
        path[index] = 1;

        // Store it as last visited
        lastVisitedTileIndex = index;
        AddCellToList(index);
    }

    /// <summary>
    /// Call to create the GridCell obj and add it to the visitedTiles list.
    /// </summary>
    /// <param name="index">The index corresponding to the tilePositions array element.</param>
    void AddCellToList(int index)
    {
        GridCell newCell;
        newCell.Index = index;
        newCell.PosX = tilePositions[index].x;
        newCell.PosZ = tilePositions[index].z;
        newCell.Visited = true;

        visitedTiles.Add(newCell);
    }

    //PATH BUILDING
    IEnumerator PlaceCorrectTiles()
    {
        //Start tile
        GameObject pathTile0 = pathTilePrefab;
        Vector3 pos0 = new Vector3(visitedTiles[0].PosX, 0, visitedTiles[0].PosZ);
        Instantiate(pathTile0, pos0, Quaternion.identity, root.transform);

        //End tile
        GameObject pathTileEnd = pathTilePrefab;
        Vector3 posL = new Vector3(visitedTiles[visitedTiles.Count - 1].PosX, 0, visitedTiles[visitedTiles.Count - 1].PosZ);
        Instantiate(pathTileEnd, posL, Quaternion.identity, root.transform);

        //Calculate the obj to be spawned indexes.
        int startIndex = 1;
        int midIndex = visitedTiles.Count > 0 ? visitedTiles.Count / 2 : 0; //In case the count is zero
        int endIndex = visitedTiles.Count - 2;
        int pressureIndex = visitedTiles.Count > 0 ? visitedTiles.Count / 3 : 0; //In case the count is zero

        //Setup the house transform anchor in hierarchy
        GameObject parent = new GameObject();
        parent.name = "HouseAnchor";
        parent.transform.position = Vector3.zero;
        houseRoot = parent;

        //Build the path and houses
        for (int i = 1; i < visitedTiles.Count - 1; i++)
        {
            GameObject pathTile = pathTilePrefab;
            Vector3 pos = new Vector3(visitedTiles[i].PosX, 0, visitedTiles[i].PosZ);

            //Create the fireworks platform
            if (i == startIndex)
            {
                Instantiate(fireworksPlatformPrefab, pos, fireworksPlatformPrefab.transform.rotation, parent.transform);
            }

            //Create the house gameObject in the 0th element of the housePrefabs array.
            if (i == midIndex)
            {
                if (housePrefabs.Count > 0)
                {
                    Instantiate(housePrefabs[0], pos, housePrefabs[0].transform.rotation, parent.transform);
                }
                else
                {
                    Debug.LogWarning("HousePrefabs list does not contain a first element OR is empty.");
                }
            }

            //Create the house gameObject in the 1st element of the housePrefabs array.
            if (i == endIndex)
            {
                if (housePrefabs.Count > 1)
                {
                    Instantiate(housePrefabs[1], pos, housePrefabs[1].transform.rotation, parent.transform);
                }
                else
                {
                    Debug.LogWarning("HousePrefabs list does not contain a second element OR is empty.");
                }
            }


            //Create the pressuirePlate gameObject.
            if (i == pressureIndex)
            {
                Instantiate(PressurePlatePrefab, pos + new Vector3(0, 0.3f, 0f), PressurePlatePrefab.transform.rotation, parent.transform);
            }

            //Instantiate the path tile
            Instantiate(pathTile, pos, Quaternion.identity, root.transform);
            yield return new WaitForSecondsRealtime(pathGenerationSpeed);
        }

        //Start the tree instantiation sequence
        StartCoroutine(PlaceTrees());

        yield return null;
    }

    /// <summary>
    /// Invoke to start placing trees in empty tile cells with a density set from the inspector. 
    /// </summary>
    IEnumerator PlaceTrees()
    {
        //Create the tree anchor in the hierarchy
        GameObject tempTree = new GameObject();
        tempTree.name = "TreeAnchor";
        tempTree.transform.position = Vector3.zero;
        treeParent = tempTree;

        for (int i = 0; i < tilePositions.Length; i++)
        {
            //If the i path element is not marked as a path tile...
            if (path[i] == 0)
            {
                int random = new System.Random().Next(0, 100);

                if (random >= treeDensity)
                {
                    continue;
                }
                else
                {
                    //Instantiate a tree in its place.
                    Instantiate(treePrefab, new Vector3(tilePositions[i].x, 0f, tilePositions[i].z), Quaternion.identity, treeParent.transform);
                }
            }

            yield return new WaitForSecondsRealtime(treeGenerationSpeed);
        }

        yield return null;
    }

    /// <summary>
    /// Call to instantiate a player in the 3rd created tile of the map and then cache his reference inside playerInstance.
    /// </summary>
    void SpawnPlayer()
    {
        Vector3 playerPos = new Vector3(visitedTiles[3].PosX, player.transform.localScale.y, visitedTiles[3].PosZ);
        playerInstance = Instantiate(player, playerPos, Quaternion.identity);
    }

    /// <summary>
    /// Call to get the NavMeshSurface component of the first created tile.
    /// </summary>
    public NavMeshSurface GetNavMeshSurface()
    {
        return rootNavMeshSurface;
    }

    private void OnValidate()
    {
        /*
         * The below adjustments are made so we don't get any index out of bounds error.
         */

        //Limit the map size
        if (mapSize < 5)
        {
            mapSize = 5;
        }

        //Check if the map size is smaller than the map length
        if (mapSize < pathLength)
        {
            pathLength = mapSize;
        }

        //Limit the path length
        if (pathLength < 5)
        {
            pathLength = 5;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.S != null)
        {
            GameManager.S.GameEventsHandler.onMapGenerationFinish -= SpawnPlayer;
        }
    }
}
