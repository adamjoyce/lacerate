using UnityEngine;

public class Bounds : MonoBehaviour 
{
    public GameObject topBound;                     // The upper bound of the playable area.
    public GameObject bottomBound;                  // The lower bound of the playable area.
    public GameObject leftBound;                    // The left-most bound of the playable area.
    public GameObject rightBound;                   // The right-most bound of the playable area.

	/* Use this for initialization. */
	private void Start() 
	{
        // Calculate the height and width of the playable (viewable) area.
        float height = Camera.main.orthographicSize * 2;
        float width = Camera.main.aspect * height;

        // Scale and position the bounds.
        topBound.transform.position = new Vector2(0, height * 0.5f);
        topBound.transform.localScale = new Vector2(width, 1);
        bottomBound.transform.position = new Vector2(0, -height * 0.5f);
        bottomBound.transform.localScale = new Vector2(width, 1);
        leftBound.transform.position = new Vector2(-width * 0.5f, 0);
        leftBound.transform.localScale = new Vector2(height, 1);
        rightBound.transform.position = new Vector2(width * 0.5f, 0);
        rightBound.transform.localScale = new Vector2(height, 1);
	}
}
