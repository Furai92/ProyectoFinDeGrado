using UnityEngine;

public class test_deleteme : MonoBehaviour
{
    public MeshRenderer m1;
    public MeshRenderer m2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m1.material.SetColor("_Color", Color.red);
        m2.material.SetColor("_Color", Color.white);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
