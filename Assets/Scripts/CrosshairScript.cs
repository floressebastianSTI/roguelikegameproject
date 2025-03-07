using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CrosshairScript : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        transform.position = worldPosition; // Move crosshair to mouse position
    }
}