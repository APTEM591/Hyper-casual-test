using UnityEngine;

public enum FoodType
{
    Apple,
    Orange,
    Banana,
    Potato
}

public class Food : MonoBehaviour
{
    public FoodType FoodType { get { return foodType; } }
    [SerializeField] private FoodType foodType;

    private Vector3 startPosition;

    private BeltMove beltMove;

    private void Start()
    {
        startPosition = transform.position;

        beltMove = GetComponentInParent<BeltMove>();
        beltMove.OnPositionChange.AddListener(UpdatePosition);
    }

    private void UpdatePosition(Vector2 velocity)
    {
        if(transform.parent == null)
        {
            RemovePositionListener();
            return;
        }

        transform.position += (Vector3)velocity;
        if (transform.position.x < -startPosition.x)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        RemovePositionListener();
    }

    private void RemovePositionListener() => beltMove.OnPositionChange.RemoveListener(UpdatePosition);


}
