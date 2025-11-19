using UnityEngine;

/// <summary>
/// Used to add flag behavior to an individual object
/// Must call the registerFlag function in other scripts to register
/// </summary>
public class FlagInteractionBehavior : MonoBehaviour
{
    [Header("Flag String Settings")]
    public string flagID;
    public bool monitorFlagsContinuously = false;

    [Header("If Flag Need Another Flag")]
    public string requiredItemID;
    public bool requiresAnotherItem = false;    // requires an another item to be flagged

    [Header("Disable If Item Is Flagged")]
    public bool disableIfFlagged = false;   // disable the object this is attached to is flagged
    public bool disableOnLoadIfFlagged = false; // diable the object on load if flagged
    public bool disableIfRequiredIdFound = false;

    [Header("Conditions (optional)")]
    public bool requiresTriggerEnter = false;   // requires trigger enter to be flagged
    public bool requiresInteract = false;       // requires an interaction to be flagged

    // private
    private bool hasBeenFlagged = false; // if the item has been flagged
    private bool playerInRange = false;


    private void Start()
    {
        // Disable or change object based on flag state
        DisableOnLoadIfFlagged();
        DisableIfRequiredIdFound();
    }

    private void Update()
    {
        if (requiresInteract && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryRegisterWithConditions();
        }

        if (monitorFlagsContinuously)
        {
            if ( !string.IsNullOrEmpty(requiredItemID))
                DisableIfRequiredIdFound();
            if (disableIfFlagged)
                DisableObjectIfFlagged();
        }
    }

    /// <summary>
    /// Checks if conditions are met before registering the flag
    /// </summary>
    private void TryRegisterWithConditions()
    {
        if (requiresAnotherItem && !FlagManager.Instance.HasFlag(requiredItemID))
        {
            Debug.LogWarning($"Required flag {requiredItemID} not found for: {flagID}");
            return;
        }

        RegisterFlag();
    }
    void DisableOnLoadIfFlagged() 
    {
        bool hasThisFlag = FlagManager.Instance.HasFlag(flagID);
        if (disableOnLoadIfFlagged && hasThisFlag)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    /// <summary>
    /// Deactivate the current gameobject if it should not exist in the scene if flagged
    /// For example, could be for a key or puzzle
    /// </summary>
    void DisableObjectIfFlagged()
    {
        if (disableIfFlagged && FlagManager.Instance.HasFlag(flagID))
        {
            gameObject.SetActive(false);
        }
    }

    void DisableIfRequiredIdFound() 
    {
        bool hasRequiredFlag = FlagManager.Instance.HasFlag(requiredItemID);
        if (disableIfRequiredIdFound && hasRequiredFlag)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    /// <summary>
    /// Register this gameobjects flag
    /// Can trigger a flag via another script
    /// </summary>
    public void RegisterFlag()
    {
        if (hasBeenFlagged)
        {
            Debug.LogWarning(flagID + " has already been registered");
            return;
        }

        if (string.IsNullOrEmpty(flagID))
        {
            Debug.LogWarning("No flagID assigned to " + gameObject.name);
            return;
        }

        FlagManager.Instance.AddFlag(flagID);
        hasBeenFlagged = true;
        
        Debug.Log("added flag for " + flagID);
        if (disableIfFlagged)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Auto flag on trigger enter
    /// For example, walking through a door
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (requiresTriggerEnter && !requiresInteract)
        {
            TryRegisterWithConditions();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }
}
