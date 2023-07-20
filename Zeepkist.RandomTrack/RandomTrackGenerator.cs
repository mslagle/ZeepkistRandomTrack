using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zeepkist.RandomTrack.Models;
using Zeepkist.RandomTrack.Repositories;

namespace Zeepkist.RandomTrack
{
    public class RandomTrackGenerator
    {
        const int START_BLOCK = 1;
        const int BOOSTER_BLOCK = 69;
        const int END_BLOCK = 2;

        Vector3 START_POSITION = new Vector3(0, 16 * 5, 0);
        public Vector3 currentPosition = new Vector3();
        public Vector3 currentVector = new Vector3();
        public Quaternion currentQuaternion = new Quaternion();

        double length = 0;
        double height = 0;

        List<RandomTrackPart> randomParts;

        public bool isBuilding = false;
        public int numberOfBlocks = 0;

        public int[] blockOverrides = new int[] { };// { 3, 1189 };

        public List<SelectedTrackPart> placedTrackParts = new List<SelectedTrackPart>();
        public SelectedTrackPart pendingTrackPart = null;

        private readonly IZeepkistRepository zeepkist;

        public RandomTrackGenerator(IZeepkistRepository zeepkist, IRandomPartRepository randomPartRepository) {
            this.zeepkist = zeepkist;
            this.randomParts = randomPartRepository.GetParts();
        }

        public void Create()
        {
            currentPosition = START_POSITION;
            currentVector = new Vector3(0, 0, 0);
            currentQuaternion = new Quaternion(0,0,0,1);
            isBuilding = true;
            placedTrackParts.Clear();
            pendingTrackPart = null;

            CreateStart();
        }

        public void Update(List<TrackPartType> twitchActions = null)
        {
            if (!isBuilding)
            {
                return;
            }

            if (blockOverrides.Length > placedTrackParts.Count)
            {
                GenerateNextBlock(blockOverrides[numberOfBlocks], true, false);
            } else
            {
                GenerateNextBlock(twitchActions);
            }
            
            numberOfBlocks++;
        }

        public void End()
        {
            if (!isBuilding)
            {
                return;
            }

            GenerateNextBlock(END_BLOCK);
            PlacePendingBlock();
            isBuilding = false;
        }

        public void CreateStart()
        {
            // Add start block
            GenerateNextBlock(START_BLOCK);
            PlacePendingBlock();

            // Add a booster directly after start
            GenerateNextBlock(BOOSTER_BLOCK);
            PlacePendingBlock();
        }

        public void CalculateNextPosition(SelectedTrackPart selectedTrackPart)
        {
            var tempOffset = selectedTrackPart.Part.Offset;
            var tempVector = selectedTrackPart.Rotated ? -selectedTrackPart.Part.StartingVector : selectedTrackPart.Part.EndingVector;

            // If flipped - do some magic
            if (selectedTrackPart.Flipped)
            {
                tempOffset.x = -1 * tempOffset.x;
                tempVector.y = -1 * tempVector.y;
            }

            // If rotated - do more magic
            if (selectedTrackPart.Rotated) {
                tempOffset.y = -1 * tempOffset.y;
            }

            // Calculate new rotation vector
            Vector3 tempRotation = new Vector3(tempVector.x, currentVector.y + tempVector.y, tempVector.z);
            Vector3 newRotation = new Vector3(0, tempRotation.y, 0);
            currentVector = tempRotation;

            Quaternion rotation = Quaternion.Euler(newRotation);
            currentQuaternion = rotation;

            // Calculate offset from last block
            var finalOffset = tempOffset;
            finalOffset.z += selectedTrackPart.Properties.boundingBoxSize.z;
            Vector3 finalPosition = rotation * finalOffset;
            currentPosition += finalPosition;

            Vector3 lengthVector = new Vector3(selectedTrackPart.Part.Offset.x, 0, selectedTrackPart.Properties.boundingBoxSize.z);
            length += lengthVector.magnitude;
            height += tempOffset.y;
        }

        public SelectedTrackPart GenerateNextBlock(List<TrackPartType> allowedTrackParts = null)
        {
            SelectedTrackPart selectedTrackPart = GetNextRandomBlock(allowedTrackParts);
            selectedTrackPart.GeneratePosition(currentPosition, currentVector, currentQuaternion);
            pendingTrackPart = selectedTrackPart;

            CalculateNextPosition(selectedTrackPart);
            return selectedTrackPart;
        }

        public void PlacePendingBlock()
        {
            if (pendingTrackPart != null)
            {

                BlockProperties newBlock = zeepkist.CreateBlock(pendingTrackPart.Properties.blockID, pendingTrackPart.CreatedBlockPosition.Position, 
                    pendingTrackPart.CreatedBlockPosition.Rotation, pendingTrackPart.CreatedBlockPosition.Scale);
                placedTrackParts.Add(pendingTrackPart);

                pendingTrackPart = null;
            }
        }

