using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 3.0f;

    private void Update()
    {
        //up
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += transform.up * speed * Time.deltaTime;
        }

        //down
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += transform.up * speed * Time.deltaTime * -1;
        }
    }
}
