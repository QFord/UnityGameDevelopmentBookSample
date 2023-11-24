using UnityEngine;

public class ResetExample : MonoBehaviour
{
    public GameObject target;

    void Reset()
    {
        //Output the message to the Console
        Debug.Log("Reset");
        if (!target)
            target = GameObject.FindWithTag("Player");
    }
}
