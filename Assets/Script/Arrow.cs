using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  Arrow: MonoBehaviour
{
    public float speed = 3.0f;

    // Update is called once per frame
    void Update()
    {
        
        //left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += transform.up * speed * Time.deltaTime;
        }

        //right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += transform.up * speed * Time.deltaTime * -1;
        }

        //forward
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += transform.forward * speed * Time.deltaTime * -1;
        }

        //back
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position += transform.forward * speed * Time.deltaTime ;
        }
    }
}
