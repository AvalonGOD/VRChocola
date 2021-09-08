using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightArrowSpell : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 200f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
