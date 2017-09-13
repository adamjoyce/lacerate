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
    private bool canFire = true;            // True when a new firing touch can occur.
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
        if (!canFire && Input.touchCount == 0 && rb.velocity.magnitude < 0.1f)
        {
            // Allow a new firing touch to occur.
            canFire = true;
        }
        else if (canFire)
        {
            // A firing touch is ready to be performed.
            if (Input.touchCount == 1)
            {
                // A finger is touching the screen.
                Touch touch = Input.GetTouch(0);
                SpriteRenderer touchSpriteRenderer = touchIndicator.GetComponent<SpriteRenderer>();

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        firstTouchPosition = touch.position;
                        touchIndicator.position = (Vector2)Camera.main.ScreenToWorldPoint(firstTouchPosition);
                        touchSpriteRenderer.enabled = true;
                        break;

                    case TouchPhase.Moved:
                        direction = touch.position - firstTouchPosition;
                        ScaleDirectionIndicator(direction);
                        break;

                    case TouchPhase.Ended:
                        directionChosen = true;
                        directionIndicator.localScale = Vector2.one;
                        touchSpriteRenderer.enabled = false;
                        directionIndicator.gameObject.SetActive(false);
                        break;
                }
            }
            else if (Input.touchCount > 1)
            {
                // Cancel the current firing.
                // Hide the touch sprite.
                SpriteRenderer touchSpriteRenderer = touchIndicator.GetComponent<SpriteRenderer>();
                if (touchSpriteRenderer.enabled)
                    touchSpriteRenderer.enabled = false;

                // Hide the direction indicator is necessary.
                if (directionIndicator.gameObject.activeInHierarchy)
                    directionIndicator.gameObject.SetActive(false);

                // Reset our direction selected control variable.
                if (directionChosen)
                    directionChosen = false;

                // Ensure the player cannot fire again without lifting their finger.
                if (canFire)
                    canFire = false;
            }
        }
	}

    /* Fixed Update is called once per physics cycle. */
    private void FixedUpdate()
    {
        if (directionChosen)
        {
            // The player has lifted their finger so fire the player avatar.
            rb.AddForce(-direction.normalized * scaledMagnitude * forceMultiplier);
            directionChosen = false;
            canFire = false;
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

        if (!directionIndicator.gameObject.activeInHierarchy)
            directionIndicator.gameObject.SetActive(true);
    }

    /* Scales a value within a certain range to be within another range, maintaining the original ratio. */
    private float ScaleValue(float currentValue, float currentMin, float currentMax, float newMin, float newMax)
    {
        float newValue = ((currentValue - currentMin) / (currentMax - currentMin)) * (newMax - newMin) + newMin;
        return newValue;
    }
}
