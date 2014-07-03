﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class ReactorEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		public static string ReactorNamespace = "";
		public static string ReactorClass = "";

		public static string ReactorGetInventoryMethod = "GetInventory";

		#endregion

		#region "Constructors and Initializers"

		public ReactorEntity(MyObjectBuilder_Reactor definition)
			: base(definition)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		public ReactorEntity(MyObjectBuilder_Reactor definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_Inventory = new InventoryEntity(definition.Inventory, InternalGetCharacterInventory());
		}

		#endregion

		#region "Properties"

		[Category("Reactor")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get
			{
				if (m_Inventory.BackingObject == null)
					m_Inventory.BackingObject = InternalGetCharacterInventory();
				return m_Inventory;
			}
		}

		[Category("Reactor")]
		public float Fuel
		{
			get
			{
				float fuelMass = 0;
				foreach (var item in m_Inventory.Items)
				{
					fuelMass += item.Mass * item.Amount;
				}
				return fuelMass;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Reactor GetSubTypeEntity()
		{
			MyObjectBuilder_Reactor reactor = (MyObjectBuilder_Reactor)BaseEntity;

			//Make sure the inventory is up-to-date
			reactor.Inventory = Inventory.GetSubTypeEntity();

			return reactor;
		}

		#region "Internal"

		protected Object InternalGetCharacterInventory()
		{
			try
			{
				Object baseObject = BackingObject;
				Object actualObject = GetActualObject();
				Object inventory = InvokeEntityMethod(actualObject, ReactorGetInventoryMethod, new object[] { 0 });

				return inventory;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}
}
