using TMPro;
using UnityEngine;

namespace Flexus.Visual
{
    /// <summary>
    /// Displays a numeric value on a <see cref="TextMeshProUGUI"/> component.
    /// Intended for HUD elements that reflect live slider values (e.g. launch power).
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class SliderValueDisplay : MonoBehaviour
    {
        // ── Private state ────────────────────────────────────────────────

        private TextMeshProUGUI _label;

        // ── Unity lifecycle ──────────────────────────────────────────────

        private void Awake() => _label = GetComponent<TextMeshProUGUI>();

        // ── Public API ───────────────────────────────────────────────────

        // Cache common values to avoid garbage collection on slider drag
        private static readonly string[] CachedStrings = new string[201];

        static SliderValueDisplay()
        {
            for (int i = 0; i <= 200; i++)
            {
                CachedStrings[i] = i.ToString();
            }
        }

        /// <summary>Updates the displayed text to show <paramref name="value"/>.</summary>
        public void ChangeValue(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            if (intValue >= 0 && intValue <= 200)
            {
                _label.text = CachedStrings[intValue];
            }
            else
            {
                _label.text = value.ToString("F0");
            }
        }
    }
}
