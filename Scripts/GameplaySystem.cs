using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum GameplayState
{
    Ready,
    Main,
    Failed,
    Succeed
}

public class GameplaySystem : StaticSerializedMonoBehaviour<GameplaySystem>
{
    public class ScrollLevel
    {
        public float speed;
        public float duration;
    }

    [SerializeField] private ScrollLevel[] scrollLevels;
    [SerializeField] private float fullPlaytime;
    [SerializeField,FoldoutGroup("Playables")] private PlayableAsset introTimeline;

    [Title("")]
    [SerializeField] private GameObject whiteFade;
    [SerializeField] private AudioSource music;
    [SerializeField] private Transform generateThreshold;
    [SerializeField] private Transform BubbleObject;
    [SerializeField] private ScrollMove backgroundScroll;
    public Vector2 BubblePosition { get { return new Vector2(BubbleObject.position.x, BubbleObject.position.y); } }
    [SerializeField] private Slider timeBar;

    private float gameplayTimer = 0f;
    private float currentSpeed = 0f;
    private int currentScrollLevel = 0;
    private bool playerHarmedFlag = false;
    private bool bubblePoppedFlag = false;
    private GameplayState gameplayState = GameplayState.Ready;
    public GameplayState GameState { get { return gameplayState; } }

    private PlayableDirector playable;

    protected override void Awake()
    {
        base.Awake();
        playable = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        StartCoroutine(Cor_GameplaySequence());
    }

    private IEnumerator Cor_GameplaySequence()
    {
        while(PatternGenerator.Instance.LastGeneratedEnd.y < generateThreshold.position.y)
        {
            PatternGenerator.Instance.GeneratePattern(gameplayTimer);
        }

        playable.playableAsset = introTimeline;
        playable.Play();
        yield return new WaitUntil(() => playable.state != PlayState.Playing);

        gameplayState = GameplayState.Main;

        NightmareSpawn.Instance.StartSpawn();

        music.Play();

        yield return StartCoroutine(Cor_MainGame());

        if(gameplayState == GameplayState.Failed)
        {
            Debug.Log("GameFailed");
            SceneLoader.Instance.LoadNewScene("Failed");
        }
        else if(gameplayState == GameplayState.Succeed)
        {
            NightmareSpawn.Instance.AbortSpawn();
            NightmareSpawn.Instance.ClearSpawned();

            PlayerCore.Instance.controlEnabled = false;
            PlayerCore.Instance.RigBody.gravityScale = 0f;
            PlayerCore.Instance.RigBody.freezeRotation = false;
            PlayerCore.Instance.RigBody.drag = 0.5f;
            PlayerCore.Instance.RigBody.totalTorque = 1f;
            PlayerCore.Instance.GetComponent<BoxCollider2D>().enabled = false;

            for (float time = 0; time < 10f; time += Time.fixedDeltaTime)
            {
                PlayerCore.Instance.RigBody.AddForce((BubbleObject.position + new Vector3(Mathf.Cos(time*2)*2,Mathf.Sin(time*2)*2) - PlayerCore.Instance.transform.position).normalized * 30);
                BubbleObject.position += new Vector3(0f, 0.07f, 0f);
                yield return new WaitForFixedUpdate();
            }

            whiteFade.SetActive(true);

            yield return new WaitForSeconds(1f);

            Debug.Log("GameSucceed");
            SceneLoader.Instance.LoadNewScene("Clear");
        }
        else
        {
            Debug.Log("GameResultError");
        }

    }

    float levelTimer = 0f;

    private IEnumerator Cor_MainGame()
    {
        if (PatternGenerator.Instance.LastGeneratedEnd.y < generateThreshold.position.y)
        {
            PatternGenerator.Instance.GeneratePattern(gameplayTimer);
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.5f);

        PlayerCore.Instance.controlEnabled = true;

        while(gameplayTimer < fullPlaytime)
        {
            if (PatternGenerator.Instance.LastGeneratedEnd.y < generateThreshold.position.y)
            {
                PatternGenerator.Instance.GeneratePattern(gameplayTimer);
            }

            if (gameplayState != GameplayState.Main) yield break;

            if (playerHarmedFlag)
            {
                currentSpeed = 0f;
                yield return new WaitForSeconds(3.0f);
                gameplayState = GameplayState.Failed;
                yield break;
            }

            if (bubblePoppedFlag)
            {
                currentSpeed = 0f;
                gameplayState = GameplayState.Failed;
                yield break;
            }

            BubbleObject.transform.position += Vector3.up * Mathf.Lerp(currentSpeed,scrollLevels[currentScrollLevel].speed,0.05f) * Time.timeScale;

            if (levelTimer > scrollLevels[currentScrollLevel].duration)
            {
                if (currentScrollLevel <= scrollLevels.Length)
                {
                    currentScrollLevel++;
                    levelTimer = 0f;
                }
            }

            timeBar.value = gameplayTimer / fullPlaytime;
            backgroundScroll.scrollValue = gameplayTimer / fullPlaytime;

            gameplayTimer += Time.deltaTime;
            levelTimer += Time.deltaTime;
            yield return null;
        }

        gameplayState = GameplayState.Succeed;
    }

    public void OnPlayerHarmed()
    {
        PlayerCore.Instance.controlEnabled = false;
        playerHarmedFlag = true;
        NightmareSpawn.Instance.AbortSpawn();
        NightmareSpawn.Instance.ClearSpawned();
    }

    public void OnBubblePopped()
    {
        PlayerCore.Instance.controlEnabled = false;
        bubblePoppedFlag = true;
        NightmareSpawn.Instance.AbortSpawn();
        NightmareSpawn.Instance.ClearSpawned();
    }
}
