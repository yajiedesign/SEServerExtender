﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

namespace SEModAPI.API.Internal
{
	public class CubeBlockInternalWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static CubeBlockInternalWrapper m_instance;

		private static Assembly m_assembly;

		private static Object m_cubeBlockToUpdate;
		private static string m_terminalBlockCustomName;
		private static bool m_functionalBlockEnabled;

		public static string CubeGridGetCubeBlocksHashSetMethod = "E38F3E9D7A76CD246B99F6AE91CC3E4A";

		public static string CubeBlockGetObjectBuilderMethod = "CBB75211A3B0B3188541907C9B1B0C5C";
		public static string CubeBlockGetSpecialBlockMethod = "7D4CAA3CE7687B9A7D20CCF3DE6F5441";

		public static string TerminalBlockClass = "6DDCED906C852CFDABA0B56B84D0BD74.CCFD704C70C3F20F7E84E8EA42D7A730";
		public static string TerminalBlockGetCustomNameMethod = "DE9705A29F3FE6F1E501595879B2E54F";
		public static string TerminalBlockSetCustomNameMethod = "774FC8084C0899CEF5C8DAE867B847FE";

		public static string FunctionalBlockClass = "6DDCED906C852CFDABA0B56B84D0BD74.7085736D64DCC58ED5DCA05FFEEA9664";
		public static string FunctionalBlockSetEnabledMethod = "97EC0047E8B562F4590B905BD8571F51";

		public static string ReactorBlockSetEnabledMethod = "E07EE72F25C9CA3C2EE6888D308A0E8D";

		#endregion

		#region "Constructors and Initializers"

		protected CubeBlockInternalWrapper(string basePath)
			: base(basePath)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			Console.WriteLine("Finished loading CubeBlockInternalWrapper");
		}

		new public static CubeBlockInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new CubeBlockInternalWrapper(basePath);
			}
			return (CubeBlockInternalWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		new public static bool IsDebugging
		{
			get
			{
				CubeBlockInternalWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				CubeBlockInternalWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		#region APIEntityLists

		public HashSet<Object> GetCubeBlocksHashSet(CubeGrid cubeGrid)
		{
			var rawValue = InvokeEntityMethod(cubeGrid.BackingObject, CubeGridGetCubeBlocksHashSetMethod, new object[] { });
			HashSet<Object> convertedSet = ConvertHashSet(rawValue);

			return convertedSet;
		}

		private List<T> GetAPIEntityCubeBlockList<T, TO>(CubeGrid cubeGrid, MyObjectBuilderTypeEnum type)
			where TO : MyObjectBuilder_CubeBlock
			where T : CubeBlockEntity<TO>
		{
			HashSet<Object> rawEntities = GetCubeBlocksHashSet(cubeGrid);
			List<T> list = new List<T>();

			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_CubeBlock baseEntity = (MyObjectBuilder_CubeBlock)InvokeEntityMethod(entity, CubeBlockGetObjectBuilderMethod, new object[] { });

					if (baseEntity.TypeId == type)
					{
						TO objectBuilder = (TO)baseEntity;
						T apiEntity = (T)Activator.CreateInstance(typeof(T), new object[] { objectBuilder });
						apiEntity.BackingObject = entity;
						apiEntity.BackingThread = GameObjectManagerWrapper.GetInstance().GameThread;

						list.Add(apiEntity);
					}
				}
				catch (Exception ex)
				{
					SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				}
			}

			return list;
		}

		public List<CubeBlockEntity<MyObjectBuilder_CubeBlock>> GetStructuralBlocks(CubeGrid cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CubeBlockEntity<MyObjectBuilder_CubeBlock>, MyObjectBuilder_CubeBlock>(cubeGrid, MyObjectBuilderTypeEnum.CubeBlock);
		}

		public List<CargoContainerEntity> GetCargoContainerBlocks(CubeGrid cubeGrid)
		{
			return GetAPIEntityCubeBlockList<CargoContainerEntity, MyObjectBuilder_CargoContainer>(cubeGrid, MyObjectBuilderTypeEnum.CargoContainer);
		}

		public List<ReactorEntity> GetReactorBlocks(CubeGrid cubeGrid)
		{
			return GetAPIEntityCubeBlockList<ReactorEntity, MyObjectBuilder_Reactor>(cubeGrid, MyObjectBuilderTypeEnum.Reactor);
		}

		#endregion

		public static string GetTerminalBlockCustomName(Object cubeBlockEntity)
		{
			try
			{
				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				StringBuilder customName = (StringBuilder)InvokeEntityMethod(specialCubeObject, TerminalBlockGetCustomNameMethod);

				return customName.ToString();
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return "";
			}
		}

		#region Updates

		public bool UpdateTerminalBlockCustomName(Object cubeBlockEntity, string customName)
		{
			try
			{
				m_terminalBlockCustomName = customName;
				m_cubeBlockToUpdate = cubeBlockEntity;

				Action action = InternalSetCustomName;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				throw ex;
			}
		}

		public bool UpdateFunctionalBlockEnabled(Object cubeBlockEntity, bool isBlockEnabled)
		{
			try
			{
				m_functionalBlockEnabled = isBlockEnabled;
				m_cubeBlockToUpdate = cubeBlockEntity;

				Action action = InternalSetFunctionalState;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				throw ex;
			}
		}

		#endregion

		#region Internal

		public static void InternalSetFunctionalState()
		{
			try
			{
				if (m_cubeBlockToUpdate == null)
					return;

				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				string blockName = GetTerminalBlockCustomName(specialCubeObject);

				if (m_isDebugging)
				{
					if (m_functionalBlockEnabled)
						Console.WriteLine("FunctionalBlock '" + blockName + "': Enabling");
					else
						Console.WriteLine("FunctionalBlock '" + blockName + "': Disabling");
				}

				InvokeEntityMethod(specialCubeObject, FunctionalBlockSetEnabledMethod, new object[] { m_functionalBlockEnabled });
				InvokeEntityMethod(specialCubeObject, ReactorBlockSetEnabledMethod, new object[] { m_functionalBlockEnabled });
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		public static void InternalSetCustomName()
		{
			try
			{
				if (m_cubeBlockToUpdate == null)
					return;

				Object specialCubeObject = InvokeEntityMethod(m_cubeBlockToUpdate, CubeBlockGetSpecialBlockMethod);
				string blockName = GetTerminalBlockCustomName(specialCubeObject);

				if (m_isDebugging)
				{
					Console.WriteLine("TerminalBlock '" + blockName + "': Setting custom name to '" + m_terminalBlockCustomName + "'");
				}

				StringBuilder newCustomName = new StringBuilder(m_terminalBlockCustomName);

				InvokeEntityMethod(specialCubeObject, TerminalBlockSetCustomNameMethod, new object[] { newCustomName });
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
			}
		}

		#endregion

		#endregion
	}
}
