using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Portal : MonoBehaviour
{
    [SerializeField] private O_Region regionIn;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private TraverseDirection toDirection;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool isOpen;
    [HideInInspector] public O_Region regionTeleportTo;

    private void Start()
    {
        isOpen = false;
    }

    private void Update()
    {
        if (isOpen)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen)
        {
            if (collision.TryGetComponent<O_Character>(out O_Character player))
            {
                TraverseDirection fromDirection = TraverseDirection.Leftwards;
                switch (toDirection)
                {
                    case TraverseDirection.Leftwards: fromDirection = TraverseDirection.Rightwards; break;
                    case TraverseDirection.Rightwards: fromDirection = TraverseDirection.Leftwards; break;
                    case TraverseDirection.Upwards: fromDirection = TraverseDirection.Downwards; break;
                    case TraverseDirection.Downwards: fromDirection = TraverseDirection.Upwards; break;
                }
                regionIn.PlayerTryTraverse(regionTeleportTo, fromDirection);
            }
        }
    }

    public void OpenAndLinkRegion(O_Region regionLinkedTo)
    {
        isOpen = true;
        regionTeleportTo = regionLinkedTo;
    }

}
