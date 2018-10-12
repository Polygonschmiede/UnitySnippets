using System.Collections.Generic;
using UnityEngine;


public class Teleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField]
    [Range(1, 10)]
    private float TeleportDistance = 10;

    [SerializeField]
    private GameObject OVRCameraRig;
    [SerializeField]
    private GameObject RightController;
    [SerializeField]
    private GameObject LeftController;

    [SerializeField]
    private GameObject TeleportPoint;
    [SerializeField]
    [Range(0, 12)]
    private int LayerMask = 8;


    [Header("LineRenderer Stettings")]
    [SerializeField]
    [Range(0.01f, 1)]
    private float LineSizeBegin = 0.01f;

    [SerializeField]
    [Range(0.01f, 1)]
    private float LineSizeEnd = 0.01f;
    [SerializeField]
    [Range(0.1f, 1)]
    private float alpha = 0.5f;
    [SerializeField]
    private Color ColorPos;
    [SerializeField]
    private Color ColorNeg;
    [SerializeField]
    private Material MaterialPos;
    [SerializeField]
    private Material MaterialNeg;

    private Vector3 Point1;
    private Vector3 Point2;
    private Vector3 Point3;
    AnimationCurve LineSize = new AnimationCurve();

    //private LineRenderer LineRenderer;
    private int vertexCount = 12;
    private bool CanTeleportLeft;
    private bool CanTeleportRight;
    private bool redBeam;
    private bool isMaterial = false;





    // Use this for initialization
    void Start()
    {
        TeleportPoint.GetComponent<MeshRenderer>().enabled = false;
        //-----------------------
        // If TeleportdestionationPoint has a Collider it will be Destroid
        if (TeleportPoint.GetComponent<Collider>())
        {
            Destroy(TeleportPoint.GetComponent<Collider>(), 0.1f);
        }
        //-------------------------------
        //Start Conditions for the LineRedndere (Size,Color)
        LineSize.AddKey(0, LineSizeBegin);
        LineSize.AddKey(1, LineSizeEnd);
        LineRenderer lRRight = RightController.AddComponent(typeof(LineRenderer)) as LineRenderer;
        LineRenderer lRLeft = LeftController.AddComponent(typeof(LineRenderer)) as LineRenderer;

        RightController.GetComponent<LineRenderer>().widthCurve = LineSize;
        RightController.GetComponent<LineRenderer>().widthCurve = LineSize;
        LeftController.GetComponent<LineRenderer>().widthCurve = LineSize;
        LeftController.GetComponent<LineRenderer>().widthCurve = LineSize;

        //Choose Color or Material
        if ((MaterialPos == null) && (MaterialNeg == null))
        {

            RightController.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            LeftController.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));
            isMaterial = false;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(ColorPos, 0.0f), new GradientColorKey(ColorPos, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );


            RightController.GetComponent<LineRenderer>().colorGradient = gradient;
            LeftController.GetComponent<LineRenderer>().colorGradient = gradient;

        }
        else
        {
            RightController.GetComponent<LineRenderer>().material = MaterialPos;
            LeftController.GetComponent<LineRenderer>().material = MaterialPos;
            isMaterial = true;
        }






    }


    void FixedUpdate()
    {
        //Bezier Calc

        var pointList = new List<Vector3>();

        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(Point1, Point2, ratio);
            var tangentLineVertex2 = Vector3.Lerp(Point2, Point3, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierPoint);

        }
        if (proofInput() == 1)
        {

        }
        else if (proofInput() == 2)
        {



        }
        LeftController.GetComponent<LineRenderer>().positionCount = pointList.Count;
        LeftController.GetComponent<LineRenderer>().SetPositions(pointList.ToArray());
        RightController.GetComponent<LineRenderer>().positionCount = pointList.Count;
        RightController.GetComponent<LineRenderer>().SetPositions(pointList.ToArray());



        //Layer choose to Teleport with bit shift
        RaycastHit hit;

        int layerMask = 1 << LayerMask;

        //Teleport Logic
        if (proofInput() == 2 && !(proofInput() == 1))
        {//true RightStick
            disableController(proofInput() == 2);
            if (Physics.Raycast(RightController.transform.position, RightController.transform.TransformDirection(Vector3.forward), out hit, TeleportDistance, layerMask))
            {
                //Debug.Log("R_ 1");
                CanTeleportRight = true;
                UpdateLineRenderer(hit, true);
                //PointerVisible(true);

            }//Red because false Mask
            else if (Physics.Raycast(RightController.transform.position, RightController.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, ~layerMask))
            {
                //Debug.Log("R_ 2");
                CanTeleportRight = false;
                UpdateLineRenderer(hit, false);
                redBeam = true;
            }//Red because false Distance
            else if (Physics.Raycast(RightController.transform.position, RightController.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("R_ 3");
                CanTeleportRight = false;
                UpdateLineRenderer(hit, false);
                redBeam = true;
            }
        }
        else if (proofInput() == 1 && !(proofInput() == 2))
        {//false LeftStick
            disableController(proofInput() != 1);
            if (Physics.Raycast(LeftController.transform.position, LeftController.transform.TransformDirection(Vector3.forward), out hit, TeleportDistance, layerMask))
            {

                //Debug.Log("L_ 1");
                CanTeleportLeft = true;
                UpdateLineRenderer(hit, true);
                //PointerVisible(true);
            }//Red because false Mask
            else if (Physics.Raycast(LeftController.transform.position, LeftController.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, ~layerMask))
            {
                //Debug.Log("L_ 2");
                CanTeleportLeft = false;
                UpdateLineRenderer(hit, false);
                redBeam = true;
            }//Red because false Distance
            else if (Physics.Raycast(LeftController.transform.position, LeftController.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("L_ 3");
                CanTeleportLeft = false;
                UpdateLineRenderer(hit, false);
                redBeam = true;
            }
        }//Teleporting
        else if ((proofInput() == -1) && CanTeleportRight && Physics.Raycast(RightController.transform.position, RightController.transform.TransformDirection(Vector3.forward), out hit, TeleportDistance, layerMask))
        {
            //Debug.Log("R_ 4");
            OVRCameraRig.transform.position = hit.point;
            CanTeleportRight = false;
            //PointerVisible(false);


        }
        else if ((proofInput() == -1) && CanTeleportLeft && Physics.Raycast(LeftController.transform.position, LeftController.transform.TransformDirection(Vector3.forward), out hit, TeleportDistance, layerMask))
        {
            //Debug.Log("L_ 4");
            OVRCameraRig.transform.position = hit.point;
            CanTeleportLeft = false;
            //PointerVisible(false);

        }


        //Toggle RedBeam and Pointer
        if (redBeam && !(proofInput() == -1))
        {
            redBeam = false;
                        
        } 
        
        if ((proofInput() == -1))
        {
            PointerVisible(false);
        } else if (!(proofInput() == -1))
        {
            PointerVisible(true);
        }

    }

    private void PointerVisible(bool v)
    {
        if (v)
        {
            TeleportPoint.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            TeleportPoint.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    //Disable Teleport
    private void disableController(bool v)
    {
        if (v)
        {
            RightController.GetComponent<LineRenderer>().enabled = true;
            LeftController.GetComponent<LineRenderer>().enabled = false;
        }
        else
        {
            LeftController.GetComponent<LineRenderer>().enabled = true;
            RightController.GetComponent<LineRenderer>().enabled = false;

        }

    }

    /*
    * which controller are be used
    * */
    private int proofInput()
    {
        //
        bool left = OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp);
        bool right = OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp);


        int ret = -1;
        if (left && !right)
        {
            ret = 1;

            //Debug.Log("Links");
        }

        if (right && !left)
        {

            //Debug.Log("Rechts");
            ret = 2;
        }
        if ((!left && !right)||(left && right))
        {
            ret = -1;
            LeftController.GetComponent<LineRenderer>().enabled = false;
            RightController.GetComponent<LineRenderer>().enabled = false;
            //Debug.Log("Nichts ist gedrückt");
            
        }
        


        return ret;
        //return OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp);
    }

    //Coloring the Line
    private void UpdateLineRenderer(RaycastHit hit, bool layerCol)
    {
        //Normal
        if (hit.distance < TeleportDistance && layerCol)
        {

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(ColorPos, 0.0f), new GradientColorKey(ColorPos, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );
            if (proofInput() == 1)
            {
                LeftController.GetComponent<LineRenderer>().colorGradient = gradient;
            }
            else
            {
                RightController.GetComponent<LineRenderer>().colorGradient = gradient;
            }

        }
        else
        {

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(ColorNeg, 0.0f), new GradientColorKey(ColorNeg, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
                );

            if (proofInput() == 1)
            {
                LeftController.GetComponent<LineRenderer>().colorGradient = gradient;
            }
            else
            {
                RightController.GetComponent<LineRenderer>().colorGradient = gradient;
            }


        }
        // Material
        if (hit.distance < TeleportDistance && layerCol && isMaterial)
        {

            if (proofInput() == 1)
            {
                LeftController.GetComponent<LineRenderer>().material = MaterialPos;
            }
            else
            {
                RightController.GetComponent<LineRenderer>().material = MaterialPos;
            }

        }
        else if (hit.distance > TeleportDistance && layerCol && isMaterial)
        {
            if (proofInput() == 1)
            {
                LeftController.GetComponent<LineRenderer>().material = MaterialNeg;
            }
            else
            {
                RightController.GetComponent<LineRenderer>().material = MaterialNeg;
            }


        }


        try
        {
            TeleportPoint.transform.position = hit.point;
        }
        catch
        {
            Debug.LogError("Teleport Destionationpoint is not set.");
        }
        //SetPosition of the Ray start and ending Point

        if (proofInput() == 2)
        {
            Point1 = RightController.transform.position;
        }
        else if (proofInput() == 1)
        {
            Point1 = LeftController.transform.position;
        }
        //Point1 = RightController.transform.position;
        Point3 = hit.point;
        //Point2 = new Vector3(Mathf.Abs((Point3-Point1) / 2), Mathf.Abs(Point3 - Point1 / 2), Mathf.Abs(Point3 - Point1 / 2));
        Vector3 a = new Vector3(0, 0.5f, 0);
        //Ist nicht die beste rechnung
        Point2 = ((Point1 + Point3) / 2) + a;

    }



}

