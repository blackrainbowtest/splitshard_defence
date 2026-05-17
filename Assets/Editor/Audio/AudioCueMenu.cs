#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using SHD.Core.Audio;

namespace SHD.Editor.Audio
{
	public static class AudioCueMenu
	{
		private const string CuesRoot = "Assets/Audio/Cues";

		[MenuItem("Tools/Audio/Cue", false, 100)]
		public static void CreateAudioCue()
		{
			CreateCueAsset("audio_cue_default", AudioCueBus.SfxSystem);
		}

		[MenuItem("Tools/Audio/Cue/Default", false, 110)]
		public static void CreateDefaultCue()
		{
			CreateCueAsset("audio_cue_default", AudioCueBus.SfxSystem);
		}

		[MenuItem("Tools/Audio/Cue/Music", false, 111)]
		public static void CreateMusicCue()
		{
			CreateCueAsset("audio_cue_music", AudioCueBus.Music, loop: true, max_simultaneous: 1);
		}

		[MenuItem("Tools/Audio/Cue/UI", false, 112)]
		public static void CreateUiCue()
		{
			CreateCueAsset("audio_cue_ui", AudioCueBus.UI, overlap: true, cooldown_seconds: 0.02f, max_simultaneous: 8);
		}

		[MenuItem("Tools/Audio/Cue/SFX Combat", false, 113)]
		public static void CreateSfxCombatCue()
		{
			CreateCueAsset("audio_cue_sfx_combat", AudioCueBus.SfxCombat, overlap: true, cooldown_seconds: 0.03f, max_simultaneous: 12);
		}

		[MenuItem("Tools/Audio/Cue/SFX World", false, 114)]
		public static void CreateSfxWorldCue()
		{
			CreateCueAsset("audio_cue_sfx_world", AudioCueBus.SfxWorld, overlap: true, cooldown_seconds: 0.03f, max_simultaneous: 10);
		}

		[MenuItem("Tools/Audio/Cue/SFX System", false, 115)]
		public static void CreateSfxSystemCue()
		{
			CreateCueAsset("audio_cue_sfx_system", AudioCueBus.SfxSystem, overlap: false, cooldown_seconds: 0.08f, max_simultaneous: 4);
		}

		[MenuItem("Tools/Audio/Cue/Ambience", false, 116)]
		public static void CreateAmbienceCue()
		{
			CreateCueAsset("audio_cue_ambience", AudioCueBus.Ambience, loop: true, overlap: false, max_simultaneous: 2);
		}

		[MenuItem("Tools/Audio/Cue/Voice", false, 117)]
		public static void CreateVoiceCue()
		{
			CreateCueAsset("audio_cue_voice", AudioCueBus.Voice, overlap: false, max_simultaneous: 2);
		}

		private static void CreateCueAsset(
			string file_name,
			AudioCueBus bus,
			bool loop = false,
			bool overlap = true,
			float cooldown_seconds = 0f,
			int max_simultaneous = 6)
		{
			AudioCue cue;
			string asset_path;
			SerializedObject serialized_cue;

			EnsureFolderExists("Assets/Audio");
			EnsureFolderExists(CuesRoot);

			cue = ScriptableObject.CreateInstance<AudioCue>();
			serialized_cue = new SerializedObject(cue);

			serialized_cue.FindProperty("_cue_id").stringValue = file_name;
			serialized_cue.FindProperty("_bus").enumValueIndex = (int)bus;
			serialized_cue.FindProperty("_loop").boolValue = loop;
			serialized_cue.FindProperty("_allow_overlap").boolValue = overlap;
			serialized_cue.FindProperty("_cooldown_seconds").floatValue = cooldown_seconds;
			serialized_cue.FindProperty("_max_simultaneous").intValue = max_simultaneous;
			serialized_cue.FindProperty("_spatial_blend").floatValue = 0f;
			serialized_cue.ApplyModifiedPropertiesWithoutUndo();

			asset_path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(CuesRoot, file_name + ".asset"));
			AssetDatabase.CreateAsset(cue, asset_path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = cue;
		}

		private static void EnsureFolderExists(string full_path)
		{
			string[] parts;
			string current_path;
			int i;

			if (AssetDatabase.IsValidFolder(full_path) == true)
				return;

			parts = full_path.Split('/');
			current_path = parts[0];

			i = 1;
			while (i < parts.Length)
			{
				if (AssetDatabase.IsValidFolder(current_path + "/" + parts[i]) == false)
					AssetDatabase.CreateFolder(current_path, parts[i]);

				current_path = current_path + "/" + parts[i];
				i++;
			}
		}
	}
}
#endif
