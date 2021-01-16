using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject world;
    public GameObject player;
    private WorldManager worldManager;
    public ChunkedWorldManager chunkedWorldManager;

    Dictionary<string, int> types = new Dictionary<string, int> { 
        { "BlockGrass(Clone)", 0 }, 
        { "BlockStone(Clone)", 1 }, 
        { "BlockDirt(Clone)", 2 }, 
        { "BlockSand(Clone)", 3 } };
    private string saveFileName = "gamesave.save";
    
    public void SaveGame()
    {

        SaveData save = new SaveData();

        for (int y = 0; y < ChunkedWorldManager.numChunk; y++)
            for (int x = 0; x < ChunkedWorldManager.numChunk; x++)
                for (int z = 0; z < ChunkedWorldManager.numChunk; z++)
                {
                    ChunkData chunkData = new ChunkData();
                    Vector3S position = new Vector3S(x, y, z);

                    for (int j = 0; j < ChunkedWorldManager.numChunk; j++)
                        for (int i = 0; i < ChunkedWorldManager.numChunk; i++)
                            for (int k = 0; k < ChunkedWorldManager.numChunk; k++)
                            {
                                int type = chunkedWorldManager.chunks[x, y, z].chunkGrid[i, j, k];
                                if (type != 0)
                                {
                                    chunkData.blockPositions.Add(new Vector3S(i, j, k));
                                    chunkData.blockTypes.Add(type);
                                }
                            }

                    save.chunks.Add(position, chunkData);
                }

        Vector3 playerPos = GameObject.Find("Player").transform.position;
        save.playerPosition = new Vector3S(playerPos.x, playerPos.y, playerPos.z);

        //Debug.Log("Saving game to: " + Application.persistentDataPath);

        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + "/" + saveFileName);
        //bf.Serialize(file, save);
        //file.Close();
    }

    public void LoadGame()
    {
        //if (File.Exists(Application.persistentDataPath + "/" + saveFileName))
        //{
        //    // empty scene
        //    foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block"))
        //    {
        //        Destroy(block);
        //    }

        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Open(Application.persistentDataPath + "/" + saveFileName, FileMode.Open);
        //    SaveData data = (SaveData)bf.Deserialize(file);
        //    file.Close();

        //    worldManager = world.GetComponent<WorldManager>();

        //    for (int i = 0; i < data.blockPositions.Count; i++)
        //    {
        //        worldManager.AddBlock(data.blockPositions[i].ToVector3(), data.blockTypes[i]);
        //    }
        //    player.transform.position = data.playerPosition.ToVector3();
        
        //    Debug.Log("Loading successful");
        //}
        //else
        //    Debug.LogError("Loading unsuccessful");
    }
}
