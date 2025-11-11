using UnityEngine;

public class ColorBounceEffect : MonoBehaviour
{
    private Renderer colorRenderer;
    public Color initialColor;
    public Color transitionColor;
    public float speed = 1.0f;
    private float step;

    void Start()
    {
        colorRenderer = GetComponent<Renderer>();                           // find color renderer on the object

        if (colorRenderer == null) {                                        // see if renderer was assigned
            Debug.LogError("Renderer does not exist, please assign");           // log error
            return;                                                             // exit
        }

        Debug.Log("Starting color transition on " + gameObject.name);       // log renderer assignment on object
    }

    // Update is called once per frame
    void Update()
    {
        step = Mathf.PingPong(Time.time * speed, 1);                                    // calcuate step between two values
        colorRenderer.material.color = Color.Lerp(initialColor, transitionColor, step); // change color of renderer based on the two colors and step we're on
    }
}
