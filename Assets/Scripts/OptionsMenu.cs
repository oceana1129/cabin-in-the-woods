using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown displayModeDropdown;
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public TMP_Text volumePercentageText;

    [Header("Panels")]
    public GameObject optionsMenuUI;
    public GameObject pauseMenuUI;

    private struct Aspect { public int w, h; public float ratio; public string label; }
    private readonly List<Aspect> aspects = new List<Aspect>();
    private readonly List<Resolution> filteredRes = new List<Resolution>();
    private const float RATIO_EPS = 0.01f;

    void Start()
    {
        float initialVolume = PlayerPrefs.HasKey("opt_volume") ? PlayerPrefs.GetFloat("opt_volume") : 0.5f;
        PlayerPrefs.SetFloat("opt_volume", initialVolume);
        AudioListener.volume = initialVolume;
        if (volumeSlider)
        {
            volumeSlider.value = initialVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        UpdateVolumeText(initialVolume);
        InitDisplayModeDropdown();
        BuildAspectRatios();
        SyncAspectDropdownState();
        BuildResolutionList();
    }

    public void Return()
    {
        if (optionsMenuUI) optionsMenuUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("opt_volume", AudioListener.volume);
        UpdateVolumeText(value);
    }

    private void UpdateVolumeText(float value)
    {
        if (volumePercentageText)
            volumePercentageText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void InitDisplayModeDropdown()
    {
        if (!displayModeDropdown) return;
        displayModeDropdown.options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Windowed"),
            new TMP_Dropdown.OptionData("Borderless"),
            new TMP_Dropdown.OptionData("Fullscreen")
        };
        displayModeDropdown.value = ModeToIndex(Screen.fullScreenMode);
        displayModeDropdown.RefreshShownValue();
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
    }

    private int ModeToIndex(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.Windowed: return 0;
            case FullScreenMode.FullScreenWindow: return 1;
            case FullScreenMode.ExclusiveFullScreen: return 2;
            default: return Screen.fullScreen ? 2 : 0;
        }
    }

    public void SetDisplayMode(int index)
    {
        int curW = Screen.width;
        int curH = Screen.height;
        if (index == 0)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(curW, curH, FullScreenMode.Windowed);
        }
        else if (index == 1)
        {
            int sw = Display.main.systemWidth;
            int sh = Display.main.systemHeight;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(sw, sh, FullScreenMode.FullScreenWindow);
        }
        else
        {
            int sw = Display.main.systemWidth;
            int sh = Display.main.systemHeight;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(sw, sh, FullScreenMode.ExclusiveFullScreen);
        }
        SyncAspectDropdownState();
        SyncResolutionDropdownState();
        BuildResolutionList();
    }

    private void SyncAspectDropdownState()
    {
        bool windowed = Screen.fullScreenMode == FullScreenMode.Windowed;
        if (aspectRatioDropdown) aspectRatioDropdown.interactable = windowed;
    }

    private void SyncResolutionDropdownState()
    {
        bool borderless = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        if (resolutionDropdown) resolutionDropdown.interactable = !borderless;
    }

    private void BuildAspectRatios()
    {
        if (!aspectRatioDropdown) return;
        aspects.Clear();
        foreach (var r in Screen.resolutions)
        {
            Reduce(r.width, r.height, out int rw, out int rh);
            float ar = (float)rw / rh;
            bool exists = false;
            for (int i = 0; i < aspects.Count; i++)
            {
                if (Mathf.Abs(aspects[i].ratio - ar) < RATIO_EPS) { exists = true; break; }
            }
            if (!exists) aspects.Add(new Aspect { w = rw, h = rh, ratio = ar, label = $"{rw}:{rh}" });
        }
        aspects.Sort((a, b) => a.ratio.CompareTo(b.ratio));
        var opts = new List<TMP_Dropdown.OptionData>();
        int currentIndex = 0;
        float currentRatio = (float)Screen.width / Screen.height;
        for (int i = 0; i < aspects.Count; i++)
        {
            opts.Add(new TMP_Dropdown.OptionData(aspects[i].label));
            if (Mathf.Abs(aspects[i].ratio - currentRatio) < 0.02f) currentIndex = i;
        }
        aspectRatioDropdown.options = opts;
        aspectRatioDropdown.value = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, opts.Count - 1));
        aspectRatioDropdown.RefreshShownValue();
        aspectRatioDropdown.onValueChanged.AddListener(SetAspectRatio);
    }

    public void SetAspectRatio(int index)
    {
        if (index < 0 || index >= aspects.Count) return;
        if (Screen.fullScreenMode != FullScreenMode.Windowed) return;
        var target = aspects[index];
        int height = Mathf.Max(Screen.height, 480);
        int width = Mathf.RoundToInt(height * target.ratio);
        width = Mathf.Clamp(width, 640, Display.main.systemWidth - 40);
        height = Mathf.Clamp(height, 480, Display.main.systemHeight - 80);
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
        BuildResolutionList();
    }

    private void BuildResolutionList()
    {
        if (!resolutionDropdown) return;
        filteredRes.Clear();
        var all = Screen.resolutions;
        bool windowed = Screen.fullScreenMode == FullScreenMode.Windowed;
        bool exclusive = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
        bool borderless = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        float targetRatio = (float)Screen.width / Screen.height;
        if (windowed && aspectRatioDropdown && aspects.Count > 0)
        {
            int arIndex = Mathf.Clamp(aspectRatioDropdown.value, 0, aspects.Count - 1);
            targetRatio = aspects[arIndex].ratio;
        }
        for (int i = 0; i < all.Length; i++)
        {
            var r = all[i];
            if (windowed)
            {
                float rRatio = (float)r.width / r.height;
                if (Mathf.Abs(rRatio - targetRatio) > 0.02f) continue;
                if (r.width < 640 || r.height < 480) continue;
            }
            bool dupe = false;
            for (int j = 0; j < filteredRes.Count; j++)
            {
                if (filteredRes[j].width == r.width && filteredRes[j].height == r.height) { dupe = true; break; }
            }
            if (!dupe) filteredRes.Add(r);
        }
        bool hasCurrent = false;
        for (int i = 0; i < filteredRes.Count; i++)
        {
            if (filteredRes[i].width == Screen.width && filteredRes[i].height == Screen.height) { hasCurrent = true; break; }
        }
        if (!hasCurrent)
        {
            filteredRes.Add(new Resolution { width = Screen.width, height = Screen.height, refreshRate = Screen.currentResolution.refreshRate });
        }
        filteredRes.Sort((a, b) =>
        {
            int aa = a.width * a.height;
            int bb = b.width * b.height;
            if (aa != bb) return bb.CompareTo(aa);
            return b.width.CompareTo(a.width);
        });
        var opts = new List<TMP_Dropdown.OptionData>();
        int currentIndex = 0;
        for (int i = 0; i < filteredRes.Count; i++)
        {
            var r = filteredRes[i];
            string label = $"{r.width} x {r.height}";
            opts.Add(new TMP_Dropdown.OptionData(label));
            if (r.width == Screen.width && r.height == Screen.height) currentIndex = i;
        }
        resolutionDropdown.options = opts;
        resolutionDropdown.value = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, opts.Count - 1));
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        SyncResolutionDropdownState();
        if (borderless)
        {
            int sw = Display.main.systemWidth;
            int sh = Display.main.systemHeight;
            Screen.SetResolution(sw, sh, FullScreenMode.FullScreenWindow);
            resolutionDropdown.interactable = false;
        }
    }

    private void ApplyResolution(int index)
    {
        if (index < 0 || index >= filteredRes.Count) return;
        var r = filteredRes[index];
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            int sw = Display.main.systemWidth;
            int sh = Display.main.systemHeight;
            Screen.SetResolution(sw, sh, FullScreenMode.FullScreenWindow);
            return;
        }
        var mode = Screen.fullScreenMode;
        Screen.SetResolution(r.width, r.height, mode);
    }

    private static void Reduce(int w, int h, out int rw, out int rh)
    {
        int g = GCD(w, h);
        rw = w / g; rh = h / g;
        if (Nearly((float)rw / rh, 21f / 9f)) { rw = 21; rh = 9; }
        else if (Nearly((float)rw / rh, 16f / 9f)) { rw = 16; rh = 9; }
        else if (Nearly((float)rw / rh, 16f / 10f)) { rw = 16; rh = 10; }
        else if (Nearly((float)rw / rh, 4f / 3f)) { rw = 4; rh = 3; }
        else if (Nearly((float)rw / rh, 32f / 9f)) { rw = 32; rh = 9; }
    }

    private static bool Nearly(float a, float b, float eps = 0.01f) => Mathf.Abs(a - b) < eps;

    private static int GCD(int a, int b)
    {
        while (b != 0) { int t = b; b = a % b; a = t; }
        return Mathf.Abs(a);
    }
}
