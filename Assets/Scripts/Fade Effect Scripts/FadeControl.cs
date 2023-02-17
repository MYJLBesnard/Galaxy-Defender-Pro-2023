using UnityEngine;

public class FadeControl : MonoBehaviour
{
    public GameObject CanvasObjectToFade;
    private FadeEffect _fadeEffect;

    // Start is called before the first frame update
    void Start()
    {
       _fadeEffect = GameObject.Find("CanvasFader").GetComponent<FadeEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
           _fadeEffect.FadeIn();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _fadeEffect.FadeOut();
        }
    }
}
