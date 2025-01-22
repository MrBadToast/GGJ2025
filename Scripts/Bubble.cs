using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Bubble : StaticSerializedMonoBehaviour<Bubble>
{
    [SerializeField] private Sprite lifeFilled;
    [SerializeField] private Sprite lifeBlank;
    [SerializeField] private float barrierDuration = 5f;

    [Title("")]
    [SerializeField] private PlayableDirector playable;
    [SerializeField] private GameObject barrierObejct;
    [SerializeField] private SpriteRenderer[] lifeSprites;
    [SerializeField] private DOTweenAnimation lifeTween;

    [SerializeField,ReadOnly] int lifeCounts;

    private void Start()
    {
        lifeCounts = lifeSprites.Length;
    }

    [Button()]
    public void DimisihLife()
    {
        if (lifeCounts == 0) return;

        lifeCounts--;
        lifeSprites[lifeCounts].sprite = lifeBlank;
        lifeTween.DORestart();


        if(lifeCounts == 0)
        {
            StopAllCoroutines();
            GameplaySystem.Instance.OnBubblePopped();
            lifeTween.gameObject.SetActive(false);
            StartCoroutine(Cor_Gameover());
        }
    }

    public void ActivateBarrier()
    {
        StopCoroutine("Cor_Barrier");
        StartCoroutine("Cor_Barrier");
    }

    private IEnumerator Cor_Barrier()
    {
        barrierObejct.SetActive(true);
        barrierObejct.GetComponent<Animator>().Play("BarrierActive");

        yield return new WaitForSeconds(barrierDuration);

        barrierObejct.SetActive(false);
    }

    private IEnumerator Cor_Gameover()
    {
        Time.timeScale = 0f;

        playable.Play();

        yield return new WaitUntil(()=>playable.state != PlayState.Playing);

        Time.timeScale = 1f;

        GameplaySystem.Instance.OnBubblePopped();
    }
}
