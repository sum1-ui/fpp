using JetBrains.Annotations;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{

    public Transform cameraPosition;


    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
