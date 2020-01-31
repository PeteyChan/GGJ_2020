using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[Attributes.AlwaysRepaint]
public class Animate : MonoBehaviour
{
    float blendTime = 0;
    float changeTime;

    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    Dictionary<int, AnimationClipPlayable> playableLookup = new Dictionary<int, AnimationClipPlayable>();
    
    
    [Attributes.Label]
    string CurrentClip => Clip ? Clip.name : "";
        
    [Attributes.Label]
    string Frame => $"{Mathf.RoundToInt(CurrentFrame)} / {TotalFrames}";

    [Attributes.Label]
    int TotalClips => graph.IsValid() ? graph.GetPlayableCount() -1 : 0;

    /// <summary>
    /// Current Frame of current Clip
    /// </summary>
    public float CurrentFrame
    {
        get
        {
            if (mixer.IsValid())
            {
                var playable = (AnimationClipPlayable)mixer.GetInput(0);
                if (playable.IsValid())
                    return ((float)playable.GetTime() * TotalFrames) % TotalFrames;
            }
            return 0;
        }
    }

    /// <summary>
    /// Total Frames in current Clip
    /// </summary>
    public int TotalFrames => Clip ? Mathf.RoundToInt(Clip.frameRate * Clip.length ) : 0;

    /// <summary>
    /// animate playback speed
    /// </summary>
    public float Speed = 1;

    /// <summary>
    /// current playback clip
    /// </summary>
    public AnimationClip Clip
    {
        get
        {
            if (mixer.IsValid())
            {
                var playable = (AnimationClipPlayable)mixer.GetInput(0);
                if (playable.IsValid())
                    return playable.GetAnimationClip();
            }
            return null;
        }
    }

    void CreateGraph()
    {
        if (!gameObject.TryFind(out Animator animator))
        {
            enabled = false;
            Debug.LogError($"Animate on {transform.root.name} is unable to create it's animation graph since there is no Animator in the heirarchy.", this);
            return;
        }

        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        var output = AnimationPlayableOutput.Create(graph, "Animate", animator);
        mixer = AnimationMixerPlayable.Create(graph, 2);
        output.SetSourcePlayable(mixer);
        graph.Play();
    }

    void Update()
    {
        if (!mixer.IsValid() || Speed == 0) return;
        var cWeight = mixer.GetInputWeight(0);
        if (cWeight < 1)
        {
            cWeight = Mathf.Lerp(0, 1, (Time.time - changeTime) / blendTime);
            mixer.SetInputWeight(0, cWeight);
            mixer.SetInputWeight(1, 1 - cWeight);
        }
        graph.Evaluate(Time.deltaTime * Speed);
    }
    
    void OnDestroy()
    {
        graph.Destroy();
    }

    /// <summary>
    /// Removes clip from animate Graph
    /// </summary>
    public void Remove(AnimationClip clip)
    {
        if (!clip) return;
        if (playableLookup.TryGetValue(clip.GetInstanceID(), out var playable))
        {
            graph.DestroyPlayable(playable);
            playableLookup.Remove(clip.GetInstanceID());
        }
    }
    

    /// <summary>
    /// Adds clip to Aniamtion Graph and then plays the clip.
    /// </summary>
    /// <param name="blendTime">Time taken to blend between current clip and next clip</param>
    /// <param name="clipOffset">How many seconds to offset the clip's start time</param>
    public void Play(AnimationClip clip, float blendTime = .2f, float clipOffset = 0)
    {
        if (!clip)
        {
            Debug.LogError("Cannot Play Null Clip", gameObject);
            return;
        }
        if (!playableLookup.TryGetValue(clip.GetInstanceID(), out var next))
        {
            if (!graph.IsValid())
            {
                CreateGraph();
            }
            
            playableLookup[clip.GetInstanceID()] = next = AnimationClipPlayable.Create(graph, clip);
        }

        var current = mixer.GetInput(0);

        next.SetTime(clipOffset);
        mixer.DisconnectInput(0);
        mixer.DisconnectInput(1);

        mixer.ConnectInput(0, next, 0);

        if (blendTime <= 0)
            mixer.SetInputWeight(0, 1);
        else
        {
            if (current.IsValid())
                mixer.ConnectInput(1, current, 0);
            mixer.SetInputWeight(0, 0);
            mixer.SetInputWeight(1, 1);
        }
        this.blendTime = blendTime;
        changeTime = Time.time;
    }
}
