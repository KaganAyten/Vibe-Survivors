using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    
    // Animator parametreleri
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Jump = Animator.StringToHash("Jump");
    
    private void Awake()
    {
   animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Ko?ma animasyonunu ayarlar
    /// </summary>
    /// <param name="isRunning">Karakter hareket ediyor mu?</param>
    public void SetRunning(bool isRunning)
    {
        animator.SetBool(IsRunning, isRunning);
    }
    
    /// <summary>
    /// Z?plama animasyonunu tetikler
    /// </summary>
    public void TriggerJump()
    {
        animator.SetTrigger(Jump);
    }
}
