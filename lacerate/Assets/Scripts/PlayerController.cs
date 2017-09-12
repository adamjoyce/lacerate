using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    public Transform touchIndicator;        // Represents the location the play began their screen touch.
    public float powerScaleMin = 0.0f;      // The minimum value of the firing scale.
    public float powerScaleMax = 10.0f;     // The maximum value of the firing scale.
    public float forceMultiplier = 500.0f;  // The power scale value is multiplied by this value to achieve the final force applies to the player.

    private Rigidbody2D rb;                 // The rigidbody attached to the player sprite.
    private Transform directionIndicator;   // The indicator used to show the predicted direction and magnitude of the player.
    private Vector2 firstTouchPosition;     // The position of the finger when it touches the screen.
    private Vector2 startPosInWorld;        // The first touch position in world space.
    private Vector2 direction;              // The direction the player will be fired.
    private bool directionChosen = false;   // True when the player has chosen a direction to fire.
    private float scaledMagnitude = 0.0f;   // The current magnitude of the force scaled to fall between powerScaleMin and powerScaleMax.

	/* Use this for initialization. */
	private void Start() 
	{
        if (!touchIndicator) { touchIndicator = GameObject.Find("TouchIndicator").transform; }

        rb = GetComponent<Rigidbody2D>();
        directionIndicator = transform.Find("DirectionIndicator").transform;
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
                    touchIndicator.position = (Vector2)Camera.main.ScreenToWorldPoint(firstTouchPosition);
                    touchIndicator.GetComponent<SpriteRenderer>().enabled = true;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - firstTouchPosition;
                    ScaleDirectionIndicator(direction);
                    break;

                case TouchPhase.Ended:
                    directionChosen = true;
                    directionIndicator.localScale = new Vector2(1, 1);
                    touchIndicator.GetComponent<SpriteRenderer>().enabled = false;
                    break;
            }
        }
        else if (directionChosen)
        {
            // The player has lifted their finger so fire the player avatar.
            rb.AddForce(-direction.normalized * scaledMagnitude * forceMultiplier);
            directionChosen = false;
        }
	}

    /* Scales the direction indicator. */
    private void ScaleDirectionIndicator(Vector2 direction)
    {
        // Scale the magnitude to be within the power range.
        scaledMagnitude = ScaleValue(direction.magnitude, 0, Screen.width, powerScaleMin, powerScaleMax);
        directionIndicator.localScale = new Vector2(scaledMagnitude * 3, 1);

        // Face the indicator in the correct direction.
        float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        directionIndicator.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /* Scales a value within a certain range to be within another range, maintaining the original ratio. */
    private float ScaleValue(float currentValue, float currentMin, float currentMax, float newMin, float newMax)
    {
        float newValue = ((currentValue - currentMin) / (currentMax - currentMin)) * (newMax - newMin) + newMin;
        return newValue;
    }
}
