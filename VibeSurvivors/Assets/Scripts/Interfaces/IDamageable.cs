using UnityEngine;

/// <summary>
/// Hasar alabilir nesneler için interface
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Nesneye hasar verir
    /// </summary>
    /// <param name="damage">Verilecek hasar miktar?</param>
    void TakeDamage(float damage);
    
    /// <summary>
    /// Nesnenin hala hayatta olup olmad???n? kontrol eder
    /// </summary>
    bool IsAlive { get; }
}
