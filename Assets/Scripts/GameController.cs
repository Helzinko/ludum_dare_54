using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] private int evilChooseNumber = 5;

    [SerializeField] private PlayFabManager playFabManager;
    [SerializeField] private UIController uIController;
    [SerializeField] private EnemySpawner enemySpawner;

    [SerializeField] private SpriteRenderer pickupSpawnIndication;

    [SerializeField] private Pickup pickupObject;
    [SerializeField] private Pickup evilPickupObject;

    [SerializeField] private BoxCollider2D spawnCollider;

    [SerializeField] private GameObject arena;

    [SerializeField] private GameObject gameOverCircle;

    private Bounds spawnBounds;

    private int score = 0;
    private int highscore = 0;

    private int pickupsCollected = 0;

    [SerializeField] private int defaultScore = 10;
    [SerializeField] private float speedScoreMultiplication = 2;
    [SerializeField] private float speedComboScoreMultiplication = 2;

    [SerializeField] private float timeBonusTime = 2.5f;

    private float currentBonusTime = 0f;
    private int currentComboCount = 0;

    [SerializeField] private PlayerController player;

    public bool isPaused { private set; get; } = false;

    private GameObject gameOverVisual;

    public bool canRestart { private set; get; } = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spawnBounds = spawnCollider.bounds;

        nextSpawnPos = GetRandomArenaPosition();
        pickupSpawnIndication.transform.position = GetRandomArenaPosition();

        SpawnPickup();
    }

    public Vector2 GetRandomArenaPosition()
    {
        return new Vector2(
        Random.Range(spawnBounds.min.x, spawnBounds.max.x),
        Random.Range(spawnBounds.min.y, spawnBounds.max.y));
    }

    private void Update()
    {
        currentBonusTime -= 1.0f / timeBonusTime * Time.deltaTime;
        uIController.hud.UpdateBonusScoreFill(currentBonusTime);
    }

    private bool firstPlay = true;
    private bool showedThisPlayHighscore = false;

    public void PickedPickup(bool isEvil, Vector2 position)
    {
        if (isEvil)
        {
            uIController.evilSelectionScreen.Open();
        }

        pickupsCollected++;

        float scoreToAdd = 0;

        if (currentBonusTime > 0)
        {
            currentComboCount++;
            scoreToAdd = defaultScore * speedScoreMultiplication * (currentComboCount >= 5 ? speedComboScoreMultiplication : 1f);
        }
        else
        {
            currentComboCount = 0;
            scoreToAdd = defaultScore;
        }

        score += (int)scoreToAdd;
        uIController.hud.UpdateScoreIndicationPosition(position, (int)scoreToAdd, currentBonusTime > 0, currentComboCount);
        uIController.hud.UpdateScore(score);

        currentBonusTime = 1f;

        if (score > highscore)
        {
            highscore = score;

            if (!firstPlay && !showedThisPlayHighscore)
            {
                uIController.hud.UpdateHighscorePosition(new Vector2(position.x, position.y + 0.5f));
                showedThisPlayHighscore = true;
            }
        }

        SpawnPickup();
    }

    private Vector2 nextSpawnPos;

    private void SpawnPickup()
    {
        if (player.isDead) return;

        var evilCheck = pickupsCollected % evilChooseNumber == 0;

        if (evilCheck) pickupsCollected = 0;

        var pickup = Instantiate(evilCheck ? evilPickupObject : pickupObject, nextSpawnPos, default, transform);
        pickup.isEvil = evilCheck;

        nextSpawnPos = new Vector2(
        Random.Range(spawnBounds.min.x, spawnBounds.max.x),
        Random.Range(spawnBounds.min.y, spawnBounds.max.y));

        pickupSpawnIndication.transform.position = nextSpawnPos;
    }

    public void GameOver()
    {
        playFabManager.SendLeaderboard(score);

        uIController.HudActive(false);

        DOVirtual.DelayedCall(0.25f, () => {
            gameOverVisual = Instantiate(gameOverCircle, player.transform.position, default, transform);
            gameOverVisual.transform.DOScale(25f, 1f).SetEase(Ease.InOutSine).OnComplete(() => {
                uIController.GameOverScreenActive(true);
                arena.SetActive(false);
                ClearLevel();

                uIController.HudActive(false);

                DOVirtual.DelayedCall(0.5f, () => canRestart = true);
            });
        });

        uIController.gameOverScreen.UpdateScore(score, highscore);

        firstPlay = false;
    }

    public void Restart()
    {
        pickupSpawnIndication.transform.position = GetRandomArenaPosition();

        uIController.HudActive(true);
        uIController.GameOverScreenActive(false);

        arena.SetActive(true);

        uIController.evilSelectionScreen.Restart();

        showedThisPlayHighscore = false;
        currentComboCount = 0;
        currentBonusTime = 0f;
        pickupsCollected = 0;
        score = 0;

        uIController.hud.UpdateScore(score);
        SpawnPickup();

        Destroy(gameOverVisual);
        canRestart = false;
    }

    private void ClearLevel()
    {
        foreach (var pickup in FindObjectsOfType<Pickup>()) Destroy(pickup.gameObject);
        foreach (var enemy in FindObjectsOfType<Enemy>()) Destroy(enemy.gameObject);
        foreach (var clearableItem in GameObject.FindGameObjectsWithTag("DestroyOnRestart")) Destroy(clearableItem);
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
        Time.timeScale = pause ? 0 : 1;
    }

    public void AddEnemy(Enemy enemy)
    {
        enemySpawner.AddEnemy(enemy);
    }

    public List<IModification> GetCurrentModifications()
    {
        return uIController.evilSelectionScreen.currentlyActiveModifications;
    }
}
