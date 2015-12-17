//
//Filename: MouseCameraControl.cs
//

using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse")]
public class MouseCameraControl : MonoBehaviour
{
    // Mouse buttons in the same order as Unity
    public enum MouseButton { Left = 0, Right = 1, Middle = 2, None = 3 }

    [System.Serializable]
    // Handles left modifiers keys (Alt, Ctrl, Shift)
    public class Modifiers
    {
        public bool leftAlt;
        public bool leftControl;
        public bool leftShift;

        public bool checkModifiers()
        {
            return (!leftAlt ^ Input.GetKey(KeyCode.LeftAlt)) &&
                (!leftControl ^ Input.GetKey(KeyCode.LeftControl)) &&
                    (!leftShift ^ Input.GetKey(KeyCode.LeftShift));
        }
    }

    [System.Serializable]
    // Handles common parameters for translations and rotations
    public class MouseControlConfiguration
    {

        public bool activate;
        public MouseButton mouseButton;
        public Modifiers modifiers;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && Input.GetMouseButton((int)mouseButton) && modifiers.checkModifiers();
        }
    }

    [System.Serializable]
    // Handles scroll parameters
    public class MouseScrollConfiguration
    {

        public bool activate;
        public Modifiers modifiers;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && modifiers.checkModifiers();
        }
    }

    // Vertical translation default configuration
    public MouseControlConfiguration verticalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };

    // Horizontal translation default configuration
    public MouseControlConfiguration horizontalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };

    // Depth (forward/backward) translation default configuration
    public MouseControlConfiguration depthTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Left, sensitivity = 2F };

    // Scroll default configuration
    public MouseScrollConfiguration scroll = new MouseScrollConfiguration { sensitivity = 2F };

    //Rotate default configuration 
    public MouseControlConfiguration rotate = new MouseControlConfiguration { mouseButton = MouseButton.Left, sensitivity = 20f };

    // Default unity names for mouse axes
    public string mouseHorizontalAxisName = "Mouse X";
    public string mouseVerticalAxisName = "Mouse Y";
    public string scrollAxisName = "Mouse ScrollWheel";
    private Vector3 pos;
    private RaycastHit hit;
    private Renderer rend = null;
    private Ray r;
    private bool white = true;
    private Vector3 head;
    private Vector3 body;
    private Vector3 legL;
    private Vector3 legR;
    private Vector3 armL;
    private Vector3 armR;
    private Shader shader1;
    private Color ok;
    private bool rota;

    // private float distance;


    void Start() {
        rota = false;
        shader1 = Shader.Find("Standard");
        pos = new Vector3(0.0f, transform.position.y, 0.0f);
        head = new Vector3(0.0f, 175.0f, 0.0f);
        body = new Vector3(0.0f, 125.0f,0.0f);
        armL = new Vector3(-30.0f, 125.0f, 0.0f);
        armR = new Vector3(30.0f, 125.0f, 0.0f);
        legL  = new Vector3(-10.0f, 50.0f, 0.0f);
        legR = new Vector3(10.0f, 50.0f, 0.0f);
    }
    

      public void Centra (String dove) {
        if (dove.Equals("head")) { pos = head; }
        else if (dove.Equals("body")) { pos = body; }
        else if (dove.Equals("armL")) { pos = armL; }
        else if (dove.Equals("armR")) { pos = armR; }
        else if (dove.Equals("legL")) { pos = legL; }
        else if (dove.Equals("legR")) { pos = legR; }
        else if (dove.Equals("Centra")) {pos= new Vector3(0.0f, 130.0f,0.0f); }
        transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        StopAllCoroutines();
        StartCoroutine("Lerpone", pos);
        if (rend != null) rend.material.color = ok;
        }

    void LateUpdate()
    {
        
        if (verticalTranslation.isActivated())
        {
            float translateY = Input.GetAxis(mouseVerticalAxisName) * verticalTranslation.sensitivity;
            transform.Translate(0, translateY, 0);
        }

        if (horizontalTranslation.isActivated())
        {
            float translateX = Input.GetAxis(mouseHorizontalAxisName) * horizontalTranslation.sensitivity;
            transform.Translate(translateX, 0, 0);
        }

        if (depthTranslation.isActivated())
        {
            float translateZ = Input.GetAxis(mouseVerticalAxisName) * depthTranslation.sensitivity;
            transform.Translate(0, 0, translateZ);

        }

        if (scroll.isActivated())
        {
            float translateZ = Input.GetAxis(scrollAxisName) * scroll.sensitivity;

            transform.Translate(0, 0, translateZ);
        }

        if (rotate.isActivated())
        {

            r = Camera.main.ScreenPointToRay(Input.mousePosition);
              
               if (Input.GetMouseButtonDown(0))
               {
                if (Physics.Raycast(r, out hit))
                {
                   // Debug.Log(hit.collider.gameObject.layer);
                    if (!(hit.collider.bounds.center.Equals(pos))&& (hit.collider.gameObject.GetComponent<Renderer>().material.shader.Equals(shader1)))
                    {
                        
                        if (rend != null)
                        {
                                rend.material.color = ok;
                                white = true;     
                        }
                        pos = hit.collider.bounds.center;
                        rend = hit.collider.gameObject.GetComponent<Renderer>();

                            ok = rend.material.color;
                            rend.material.color = Color.red;
                            white = false;
                    }
                    else
                    {
                        if (rend == null)
                        {
                            rend = hit.collider.gameObject.GetComponent<Renderer>();
                            if (rend.material.shader.Equals(shader1))
                            {
                                ok = rend.material.color;
                                rend.material.color = Color.red;
                                white = false;
                            }
                            
                        }
                        else
                        {
                            if (rend.material.shader.Equals(shader1))
                            {
                                rend.material.color =ok;
                                rend = null;
                                white = true;
                            }

                        }
                    }
            }
                if (white) pos.y = transform.position.y;
            }
                            
                            Vector3 angles = transform.eulerAngles;
                            float x = angles.y;
                            float y = angles.x;

                            float distance = Vector3.Distance(transform.position, pos);

                            x += Input.GetAxis("Mouse X") * 120f * 0.02f;
                            y -= Input.GetAxis("Mouse Y") * 120f * 0.02f;
            
                            var rotation = Quaternion.Euler(0, x, 0);
                            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + pos;
            
                            transform.rotation = rotation;
                            transform.position = position;
        }
    }

   
    public void Rot(bool c)
     {
       rota = c;
     } 

    IEnumerator Ruotare()
    {
        float distance = Vector3.Distance(transform.position, pos);
        transform.RotateAround(pos, Vector3.up, 40 * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, transform.rotation * new Vector3(0.0f, 0.0f, -distance) + pos, 15.0f * Time.deltaTime);
        yield return null ;
    }


    void Update()
    {
        if (rota)
        {
            StartCoroutine(Ruotare());
        }


    }


    IEnumerator Lerpone(Vector3 targ)
    {
        float distance = Vector3.Distance(transform.position, targ);
        Vector3 finish = transform.rotation * new Vector3(0.0f, 0.0f, -distance) + targ;
        while (Vector3.Distance(transform.position, finish) > 0.005f)
        {
          //  Debug.Log("position " + transform.position);
            transform.position = Vector3.Lerp(transform.position, transform.rotation * new Vector3(0.0f, 0.0f, -distance) + targ, 8f * Time.deltaTime);//

            yield return null;

        }
        
    }

    

}