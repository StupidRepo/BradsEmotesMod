using BepInEx;
using EmotesAPI;
using LethalEmotesAPI.ImportV2;
using UnityEngine;

namespace BradsEmotesMod
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.stupidrepo.BradsEmotesMod";
        private const string PluginName = "Brad's Emotes Mod";
        private const string PluginVersion = "1.0.0";
        
        public static PluginInfo PInfo { get; private set; }
        public static Plugin Instance;

        private static AnimationClip LoadAnim(string animName)
        {
            return Assets.Load<AnimationClip>($"Anims/{animName}.anim");
        }
        
        private static AudioClip LoadAudio(string audioName, string animName)
        {
            return Assets.Load<AudioClip>($"Audios/{animName}/{audioName}.ogg");
        }
        
        public void Awake()
        {
            Instance = this;
            PInfo = Info;
            Assets.LoadAssetBundlesFromFolder("FunBundles");
            
            // Primary animation with 2 tracks (start & loop)
            PrimAnim2Track("Mudder On Da Dance Flo'", "murder");
            
            // Primary animation with 1 track (can loop)
            PrimAnim1Track("Daft Dumb Punks", "daft");
            
            // Other
            // ImportAnimation("Mudder On Da Dance Flo'", [ LoadAnim("murder-start") ], [ LoadAudio("murder-start", "murder") ], null, [ LoadAudio("murder-loop", "murder") ], dmca: true);
            // ImportAnimation("Daft Dumb Punks", [ LoadAnim("daft-start") ], [ LoadAudio("daft-start", "daft") ], dmca: true);
            ImportAnimation("Marry Meh", [ LoadAnim("marryme-start") ], [ LoadAudio("marryme-loop", "marryme") ], [ LoadAnim("marryme-loop") ], looping: true);
        }

        public void PrimAnim1Track(string dispName, string baseName, bool loop = true)
        {
            string starting = $"{baseName}-starting";
            ImportAnimation(dispName, [ LoadAnim(baseName) ], [ LoadAudio(starting, baseName) ], looping: loop, dmca: true);
        }
        
        public void PrimAnim2Track(string dispName, string baseName)
        {
            string starting = $"{baseName}-start";
            string looping = $"{baseName}-loop";
            
            ImportAnimation(dispName, [ LoadAnim(starting) ], [ LoadAudio(starting, baseName) ], null, [ LoadAudio(looping, baseName) ], dmca: true);
        }

        public void ImportAnimation(string animName, AnimationClip[] pAnimationClips, AudioClip[] pAudioClips, AnimationClip[] sAnimationClips = null, AudioClip[] sAudioClips = null, bool looping = false, bool dmca = false, bool thirdPerson = true)
        {
            CustomEmoteParams emoteParams = new()
            {
                primaryAnimationClips = pAnimationClips,
                secondaryAnimationClips = sAnimationClips,
                audioLoops = looping,
                primaryAudioClips = pAudioClips,
                secondaryAudioClips = sAudioClips,
                primaryDMCAFreeAudioClips = null,
                secondaryDMCAFreeAudioClips = null,
                visible = true,
                syncAnim = false,
                syncAudio = false,
                startPref = -1,
                joinPref = -1,
                joinSpots = null,
                internalName = "",
                lockType = AnimationClipParams.LockType.headBobbing,
                willGetClaimedByDMCA = dmca,
                audioLevel = .5f,
                rootBonesToIgnore = null,
                soloBonesToIgnore = null,
                stopWhenMove = false,
                thirdPerson = thirdPerson,
                displayName = animName
            };
            EmoteImporter.ImportEmote(emoteParams);
        }
    }
}
