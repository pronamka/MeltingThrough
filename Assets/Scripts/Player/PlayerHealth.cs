using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [Header("Death UI Settings")]
    [SerializeField] protected GameObject deathCanvas;
    [SerializeField] protected float showCanvasDelay = 2f;
    [SerializeField] protected float canvasDisplayTime = 60f;
    [SerializeField] protected string menuSceneName = "MenuScene";

    protected void Start()
    {
        deathCanvas.SetActive(false);
    }

    public override void HandleDeath()
    {
        isDead = true;
        animator.SetTrigger(AnimationParameters.Death);
        SoundManager.instance.PlaySound(deathSound);

        float deathAnimationDuration = utils.GetAnimationDuration(AnimationNames.Death);
        Invoke(nameof(DisableEntity), deathAnimationDuration);

        StartCoroutine(DeathSequence());

    }

    public IEnumerator DeathSequence()
    {

        yield return new WaitForSeconds(showCanvasDelay);

        deathCanvas.SetActive(true);

        yield return new WaitForSeconds(canvasDisplayTime);

        SceneManager.LoadScene(menuSceneName);
    }

}
