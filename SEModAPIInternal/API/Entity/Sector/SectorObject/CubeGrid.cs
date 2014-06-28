﻿using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CubeGridEntity : BaseEntity
	{
		#region "Attributes"

		private CubeBlockManager m_cubeBlockManager;

		#endregion

		#region "Constructors and Initializers"

		public CubeGridEntity(FileInfo prefabFile)
			: base(null)
		{
			BaseEntity = BaseEntityManager.LoadContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(prefabFile);

			m_cubeBlockManager = new CubeBlockManager();
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach(var cubeBlock in GetSubTypeEntity().CubeBlocks)
			{
				cubeBlockList.Add(new CubeBlockEntity(cubeBlock));
			}
			m_cubeBlockManager.Load(cubeBlockList.ToArray());
		}

		public CubeGridEntity(MyObjectBuilder_CubeGrid definition)
			: base(definition)
		{
			m_cubeBlockManager = new CubeBlockManager();
			List<CubeBlockEntity> cubeBlockList = new List<CubeBlockEntity>();
			foreach(var cubeBlock in GetSubTypeEntity().CubeBlocks)
			{
				switch (cubeBlock.TypeId)
				{
					case MyObjectBuilderTypeEnum.CargoContainer:
						cubeBlockList.Add(new CargoContainerEntity((MyObjectBuilder_CargoContainer)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Reactor:
						cubeBlockList.Add(new ReactorEntity((MyObjectBuilder_Reactor)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.MedicalRoom:
						cubeBlockList.Add(new MedicalRoomEntity((MyObjectBuilder_MedicalRoom)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Cockpit:
						cubeBlockList.Add(new CockpitEntity((MyObjectBuilder_Cockpit)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.Beacon:
						cubeBlockList.Add(new BeaconEntity((MyObjectBuilder_Beacon)cubeBlock));
						break;
					case MyObjectBuilderTypeEnum.GravityGenerator:
						cubeBlockList.Add(new GravityGeneratorEntity((MyObjectBuilder_GravityGenerator)cubeBlock));
						break;
					default:
						cubeBlockList.Add(new CubeBlockEntity(cubeBlock));
						break;
				}
			}
			m_cubeBlockManager.Load(cubeBlockList.ToArray());
		}

		#endregion

		#region "Properties"

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (GetSubTypeEntity() == null)
					return "";

				string name = "";
				foreach (var cubeBlock in GetSubTypeEntity().CubeBlocks)
				{
					if (cubeBlock.TypeId == MyObjectBuilderTypeEnum.Beacon)
					{
						if (name.Length > 0)
							name += "|";
						name += ((MyObjectBuilder_Beacon)cubeBlock).CustomName;
					}
				}
				if (name.Length == 0)
					return GetSubTypeEntity().EntityId.ToString();
				else
					return name;
			}
		}

		[Category("Cube Grid")]
		[ReadOnly(true)]
		public MyCubeSize GridSizeEnum
		{
			get { return GetSubTypeEntity().GridSizeEnum; }
			set
			{
				if (GetSubTypeEntity().GridSizeEnum == value) return;
				GetSubTypeEntity().GridSizeEnum = value;
				Changed = true;
			}
		}

		[Category("Cube Grid")]
		public bool IsStatic
		{
			get { return GetSubTypeEntity().IsStatic; }
			set
			{
				if (GetSubTypeEntity().IsStatic == value) return;
				GetSubTypeEntity().IsStatic = value;
				Changed = true;

				if (BackingObject != null)
					CubeGridInternalWrapper.UpdateEntityIsStatic(BackingObject, value);
			}
		}

		[Category("Cube Grid")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 LinearVelocity
		{
			get { return GetSubTypeEntity().LinearVelocity; }
			set
			{
				if (GetSubTypeEntity().LinearVelocity.Equals(value)) return;
				GetSubTypeEntity().LinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
					BaseEntityManagerWrapper.GetInstance().UpdateEntityVelocity(BackingObject, value);
			}
		}

		[Category("Cube Grid")]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public SerializableVector3 AngularVelocity
		{
			get { return GetSubTypeEntity().AngularVelocity; }
			set
			{
				if (GetSubTypeEntity().AngularVelocity.Equals(value)) return;
				GetSubTypeEntity().AngularVelocity = value;
				Changed = true;

				if (BackingObject != null)
					BaseEntityManagerWrapper.GetInstance().UpdateEntityAngularVelocity(BackingObject, value);
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<CubeBlockEntity> CubeBlocks
		{
			get
			{
				List<CubeBlockEntity> newList = new List<CubeBlockEntity>();
				foreach (var def in m_cubeBlockManager.Definitions)
				{
					newList.Add((CubeBlockEntity)def);
				}
				return newList;
			}
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<BoneInfo> Skeleton
		{
			get { return GetSubTypeEntity().Skeleton; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return GetSubTypeEntity().ConveyorLines; }
		}

		[Category("Cube Grid")]
		[Browsable(false)]
		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return GetSubTypeEntity().BlockGroups; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new public MyObjectBuilder_CubeGrid GetSubTypeEntity()
		{
			return (MyObjectBuilder_CubeGrid)BaseEntity;
		}

		public void Export(FileInfo fileInfo)
		{
			BaseEntityManager.SaveContentFile<MyObjectBuilder_CubeGrid, MyObjectBuilder_CubeGridSerializer>(GetSubTypeEntity(), fileInfo);
		}

		#endregion
	}

	public class CubeGridManager : BaseEntityManager
	{
		#region "Methods"

		new protected bool GetChangedState(CubeGridEntity overLayer)
		{
			foreach (var def in overLayer.CubeBlocks)
			{
				CubeBlockEntity cubeBlock = (CubeBlockEntity)def;
				if (cubeBlock.Changed)
					return true;
			}

			return overLayer.Changed;
		}

		#endregion
	}

	public class CubeGridInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeGridInternalWrapper m_instance;

		private static Assembly m_assembly;

		private static CubeGridEntity m_cubeGridToUpdate;

		private static Type m_baseCubeGridType;

		public static string CubeGridClass = "5BCAC68007431E61367F5B2CF24E2D6F.98262C3F38A1199E47F2B9338045794C";
		public static string CubeGridIsStaticField = "";
		public static string CubeGridBlockGroupsField = "24E0633A3442A1F605F37D69F241C970";
		public static string CubeGridSetDampenersEnabledMethod = "86B66668D555E1C1B744C17D2AFA77F7";

		#endregion

		#region "Constructors and Initializers"

		protected CubeGridInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_baseCubeGridType = m_assembly.GetType(CubeGridClass);

			Console.WriteLine("Finished loading CubeGridInternalWrapper");
		}

		new public static CubeGridInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new CubeGridInternalWrapper(basePath);
			}
			return (CubeGridInternalWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		new public static bool IsDebugging
		{
			get
			{
				CubeGridInternalWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				CubeGridInternalWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		public static bool UpdateEntityIsStatic(Object gameEntity, bool isStatic)
		{
			try
			{
				FieldInfo isStaticField = GetEntityField(gameEntity, CubeGridIsStaticField);
				isStaticField.SetValue(gameEntity, isStatic);

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				throw ex;
			}
		}

		public static bool AddCubeGrid(CubeGridEntity cubeGrid)
		{
			try
			{
				m_cubeGridToUpdate = cubeGrid;

				Action action = InternalAddCubeGrid;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				throw ex;
			}
		}

		public static void InternalAddCubeGrid()
		{
			try
			{
				if (m_cubeGridToUpdate == null)
					return;

				if (m_isDebugging)
					Console.WriteLine("CubeGrid '" + m_cubeGridToUpdate.Name + "' is being added ...");

				//Force the constructor to initialize before we continue
				GetInstance();

				m_baseCubeGridType = m_assembly.GetType(CubeGridClass);

				//Create a blank instance of the base type
				m_cubeGridToUpdate.BackingObject = Activator.CreateInstance(m_baseCubeGridType);

				//Invoke 'Init' using the sub object of the grid which is the MyObjectBuilder_CubeGrid type
				InvokeEntityMethod(m_cubeGridToUpdate.BackingObject, "Init", new object[] { m_cubeGridToUpdate.GetSubTypeEntity() });

				//Add the entity to the scene
				BaseEntityManagerWrapper.GetInstance().AddEntity(m_cubeGridToUpdate.BackingObject);

				m_cubeGridToUpdate = null;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
			}
		}

		#endregion
	}
}
