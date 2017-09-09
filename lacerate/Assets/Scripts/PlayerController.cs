﻿using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    private Rigidbody2D rb;                 // The rigidbody attached to the player sprite.
    private Vector2 firstTouchPosition;     // The position of the finger when it touches the screen.
    private Vector2 direction;              // The direction the player will be fired.
    private bool directionChosen = false;   // True when the player has chosen a direction to fire.

	/* Use this for initialization. */
	private void Start() 
	{
        rb = GetComponent<Rigidbody2D>();
        firstTouchPosition = new Vector2();
        direction = new Vector2();
	}
	
	/* Update is called once per frame. */
	private void Update() 
	{
        // Firing Controls.
        if (Input.touchCount > 0)
        {
            // A finger is touching the screen.
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    firstTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - firstTouchPosition;
                    break;

                case TouchPhase.Ended:
                    directionChosen = true;
                    break;
            }
        }
        else if (directionChosen)
        {
            //float fireStrength = (direction.magnitude - 0) / (Screen.width - 0) * (300 - 1) + 1;
            rb.AddForce(-direction * 5); //* fireStrength);
            directionChosen = false;

            Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)));
        }
	}
}
