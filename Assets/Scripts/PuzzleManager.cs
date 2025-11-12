using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages puzzle-related objects that become active/inactive based on flag conditions.
/// Place this on a central manager object in the scene.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public class PuzzleElement
    {
        public GameObject target;
         public string targetName;
        public string requiredFlag;
        public bool enableIfFlagPresent = true; // enable the object if the flag is present
        public bool disableIfFlagPresent = false; // sisable the object if the flag is present
        public bool oneTimeCheck = true; // check the flag only at start

        [HideInInspector] public bool hasAppliedEffect = false;
    }

    [Header("Puzzle Elements")]
    public List<PuzzleElement> puzzleElements;

    void Start()
    {
        // Run checks once for any one-time flags
        foreach (var element in puzzleElements)
        {
            
            if (element.target == null && !string.IsNullOrEmpty(element.targetName))
            {
                GameObject found = GameObject.Find(element.targetName);
                if (found != null)
                {
                    element.target = found;
                    Debug.Log($"Auto-assigned puzzle target: {element.targetName}");
                }
                else
                {
                    Debug.LogWarning($"Could not find puzzle element with name: {element.targetName}");
                }
            }
        }
    }

    void Update()
    {
        foreach (var element in puzzleElements)
        {
            if (!element.oneTimeCheck)
            {
                CheckFlagAndApply(element);
            }
        }
    }

    void CheckFlagAndApply(PuzzleElement element)
    {
        if (element == null || element.target == null || string.IsNullOrEmpty(element.requiredFlag))
            return;

        bool flagSet = FlagManager.Instance.HasFlag(element.requiredFlag);

        if (element.enableIfFlagPresent && !flagSet)
        {
            if (element.target.activeSelf)
            {
                element.target.SetActive(false);
                Debug.Log($"[PuzzleManager] Disabled {element.target.name} because flag '{element.requiredFlag}' is missing");
            }
            if (element.oneTimeCheck) element.hasAppliedEffect = true;
        }

        if (element.enableIfFlagPresent && flagSet && !element.target.activeSelf)
        {
            element.target.SetActive(true);
            if (element.oneTimeCheck) element.hasAppliedEffect = true;
            Debug.Log($"Enabled {element.target.name} due to flag '{element.requiredFlag}'");
        }


        if (element.disableIfFlagPresent && flagSet && element.target.activeSelf)
        {
            element.target.SetActive(false);
            if (element.oneTimeCheck) element.hasAppliedEffect = true;
            Debug.Log($"Disabled {element.target.name} due to flag '{element.requiredFlag}'");
        }
    }
}
