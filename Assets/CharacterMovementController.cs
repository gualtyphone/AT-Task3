using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovementController : MonoBehaviour {

    CharacterController rb;

    private void Awake()
    {
        rb = GetComponent<CharacterController>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        rb.SimpleMove(new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")));

        if (Input.GetButtonDown("Jump")) { rb.SimpleMove(new Vector3(0.0f, 1.0f, 0.0f)); }
	}
}
