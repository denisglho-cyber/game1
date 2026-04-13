using UnityEngine;

public class SwordSlash : MonoBehaviour {
    [SerializeField] private Sword sword;
    private Animator animator;
    private const string ATTACK = "attack";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Sword_OnSwordSwing(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }
}
