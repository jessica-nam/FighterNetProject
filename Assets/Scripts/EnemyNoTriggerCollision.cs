using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNoTriggerCollision : MonoBehaviour
{
    public bool isCollided = false;
    

    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if(collision.gameObject.tag == "PlayerTag")
        {
            isCollided = true;
        }
    }

    void OnCollisionStay(UnityEngine.Collision collision)
    {
        if(collision.gameObject.tag == "PlayerTag")
        {
            isCollided = true;
        }
    }

    void OnCollisionExit(UnityEngine.Collision collision)
    {
        if(collision.gameObject.tag == "PlayerTag")
        {
            isCollided = false;
        }
    }
}
