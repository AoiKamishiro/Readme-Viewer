//Created by UnityTechnologies
//Modified by AoiKamishiro


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Math = System.Math;

namespace online.kamishiro.readmeviewer
{
    [InitializeOnLoad]
    [CustomEditor(typeof(ReadmeAsset))]
    public class ReadmeAssetEditor : Editor
    {
        #region Properties/Fields
        private Rect IconRect;
        private Rect LabelRect;
        private Rect ButtonRect;

        private static readonly float space = 16f;
        private static readonly int tab = 8;

        private static GUIStyle m_LinkStyle;
        private static GUIStyle m_TitleStyle;
        private static GUIStyle m_ChapterTitleStyle;
        private static GUIStyle m_ChapterTextStyle;
        private static GUIStyle m_SectionTitleStyle;
        private static GUIStyle m_LineStyle;

        private static GUIStyle TitleStyle
        {
            get
            {
                if (m_TitleStyle == null)
                {
                    m_TitleStyle = new GUIStyle(ChapterTextStyle)
                    {
                        fontSize = 26
                    };
                }
                return m_TitleStyle;
            }
        }
        private static GUIStyle ChapterTitleStyle
        {
            get
            {
                if (m_ChapterTitleStyle == null)
                {
                    m_ChapterTitleStyle = new GUIStyle(ChapterTextStyle)
                    {
                        fontSize = 18,
                        fontStyle = FontStyle.Bold
                    };
                }
                return m_ChapterTitleStyle;
            }
        }
        private static GUIStyle ChapterTextStyle
        {
            get
            {
                if (m_ChapterTextStyle == null)
                {
                    m_ChapterTextStyle = new GUIStyle(EditorStyles.label)
                    {
                        wordWrap = true,
                        fontSize = 16,
                    };
                }
                return m_ChapterTextStyle;
            }
        }
        private static GUIStyle SectionTitleStyle
        {
            get
            {
                if (m_SectionTitleStyle == null)
                {
                    m_SectionTitleStyle = new GUIStyle(ChapterTextStyle)
                    {
                        fontSize = 14,
                        fontStyle = FontStyle.Bold
                    };
                }
                return m_SectionTitleStyle;
            }
        }
        private static GUIStyle LineStyle
        {
            get
            {
                if (m_LineStyle == null)
                {
                    m_LineStyle = new GUIStyle(ChapterTextStyle)
                    {
                        fontSize = 14
                    };
                }
                return m_LineStyle;
            }
        }
        private static GUIStyle LinkStyle
        {
            get
            {
                if (m_LinkStyle == null)
                {
                    ColorUtility.TryParseHtmlString("#4F80F8", out Color color);
                    m_LinkStyle = new GUIStyle(LineStyle);
                    m_LinkStyle.normal.textColor = color; ;
                    m_LinkStyle.stretchWidth = false;
                }
                return m_LinkStyle;
            }
        }
        #endregion

        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        static ReadmeAssetEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
        }

        /// <summary>
        /// アセットの中からReadmeAssetを自動的に選択します。
        /// </summary>
        private static void SelectReadmeAutomatically()
        {
            //アセットに含まれるすべてのReadmeAssetのうち、まだ読み込まれていない物を取得
            IEnumerable<ReadmeAsset> readmes = FindAssetsByType<ReadmeAsset>().Where(x => !x.isLoaded);

            //一つもなければ終了
            if (!readmes.Any()) return;

            //取得したうちの一つに"読み込み済み"の目印を立てて、選択する
            ReadmeAsset readme = readmes.First();
            readme.isLoaded = true;
            EditorUtility.SetDirty(readme);
            Selection.objects = new Object[] { readme };
        }

        /// <summary>
        /// インスペクターの表題の表示を行います
        /// </summary>
        protected override void OnHeaderGUI()
        {
            ReadmeAsset readme = (ReadmeAsset)target;

            float iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, readme.iconMaxWidth);

