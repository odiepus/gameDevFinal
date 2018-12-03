using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    float gravity = -50;

    //PhysicsController controller;

    Vector3 velocity;

    void Start() {
        //controller = GetComponent<PhysicsController>();
    }

    void Update () {
        /*
        if (controller.PhysCollisions().above || controller.PhysCollisions().below) {
            if (controller.PhysCollisions().slidingDownMaxSlope) {
                velocity.y += controller.PhysCollisions().slopeNormal.y * -gravity * Time.deltaTime;
            }
            else {
                velocity.y = 0;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        */
    }
}
