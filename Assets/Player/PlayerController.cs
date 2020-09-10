using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	public Rigidbody2D body;
	public LayerMask ground;

    public float speedDamp = 3f;
	public float jumpForce = 3f;
	public float groundCheckRadius = 0.15f;
	public float groundCheckTolerance = 1f;

	private Inputs controls;
	private bool jump = false;
	private float xVel = 0f;

    void Awake() {
    	controls = new Inputs();
        controls.Player.Jump.started += _ => jump = true;
        controls.Player.Jump.performed += _ => jump = false;
    }

    void FixedUpdate() {
        Debug.Log(body.velocity.x);
        float decel = speedDamp * Time.fixedDeltaTime;
        if(xVel > decel) xVel -= decel;
        else xVel = 0;

    	if(Grounded()) {
    		body.velocity = new Vector2(xVel, Mathf.Max(body.velocity.y, 0));
    		if(jump) {
	        	body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
	        	jump = false;
	        }
    	} else body.velocity = new Vector2(xVel, body.velocity.y);
    }

    void OnEnable() {
    	controls.Enable();
    }

    void OnDisable() {
    	controls.Disable();
    }

    bool Grounded() {
    	RaycastHit2D hit = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y), groundCheckRadius, Vector2.down,
    		groundCheckTolerance, ground.value);
    	return (hit.collider != null && hit.point.y < transform.position.y);
    }

    public void boost(float speed) {
        xVel += speed;
    }
}
