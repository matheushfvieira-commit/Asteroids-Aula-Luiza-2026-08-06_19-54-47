using UnityEngine;
using UnityEngine.InputSystem;


public class Players : MonoBehaviour
{
    public Animator animator;

    public float Thurst;
    public Rigidbody2D rb; 
    void Start()
    {
        animator  = GetComponent<Animator>();
    }
    private void HandleMovment()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.current.position.value;
            animator.SetBool("isMoving", true);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2  direction = (mousePos - transform.position);
        }
        else if (animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving" = false);
        }  
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovment();
       //Mouse.current.leftButton.isPressed
       
       //Mouse.current.position.value
    }
}


//Mouse.current.leftButton.isPressed


//Mouse.current.position.value

//Camera.main.ScreenToWorldPoint(vector3 position)

//RayCast estudar pra fazer inimigos no nosso jogo de terror