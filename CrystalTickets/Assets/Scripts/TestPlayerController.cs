﻿using UnityEngine;
using System.Collections;

// Replace this; it was just for testing the platforms.
// See https://www.youtube.com/watch?v=Xnyb2f6Qqzg
public class TestPlayerController : MonoBehaviour {

    private Animator animator;
    private bool isFacingRight;

    //This will be our maximum speed as we will always be multiplying by 1
    public float maxSpeed;
    //to check ground and to have a jumpforce we can change in the editor
    bool grounded = false;
    public Transform groundCheck;
    public float groundRadius = 0.1f;

    // Should be set to Ground layer - put anything that you want to treat as ground on this layer
    public LayerMask whatIsGround;

    public float jumpForce = 700f;
    private Rigidbody2D rigidBody;

	public ItemScript.ItemTypes currentItem = ItemScript.ItemTypes.Pistol;

    // Shooting stuff. gun = position of the gun; where bullets will start from.
	public GameObject bulletPrefab, gun, grenadePrefab;
    public float firingIntervalInSeconds = 0.1f; // How often can we fire a bullet
    private float timeLastFired;
	private Movement movement;

	PlayerStatsUI statsUI;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
        timeLastFired = -firingIntervalInSeconds;
		statsUI = GetComponent<PlayerStatsUI>();
		movement = GetComponent<Movement>();
    }



    void FixedUpdate() {
        //set our grounded bool
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        float move = Input.GetAxis("Horizontal");//Gives us of one if we are moving via the arrow keys
                                                 //move our Players rigidbody
        rigidBody.velocity = new Vector3(move * maxSpeed, rigidBody.velocity.y);
    }

    void Update() {

        //if we are on the ground and the space bar was pressed, change our ground state and add an upward force
        if (grounded) {

            bool isMovingDown = rigidBody.velocity.y < 0;

            if (Input.GetButtonDown("Jump")) {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
                // Updates the animations to show jumping
                animator.SetBool("Jump", true);
            } else if (isMovingDown) {
                // Stop the 'jump' state if the player's about to hit the ground
                animator.SetBool("Jump", false);
            }
        }

        /* Putting the animation transitions here seems to make it more responsive than when it's in FixedUpdate(),
           but this could use a bit more testing. */

        float move = Input.GetAxis("Horizontal");

        // Update the animations to show running if the player is moving
        bool isRunning = move == 0 ? false : true;
        animator.SetBool("Run", isRunning);

        // Update animations to reflect which way the player is moving
        bool changedDirection = move > 0 && !isFacingRight || move < 0 && isFacingRight;
        if (changedDirection)
            Flip();

        // Only shoot a bullet if a sane amount of time has passed
        float secondsSinceLastFired = Time.time - timeLastFired;

        // Shooting - doesn't work if you just set 'Shoot' to the value of Input.GetKeyDown(KeyCode.Q) (hence second condition)
        if (Input.GetButton("Fire") && secondsSinceLastFired > firingIntervalInSeconds) {
            timeLastFired = Time.time;
            animator.SetBool("Shoot", true);
            FireBullet(gun.transform.position);
        }
        if (Input.GetButtonUp("Fire"))
            animator.SetBool("Shoot", false);
    }

    private void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 flippedScale = transform.localScale;
        flippedScale.x *= -1;
        transform.localScale = flippedScale;
    }
	
	private void FireBullet(Vector3 position) {
		// Bullet script in prefab should take care of actually moving the bullet once it's instantiated...
		
		GameObject prefab = bulletPrefab;
		
		if (this.currentItem == ItemScript.ItemTypes.Grenade) {
			prefab = grenadePrefab;
			GameObject bullet = (GameObject)Instantiate (prefab, position, Quaternion.identity);
			bullet.GetComponent<Grenade>().Fire(movement.isFacingRight); // ... but we need to tell it which way to move
		} else {
			GameObject bullet = (GameObject) Instantiate(prefab, position, Quaternion.identity);
			bullet.GetComponent<Bullet>().Fire(movement.isFacingRight); // ... but we need to tell it which way to move
		}
	}
	public void LoseHealth() {
			statsUI.setHp (statsUI.getHp() - 1);
			//cooldownPeriod = 1f;
	}

    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir){
        float timer = 0;
        Debug.Log("knockback");
        while( knockDur > timer){
            timer+=Time.deltaTime;
            rigidBody.velocity = new Vector2 (0, 0);
            rigidBody.AddForce(new Vector3(1000,100, transform.position.z));    
        }
        yield return 0;
    }


}
