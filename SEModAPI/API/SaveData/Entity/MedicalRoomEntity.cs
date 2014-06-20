﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class MedicalRoomEntity : CubeBlockEntity<MyObjectBuilder_MedicalRoom>
	{
		#region "Constructors and Initializers"

		public MedicalRoomEntity(MyObjectBuilder_MedicalRoom definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Medical Room")]
		[Browsable(false)]
		new public MyObjectBuilder_MedicalRoom BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		[Category("Medical Room")]
		public ulong SteamUserId
		{
			get { return m_baseDefinition.SteamUserId; }
			set
			{
				if (m_baseDefinition.SteamUserId == value) return;
				m_baseDefinition.SteamUserId = value;
				Changed = true;
			}
		}

		#endregion
	}
}
