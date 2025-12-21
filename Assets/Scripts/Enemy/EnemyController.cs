using UnityEngine;

/// <summary>
/// Enemy kontrolü - Kill animation sonrasý belirli pozisyon/rotation'a ayarla
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Death Transform")]
    [SerializeField] private Vector3 deathPosition;
    [SerializeField] private Vector3 deathRotation = new Vector3(90f, 0f, 0f);

    private bool isDead = false;

    /// <summary>
    /// Kill animation bittikten sonra çaðrýlýr
    /// Inspector'daki pozisyon/rotation'a ayarla
    /// </summary>
    public void SetDeathTransform()
    {
        if (isDead) return;

        isDead = true;

        transform.position = deathPosition;
        transform.rotation = Quaternion.Euler(deathRotation);
    }

    public bool IsDead() => isDead;
}