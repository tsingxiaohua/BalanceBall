using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        SwipeDirection dir = checkHorizontalSwipes();
        if (dir == SwipeDirection.Left)
        {
            transform.RotateAround(player.transform.position, new Vector3(0f,1f,0f), 45);
            offset = transform.position - player.transform.position;
        }
        else if (dir == SwipeDirection.Right)
        {
            transform.RotateAround(player.transform.position, new Vector3(0f, 1f, 0f), -45);
            offset = transform.position - player.transform.position;
        }
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }

    public float minSwipeDist, maxSwipeTime;
    private Vector2 startPos;
    private float swipeStartTime;
    private bool couldBeSwipe;

    private enum SwipeDirection
    {
        None,
        Left,
        Right
    }

    SwipeDirection checkHorizontalSwipes()
    {
        if (Input.touches.Length == 0)
            return SwipeDirection.None;

        Touch touch = Input.touches[0];

        if (touch.phase == TouchPhase.Began)
        {
            couldBeSwipe = true;
            startPos = touch.position;
            swipeStartTime = Time.time; 
        }
        else if(touch.phase == TouchPhase.Stationary)
        {
            couldBeSwipe = false;
        }
        float swipeTime = Time.time - swipeStartTime;
        float swipeDist = Mathf.Abs(touch.position.x - startPos.x);

        if (couldBeSwipe && swipeTime < maxSwipeTime && swipeDist > minSwipeDist)
        {
            couldBeSwipe = false;
            return (touch.position.x - startPos.x >= 0f) ? SwipeDirection.Right : SwipeDirection.Left;
        }

        return SwipeDirection.None;
    }
}
