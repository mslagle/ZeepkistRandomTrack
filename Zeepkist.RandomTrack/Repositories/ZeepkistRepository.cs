using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zeepkist.RandomTrack.Repositories
{
    public interface IZeepkistRepository
    {
        public BlockProperties GetGameBlock(int blockId);
        public List<BlockProperties> GetAllGameBlocks();
        public BlockProperties CreateBlock(int blockId, Vector3 position, Quaternion rotation, Vector3 scale);
        public void UnSelectEverything();
    }

    public class ZeepkistRepository : IZeepkistRepository
    {
        private readonly List<BlockProperties> gameBlocks;
        private readonly LEV_LevelEditorCentral central;

        public ZeepkistRepository()
        {
            this.central = RandomTrackManager.central;
            this.gameBlocks = central.manager.loader.globalBlockList.blocks;
        }

        public List<BlockProperties> GetAllGameBlocks()
        {
            return gameBlocks;
        }

        public BlockProperties GetGameBlock(int blockId)
        {
            return gameBlocks[blockId];
        }

        public BlockProperties CreateBlock(int blockId, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            BlockProperties newBlock = UnityEngine.Object.Instantiate<BlockProperties>(gameBlocks[blockId]);
            newBlock.isEditor = true;
            newBlock.propertyScripts.ForEach(x => x.CreateBlock(newBlock));
            newBlock.transform.position = position;
            newBlock.transform.rotation = rotation;
            newBlock.transform.localScale = scale;

            newBlock.CreateBlock();
            return newBlock;
        }

        public void UnSelectEverything()
        {
            central.selection.DeselectAllBlocks(true, "ClickNothing");
        }

        public void CreateLabel(string text, LabelPosition position)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
            labelStyle.wordWrap = true;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = Mathf.FloorToInt(Screen.height / 40);
            labelStyle.normal.textColor = Color.white;

            GUIContent labelContent = new GUIContent(text);
            Vector2 labelSize = labelStyle.CalcSize(labelContent);
            int padding = Mathf.CeilToInt(Screen.width / 200f);
            Vector2 newSize = new Vector2(labelSize.x + padding, labelSize.y + padding);
            Rect boxRect = new Rect(0, 0, 0, 0);
            boxRect.width = newSize.x;
            boxRect.height = newSize.y;

            if (position == LabelPosition.BottomLeft)
            {

            }
            Vector2 bottomScreenPosition = new Vector2(Display.main.renderingWidth / 2 - boxRect.width / 2, Display.main.renderingHeight - boxRect.height - 50);
            boxRect.position = bottomScreenPosition;
            GUI.Box(boxRect, labelContent, labelStyle);
        }
    }

    public enum LabelPosition
    {
        BottomLeft,
        TopLeft
    }
}
