using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Measurement : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;
    [SerializeField] GameObject pointPerfab;
  

    [Header("UI")]
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] float measurementConversionFactor;
    [SerializeField] Button targetButton;
    [SerializeField] Button setHeightButton;
    [SerializeField] Button setWidthButton;
    [SerializeField] Button resetButton;

    [SerializeField] TextMeshProUGUI detectionText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] TextMeshProUGUI widthText;

    LineRenderer line;
    ARRaycastManager raycastManager;
    ARCameraManager cameraManager;
    Camera arCamera;

    List<ARRaycastHit> hit = new List<ARRaycastHit>();

    int buttonCounter = 0;
    float distance;

    void Awake()
    {
        startPoint = Instantiate(pointPerfab, Vector3.zero, Quaternion.identity);
        endPoint = Instantiate(pointPerfab, Vector3.zero, Quaternion.identity);
        

        line = GetComponent<LineRenderer>();
        cameraManager = GetComponent<ARCameraManager>();
        arCamera = Camera.main;
        raycastManager = GetComponent<ARRaycastManager>();

        ResetDetection();
        //add targetButton Listener
        targetButton.onClick.AddListener(TargetButtonHandler);
        setHeightButton.onClick.AddListener(SetHeight);
        setWidthButton.onClick.AddListener(SetWidth);
        resetButton.onClick.AddListener(ResetDetection);
    }

    void Update()
    {
        if (buttonCounter > 0)
        {
            Ray raycastRay = new Ray(arCamera.transform.position, arCamera.transform.forward);
            if(buttonCounter == 1)
            {
                if (raycastManager.Raycast(raycastRay, hit, UnityEngine.XR.ARSubsystems.TrackableType.All))
                {
                    detectionText.text = "Set Start Point";
                    var hitPos = hit[0].pose;
                    startPoint.SetActive(true);

                    startPoint.transform.SetPositionAndRotation(hitPos.position, hitPos.rotation);
                }
            }
            else if(buttonCounter == 2)
            {
                if(raycastManager.Raycast(raycastRay, hit, UnityEngine.XR.ARSubsystems.TrackableType.All))
                {
                    detectionText.text = "Set End Point";
                    var hitPos = hit[0].pose;
                    endPoint.SetActive(true);
                    line.gameObject.SetActive(true);

                    endPoint.transform.SetPositionAndRotation(hitPos.position, hitPos.rotation);
                }
            }
            else
            {
                detectionText.text = "Set Height or Width";
            }
        }
        if(startPoint.activeSelf && endPoint.activeSelf)
        {
            line.positionCount = 2;
            line.SetPosition(0, startPoint.transform.position);
            line.SetPosition(1, endPoint.transform.position);

            distance = Vector3.Distance(startPoint.transform.position, endPoint.transform.position) * measurementConversionFactor;
            distanceText.text = "Distance: " + distance.ToString()+" cm.";
        }
    }

    void ResetDetection()
    {
        detectionText.text = "";
        line.positionCount = 0;
        startPoint.SetActive(false);
        endPoint.SetActive(false);
        buttonCounter = 0;
    }

    void TargetButtonHandler()
    {
        buttonCounter++;
    }

    void SetHeight() 
    {
        string distanceText = (distance > 1) ? distance.ToString() : "Invalid";
        heightText.text = "Height: " + distanceText;
        ResetDetection();
    }

    void SetWidth()
    {
        string distanceText = (distance > 1) ? distance.ToString() : "Invalid";
        widthText.text = "Width: " + distanceText;
        ResetDetection();
    }
}

