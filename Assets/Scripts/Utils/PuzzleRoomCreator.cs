#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace PxlSpace.Fox
{
	public class PuzzleRoomCreator : MonoBehaviour
	{
		public enum WallSide { Front, Back, Left, Right }
		private enum BaseboardType { None, Straight, InnerCorner, OuterCorner }

		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject floorPrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject wallPrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject ceilingPrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject lightPrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private ReflectionProbe reflectionProbePrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject lightProbePrefab;
		[FoldoutGroup("Structure Prefabs")]
		[SerializeField] private GameObject securityCamPrefab;
		[FoldoutGroup("Structure Prefabs/Baseboards")]
		[SerializeField] private GameObject baseboardStraight;
		[FoldoutGroup("Structure Prefabs/Baseboards")]
		[SerializeField] private GameObject baseboardCornerInner;
		[FoldoutGroup("Structure Prefabs/Baseboards")]
		[SerializeField] private GameObject baseboardCornerOuter;
		[FoldoutGroup("Room Setup")]
		[SerializeField] private Vector3 tileDimensions;
		[FoldoutGroup("Room Setup")]
		[OnValueChanged(nameof(CheckDoorCoordsInRange))]
		[SerializeField] private Vector3Int roomDimensions;
		[FoldoutGroup("Doors")]
		[OnValueChanged(nameof(CheckDoorCoordsInRange), true)]
		[SerializeField] private DoorPositioning entryDoor;
		[FoldoutGroup("Doors")]
		[OnValueChanged(nameof(CheckDoorCoordsInRange), true)]
		[SerializeField] private DoorPositioning exitDoor;

		private Transform roomRoot;
		private Transform floorRoot;
		private Transform wallRoot;
		private Transform ceilingRoot;
		private Transform doorRoot;
		private Transform lightRoot;
		private Transform propsRoot;

		[SerializeField] private bool debug = false;

		private List<WallData> walls = new List<WallData>();

		[Button]
		private void DeleteRoom()
		{
			if (roomRoot != null)
			{
				DestroyImmediate(roomRoot.gameObject);
				walls = new List<WallData>();
			}
		}

		[Button]
		private void GenerateRoom()
		{
			DeleteRoom();

			SpawnRoots();

			for (int y = 0; y < roomDimensions.y; y++)
			{
				for (int x = 0; x < roomDimensions.x; x++)
				{
					for (int z = 0; z < roomDimensions.z; z++)
					{
						Vector3Int coord = new Vector3Int(x, y, z);
						Vector3 pos = Vector3.Scale(coord, tileDimensions);

						if (ShouldSpawnFloor(y))
							SpawnPrefab(floorPrefab, pos, floorRoot);
						if (ShouldSpawnWall(x, z))	
							SpawnWall(pos, coord);
						if (ShouldSpawnCeiling(y))
							SpawnCeiling(pos);
						if (ShouldSpawnLight(coord))
							SpawnLight(pos);
					}
				}
			}
			SpawnDoors(entryDoor, "EntryDoor");
			SpawnDoors(exitDoor, "ExitDoor");

			Vector3 roomSize = Vector3.Scale(roomDimensions, tileDimensions);
			Vector3 roomCenter = roomSize * 0.5f;

			ReflectionProbe rp = SpawnPrefab(reflectionProbePrefab.gameObject, roomCenter, roomRoot).GetComponent<ReflectionProbe>();
			rp.size = roomSize;

			SpawnLightProbeGroup(roomSize, roomCenter);
		}

		private void SpawnRoots()
		{
			roomRoot = new GameObject("Room").transform;
			floorRoot = new GameObject("Floor").transform;
			floorRoot.parent = roomRoot;
			wallRoot = new GameObject("Walls").transform;
			wallRoot.parent = roomRoot;
			ceilingRoot = new GameObject("Ceiling").transform;
			ceilingRoot.parent = roomRoot;
			doorRoot = new GameObject("Doors").transform;
			doorRoot.parent = roomRoot;
			lightRoot = new GameObject("Lights").transform;
			lightRoot.parent = roomRoot;
			propsRoot = new GameObject("Props").transform;
			propsRoot.parent = roomRoot;
			PrefabUtility.InstantiatePrefab(securityCamPrefab, propsRoot);
		}

		private Transform SpawnPrefab(GameObject _prefab, Vector3 _pos, Transform _parent)
		{
			Transform obj = (PrefabUtility.InstantiatePrefab(_prefab, _parent) as GameObject).transform;
			obj.localPosition = _pos;
			return obj;
		}

		private void SpawnWall(Vector3 _pos, Vector3Int _coord)
		{
			Transform wall = SpawnPrefab(wallPrefab, _pos, wallRoot);
			WallSide side = PlaceWall(wall, _coord);
			walls.Add(new WallData(_coord, wall, side));
			if (CheckCornerWall(_coord))
			{
				Transform cornerWall = SpawnPrefab(wallPrefab, _pos, wallRoot);
				side = PlaceWall(cornerWall, _coord, true);
				if (_coord.y == 0)
				{
					Vector3 bbPos = _coord.x == 0 ? Vector3.zero : Vector3.right * tileDimensions.x / 2f;
					SpawnPrefab(baseboardStraight, bbPos, cornerWall);
				}
				walls.Add(new WallData(_coord, cornerWall, side));
			}
			SpawnBaseboard(_coord, wall);
		}

		private void SpawnBaseboard(Vector3Int _coord, Transform parentWall)
		{
			if (_coord.y > 0) return;
			GameObject firstPrefab = baseboardStraight;
			Vector3 firstPos = Vector3.zero;
			Vector3 firstEuler = firstPrefab.transform.localEulerAngles;
			GameObject secondPrefab = baseboardStraight;
			Vector3 secondPos = Vector3.right * tileDimensions.x / 2f;
			Vector3 secondEuler = secondPrefab.transform.localEulerAngles;
			if (CheckCornerWall(_coord))
			{
				if (_coord.x == 0)
				{
					firstPrefab = baseboardCornerInner;
					firstEuler = baseboardCornerInner.transform.localEulerAngles;
				}
				else
				{
					secondPrefab = baseboardCornerInner;
					secondPos = Vector3.right * tileDimensions.x;
					secondEuler = Vector3.zero;
				}
			}
			SpawnPrefab(firstPrefab, firstPos, parentWall).localEulerAngles = firstEuler;
			SpawnPrefab(secondPrefab, secondPos, parentWall).localEulerAngles = secondEuler;
		}

		private bool CheckCornerWall(Vector3Int _coord)
		{
			return (_coord.x == 0 && (_coord.z == 0 || _coord.z == roomDimensions.z - 1))
				|| (_coord.x == roomDimensions.x - 1 && (_coord.z == 0 || _coord.z == roomDimensions.z - 1));
		}

		private WallSide PlaceWall(Transform _wall, Vector3Int _coord, bool _corner = false)
		{
			if (_coord.x == 0)
			{
				if ((_coord.z == 0) == _corner)
				{
					PlaceLeftWall(_wall);
					return WallSide.Left;
				}
				if (_coord.z == roomDimensions.z - 1 && _corner)
				{
					PlaceFarWall(_wall);
					return WallSide.Back;
				}
			}
			else if (_coord.x == roomDimensions.x - 1)
			{
				if ((_coord.z == 0) == _corner)
				{
					PlaceRightWall(_wall);
					return WallSide.Right;
				}
				if (_coord.z == roomDimensions.z - 1 && _corner)
				{
					PlaceFarWall(_wall);
					return WallSide.Back;
				}
			}
			else if (_coord.z == roomDimensions.z - 1)
			{
				PlaceFarWall(_wall);
				return WallSide.Back;
			}
			return WallSide.Front;
		}

		private void PlaceLeftWall(Transform _wall)
		{
			_wall.localPosition += Vector3.forward * tileDimensions.z;
			_wall.localRotation = Quaternion.Euler(0, 90, 0);
		}
		private void PlaceRightWall(Transform _wall)
		{
			_wall.localPosition += Vector3.right * tileDimensions.x;
			_wall.localRotation = Quaternion.Euler(0, -90, 0);
		}
		private void PlaceFarWall(Transform _wall)
		{
			_wall.localPosition += Vector3.forward * tileDimensions.z + Vector3.right * tileDimensions.x;
			_wall.localRotation = Quaternion.Euler(0, 180, 0);
		}

		private void SpawnCeiling(Vector3 _pos)
		{
			Transform ceiling = SpawnPrefab(ceilingPrefab, _pos, ceilingRoot);
			ceiling.localPosition += Vector3.up * tileDimensions.y;
		}

		private void SpawnLight(Vector3 _pos)
		{
			Transform light = SpawnPrefab(lightPrefab, _pos, lightRoot);
			light.localPosition += Vector3.up * tileDimensions.y;
		}

		private void SpawnLightProbeGroup(Vector3 roomSize, Vector3 roomCenter)
		{
			LightProbeGroup lpg = SpawnPrefab(lightProbePrefab, roomCenter, lightRoot).GetComponent<LightProbeGroup>();
			Vector3Int arraySize = new Vector3Int((int)roomCenter.x + 1, (int)roomSize.y + 1, (int)roomCenter.z + 1);
			Vector3[] probes = new Vector3[arraySize.x * arraySize.y * arraySize.z];
			int xIdx = 0;
			for (float x = -roomCenter.x; x <= roomCenter.x; x += 2)
			{
				float xPos = x;
				if (xPos == -roomCenter.x) xPos += 0.25f;
				if (xPos == roomCenter.x) xPos -= 0.25f;
				int yIdx = 0;
				for (float y = -roomCenter.y; y <= roomCenter.y; y++)
				{
					float yPos = y;
					if (yPos == -roomCenter.y) yPos += 0.25f;
					if (yPos == roomCenter.y) yPos -= 0.25f;
					int zIdx = 0;
					for (float z = -roomCenter.z; z <= roomCenter.z; z += 2)
					{
						float zPos = z;
						if (zPos == -roomCenter.z) zPos += 0.25f;
						if (zPos == roomCenter.z) zPos -= 0.25f;

						int i = xIdx + arraySize.x * (yIdx + arraySize.y * zIdx);
						probes[i] = new Vector3(xPos, yPos, zPos);
						zIdx++;
					}
					yIdx++;
				}
				xIdx++;
			}
			lpg.probePositions = probes;
		}

		private void SpawnDoors(DoorPositioning _doorData, string _doorName)
		{
			WallData existingWall = FindWall(_doorData.coords, _doorData.side);
			if (existingWall != null)
			{
				walls.Remove(existingWall);
				DestroyImmediate(existingWall.wall.gameObject);
			}
			Vector3 pos = Vector3.Scale(_doorData.coords, tileDimensions);
			Transform door = SpawnPrefab(_doorData.prefab, pos, doorRoot);
			door.name = _doorName;
			switch (_doorData.side)
			{
				case WallSide.Back:
					PlaceFarWall(door);
					break;
				case WallSide.Left:
					PlaceLeftWall(door);
					break;
				case WallSide.Right:
					PlaceRightWall(door);
					break;
				default:
					break;
			}
		}

		private WallData FindWall(Vector3Int _coord, WallSide _side)
		{
			return walls.Find(w => w.coords == _coord && w.side == _side);
		}

		private bool ShouldSpawnFloor(int _y)
		{
			return _y == 0;
		}
		private bool ShouldSpawnWall(int _x, int _z)
		{
			return _x == 0 || _x == roomDimensions.x - 1 || _z == 0 || _z == roomDimensions.z - 1;
		}
		private bool ShouldSpawnCeiling(int _y)
		{
			return _y == roomDimensions.y - 1;
		}

		private bool ShouldSpawnLight(Vector3Int _coord)
		{
			return _coord.y == roomDimensions.y - 1 && _coord.x > 0/* && _coord.x < roomDimensions.x - 1*/ && _coord.z > 0;// && _coord.z < roomDimensions.z - 1;
		}

		private void OnDrawGizmosSelected()
		{
			if (!debug) return;
			for (int x = 0; x < roomDimensions.x; x++)
			{
				for (int z = 0; z < roomDimensions.z; z++)
				{
					Vector3 pos = Vector3.Scale(tileDimensions, new Vector3(x + 0.5f, 0, z + 0.5f));
					Handles.Label(pos, $"({x}, {z})");
				}
			}
		}
		private void CheckDoorCoordsInRange()
		{
			entryDoor.coords.Clamp(Vector3Int.zero, roomDimensions - Vector3Int.one);
			exitDoor.coords.Clamp(Vector3Int.zero, roomDimensions - Vector3Int.one);
		}

		[System.Serializable]
		public class DoorPositioning
		{
			public GameObject prefab;
			public Vector3Int coords;
			public WallSide side;

		}

		private class WallData
		{
			public Vector3Int coords;
			public Transform wall;
			public WallSide side;

			public WallData(Vector3Int _c, Transform _t, WallSide _s)
			{
				coords = _c;
				wall = _t;
				side = _s;
			}
		}
	}
}
#endif
