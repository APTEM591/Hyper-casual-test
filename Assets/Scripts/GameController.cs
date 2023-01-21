using System.Collections;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the game state (goal, progress, etc.)
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private HandsController handsController;

    [SerializeField] private Transform basket;

    [Header("Goal generation")]
    [SerializeField] private TMPro.TextMeshProUGUI goalText;

    [Tooltip("+1 text which shows up when new item appeared in basket.")]
    [SerializeField] private Animation newItemTextPrefab;

    [Range(1, 10)]
    [Tooltip("Maximum amount of items, which may be choosen as goal.")]
    [SerializeField] private int maxCount;

    [System.Serializable]
    public class GoalCompletedEvent : UnityEvent { }
    public GoalCompletedEvent OnGoalCompleted = new GoalCompletedEvent();

    private int goalCount;
    private FoodType goalType;

    private void Awake()
    {
        GenerateGoal();
    }

    private void GenerateGoal()
    {
        goalType = (FoodType)Random.Range(0, System.Enum.GetNames(typeof(FoodType)).Length);
        goalCount = Random.Range(1, maxCount+1);

        UpdateGoal();
    }

    private void UpdateGoal()
    {
        goalText.text = $"Collect {goalCount} {goalType.ToString()}s";

        if (goalCount == 0)
            OnGoalCompleted.Invoke();
    }

    private IEnumerator ShowAddedCount()
    {
        var text = Instantiate(newItemTextPrefab, basket.position, default);
        yield return new WaitForSeconds(newItemTextPrefab.clip.length);

        Destroy(text.gameObject);
    }

    private void Update()
    {
        foreach (var touch in Input.touches)
        {
            if (handsController.IsBusy)
                return;

            var ray = Camera.main.ScreenPointToRay(touch.position);

            RaycastHit hit;
            if (!Physics.Raycast(ray.origin, ray.direction, out hit))
                continue;

            if (!hit.transform.CompareTag("Food"))
                continue;

            //Checking if food type matches goal
            var food = hit.transform.GetComponent<Food>();
            if (food.FoodType != goalType)
                continue;

            if (goalCount == 0)
                return;

            SendTakeItem(food.transform);
        }
    }

    private async void SendTakeItem(Transform item)
    {
        await handsController.TakeItem(item);
        goalCount--;
        UpdateGoal();
        
        //Show the "+1" text
        StartCoroutine(ShowAddedCount());
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
