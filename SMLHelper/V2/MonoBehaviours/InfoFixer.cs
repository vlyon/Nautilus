﻿namespace SMLHelper.V2.MonoBehaviours
{
    using UnityEngine;

    public class TechTypeFixer : MonoBehaviour, IProtoEventListener
    {
        [SerializeField] // I think this is important, it started working after I added it
        public TechType techType;

        [SerializeField]
        public string ClassId;

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            var constructable = GetComponent<Constructable>();
            if (constructable != null)
            {
                constructable.techType = techType;
            }

            var techTag = GetComponent<TechTag>();
            if (techTag != null)
            {
                techTag.type = techType;
            }
        }
    }
}
