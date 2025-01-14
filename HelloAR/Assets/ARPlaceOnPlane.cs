﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceOnPlane : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject placeObject;
    GameObject spawnObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCenterObject();
        PlaceObjectByTouch();
    }

    private void PlaceObjectByTouch()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); //0번째 손가락 터치
            List<ARRaycastHit> hits = new List<ARRaycastHit>(); //Raycast hit
            if(arRaycaster.Raycast(touch.position, hits, TrackableType.Planes)) //터치 일어난곳에 ray쏘기
            {
                Pose hitPose = hits[0].pose;
                
                if (!spawnObject) //null이면
                {
                   
                    spawnObject = Instantiate(placeObject, hitPose.position, hitPose.rotation); //실체화 
                    
                }
                else //터치 일어난 곳으로 바꿔주기
                {
                    spawnObject.transform.position = hitPose.position;
                    spawnObject.transform.rotation = hitPose.rotation;
                    
                }
                
            }
        }
    }

    private void UpdateCenterObject()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arRaycaster.Raycast(screenCenter, hits, TrackableType.Planes);

        if(hits.Count > 0)
        {
            Pose placementPose = hits[0].pose;
            placeObject.SetActive(true);
            placeObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

        }
        else
        {
            placeObject.SetActive(false);
        }

    }
}
