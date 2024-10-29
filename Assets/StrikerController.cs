using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StrikerController : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] LineRenderer directionIndicator;
    [SerializeField] private Slider slider;
    [SerializeField] float maxPower = 10f; 
    [SerializeField] float minDragDistance = 0.5f; 
    [SerializeField] float powerScale = 1f; 
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 initialPosition;
    private bool strikerLaunched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        directionIndicator.enabled = false;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (!strikerLaunched)
        {
            HandleTouchInput();
        }
        else
        {
            if (rb.velocity.magnitude < 0.05f)
            {
                ResetStriker();
            }
        }
        
    }

    public void StrikerPosition()
    {
        transform.position = new Vector3(slider.value, transform.position.y, transform.position.z);
    }

    private void ResetStriker()
    {
        rb.velocity = Vector2.zero;
        slider.value = 0f;
        transform.position = initialPosition;
        strikerLaunched = false;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Vector2.Distance(touchPosition, transform.position) < 1f)
                    {
                        startTouchPosition = touchPosition;
                        isDragging = true;
                        directionIndicator.enabled = true;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        endTouchPosition = touchPosition;
                        Vector2 direction = startTouchPosition - endTouchPosition;
                        float distance = direction.magnitude;

                        if (distance > minDragDistance)
                        {
                            UpdateDirectionIndicator(direction, distance);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        endTouchPosition = touchPosition;
                        Vector2 launchDirection = startTouchPosition - endTouchPosition;
                        float launchPower = Mathf.Clamp(launchDirection.magnitude * powerScale, 0, maxPower);
                        LaunchStriker(launchDirection.normalized, launchPower);
                        isDragging = false;
                        directionIndicator.enabled = false;
                        strikerLaunched = true;
                    }
                    break;
            }
        }
    }

    private void UpdateDirectionIndicator(Vector2 direction, float distance)
    {
        directionIndicator.SetPosition(0, transform.position);
        directionIndicator.SetPosition(1, (Vector2)transform.position + direction.normalized * Mathf.Clamp(distance, 0, maxPower));
        directionIndicator.startWidth = 0.05f;
        directionIndicator.endWidth = 0.2f;
    }

    private void LaunchStriker(Vector2 direction, float power)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * power, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            ResetStriker();
        };
    }
}