            GUILayout.BeginHorizontal("In BigTitle");
            {
                GUILayout.Label(readme.Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                IconRect = GUILayoutUtility.GetLastRect();
                GUILayout.Label(readme.title, TitleStyle);
                LabelRect = GUILayoutUtility.GetLastRect();

                if (readme.showEditButton)
                {
                    ButtonRect = new Rect(
                        EditorGUIUtility.currentViewWidth - 52 - 10,
                        Math.Max(IconRect.y + IconRect.height, LabelRect.y + LabelRect.height) - 16,
                        52,
                        16);
                    if (GUI.Button(ButtonRect, new GUIContent("Edit"), EditorStyles.miniButton)) ReadmeAssetEditorWindow.OpenEditor(readme);
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// インスペクターの内容の表示を行います
        /// </summary>
        public override void OnInspectorGUI()
        {
            ReadmeAsset readme = (ReadmeAsset)target;

            GUILayout.Space(space / 2);

            //章が存在しない場合、そこで終了
            if (readme.chapters == null || readme.chapters.Length == 0) return;

            //章の反復処理
            foreach (ReadmeAsset.Chapter chapter in readme.chapters)
            {
                GUILayout.Space(space / 2);

                //章の表題を表示
                if (!string.IsNullOrEmpty(chapter.chapterTitle)) Label(chapter.chapterTitle, ChapterTitleStyle);

                //章の本文を表示
                if (!string.IsNullOrEmpty(chapter.chapterText)) Label(chapter.chapterText, ChapterTextStyle);

                //節が存在しない場合、そこで現在の章を終了
                if (chapter.sections == null || chapter.sections.Length == 0) continue;

                //節の反復処理
                foreach (ReadmeAsset.Section section in chapter.sections)
                {
                    GUILayout.Space(space / 4);

                    //節の表題を表示
                    if (!string.IsNullOrEmpty(section.sectionTitle)) Label(section.sectionTitle, SectionTitleStyle);

                    //文が存在しない場合、そこで現在の節を終了
                    if (section.sentences == null || section.sentences.Length == 0) continue;

                    //文の反復処理
                    foreach (ReadmeAsset.Sentence line in section.sentences)
                    {
                        //文が空行の場合、そこで現在の文を終了
                        if (string.IsNullOrEmpty(line.text)) continue;

                        using (new GUILayout.HorizontalScope())
                        {
                            //指定された文字数の字下げを行う
                            GUILayout.Space(line.indent * tab);

                            //文が参照文であれば、参照先を開けるように表示
                            if (line.isLink) LinkLabel(line.text, line.url, LinkStyle);
                            //そうでなければ単純に文を表示
                            else Label(line.text, LineStyle);
                        }
                    }
                }
                GUILayout.Space(space);
            }
        }
        /// <summary>
        /// 文の表示の処理を行います。
        /// </summary>
        /// <param name="text">表示文</param>
        /// <param name="style">参照先</param>
        /// <param name="options">書式</param>
        private static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);
        }
        /// <summary>
        /// 参照文の表示と、参照先を開く処理を行います。
        /// </summary>
        /// <param name="text">表示文</param>
        /// <param name="url">参照先</param>
        /// <param name="style">書式</param>
        /// <param name="options">オプション</param>
        private static void LinkLabel(string text, string url, GUIStyle style, params GUILayoutOption[] options)
        {
            GUIContent label = new GUIContent(text);
            Rect position = GUILayoutUtility.GetRect(label, style, options);

            Handles.BeginGUI();
            Handles.color = style.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            if (GUI.Button(position, label, style)) Application.OpenURL(url);
        }

        /// <summary>
        /// アセットの中から指定された型に属する物の一覧を返します。
        /// </summary>
        /// <typeparam name="T">探したい型</typeparam>
        /// <returns>指定された型に属する物の一覧</returns>
        private static IEnumerable<T> FindAssetsByType<T>() where T : Object
        {
            IEnumerable<T> assets = AssetDatabase.FindAssets($"t:{typeof(T).FullName}").
                SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x)).
                Where(x => x is T).
                Select(x => (T)x);
            return assets;
        }
    }
}
