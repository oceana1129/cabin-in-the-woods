using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // movement relative to facing direction
        Vector3 move = (transform.right * h + transform.forward * v).normalized;

        Vector3 velocity = move * speed;

        controller.Move(velocity * Time.deltaTime);
    }
}
