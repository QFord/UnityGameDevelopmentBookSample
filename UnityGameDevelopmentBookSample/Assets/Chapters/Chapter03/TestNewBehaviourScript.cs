using UnityEngine;

public class TestNewBehaviourScript : MonoBehaviour
{
    private NewBehaviourScript script;

    // Start is called before the first frame update
    void Start()
    {
        script = gameObject.AddComponent<NewBehaviourScript>();
        //script.enabled = false;
        //script.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject.Destroy(script);
            Application.Quit();
        }
    }
}
