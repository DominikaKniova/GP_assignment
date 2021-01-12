using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject grid;
    public GameObject player;
    private GridManager gridManager;

    Dictionary<string, int> types = new Dictionary<string, int> { 
        { "BlockGrass(Clone)", 0 }, 
        { "BlockStone(Clone)", 1 }, 
        { "BlockDirt(Clone)", 2 }, 
        { "BlockSand(Clone)", 3 } };
    private string saveFileName = "gamesave.save";
    
    public void SaveGame()
    {

        SaveData save = new SaveData();

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            save.blockPositions.Add(new Vector3S(block.transform.position));
            save.blockTypes.Add(types[block.name]);
        }

        Vector3 playerPos = GameObject.Find("Player").transform.position;
        save.playerPosition = new Vector3S(playerPos);

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
            // empty scene
            foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block"))
            {
                Destroy(block);
            }
            Destroy(GameObject.Find("Player"));

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + saveFileName, FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            gridManager = grid.GetComponent<GridManager>();

            for (int i = 0; i < data.blockPositions.Count; i++)
            {
                gridManager.AddBlock(data.blockPositions[i].ToVector3(), data.blockTypes[i]);
            }
            player.transform.position = data.playerPosition.ToVector3();
        
            Debug.Log("Loading successful");
        }
        else
            Debug.LogError("Loading unsuccessful");
    }
}
