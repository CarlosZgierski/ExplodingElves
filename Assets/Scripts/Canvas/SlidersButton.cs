using UnityEngine;

public class SlidersButton : MonoBehaviour
{
    [SerializeField] Animator animator;

    bool open;

    public void ButtonCallback()
    {
        open = !open;
        animator.SetBool("Open", open);
    }
}
