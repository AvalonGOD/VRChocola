using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    public Transform movementSource;
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;

    public float newPositionDistance = 0.05f;
    public GameObject debugCubePrefub;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent onRecognized;

    private List<Gesture> traningSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var item in gestureFiles)
        {
            traningSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        
        // Start The Movement
        if (!isMoving && isPressed)
        {
            StartMovement();
        }
        // Ending The Movement
        else if (isMoving && !isPressed)
        {
            EndMovement();
        }
        // Updating The Movement
        else if (isMoving && isPressed)
        {
            UpdateMovement();
        }
    
    }

    void StartMovement()
    {
        Debug.Log("Start Movement");
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        if(debugCubePrefub)
            Destroy(Instantiate(debugCubePrefub, movementSource.position, Quaternion.identity), 3);
    }

    void EndMovement()
    {
        Debug.Log("End Movement");
        isMoving = false;

        // Create The Gesture From The Position List
        Point[] pointArray = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        // Add a new Gesture to training set
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            traningSet.Add(newGesture);
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        // Recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, traningSet.ToArray());
            Debug.Log(result.GestureClass + result.Score);
            
            if (result.Score > recognitionThreshold)
            {
                onRecognized.Invoke(result.GestureClass); 
            }
        }

    }

    void UpdateMovement()
    {
        Debug.Log("Updating Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionDistance)
        {
            positionsList.Add(movementSource.position);
            if (debugCubePrefub)
                Destroy(Instantiate(debugCubePrefub, movementSource.position, Quaternion.identity), 3);

        }

    }


}
