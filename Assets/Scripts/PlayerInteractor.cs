using UnityEngine;

public class PlayerInteractor: MonoBehaviour
{
    [Header("Player Camera")]
    public Camera cam;            

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float maxDistance = 3.0f;
    public LayerMask interactMask = ~0;   
    public bool debugRay = false;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!Input.GetKeyDown(interactKey) || !cam) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (debugRay) Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.cyan, 0.25f);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactMask, QueryTriggerInteraction.Collide))
            return;

        var flaggable = hit.collider.GetComponentInParent<FlagInteractionBehavior>();
        if (!flaggable) return;

        if (flaggable.requiresAnotherItem && !FlagManager.Instance.HasFlag(flaggable.requiredItemID))
        {
            Debug.LogWarning($"Needs flag '{flaggable.requiredItemID}' before setting '{flaggable.flagID}'.");
            return;
        }

        if (flaggable.requiresInteract == false)
        {
            return;
        }

        flaggable.RegisterFlag();
    }
}
