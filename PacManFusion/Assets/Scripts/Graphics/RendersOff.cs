using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendersOff : MonoBehaviour {

    public bool TurnOffChildRenders = false;
	// Use this for initialization
	void Awake () {
        TurnOffRenderer();
	}

    void TurnOffRenderer()
    {
        if (TurnOffChildRenders) { //Turns off all renderers attached to this object
            Renderer[] rdrs = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rdrs.Length; i++) {
                rdrs[i].enabled = false;
            }
            rdrs = null;
            return; //leave
        }

        //Turn off the single renderer
        GetComponent<Renderer>().enabled = false;

    }

    public void ToggleRenderer(bool on = false)
    {
        if (TurnOffChildRenders) { //Turns off all renderers attached to this object
            Renderer[] rdrs = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rdrs.Length; i++) {
                rdrs[i].enabled = on;
            }
            rdrs = null;
            return; //leave
        }

        //Turn off the single renderer
        Renderer rdr = GetComponent<Renderer>();
        if (rdr) {
            rdr.enabled = on;
        }

    }
}