        public SelectedTrackPart GenerateNextBlock(int overrideId, bool flipped = false, bool rotated = false)
        {
            int blockId = overrideId;
            SelectedTrackPart selectedTrackPart = new SelectedTrackPart();
            selectedTrackPart.Part = randomParts.First(x => x.Id == overrideId);
            selectedTrackPart.Properties = zeepkist.GetGameBlock(overrideId);
            selectedTrackPart.Flipped = flipped;
            selectedTrackPart.Rotated = rotated;

            selectedTrackPart.GeneratePosition(currentPosition, currentVector, currentQuaternion);
            pendingTrackPart = selectedTrackPart;

            CalculateNextPosition(selectedTrackPart);
            return selectedTrackPart;
        }

        public SelectedTrackPart GetNextRandomBlock(List<TrackPartType> allowedTrackParts = null)
        {
            // Get all blocks that are specified in CSV file and not start or finish
            List<RandomTrackPart> availableBlocks = randomParts.Where(x =>
            {
                var tempBlock = zeepkist.GetGameBlock(x.Id);
                return !(tempBlock.isFinish || tempBlock.isStart);
            }).ToList();

            // Now get all blocks that match the ending vector (only care about the x vector for now)
            List<RandomTrackPart> matchingBlocks = availableBlocks.Where(x =>
            {
                return x.StartingVector.x == currentVector.x
                    || x.EndingVector.x == -currentVector.x;
            }).ToList();

            List<RandomTrackPart> allowedBlocks = matchingBlocks;
            if (allowedTrackParts != null && allowedTrackParts.Count > 0)
            {
                allowedBlocks = matchingBlocks.Where(x => allowedTrackParts.Any(y => x.TrackPartTypes.HasFlag(y))).ToList();
                if (allowedBlocks.Count == 0)
                {
                    var possibleTypes = matchingBlocks.SelectMany(x => x.TrackPartTypes.ToString().Split(',')).Distinct().ToList();
                    throw new Exception($"No blocks could be placed of types: {string.Join(", ", allowedTrackParts.Select(x => x.ToString()))}, only possible block types are of {string.Join(", ", possibleTypes)}");
                }
            }


            // Randomly choose one
            int random = new System.Random().Next(0, allowedBlocks.Count - 1);
            RandomTrackPart selectedPart = allowedBlocks[random];
            SelectedTrackPart selectedTrackPart = new SelectedTrackPart() { Part = selectedPart, Properties = zeepkist.GetGameBlock(selectedPart.Id) };

            // Must rotate if ending vector matches current vector and starting vector does not
            if (selectedPart.StartingVector.x != currentVector.x 
                && selectedPart.EndingVector.x == -currentVector.x)
            {
                selectedTrackPart.Rotated = true;
            }

            // Lets add some random-ness
            // If starting vector matches either way and part is flippable - lets sometimes flip it
            if (selectedPart.StartingVector.x == currentVector.x 
                && -selectedPart.StartingVector.x == currentVector.x
                && selectedTrackPart.Properties.flippable) 
            {
                if (new System.Random().Next(0, 1) == 0)
                {
                    selectedTrackPart.Flipped = true;
                }
            }

            // If both start and end vector matches either way - lets rotate sometimes
            // Only flip if the part can offset up or vectors up
            if (selectedPart.StartingVector.x == currentVector.x
                    && selectedPart.EndingVector.x == -currentVector.x
                    && (selectedPart.Offset.y != 0 || selectedPart.EndingVector.x != 0))
            {
                if (new System.Random().Next(0, 10) < 8)
                {
                    selectedTrackPart.Rotated = true;
                }
            }

            return selectedTrackPart;
        }

        public void UpdateCamera()
        {
            if (!isBuilding)
            {
                return;
            }

            // Force camera to look at new piece
            Vector3 directionToLook = (currentPosition - Camera.main.transform.position);
            Quaternion rotationToLook = Quaternion.LookRotation(directionToLook);
            Quaternion newCameraRotation = Quaternion.Slerp(Camera.main.transform.rotation, rotationToLook, 3 * Time.deltaTime);
            Camera.main.transform.rotation = newCameraRotation;

            // Move camera to next to new piece
            Vector3 cameraOffset = new Vector3(-60, 30, 0);  // To the left side of the piece
            Vector3 cameraOffsetRotated = currentQuaternion * cameraOffset;
            Vector3 newCameraPositionEndpoint = currentPosition + cameraOffsetRotated;
            Vector3 newCameraPosition = Vector3.Lerp(Camera.main.transform.position, newCameraPositionEndpoint, Time.deltaTime);
            Camera.main.transform.position = newCameraPosition;
        }
    }
}
