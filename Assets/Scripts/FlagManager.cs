using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Flag manager saves all potential flags in a managements system.
/// Can add flags (as strings), see flags, remove flags, and remove all flags
/// </summary>
public class FlagManager : MonoBehaviour
{
    public bool DebugRemoveFlagsFromScene = false;
    public static FlagManager Instance { get; private set; }
    private HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        // make sure there is only one flag manager in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // flag manager stays persistent across scenes

        if (DebugRemoveFlagsFromScene)
        {
            RemoveAllFlags();
            DebugRemoveFlagsFromScene = false;
        }
        
        LoadFlags();
    }

    /// <summary>
    /// Add a unique flag to the flag manager.
    /// </summary>
    /// <param name="flag">flag represented by a unique string</param>
    public void AddFlag(string flag)
    {
        if (!flags.Contains(flag))
        {
            flags.Add(flag);
            SaveFlags();
            Debug.Log("flag set: " + flag);
        }
        else 
        {
            Debug.LogWarning("A flag of this name already exists");
        }
    }

    /// <summary>
    /// See if the flag manager already has a flag by it's unique name
    /// </summary>
    /// <param name="flag">flag represented by a unique string</param>
    /// <returns></returns>
    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    /// <summary>
    /// Remove a specific flag from the flag manager
    /// </summary>
    /// <param name="flag">flag represented by a unique string</param>
    public void RemoveFlag(string flag)
    {
        if (flags.Contains(flag))
        {
            flags.Remove(flag);
            SaveFlags();
        }
    }

    /// <summary>
    /// Remove all flags from the flag manager
    /// </summary>
    public void RemoveAllFlags()
    {
        Debug.LogWarning("REMOVING ALL FLAGS. Disable debug mode to ensure flags stay across scenes!");
        flags.Clear();
        PlayerPrefs.SetInt("player_health", 100);
        SaveFlags();
    }

    /// <summary>
    /// Save all the current flags to the player prefs
    /// Should save it during a playthrough
    /// </summary>
    private void SaveFlags()
    {
        string flagsStr = string.Join("|", flags);
        PlayerPrefs.SetString("Flags", flagsStr);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load all flags in the flag manager
    /// </summary>
    private void LoadFlags()
    {
        string flagsStr = PlayerPrefs.GetString("Flags", "");
        flags = new HashSet<string>(flagsStr.Split('|', System.StringSplitOptions.RemoveEmptyEntries));
    }
}
