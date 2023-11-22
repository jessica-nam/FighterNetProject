using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerCollision : MonoBehaviour
{
    public bool isCollided = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerTag")
        {
            isCollided = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerTag")
        {
            isCollided = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerTag")
        {
            isCollided = false;
        }
    }
}
