using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Entity))]
public class PlayerInput : MonoBehaviour {

    Entity entity;

	void Start () {
        entity = GetComponent<Entity>();
	}
	
	void Update () {
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 facing = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        Debug.DrawRay(transform.position, facing);

        entity.UpdateFacing(facing);

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        entity.SetDirectionalInput(directionalInput);

        entity.CheckVerticalInput(Input.GetKey(KeyCode.LeftShift));

        entity.SetAiming(Input.GetKey(KeyCode.Mouse1));
        if (Input.GetKey(KeyCode.Mouse0)) {
            entity.Weapon.Shooting = true;
            entity.Shoot();
        }else {
            entity.Weapon.Shooting = false;
        }

        if (Input.GetKey(KeyCode.S)) {
            entity.EnterCover();
        }else {
            entity.ExitCover();
        }
    }
}
