using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Entity.Controller.CameraController {
    public class CameraController : MonoBehaviour {

        public GameObject player;

        private Vector3 offset;

        public void SetPlayerFollow(GameObject player) {
            this.player = player;
            offset = transform.position - player.transform.position;
        }

        void LateUpdate() {
            Vector3 v3 = player.transform.position + offset;
            v3.z = -10;
            transform.position = v3;
        }
    }
}
