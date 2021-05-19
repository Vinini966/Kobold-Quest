using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProfiles", menuName = "Character/Profiles", order = 1)]
[System.Serializable]
public class CharacterProfile : ScriptableObject
{
    [SerializeField]
    public List<CharData> characters;


    [System.Serializable]
    public struct CharData
    {
        public string name;

        public Sprite profilePicture;

        public AudioClip Ah;
        public AudioClip Eh;
        public AudioClip Oh;
        public AudioClip Uh;
        public AudioClip Oo;
    }

    public Dictionary<string, CharData> ToDictionary()
    {
        Dictionary<string, CharData> valuePairs = new Dictionary<string, CharData>();
        foreach(CharData cD in characters)
        {
            valuePairs.Add(cD.name, cD);
        }

        return valuePairs;
    }
}
