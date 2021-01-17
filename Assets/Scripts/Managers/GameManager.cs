using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public ChunkedWorldManager chunkedWorldManager;

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

                    for (int j = 0; j < ChunkedWorldManager.chunkSize; j++)
                        for (int i = 0; i < ChunkedWorldManager.chunkSize; i++)
                            for (int k = 0; k < ChunkedWorldManager.chunkSize; k++)
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
        save.playerPosition = new Vector3S((int)playerPos.x, (int)playerPos.y, (int)playerPos.z);
        save.heightMap = ChunkedWorldManager.heightMap;

        Debug.Log("Saving game to: " + Application.persistentDataPath);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + saveFileName);
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/" + saveFileName))
        {
            chunkedWorldManager.EmptyWorld();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + saveFileName, FileMode.Open);
            SaveData save = (SaveData)bf.Deserialize(file);
            file.Close();

            ChunkedWorldManager.heightMap = save.heightMap;

            foreach (KeyValuePair<Vector3S, ChunkData> item in save.chunks)
            {
                Vector3S chunkPos = item.Key;
                ChunkData chunkData = item.Value;

                chunkedWorldManager.chunks[chunkPos.x, chunkPos.y, chunkPos.z].ReCreateChunkFromSave(ref chunkData);
            }

            player.transform.position = save.playerPosition.ToVector3();

            Debug.Log("Loading successful");
        }
        else
            Debug.LogError("Loading unsuccessful");
    }
}
