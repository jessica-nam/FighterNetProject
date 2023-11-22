using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTapDetector
{
    public float tapTimer = 0f;
    private int tapCount = 0;
    public float maxTapTime = 0.2f;
    public float cooldownTime = 2f; // Cooldown time after double-tap is detected
    private float currentCooldown = 0f; // Current cooldown time left
    public bool isSingleTapPending = false;
    
    public bool CheckForDoubleTap(KeyCode key)
    {
        // If we're in a cooldown phase, reduce the cooldown and early exit
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            return false;
        }

        if (Input.GetKeyDown(key))
        {
            tapCount++;

            if (tapCount == 1)
            {
                tapTimer = maxTapTime;
                isSingleTapPending = true;
            }
            else if (tapCount == 2)
            {
                isSingleTapPending = false; // Clear the pending flag
                ResetTap();
                currentCooldown = cooldownTime; // Initiate cooldown
                return true;  // Double-tap detected.
            }
        }

        if (tapTimer > 0)
        {
            tapTimer -= Time.deltaTime;
            if (tapTimer <= 0)
            {
                ResetTap();
            }
        }

        return false;  // Double-tap not detected.
    }

    private void ResetTap()
    {
        tapCount = 0;
        tapTimer = 0;
    }
}
