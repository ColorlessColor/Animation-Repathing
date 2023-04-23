﻿using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    public class AARVariables
    {
        public static Vector2 scroll = Vector2.zero;
        public static int toolSelection;
        public static bool automaticIsEnabled;
        public static int manualToolSelection;
        public static string[] manualTools = new string[2];
        public static int controllerSelection;
        public static string[] controllerOptions = new string[2];
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool reparentWarning = true;
        public static bool activeInBackground = false;
        public static int languageSelection;
        public static string[] languageOptions = { "English", "日本" };

        public static List<InvalidSharedProperty> invalidSharedProperties = new List<InvalidSharedProperty>();
        public static Dictionary<string, InvalidSharedProperty> invalidPathToSharedProperty = new Dictionary<string, InvalidSharedProperty>();
        public static int invalidPosition;
        public static bool invalidPathsFoldout = true;
        public class InvalidSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
        }

        public static List<AnimationClip> clipsSelected = new List<AnimationClip>();
        public static List<ClipsSharedProperty> clipsSharedProperties = new List<ClipsSharedProperty>();
        public static Dictionary<string, ClipsSharedProperty> clipsPathToSharedProperty = new Dictionary<string, ClipsSharedProperty>();
        public static string clipsReplaceFrom = string.Empty;
        public static string clipsReplaceTo = string.Empty;
        public static bool clipsReplaceFoldout;
        public class ClipsSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
            public bool warning;
        }

        public static readonly List<AARAutomatic.HierarchyTransform> hierarchyTransforms = new List<AARAutomatic.HierarchyTransform>();
        public static readonly Dictionary<string, string> changedPaths = new Dictionary<string, string>();
        public static AnimatorController controller;
        public static Animator _animator;
        public static GameObject _avatar;
        public static Animator animator
        {
            get => _animator;
            set
            {
                if (_animator != value)
                {
                    _animator = value;
                    AARAutomatic.OnRootChanged();
                }
            }
        }
        public static GameObject avatar
        {
            get => _avatar;
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    AARAutomatic.OnRootChanged();
                }
            }
        }
        public enum Playables
        {
            Base = 1 << 0,
            Additive = 1 << 1,
            Gesture = 1 << 2,
            Action = 1 << 3,
            FX = 1 << 4,
            Sitting = 1 << 5,
            TPose = 1 << 6,
            IKPose = 1 << 7,
            all = ~0
        }
        public static Playables PlayableSelection = Playables.all;
    }

    public static class AARStyle
    {
        public static GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle toggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
        public static GUIStyle foldout = new GUIStyle(EditorStyles.foldout) { fontSize = 15, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        public static GUIStyle settings = new GUIStyle(GUI.skin.label) { fontSize = 20, richText = true };
        public static GUIStyle invalidPath = new GUIStyle(GUI.skin.box) { fontSize = 12, richText = true, stretchWidth = true, alignment = TextAnchor.MiddleLeft };
        public static GUIStyle invalidPathTip = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = true, stretchWidth = true, alignment = TextAnchor.MiddleCenter };
        public static GUIStyle customFoldout = new GUIStyle(GUI.skin.button) { fontSize = 11, richText = true };
        public static GUIStyle resetButton = new GUIStyle(GUI.skin.label) { };
        public static GUIStyle replaceFromTo = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle replacePath = new GUIStyle(GUI.skin.box) { fontSize = 15, fontStyle = FontStyle.Bold, stretchWidth = true, alignment = TextAnchor.MiddleCenter, richText = true };
    }

    public static class AARContent
    {
        public static GUIContent linkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
        public static GUIContent warningIcon = new GUIContent(EditorGUIUtility.IconContent("console.warnicon"));
        public static GUIContent passedIcon = new GUIContent(EditorGUIUtility.IconContent("TestPassed"));
    }

    [InitializeOnLoad]
    public static class AARSaveData
    {
        static AARSaveData()
        {
            LoadData();

            if (languageSelection == 0)
            {
                AARStrings.loadEnglisch();
            }
            else
            {
                AARStrings.loadJapanese();
            }
        }

        public static void SaveData()
        {
            EditorPrefs.SetInt("AAR BaseSelection", controllerSelection);
            EditorPrefs.SetInt("AAR LanguageSelection", languageSelection);

            EditorPrefs.SetBool("AAR Toggle", automaticIsEnabled);
            EditorPrefs.SetBool("AAR RenameActive", renameActive);
            EditorPrefs.SetBool("AAR ReparentActive", reparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", renameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", reparentWarning);
            EditorPrefs.SetBool("AAR ActiveInBackground", activeInBackground);
            EditorPrefs.SetString("AAR Controller", animator == null ? null : animator.gameObject.transform.name);

            EditorPrefs.SetString("AAR Avatar", avatar == null ? null : avatar.name);

            EditorPrefs.SetInt("AAR PlayableSelection", (int)PlayableSelection);
            LoadData();
        }

        public static void LoadData()
        {
            controllerSelection = EditorPrefs.GetInt("AAR BaseSelection");
            automaticIsEnabled = EditorPrefs.GetBool("AAR Toggle");
            renameActive = EditorPrefs.GetBool("AAR RenameActive");
            reparentActive = EditorPrefs.GetBool("AAR ReparentActive");
            renameWarning = EditorPrefs.GetBool("AAR RenameWarning");
            reparentWarning = EditorPrefs.GetBool("AAR ReparentWarning");
            activeInBackground = EditorPrefs.GetBool("AAR ActiveInBackground");
            PlayableSelection = (Playables)EditorPrefs.GetInt("AAR PlayableSelection");

            string findController = EditorPrefs.GetString("AAR Controller");
            GameObject animatorHolder = GameObject.Find(findController);
            if (animatorHolder != null)
            {
                animator = animatorHolder.GetComponent(typeof(Animator)) as Animator;
                controller = animator == null ? null : animator.runtimeAnimatorController as AnimatorController;
            }

            avatar = GameObject.Find(EditorPrefs.GetString("AAR Avatar"));
        }

        public static void ResetData()
        {
            controllerSelection = 0;

            animator = null;
            avatar = null;

            automaticIsEnabled = false;
            renameActive = true;
            reparentActive = true;
            renameWarning = true;
            reparentWarning = true;
            activeInBackground = false;

            PlayableSelection = Playables.all;

            SaveData();
        }
    }

    public static class AARStrings
    {
        public static class Main
        {
            public static string windowName;
            public static string automatic;
            public static string manual;
        }

        public static class Automatic
        {
            public static string title;
            public static string credit;
            public static string disabled;
            public static string enabled;
        }

        public static class Manual
        {
            public static string title;
            public static string credit;
            public static string fixPaths;
            public static string editClips;
        }

        public static class InvalidPaths
        {
            public static string invalidPaths;
            public static string apply;
            public static string affectedClips;
            public static string noInvalidPaths;
            public static string dragAndDrop;
        }

        public static class ClipEditing
        {
            public static string replacePartOfAll;
            public static string from;
            public static string to;
            public static string apply;
            public static string warning;
            public static string replaceIndividual;
            public static string noClipsSelected;
            public static string dragAndDrop;
        }

        public static class Settings
        {
            public static string settings;
            public static string target;
            public static string animatorComponent;
            public static string vrchatAvatar;
            public static string controllerToUse;
            public static string layersToUse;
            public static string avatarToUse;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
            public static string language;
        }

        public static class ToolTips
        {
            public static string automatic;
            public static string manual;
            public static string toggleButton;
            public static string fixPaths;
            public static string editClips;
            public static string resetInvalidPaths;
            public static string resetInvalidPath;
            public static string applyValidPath;
            public static string replacePartOfAll;
            public static string resetPartOfAll;
            public static string replaceFrom;
            public static string replaceTo;
            public static string applyPartOfAll;
            public static string replaceIndividual;
            public static string resetIndividual;
            public static string applyIndividual;
            public static string resetSettings;
            public static string target;
            public static string controllerToUse;
            public static string avatarToUse;
            public static string layersToTarget;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
        }

        public static void loadEnglisch()
        {
            Main.windowName = "Animation Repathing";
            Main.automatic = "Automatic";
            Main.manual = "Manual";

            Automatic.title = "Auto Animation Repathing";
            Automatic.credit = "Made by hfcRed";
            Automatic.disabled = "Disabled";
            Automatic.enabled = "Enabled";

            Manual.title = "Manual Animation Repathing";
            Manual.credit = "Made by hfcRed";
            Manual.fixPaths = "Fix Invalid Paths";
            Manual.editClips = "Edit Animation Clips";
            manualTools[0] = Manual.fixPaths;
            manualTools[1] = Manual.editClips;

            InvalidPaths.invalidPaths = "Invalid Paths";
            InvalidPaths.apply = "Apply";
            InvalidPaths.affectedClips = "Affected Clips";
            InvalidPaths.noInvalidPaths = "You have no invalid Paths in your controller";
            InvalidPaths.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            ClipEditing.replacePartOfAll = "Replace part of all Paths";
            ClipEditing.from = "From";
            ClipEditing.to = "To";
            ClipEditing.apply = "Apply";
            ClipEditing.warning = "One or more Paths contain the input string multiple times within it. Pressing apply replaces all instances of the string within the Paths!";
            ClipEditing.replaceIndividual = "Replace Paths individually";
            ClipEditing.noClipsSelected = "No Animation Clips selected";
            ClipEditing.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            Settings.settings = "Settings";
            Settings.target = "Target";
            Settings.animatorComponent = "Animator Component";
            Settings.vrchatAvatar = "VRChat Avatar";
            Settings.controllerToUse = "Controller to use";
            Settings.layersToUse = "Layers to use";
            Settings.avatarToUse = "Avatar to use";
            Settings.repathWhenRenamed = "Repath when renamed";
            Settings.repathWhenReparented = "Repath when reparented";
            Settings.warnWhenRenamed = "Warn when renamed";
            Settings.warnWhenReparented = "Warn when reparented";
            Settings.runWhenWindowClosed = "Run when window is closed";
            Settings.language = "Language";
            controllerOptions[0] = Settings.animatorComponent;
            controllerOptions[1] = Settings.vrchatAvatar;

            ToolTips.automatic = "Automatically change Animation Paths if something in the hierarchy gets changed";
            ToolTips.manual = "Change Animation Paths by hand";
            ToolTips.toggleButton = "Turn the automatic Animation repathing on or off";
            ToolTips.fixPaths = "Repair broken (yellow) Animation Paths in the Animator defined in the settings";
            ToolTips.editClips = "Directly change the Animation Paths of one or more Animation Clips";
            ToolTips.resetInvalidPaths = "Refresh the list of broken Animation Paths in the Animator defined in the settings";
            ToolTips.resetInvalidPath = "Restore original Animation Path";
            ToolTips.applyValidPath = "Replace the broken Animation Path with the specified Path. Path can not be empty or the same as the broken Path";
            ToolTips.replacePartOfAll = "Replace a specified part of all Animation Paths in every selected Animation clip with a specified string";
            ToolTips.resetPartOfAll = "Clear out both textboxes";
            ToolTips.replaceFrom = "The part of all Animation Paths that you want to replace";
            ToolTips.replaceTo = "What you want the specified part of all Animation Paths to be replaced with";
            ToolTips.applyPartOfAll = "Replace the specified part of all Animation Paths with the specified string. Replace from can not be empty or the same as replace to";
            ToolTips.replaceIndividual = "Replace an entire Animation Path with a new one";
            ToolTips.resetIndividual = "Restore original Animation Path";
            ToolTips.applyIndividual = "Replace the old Animation Path with the specified Path. Path can not be empty or the same as the old Path";
            ToolTips.resetSettings = "Reset all settings to their default values";
            ToolTips.target = "The object the tool should target. Can be set to either target the Animator Controller inside of the Animator Component of a Gameobject or target all Animator Controllers on a VRChat Avatar ";
            ToolTips.controllerToUse = "The Gameobject which holds the Animator Component with the target Animator Controller";
            ToolTips.avatarToUse = "The VRChat Avatar which holds the target Animator Controllers";
            ToolTips.layersToTarget = "All of the Animator Controllers on the VRChat Avatar that the tool should target";
            ToolTips.repathWhenRenamed = "Should the tool automatically change the Animation Paths if a Gameobject is renamed in the Hierarchy?";
            ToolTips.repathWhenReparented = "Should the tool automatically change the Animations Paths if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.warnWhenRenamed = "Should the tool show a pop-up message if a Gameobject is renamed in the Hierarchy?";
            ToolTips.warnWhenReparented = "Should the tool show a pop-up message if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.runWhenWindowClosed = "Should the tool still work even if the window has been closed?";
        }

        public static void loadJapanese()
        {

        }
    }
}