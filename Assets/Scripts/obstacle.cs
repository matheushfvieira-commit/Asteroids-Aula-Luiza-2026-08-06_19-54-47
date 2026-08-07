using UnityEngine;

public class obstacle : MonoBehaviour
{
    void Start()
    {
        float tamanhometeoro = Random.Range(0.5f, 2.5f);
        transform.localScale = new Vector3(tamanhometeoro, tamanhometeoro, 1);
    }
    void Update()
    {
        
    }
}
