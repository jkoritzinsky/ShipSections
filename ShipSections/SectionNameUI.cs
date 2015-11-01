using System.Linq;
using UnityEngine;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SectionNameUI : KSPPluginFramework.MonoBehaviourWindow
    {
        private const int WindowWidth = 400, WindowHeight = 600;

        private readonly string AppLauncherIconLocation = "ShipSections/ShipSections";

        private string sectionBeingRenamed;
        private string newNameInProgress = string.Empty;
        private string currentlyHighlightedSection;

        internal override void DrawWindow(int id)
        {
            if (API.AnyCurrentVesel)
            {
                DrawSectionWindow(); 
            }
            else
            {
                GUILayout.Label("Create a craft to use ShipSections.");
            }
        }

        private void DrawSectionWindow()
        {
            using (new GuiLayout(GuiLayout.Method.Vertical))
            using (new GuiLayout(GuiLayout.Method.ScrollView, ref scrollPos))
            {
                string sectionNameToChange = null;
                foreach (var section in API.SectionNames)
                {
                    using (new GuiLayout(GuiLayout.Method.Horizontal))
                    {
                        GUILayout.Label(section);
                        if (sectionBeingRenamed == null && GUILayout.Button("Rename", GUILayout.ExpandWidth(true)))
                        {
                            newNameInProgress = sectionBeingRenamed = section;
                        }
                        else if(sectionBeingRenamed == section)
                        {
                            newNameInProgress = GUILayout.TextField(newNameInProgress);
                            if (!string.IsNullOrEmpty(newNameInProgress) && GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                            {
                                sectionNameToChange = section;
                            }
                        }
                        bool currentlyHighlighted = currentlyHighlightedSection == section;
                        if (GUILayout.Button(currentlyHighlighted ? "Unighlight": "Highlight", GUILayout.ExpandWidth(false)))
                        {
                            if (currentlyHighlighted)
                            {
                                API.PartsBySection.First(group => group.Key == currentlyHighlightedSection).ToList().ForEach(part => part.SetHighlight(false, false));
                                currentlyHighlightedSection = null;
                            }
                            else
                            {
                                var sectionParts = API.PartsBySection.First(group => group.Key == section);
                                sectionParts.ToList().ForEach(part => part.SetHighlight(true, false));
                                if (!currentlyHighlighted && currentlyHighlightedSection != null)
                                    API.PartsBySection.First(group => group.Key == currentlyHighlightedSection).ToList().ForEach(part => part.SetHighlight(false, false));
                                currentlyHighlightedSection = section;
                            }
                        }
                    }
                }
                if (sectionNameToChange != null)
                {
                    API.ChangeSectionName(sectionNameToChange, newNameInProgress);
                    newNameInProgress = string.Empty;
                    sectionBeingRenamed = null;
                }
            }
        }

        internal override void Start()
        {
            DragEnabled = true;
            WindowRect.Set((Screen.width - WindowWidth) / 4, (Screen.height - WindowHeight) / 2, WindowWidth, WindowHeight);
            WindowCaption = nameof(ShipSections);
            if (ApplicationLauncher.Instance != null && ApplicationLauncher.Ready)
                OnAppLauncherReady();
            else
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
        }

        private ApplicationLauncherButton button;
        private Vector2 scrollPos;

        private void OnAppLauncherReady()
        {
            if (button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
                button = null;
            }
            button = ApplicationLauncher.Instance.AddModApplication(
                () => Visible = true,
                () => Visible = false,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                GameDatabase.Instance.GetTexture(AppLauncherIconLocation, false));
        }

        internal override void OnDestroy()
        {
            ApplicationLauncher.Instance.RemoveModApplication(button);
        }
    }
}
