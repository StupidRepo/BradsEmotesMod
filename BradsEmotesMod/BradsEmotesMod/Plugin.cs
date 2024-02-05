using BepInEx;
using CustomEmotesAPI_Template_Mod;
using EmotesAPI;
using LethalEmotesAPI.ImportV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lethal_Company_CustomEmotesAPI_Template_Mod
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ExampleEmoteMod /*right click on ExampleEmoteMod to the left and click rename, then type someting that matches your mod*/ : BaseUnityPlugin
    {
        public const string PluginGUID = /*insert a string here to denote your plugin's GUID, such as: "com.author.modname"*/;
        public const string PluginName = /*insert a string here to denote your plugin's name, such as: "modname"*/;
        public const string PluginVersion = "1.0.0";
        public static PluginInfo PInfo { get; private set; }
        public static ExampleEmoteMod instance;


        public void Awake()
        {
            instance = this;
            PInfo = Info;
            CustomEmotesAPI_Template_Mod.Assets.LoadAssetBundlesFromFolder("assetbundles");

            ImportAnimation([CustomEmotesAPI_Template_Mod.Assets.Load<AnimationClip>("jotaropoint_start.anim")], [CustomEmotesAPI_Template_Mod.Assets.Load<AnimationClip>("jotaropoint_loop.anim")], false, [Assets.Load<AudioClip>("jotaro.ogg")], false, "Jotaro Point", false, true, false);
            ImportAnimation([CustomEmotesAPI_Template_Mod.Assets.Load<AnimationClip>("DioPose_start.anim")], [CustomEmotesAPI_Template_Mod.Assets.Load<AnimationClip>("diopose_loop.anim")], false, [Assets.Load<AudioClip>("dio.ogg")], false, "Dio Pose", false, false, true);


            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (!newAnimation.StartsWith(PluginGUID))
            {
                return;
            }
            newAnimation = newAnimation.Split("__")[1];
            int prop1;
            switch (newAnimation)
            {
                case "DioPose_start":
                    prop1 = mapper.props.Count;
                    mapper.props.Add(GameObject.Instantiate(Assets.Load<GameObject>("Menacing Prop/Menacing Prop.prefab")));
                    mapper.props[prop1].transform.SetParent(mapper.mapperBody.transform);
                    mapper.props[prop1].transform.localEulerAngles = new Vector3(90, 0, 0);
                    mapper.props[prop1].transform.localPosition = new Vector3(-0.875f, 1.373f, -0.419f);
                    break;
                default:
                    break;
            }
        }

        public void ImportAnimation(AnimationClip[] primaryClips, AnimationClip[] secondaryClips, bool looping, AudioClip[] primaryAudioClips, bool sync, string customName, bool dmca, bool cantMove, bool thirdPerson)
        {
            CustomEmoteParams emoteParams = new CustomEmoteParams();
            emoteParams.primaryAnimationClips = primaryClips; //list of primary animation clips, one of these will be picked randomly if you include more than one
            emoteParams.secondaryAnimationClips = secondaryClips;// list of secondary animation clips, must be the same size as primaryAnimationClips, will be picked randomly in the same slot (so if we pick primaryAnimationClips[3] for an animation, we will also pick secondaryAnimation[3])
            emoteParams.audioLoops = looping;//used to specify if audio is looping, you only need to set this if you are importing a single audio file
            emoteParams.primaryAudioClips = primaryAudioClips;//primary list of audio clips
            emoteParams.secondaryAudioClips = null;//secondary list of audio clips, if these are specified, the primary clip will never loop and the secondary clip that plays will always loop
            emoteParams.primaryDMCAFreeAudioClips = null;//same as _primaryAudioClips but will be played if DMCA settings allow it (if normal audio clips exist, and dmca clips do not, the dmca clips will simply be silence)
            emoteParams.secondaryDMCAFreeAudioClips = null;//same as _secondaryAudioClips but will be played if DMCA settings allow it
            emoteParams.visible = true;// If false, will hide the emote from all normal areas, however it can still be invoked through PlayAnimation, use this for emotes that are only needed in code
            emoteParams.syncAnim = sync;// If true, will sync animation between all people emoting
            emoteParams.syncAudio = sync;// If true, will sync audio between all people emoting
            emoteParams.startPref = -1;// Spot in primaryAnimationClips array where a BoneMapper will play when there is no other instance of said emote playing -1 is random, -2 is sequential, anything else is what you make it to be
            emoteParams.joinPref = -1;// Spot in primaryAnimationClips array where a BoneMapper will play when there is at least one other instance of said emote playing, -1 is random, -2 is sequential, anything else is what you make it to be
            emoteParams.joinSpots = null;// Array of join spots which will appear when the animation is playing
            emoteParams.internalName = "";// Custom internal name for emote, if not specified, the first emote from primaryAnimationClips will be used as the name
            emoteParams.lockType = AnimationClipParams.LockType.headBobbing;// determines the lock type of your emote, none, headBobbing, lockHead, or rootMotion
            emoteParams.willGetClaimedByDMCA = dmca;// Lets you mark if your normal set of audio will get claimed by DMCA
            emoteParams.audioLevel = .3f;// determines the volume of the emote in terms of alerting enemies, 0 is nothing, 1 is max
            emoteParams.rootBonesToIgnore = null; //any bones specified and any child bones (recursive) will not be animated and instead retain original animations (I.e: you could ignore the two leg root bones to let the player walk while emoting)
            emoteParams.soloBonesToIgnore = null; //same as above, but only the specified bones will be ignored
            emoteParams.stopWhenMove = cantMove;// If on, will turn off the emote when the player starts moving
            emoteParams.thirdPerson = thirdPerson;// If true, will default animation to third person, although there are user settings to override this in either direction
            emoteParams.displayName = customName;//If specified, will replace the name of the emote in places where the end user will see the emote.
            EmoteImporter.ImportEmote(emoteParams);
        }
    }
}
