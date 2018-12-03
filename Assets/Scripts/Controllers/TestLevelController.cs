using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelController : MonoBehaviour {

    public GameObject[] hallTiles;
    public GameObject[] cornerTiles;
    public GameObject[] stairLeftTiles;
    public GameObject[] stairRightTiles;
    public GameObject[] floorTiles;

    private int currentLevel;
    private int levelTileWidth;
    private int levelTileHeight;

    void Start () {
        generateLevel();
	}
	
	void Update () {
		
	}

    void generateLevel() {
        levelTileWidth = 10;
        levelTileHeight = 4;

        for(int i = 0; i < levelTileHeight; i++) {
            currentLevel = i;
            for(int j = 0; j < levelTileWidth; j++) {
                if(currentLevel == 0) {
                    Vector3 position = new Vector3(j*2, i, 0f);
                    GameObject tile = Instantiate(floorTiles[Random.Range(0, floorTiles.Length)], position, Quaternion.identity) as GameObject;
                }
            }
        }
    }
}
