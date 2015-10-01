﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlayerHealth : Health {

    public Slider healthBar;

    // Required when the player dies (change state / disable)
    private PlayerController movementController;

	protected override void Start () {
        base.Start();
        movementController = GetComponent<PlayerController>();
	}
	
	void Update () {
	
	}

    public override void RemoveHealth(int damage) {
        base.RemoveHealth(damage);
        if (healthBar != null)
            healthBar.value = Math.Max(currentHealth, 0); // Don't let the health bar drop below 
    }

    public override void AddHealth(int health) {
        base.AddHealth(health);
        if (healthBar != null)
            healthBar.value = Math.Min(currentHealth, startingHealth); // Health shouldn't exceed the max
    }

    public override void DestroyCharacter() {
        base.DestroyCharacter();
        movementController.enabled = false; // Turn off the controls
    }
}
