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
    }
}
