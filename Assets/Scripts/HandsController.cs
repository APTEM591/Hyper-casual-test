using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Animations.Rigging;

using DG.Tweening;

public class HandsController : MonoBehaviour
{
    [Tooltip("Items size, which are moved to the basket")]
    [SerializeField] private float basketItemsSize = 0.3f;

    [Tooltip("Speed of right hand's move")]
    [SerializeField] private float handMoveDuration;

    [Header("Hands")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [Header("Basket holding")]
    [SerializeField] private Transform basket;
    [SerializeField] private Vector3 basketHandOffset;
    [SerializeField] private Vector3 basketItemPutOffset;

    private Vector3 rightHandDefaultPosition;

    /// <summary>
    /// Is right hand beign used by script
    /// </summary>
    public bool IsBusy { get { return isBusy; } }
    private bool isBusy;

    private void Awake()
    {
        rightHandDefaultPosition = rightHand.position;
        TakeBasket();
    }

    /// <summary>
    /// Moving left hand to the basket
    /// </summary>
    private void TakeBasket()
    {
        leftHand.position = basket.position + basketHandOffset;

        //Anchoring basket position to object without parenting to prevent unexpected position and rotation
        var handObject = leftHand.GetComponent<TwoBoneIKConstraint>().data.tip;
        StartCoroutine(UpdateBasketPosition(handObject));
    }

    private System.Collections.IEnumerator UpdateBasketPosition(Transform anchoredObject)
    {
        while(true)
        {
            basket.position = anchoredObject.position - basketHandOffset;
            yield return new WaitForEndOfFrame();
        }
    }

    public async Task TakeItem(Transform item)
    {
        isBusy = true;
        item.SetParent(null);

        //Moving item to the hand
        var sequence = DOTween.Sequence();
        sequence.Append(rightHand.DOMove(item.position, handMoveDuration));
        sequence.Play();

        await sequence.AsyncWaitForCompletion();

        //Moving hand with an item to the basket
        item.SetParent(rightHand);
        var secondSequence = DOTween.Sequence();
        secondSequence.Append(rightHand.DOJump(basket.position + basketItemPutOffset, 1, 1, handMoveDuration));
        
        //Change item size to fit the basket bounds
        secondSequence.Join(item.DOScale(item.localScale * basketItemsSize, handMoveDuration));
        secondSequence.Play();

        await secondSequence.AsyncWaitForCompletion();

        item.GetComponent<Rigidbody>().isKinematic = false;
        item.SetParent(basket);

        rightHand.DOMove(rightHandDefaultPosition, handMoveDuration);
        isBusy = false;
    }
}
