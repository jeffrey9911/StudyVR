using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace BuildingVolumes.Streaming
{    public class GeometrySequenceClip : PlayableAsset
    {
        public string relativePath;
        public GeometrySequenceStream.PathType pathRelation;
        public float targetPlaybackFPS = 30;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<GeometrySequenceBehaviour> playable = ScriptPlayable<GeometrySequenceBehaviour>.Create(graph);

            GeometrySequenceBehaviour geoSequenceBehaviour = playable.GetBehaviour();
            geoSequenceBehaviour.relativePath = relativePath;
            geoSequenceBehaviour.pathRelation = pathRelation;
            geoSequenceBehaviour.targetPlaybackFPS = targetPlaybackFPS;

            return playable;
        }
    }
}

