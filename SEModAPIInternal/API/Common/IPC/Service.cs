﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;

namespace SEModAPIInternal.API.Common.IPC
{
	public class InternalService : IInternalServiceContract
	{
		public List<ulong> GetConnectedPlayers()
		{
			return PlayerManager.Instance.ConnectedPlayers;
		}

		public List<BaseEntity> GetSectorEntities()
		{
			return SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
		}
	}
}
