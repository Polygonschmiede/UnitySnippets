using UnityEngine;

public class ControllerGrabObject : MonoBehaviour
{

    public bool vibrationOn;
    public bool inHand;
    
    //Steam VR SDK 
    private SteamVR_TrackedObject trackedObj;

    // 1
    [SerializeField]
    private GameObject collidingObject;
    // 2
    public GameObject objectInHand;


    //Steam VR SDK gibt die Controller an.
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        inHand = false;
    }
    // Update is called once per frame
    void Update()
    {
        // 1
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
                inHand = true;
            }
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
                inHand = false;
            }
        }


    }
    private void SetCollidingObject(Collider col)
    {
        if (col != null)
        {
            if (!col.GetComponent<Rigidbody>())
            {
                return;
            }
            collidingObject = col.gameObject;
        }

    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        //Vibration, wenn Objekt gegriffen werden kann
        if (vibrationOn)
        {
            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(1000);
        }
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        SetCollidingObject(null);
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        // 2
       
        //Controller wechseln, falls mit dem anderen gegriffen wird
        //wenn linker Controller
        if (transform.gameObject == oxySkript.ControllerLeft)
        {
            if (IsInOtherHand(objectInHand, this.gameObject))
            {
                if (oxySkript.ControllerRight.GetComponent<FixedJoint>())
                {
                    oxySkript.ControllerRight.GetComponent<ControllerGrabObject>().ReleaseObject();
                }
            }
        }
        else
        {
            if (this.transform.gameObject == oxySkript.ControllerRight)
            {
                if (IsInOtherHand(objectInHand, this.gameObject))
                {
                    if (oxySkript.ControllerLeft.GetComponent<FixedJoint>())
                    {
                        oxySkript.ControllerLeft.GetComponent<ControllerGrabObject>().ReleaseObject();
                    }
                }
            }
        }

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();


    }
    /// <summary>
    /// Überladene Methode, damit man manuell erzwingen kann, dass ein GameObject gegriffen wird. 
    /// Der Controller wird hier bereits bestimmt (das Objekt, das in der Hand gehalten wird, wird nur gewechselt.
    /// </summary>
    /// <param name="obj"></param>
    private void GrabObject(GameObject obj)
    {
        objectInHand = obj;
        collidingObject = null;

        //bestehenden FixedJoint entfernen, um neuen zu machen.
        if (gameObject.GetComponent<FixedJoint>())
        {
            Destroy(gameObject.GetComponent<FixedJoint>());
        }
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private SpringJoint AddSpringJoint()
    {
        SpringJoint fx = gameObject.AddComponent<SpringJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }


    public void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3 Hier ERROR weil ich den RB vorher Lösche muss abgefangen werden
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        if (GetComponent<SpringJoint>())
        {
            // 2
            GetComponent<SpringJoint>().connectedBody = null;
            Destroy(GetComponent<SpringJoint>());
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }

        // 4
        objectInHand = null;
        //Scripts.GetComponent<Oxy_Main>().SetInHandObj(objectInHand);
        oxySkript.SetObjInHand(objectInHand, this.transform.gameObject);
    }
    /// <summary>
    /// In dieser Methode wird Gravity aktiviert und Kinematic deaktiviert
    /// </summary>
    /// <param name="obj">Zu verändernes GameObject</param>
    private void EnableGravity(GameObject obj)
    {
        oxySkript.ChangeObjectStates(obj, 0, 1, 2, 0);
    }
    /// <summary>
    /// Diese Methode "erzwingt" das Greifen auch wenn der Spieler das Event nicht selber auslöst.
    /// </summary>
    /// <param name="obj">Zu greifendes Objekt</param>
    public void GrabManually(GameObject obj)
    {
        GrabObject(obj);
    }
    private bool IsInOtherHand(GameObject obj, GameObject con)
    {
        if (con == oxySkript.ControllerLeft)
        {
            if (obj == oxySkript.ObjectInRightHand)
            {
                return true;
            }
            else if (objectInHand == GameObject.Find("Oxy") && (oxySkript.ObjectInRightHand == GameObject.Find("Top")
              || oxySkript.ObjectInRightHand == GameObject.Find("Bottom")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (con == oxySkript.ControllerRight)
        {
            if (obj == oxySkript.ObjectInLeftHand)
            {
                return true;
            }
            else if (objectInHand == GameObject.Find("Oxy") && (oxySkript.ObjectInLeftHand == GameObject.Find("Top")
              || oxySkript.ObjectInLeftHand == GameObject.Find("Bottom")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
