#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LNE.LabStreamingLayer.Visualiser
{
    using IO;
    using UI.Attributes;

    public class TagManager : MonoBehaviour
    {
        [SerializeField, Path(isFile = true, isRelative = true)]
        private string csvPath = "";

        [SerializeField]
        private int numPreviewLines = 5;

        [SerializeField]
        private Text previewBox;

        [SerializeField]
        private TagOutlet outlet;

        private CSV tags;
        private int tagIndex = 0;

        private CSV objectTags;
        private CSV avoidanceTags;
        private CSV navigationTags;

        void Start()
		{
            tags = new CSV();

#if UNITY_EDITOR
            tags.Load(UnityApp.DataPath + csvPath);
#else
            tags.Load(UnityApp.ProjectPath + "tags.csv");
#endif            
            PreviewNextTags();
		}

        private void PreviewNextTags()
        {
            previewBox.text = "";

            var n = Mathf.Min(tagIndex + numPreviewLines, tags.Height);
            for (int i = tagIndex; i < n; i++)
			{
                previewBox.text += $" {tags.GetCell(0, i)}\n";
			}
		}

        public void GotoNextTag()
        {
            tagIndex = Mathf.Min(tagIndex + 1, tags.Height - 1);
            PreviewNextTags();
        }

        public void GotoPreviousTag()
		{
            tagIndex = Mathf.Max(tagIndex - 1, 0);
            PreviewNextTags();
		}

        public void SendNextTag()
		{
            outlet.PushSample(tags.GetCell(0, tagIndex));
            GotoNextTag();
		}

        public void SendPreviousTag()
		{
            if (tagIndex == 0)
                return;

            outlet.PushSample(tags.GetCell(0, tagIndex - 1));
		}
    }
}
