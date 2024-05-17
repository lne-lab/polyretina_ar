using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
		{
            print("hi1");
		}
    }

    public void PrintHi()
    {
        print("hi2");
    }
}
