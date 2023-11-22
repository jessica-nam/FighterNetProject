using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Transform playerTransform;  // Reference to the player's transform
    public Vector3 offset;  // Offset from player's position to position the health bar above the player's head
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    private void LateUpdate()
    {
        // Update the position of the health bar canvas to be above the player's head
        //transform.position = playerTransform.position + offset;

        // Make the health bar face the camera. The second argument of LookAt determines the 'up' vector.
        // We use Vector3.up to keep it upright relative to the world.
        // transform.LookAt(transform.position + mainCamera.transform.forward, Vector3.up);
    }
}
