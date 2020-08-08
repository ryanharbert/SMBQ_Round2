using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnattachParent : MonoBehaviour
{
    private void Start()
    {
        Invoke("Unparent", 0.2f);
    }

    void Unparent ()
    {
        gameObject.transform.parent = null;
	}
}
