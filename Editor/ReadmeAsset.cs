//Created by UnityTechnologies
//Modified by AoiKamishiro

using System;
using UnityEditor;
using UnityEngine;

namespace online.kamishiro.readmeviewer
{
    [CreateAssetMenu(menuName = "Readme/ReadmeAsset", order = 100)]
    public class ReadmeAsset : ScriptableObject
    {
        [NonSerialized]
        private Texture2D _cachedIcon;
        public Texture2D Icon
        {
            get
            {
                if (_cachedIcon == null)
                {
                    _cachedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(iconGUID));
                }
                return _cachedIcon;
            }
            set
            {
                iconGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                _cachedIcon = null;
            }
        }
        public string iconGUID;
        public float iconMaxWidth = 64f;
        public string title;
        public Chapter[] chapters;
        public bool isLoaded;
        public bool showEditButton;

        public int ChapterCount
        {
            get
            {
                if (chapters == null) return 0;
                return chapters.Length;
            }
            set
            {
                if (value < 0) return;
                Chapter[] newChapters = new Chapter[value];
                for (int i = 0; i < Math.Min(value, ChapterCount); i++)
                {
                    newChapters[i] = chapters[i];
                }
                chapters = newChapters;
            }
        }

        /// <summary>
        /// ReadmeAssetの章を構成する構造体
        /// </summary>
        [Serializable]
        public struct Chapter
        {
            //章の表題と本文
            public string chapterTitle, chapterText;
            //節の配列
            public Section[] sections;

            public int SectionCount
            {
                get
                {
                    if (sections == null) return 0;
                    return sections.Length;
                }
                set
                {
                    if (value < 0) return;
                    Section[] newSections = new Section[value];
                    for (int i = 0; i < Math.Min(value, SectionCount); i++)
                    {
                        newSections[i] = sections[i];
                    }
                    sections = newSections;
                }
            }
        }

        /// <summary>
        /// ReadmeAssetの節を構成する構造体
        /// </summary>
        [Serializable]
        public struct Section
        {
            //節の表題
            public string sectionTitle;
            //文の配列
            public Sentence[] sentences;

            public int SentenceCount
            {
                get
                {
                    if (sentences == null) return 0;
                    return sentences.Length;
                }
                set
                {
                    if (value < 0) return;
                    Sentence[] newSentences = new Sentence[value];
                    for (int i = 0; i < Math.Min(value, SentenceCount); i++)
                    {
                        newSentences[i] = sentences[i];
                    }
                    sentences = newSentences;
                }
            }
        }

        /// <summary>
        /// ReadmeAssetの文を構成する構造体
        /// </summary>
        [Serializable]
        public struct Sentence
        {
            //文の内容
            public string text;
            //文の字下げする文字数
            public int indent;
            //文が参照を持つかどうか
            public bool isLink;
            //文が参照を持つ場合の参照先
            public string url;
        }
    }
}
