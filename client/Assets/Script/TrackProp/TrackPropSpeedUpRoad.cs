using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPropSpeedUpRoad : MonoBehaviour {
    public int buffId = 1;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<RoleManager>().addBuff(buffId);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<RoleManager>().removeBuff(buffId);
        }
    }
}
