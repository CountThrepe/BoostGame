using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	public Rigidbody2D body;
	public LayerMask ground;
    public GameObject speedText;

    // Public parameters
    public float speedDamp = 3f;
	public float jumpForce = 3f;
	public float groundCheckRadius = 0.15f;
	public float groundCheckTolerance = 1f;
    public float bounceThreshold = 0.14f;
    public float bounceTime = 1f;
    public float bounceTimeout = 3f;
    public float bounceSpeedKillFactor = .5f;

	private Inputs controls;
	private bool jump = false;

    // Bounce state tracking
    private bool bounce = false;
    private bool bouncing = false;
    private float goalVelocity = 0f;
    private float bounceAccel = 0f;
    private float bounceTimeElapsed = 0f;

    // Rigidbody contact tracking 
    private int contactCapacity = 5;
    private ContactPoint2D[] contacts;
    private int prevNumContacts = 0;

    // State memories
    private float lastXVel = 0f;
    private Vector2 prevBodyPos;
    private Vector2 respawnPoint;

    void Awake() {
        respawnPoint = body.position;

        speedText.GetComponent<SpeedDisplay>();

        contacts = new ContactPoint2D[contactCapacity];

    	controls = new Inputs();
        controls.Player.Jump.started += _ => jump = true;
        controls.Player.Jump.performed += _ => jump = false;
        controls.Player.Respawn.started += _ => Respawn();
        controls.Player.Escape.started += _ => Quit();
    }

    void FixedUpdate() {
        // Handle x velocity stuff
        if(!bouncing && !bounce) { // Default state
            if(body.velocity.x > 0) {
                body.AddForce(Vector2.left * speedDamp); // Apply speed damping
            } else if(body.velocity.x < 0) {
                body.AddForce(Vector2.right * body.mass * -body.velocity.x); // Prevent backwards movement not from bounce
            }
        }
        else if(bounce) { // Bounce just starting
            bounce = false;
            bouncing = true;

            goalVelocity = lastXVel * bounceSpeedKillFactor;
            bounceAccel = -1; //Don't set it till we know our backwards speed
            bounceTimeElapsed = 0;

            body.AddForce(Vector2.left * (body.mass * lastXVel), ForceMode2D.Impulse);
        }
        else if(bouncing) { // We're mid bounce
            if(bounceAccel == -1) bounceAccel = (goalVelocity - body.velocity.x) / bounceTime; // 10 is an arbitrary factor so bounceTime makes more sense
        	bounceTimeElapsed += Time.deltaTime;
        	if(bounceTimeElapsed >= bounceTimeout || body.velocity.x >= goalVelocity) { // We've reached the end of the bounce
        		bouncing = false;
	        }
	        else {
	        	body.AddForce(Vector2.right * bounceAccel);
            }
        }

        // Handle y velocity stuff
    	if(Grounded()) {
    		if(jump) {
	        	body.AddForce(Vector2.up * (jumpForce + (body.mass * -body.velocity.y)), ForceMode2D.Impulse);
	        	jump = false;
	        }
    	}

        // Update state memory
        prevBodyPos = body.position;
        lastXVel = body.velocity.x;
    }

    void LateUpdate() {
        speedText.GetComponent<SpeedDisplay>().setSpeed(body.velocity.x); // Update speed display
    }

    void OnCollisionEnter2D(Collision2D collision) {
        int numContacts = collision.GetContacts(contacts);
        for(int i = 0; i < numContacts && i < contactCapacity; i++) {
            Vector2 relativePos = contacts[i].point - prevBodyPos;
            if(relativePos.x > bounceThreshold) {
                bounce = true;
                break;
            }
        }
        
        prevNumContacts = numContacts;
    }

    void OnCollisionStay2D(Collision2D collision) {
        int numContacts = collision.GetContacts(contacts);
        if(numContacts > prevNumContacts && !bounce) { // If there are more contact points now than last time
            Vector2 bodyPos = body.position;
            for(int i = 0; i < numContacts && i < contactCapacity; i++) {
                Vector2 relativePos = contacts[i].point - prevBodyPos;
                if(relativePos.x > bounceThreshold) {
                    bounce = true;
                    break;
                }
            }
        }

        prevNumContacts = numContacts;
    }

    void OnCollisionExit2D() {
        prevNumContacts = 0;
    }

    void OnEnable() {
    	controls.Enable();
    }

    void OnDisable() {
    	controls.Disable();
    }

    bool Grounded() {
    	RaycastHit2D hit = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y), groundCheckRadius, Vector2.down,groundCheckTolerance, ground.value);
    	return (hit.collider != null && hit.point.y < transform.position.y);
    }

    void Respawn() {
        SceneManager.LoadScene("SampleScene");
    }

    void Quit() {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void boost(float speed) {
        body.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
    }
}
