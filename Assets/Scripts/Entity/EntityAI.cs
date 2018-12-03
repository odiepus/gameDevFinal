using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAI : MonoBehaviour {

    Entity entity;

    void Start () {
        entity = GetComponent<Entity>();
    }
	
	void Update () {
        entity.Shoot();
	}
}
