using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {

    // Disable the constructor so that
    // this must be a singleton.
    protected GameManager() {}

    public Company playerCompany = new Company("Thwonk Inc");


    private int weekTime = 15;
    private Month _month = Month.January;
    public string month {
        get { return _month.ToString(); }
    }
    private int _year = 1;
    public int year {
        get { return 2014 + _year; }
    }
    public int week = 0;

    private enum Month {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }


    public List<ProductType> unlockedProductTypes = new List<ProductType>();
    public List<Industry> unlockedIndustries = new List<Industry>();
    public List<Market> unlockedMarkets = new List<Market>();

    public List<Worker> unlockedWorkers = new List<Worker>();

    private List<GameEvent> gameEvents = new List<GameEvent>();

    // A list of events which could possibly occur.
    private List<GameEvent> candidateEvents = new List<GameEvent>();

    public void NewGame(string companyName) {
        playerCompany = new Company(companyName);
        Application.LoadLevel("Game");
    }

    public void HireWorker(Worker worker) {
        playerCompany.HireWorker(worker);

        // TO DO worker shouldn't be removed from unlockedWorkers
        // but instead from availableWorkers.
        unlockedWorkers.Remove(worker);
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);
        LoadResources();
    }

    void Start() {

        StartCoroutine(Weekly());
        StartCoroutine(Monthly());
        StartCoroutine(Yearly());

        //Debug.Log(gameEvents.Count);
        //Debug.Log(System.Guid.NewGuid());
    }

    void Update() {
    }

    IEnumerator Yearly() {
        int yearTime = weekTime*4*12;
        yield return new WaitForSeconds(yearTime);
        while(true) {
            _year++;
            yield return new WaitForSeconds(yearTime);
        }
    }

    IEnumerator Monthly() {
        int monthTime = weekTime*4;
        yield return new WaitForSeconds(monthTime);
        while(true) {

            if (_month == Month.December) {
                _month = Month.January;
            } else {
                _month++;
            }

            playerCompany.Pay();
            yield return new WaitForSeconds(monthTime);
        }
    }

    IEnumerator Weekly() {
        yield return new WaitForSeconds(weekTime);
        while(true) {
            if (week == 3) {
                week = 0;
            } else {
                week++;
            }
            playerCompany.DevelopProducts();
            yield return new WaitForSeconds(weekTime);
        }
    }

    public void LoadResources() {
        List<GameEvent> gameEvents = new List<GameEvent>(Resources.LoadAll<GameEvent>("GameEvents"));
        unlockedWorkers = Worker.LoadAll(WorkerType.Employee);

        unlockedProductTypes = ProductType.LoadAll();
        unlockedIndustries = Industry.LoadAll();
        unlockedMarkets = Market.LoadAll();
    }

    void EnableEvent(GameEvent gameEvent) {
        // Add to candidates.
        candidateEvents.Add(gameEvent);

        // Subscribe to its effect events.
        gameEvent.EffectEvent += OnEffect;
    }

    void DisableEvent(GameEvent gameEvent) {
        if (candidateEvents.Contains(gameEvent)) {
            // Unsubscribe and remove.
            gameEvent.EffectEvent -= OnEffect;
            candidateEvents.Remove(gameEvent);
        }
    }

    void OnEffect(GameEffect effect) {
        playerCompany.ApplyEffect(effect);
    }
}


