using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class BeltMove : MonoBehaviour
{
    [Tooltip("Move speed of the belt. If lower than 0, will move in opposite side.")]
    [SerializeField] private float moveSpeed;

    public class PositionChangedEvent : UnityEvent<Vector2> {}
    public readonly PositionChangedEvent OnPositionChange = new PositionChangedEvent();

    private MeshRenderer beltRenderer;

    private readonly string mainTextureProperty = "_MainTex";

    private void Awake()
    {
        beltRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //Updating belt texture offset to simulate moving
        var currentOffset = beltRenderer.material.GetTextureOffset(mainTextureProperty);

        var velocity = new Vector2(Time.deltaTime * moveSpeed, 0);
        currentOffset += velocity;

        beltRenderer.material.SetTextureOffset(mainTextureProperty, currentOffset);
        
        //Velocity based on speed of "moving" offset
        var scaledVelocity = velocity * (beltRenderer.bounds.size.x / beltRenderer.material.mainTextureScale.x);
        OnPositionChange.Invoke(scaledVelocity);
    }
}
