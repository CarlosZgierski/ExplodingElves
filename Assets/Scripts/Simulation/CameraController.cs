using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform cameraPivot;

    float cameraSensitivity = 1f;

    Touch touch;
    Vector2 firstPosition;
    Vector2 currentPosition;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                firstPosition = touch.position;
                currentPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                currentPosition = touch.position;

                if(firstPosition.x - currentPosition.x > 0)
                {
                    //rotate one way
                }
                else
                {
                    //rotate other way
                }

                if (firstPosition.y - currentPosition.y > 0)
                {
                    //up
                }
                else
                {
                    //down
                }

            }
        }
    }
}
