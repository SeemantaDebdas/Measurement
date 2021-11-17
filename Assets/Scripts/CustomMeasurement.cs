using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomMeasurement : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;
    [SerializeField] GameObject pointPerfab;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] float measurementConversionFactor;

    LineRenderer line;
    ARRaycastManager raycastManager;
    ARCameraManager cam;

    List<ARRaycastHit> hit = new List<ARRaycastHit>();
    Vector2 touchInputPosition;
    

    void Awake()
    {
        startPoint = Instantiate(pointPerfab, Vector3.zero, Quaternion.identity);
        endPoint = Instantiate(pointPerfab, Vector3.zero, Quaternion.identity);
        Reset();

        line = GetComponent<LineRenderer>();
        cam = GetComponent<ARCameraManager>();
        raycastManager = GetComponent<ARRaycastManager>();

    }

    void Update()
    {
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject())
        {
            Touch touchInput = Input.GetTouch(0);
            if(touchInput.phase == TouchPhase.Began)
            {
                touchInputPosition = touchInput.position;
                if (raycastManager.Raycast(touchInputPosition, hit, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint))
                {
                    var hitPos = hit[0].pose;
                    startPoint.SetActive(true);

                    startPoint.transform.SetPositionAndRotation(hitPos.position, hitPos.rotation);
                }
            }
            else if(touchInput.phase == TouchPhase.Moved)
            {
                touchInputPosition = touchInput.position;
                if(raycastManager.Raycast(touchInputPosition, hit, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint))
                {
                    var hitPos = hit[0].pose;
                    endPoint.SetActive(true);
                    line.gameObject.SetActive(true);

                    endPoint.transform.SetPositionAndRotation(hitPos.position, hitPos.rotation);
                }
            }
        }

        if(startPoint.activeSelf && endPoint.activeSelf)
        {
            line.SetPosition(0, startPoint.transform.position);
            line.SetPosition(1, endPoint.transform.position);

            float distance = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);
            distanceText.text = "Length in cm : " + (distance * measurementConversionFactor).ToString();
        }
    }

    private void Reset()
    {
        startPoint.SetActive(false);
        endPoint.SetActive(false);
    }
}

