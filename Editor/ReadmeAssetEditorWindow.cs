using UnityEditor;
using UnityEngine;

namespace online.kamishiro.readmeviewer
{
    public class ReadmeAssetEditorWindow : EditorWindow
    {
        private static ReadmeAsset target;
        private static Vector2 scroll;
        public static void OpenEditor(ReadmeAsset readmeAsset)
        {
            target = readmeAsset;
            _ = GetWindow<ReadmeAssetEditorWindow>("ReadmeAsset Editor");
        }

        private void OnGUI()
        {
            if (target == null) Close();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Readme ファイルの編集", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            target.title = EditorGUILayout.TextField("タイトル", target.title);
            target.icon = (Texture2D)EditorGUILayout.ObjectField("アイコン", target.icon, typeof(Texture2D), false);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            target.ChapterCount = EditorGUILayout.DelayedIntField("チャプター数", target.ChapterCount);
            if (GUILayout.Button("+", GUILayout.Width(30))) target.ChapterCount++;
            if (GUILayout.Button("-", GUILayout.Width(30))) target.ChapterCount--;
            EditorGUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int c = 0; c < target.ChapterCount; c++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;

                target.chapters[c].chapterTitle = EditorGUILayout.TextField("チャプタータイトル", target.chapters[c].chapterTitle);
                target.chapters[c].chapterText = EditorGUILayout.TextField("チャプターテキスト", target.chapters[c].chapterText);

                EditorGUILayout.BeginHorizontal();
                target.chapters[c].SectionCount = EditorGUILayout.DelayedIntField("セクション数", target.chapters[c].SectionCount);
                if (GUILayout.Button("+", GUILayout.Width(30))) target.chapters[c].SectionCount++;
                if (GUILayout.Button("-", GUILayout.Width(30))) target.chapters[c].SectionCount--;
                EditorGUILayout.EndHorizontal();
                for (int s = 0; s < target.chapters[c].SectionCount; s++)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUI.indentLevel++;
                    target.chapters[c].sections[s].sectionTitle = EditorGUILayout.TextField("セクションタイトル", target.chapters[c].sections[s].sectionTitle);

                    EditorGUILayout.BeginHorizontal();
                    target.chapters[c].sections[s].SentenceCount = EditorGUILayout.DelayedIntField("センテンス数", target.chapters[c].sections[s].SentenceCount);
                    if (GUILayout.Button("+", GUILayout.Width(30))) target.chapters[c].sections[s].SentenceCount++;
                    if (GUILayout.Button("-", GUILayout.Width(30))) target.chapters[c].sections[s].SentenceCount--;
                    EditorGUILayout.EndHorizontal();
                    for (int se = 0; se < target.chapters[c].sections[s].SentenceCount; se++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUI.indentLevel++;
                        target.chapters[c].sections[s].sentences[se].text = EditorGUILayout.TextField("センテンス", target.chapters[c].sections[s].sentences[se].text);
                        target.chapters[c].sections[s].sentences[se].indent = EditorGUILayout.IntField("インデント", target.chapters[c].sections[s].sentences[se].indent);
                        target.chapters[c].sections[s].sentences[se].isLink = EditorGUILayout.Toggle("リンク", target.chapters[c].sections[s].sentences[se].isLink);
                        EditorGUI.BeginDisabledGroup(!target.chapters[c].sections[s].sentences[se].isLink);
                        target.chapters[c].sections[s].sentences[se].url = EditorGUILayout.TextField("リンク先", target.chapters[c].sections[s].sentences[se].url);
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck()) AssetDatabase.SaveAssets();
        }
    }
}