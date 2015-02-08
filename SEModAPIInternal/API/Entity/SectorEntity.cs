﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Voxels;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class SectorEntity : BaseObject
	{
		#region "Attributes"

		//Sector Events
		private BaseObjectManager _eventManager;

		//Sector Objects
		private BaseObjectManager _cubeGridManager;

		private BaseObjectManager _voxelMapManager;
		private BaseObjectManager _floatingObjectManager;
		private BaseObjectManager _meteorManager;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorEntity( MyObjectBuilder_Sector definition )
			: base( definition )
		{
			_eventManager = new BaseObjectManager( );
			_cubeGridManager = new BaseObjectManager( );
			_voxelMapManager = new BaseObjectManager( );
			_floatingObjectManager = new BaseObjectManager( );
			_meteorManager = new BaseObjectManager( );

			List<Event> events = new List<Event>( );
			foreach ( MyObjectBuilder_GlobalEventBase sectorEvent in definition.SectorEvents.Events )
			{
				events.Add( new Event( sectorEvent ) );
			}

			List<CubeGridEntity> cubeGrids = new List<CubeGridEntity>( );
			List<VoxelMap> voxelMaps = new List<VoxelMap>( );
			List<FloatingObject> floatingObjects = new List<FloatingObject>( );
			List<Meteor> meteors = new List<Meteor>( );
			foreach ( MyObjectBuilder_EntityBase sectorObject in definition.SectorObjects )
			{
				if ( sectorObject.TypeId == typeof( MyObjectBuilder_CubeGrid ) )
				{
					cubeGrids.Add( new CubeGridEntity( (MyObjectBuilder_CubeGrid)sectorObject ) );
				}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_VoxelMap ) )
				{
					voxelMaps.Add( new VoxelMap( (MyObjectBuilder_VoxelMap)sectorObject ) );
				}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_FloatingObject ) )
				{
					floatingObjects.Add( new FloatingObject( (MyObjectBuilder_FloatingObject)sectorObject ) );
				}
				else if ( sectorObject.TypeId == typeof( MyObjectBuilder_Meteor ) )
				{
					meteors.Add( new Meteor( (MyObjectBuilder_Meteor)sectorObject ) );
				}
			}

			//Build the managers from the lists
			_eventManager.Load( events );
			_cubeGridManager.Load( cubeGrids );
			_voxelMapManager.Load( voxelMaps );
			_floatingObjectManager.Load( floatingObjects );
			_meteorManager.Load( meteors );
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[Category( "Sector" )]
		[Browsable( true )]
		[ReadOnly( true )]
		[Description( "The formatted name of the object" )]
		public override string Name
		{
			get { return "SANDBOX_" + Position.X + "_" + Position.Y + "_" + Position.Z + "_"; }
		}

		[Category( "Sector" )]
		[Browsable( false )]
		[ReadOnly( true )]
		internal new MyObjectBuilder_Sector ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Sector baseSector = (MyObjectBuilder_Sector)base.ObjectBuilder;

				try
				{
					//Update the events in the base definition
					baseSector.SectorEvents.Events.Clear( );
					foreach ( Event item in _eventManager.GetTypedInternalData<Event>( ) )
					{
						baseSector.SectorEvents.Events.Add( item.ObjectBuilder );
					}

					//Update the sector objects in the base definition
					baseSector.SectorObjects.Clear( );
					foreach ( CubeGridEntity item in _cubeGridManager.GetTypedInternalData<CubeGridEntity>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( VoxelMap item in _voxelMapManager.GetTypedInternalData<VoxelMap>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( FloatingObject item in _floatingObjectManager.GetTypedInternalData<FloatingObject>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
					foreach ( Meteor item in _meteorManager.GetTypedInternalData<Meteor>( ) )
					{
						baseSector.SectorObjects.Add( item.ObjectBuilder );
					}
				}
				catch ( Exception ex )
				{
					LogManager.ErrorLog.WriteLine( ex );
				}
				return baseSector;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category( "Sector" )]
		public VRageMath.Vector3I Position
		{
			get { return ObjectBuilder.Position; }
		}

		[Category( "Sector" )]
		public int AppVersion
		{
			get { return ObjectBuilder.AppVersion; }
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<Event> Events
		{
			get
			{
				List<Event> newList = _eventManager.GetTypedInternalData<Event>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<CubeGridEntity> CubeGrids
		{
			get
			{
				List<CubeGridEntity> newList = _cubeGridManager.GetTypedInternalData<CubeGridEntity>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<VoxelMap> VoxelMaps
		{
			get
			{
				List<VoxelMap> newList = _voxelMapManager.GetTypedInternalData<VoxelMap>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<FloatingObject> FloatingObjects
		{
			get
			{
				List<FloatingObject> newList = _floatingObjectManager.GetTypedInternalData<FloatingObject>( );
				return newList;
			}
		}

		[Category( "Sector" )]
		[Browsable( false )]
		public List<Meteor> Meteors
		{
			get
			{
				List<Meteor> newList = _meteorManager.GetTypedInternalData<Meteor>( );
				return newList;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public BaseObject NewEntry( Type newType )
		{
			if ( newType == typeof( CubeGridEntity ) )
				return _cubeGridManager.NewEntry<CubeGridEntity>( );
			if ( newType == typeof( VoxelMap ) )
				return _voxelMapManager.NewEntry<VoxelMap>( );
			if ( newType == typeof( FloatingObject ) )
				return _floatingObjectManager.NewEntry<FloatingObject>( );
			if ( newType == typeof( Meteor ) )
				return _meteorManager.NewEntry<Meteor>( );

			return null;
		}

		public bool DeleteEntry( Object source )
		{
			Type deleteType = source.GetType( );
			if ( deleteType == typeof( CubeGridEntity ) )
				return _cubeGridManager.DeleteEntry( (CubeGridEntity)source );
			if ( deleteType == typeof( VoxelMap ) )
				return _voxelMapManager.DeleteEntry( (VoxelMap)source );
			if ( deleteType == typeof( FloatingObject ) )
				return _floatingObjectManager.DeleteEntry( (FloatingObject)source );
			if ( deleteType == typeof( Meteor ) )
				return _meteorManager.DeleteEntry( (Meteor)source );

			return false;
		}

		#endregion "Methods"
	}

	public class SectorObjectManager : BaseObjectManager
	{
		#region "Attributes"

		private static SectorObjectManager _instance;
		private static Queue<BaseEntity> _addEntityQueue = new Queue<BaseEntity>( );

		public static string ObjectManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		/////////////////////////////////////////////////////////////////

		public static string ObjectFactoryNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ObjectFactoryClass = "E825333D6467D99DD83FB850C600395C";

		/////////////////////////////////////////////////////////////////

		//2 Packet Types
		public static string EntityBaseNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";

		public static string EntityBaseNetManagerClass = "8EFE49A46AB934472427B7D117FD3C64";
		public static string EntityBaseNetManagerSendEntity = "A6B585C993B43E72219511726BBB0649";

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorObjectManager( )
		{
			IsDynamic = true;
			_instance = this;
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public static SectorObjectManager Instance
		{
			get { return _instance ?? ( _instance = new SectorObjectManager( ) ); }
		}

		public static Type InternalType
		{
			get
			{
				Type objectManagerType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ObjectManagerNamespace, ObjectManagerClass );
				return objectManagerType;
			}
		}

		public static bool QueueFull
		{
			get
			{
				if ( _addEntityQueue.Count >= 25 )
					return true;

				return false;
			}
		}

		#endregion "Properties"

		#region "Methods"

		public static bool ReflectionUnitTest( )
		{
			try
			{
				Type type = InternalType;
				if ( type == null )
					throw new Exception( "Could not find internal type for SectorObjectManager" );
				bool result = true;
				result &= BaseObject.HasMethod( type, ObjectManagerGetEntityHashSet );
				result &= BaseObject.HasMethod( type, ObjectManagerAddEntity );

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( ObjectFactoryNamespace, ObjectFactoryClass );
				if ( type2 == null )
					throw new Exception( "Could not find object factory type for SectorObjectManager" );

				Type type3 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( EntityBaseNetManagerNamespace, EntityBaseNetManagerClass );
				if ( type3 == null )
					throw new Exception( "Could not find entity base network manager type for SectorObjectManager" );
				result &= BaseObject.HasMethod( type3, EntityBaseNetManagerSendEntity );

				return result;
			}
			catch ( Exception ex )
			{
				LogManager.APILog.WriteLine( ex );
				return false;
			}
		}

		protected override bool IsValidEntity( Object entity )
		{
			try
			{
				if ( entity == null )
					return false;

				//Skip unknowns for now until we get the bugs sorted out with the other types
				Type entityType = entity.GetType( );
				if ( entityType != CharacterEntity.InternalType &&
					entityType != CubeGridEntity.InternalType &&
					entityType != VoxelMap.InternalType &&
					entityType != FloatingObject.InternalType &&
					entityType != Meteor.InternalType
					)
					return false;

				//Skip disposed entities
				bool isDisposed = (bool)BaseObject.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetIsDisposedMethod );
				if ( isDisposed )
					return false;

				//Skip entities that have invalid physics objects
				if ( BaseEntity.GetRigidBody( entity ) == null || BaseEntity.GetRigidBody( entity ).IsDisposed )
					return false;

				//Skip entities that don't have a position-orientation matrix defined
				if ( BaseObject.InvokeEntityMethod( entity, BaseEntity.BaseEntityGetOrientationMatrixMethod ) == null )
					return false;

				return true;
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				return false;
			}
		}

		protected override void InternalRefreshBackingDataHashSet( )
		{
			try
			{
				if ( !CanRefresh )
					return;

				RawDataHashSetResourceLock.AcquireExclusive( );

				object rawValue = BaseObject.InvokeStaticMethod( InternalType, ObjectManagerGetEntityHashSet );
				if ( rawValue == null )
					return;

				//Create/Clear the hash set
				if ( RawDataHashSet == null )
					RawDataHashSet = new HashSet<object>( );
				else
					RawDataHashSet.Clear( );

				//Only allow valid entities in the hash set
				foreach ( object entry in UtilityFunctions.ConvertHashSet( rawValue ) )
				{
					if ( !IsValidEntity( entry ) )
						continue;

					RawDataHashSet.Add( entry );
				}

				RawDataHashSetResourceLock.ReleaseExclusive( );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
				if ( RawDataHashSetResourceLock.Owned )
					RawDataHashSetResourceLock.ReleaseExclusive( );
			}
		}

		protected override void LoadDynamic( )
		{
			try
			{
				HashSet<Object> rawEntities = GetBackingDataHashSet( );
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>( GetInternalData( ) );

				//Update the main data mapping
				foreach ( Object entity in rawEntities )
				{
					try
					{
						long entityId = BaseEntity.GetEntityId( entity );
						if ( entityId == 0 )
							continue;

						if ( !IsValidEntity( entity ) )
							continue;

						MyObjectBuilder_EntityBase baseEntity = BaseEntity.GetObjectBuilder( entity );
						if ( baseEntity == null )
							continue;
						if ( !EntityRegistry.Instance.ContainsGameType( baseEntity.TypeId ) )
							continue;

						//If the original data already contains an entry for this, skip creation and just update values
						if ( GetInternalData( ).ContainsKey( entityId ) )
						{
							BaseEntity matchingEntity = (BaseEntity)GetEntry( entityId );
							if ( matchingEntity == null || matchingEntity.IsDisposed )
								continue;

							matchingEntity.BackingObject = entity;
							matchingEntity.ObjectBuilder = baseEntity;
						}
						else
						{
							BaseEntity newEntity = null;

							//Get the matching API type from the registry
							Type apiType = EntityRegistry.Instance.GetAPIType( baseEntity.TypeId );

							//Create a new API entity
							newEntity = (BaseEntity)Activator.CreateInstance( apiType, new object[ ] { baseEntity, entity } );

							if ( newEntity != null )
								AddEntry( newEntity.EntityId, newEntity );
						}
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				//Cleanup old entities
				foreach ( KeyValuePair<long, BaseObject> entry in internalDataCopy )
				{
					try
					{
						if ( !rawEntities.Contains( entry.Value.BackingObject ) )
							DeleteEntry( entry.Value );
					}
					catch ( Exception ex )
					{
						LogManager.ErrorLog.WriteLine( ex );
					}
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		public void AddEntity( BaseEntity entity )
		{
			try
			{
				if ( _addEntityQueue.Count >= 25 )
				{
					throw new Exception( "AddEntity queue is full. Cannot add more entities yet" );
				}

				if ( SandboxGameAssemblyWrapper.IsDebugging )
					Console.WriteLine( entity.GetType( ).Name + " '" + entity.Name + "' is being added ..." );

				_addEntityQueue.Enqueue( entity );

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction( action );
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		protected void InternalAddEntity( )
		{
			try
			{
				if ( _addEntityQueue.Count == 0 )
					return;

				BaseEntity entityToAdd = _addEntityQueue.Dequeue( );

				if ( SandboxGameAssemblyWrapper.IsDebugging )
					Console.WriteLine( entityToAdd.GetType( ).Name + " '" + entityToAdd.GetType( ).Name + "': Adding to scene ..." );

				//Create the backing object
				Type entityType = entityToAdd.GetType( );
				Type internalType = (Type)BaseObject.InvokeStaticMethod( entityType, "get_InternalType" );
				if ( internalType == null )
					throw new Exception( "Could not get internal type of entity" );
				entityToAdd.BackingObject = Activator.CreateInstance( internalType );

				//Initialize the backing object
				BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, "Init", new object[ ] { entityToAdd.ObjectBuilder } );

				//Add the backing object to the main game object manager
				BaseObject.InvokeStaticMethod( InternalType, ObjectManagerAddEntity, new object[ ] { entityToAdd.BackingObject, true } );

				if ( entityToAdd is FloatingObject )
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[ ] { Type.Missing } );
						//TODO - Do stuff

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch ( Exception ex )
					{
						LogManager.APILog.WriteLineAndConsole( "Failed to broadcast new floating object" );
						LogManager.ErrorLog.WriteLine( ex );
					}
				}
				else
				{
					try
					{
						//Broadcast the new entity to the clients
						MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseObject.InvokeEntityMethod( entityToAdd.BackingObject, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[ ] { Type.Missing } );
						Type someManager = SandboxGameAssemblyWrapper.Instance.GetAssemblyType( EntityBaseNetManagerNamespace, EntityBaseNetManagerClass );
						BaseObject.InvokeStaticMethod( someManager, EntityBaseNetManagerSendEntity, new object[ ] { baseEntity } );

						entityToAdd.ObjectBuilder = baseEntity;
					}
					catch ( Exception ex )
					{
						LogManager.APILog.WriteLineAndConsole( "Failed to broadcast new entity" );
						LogManager.ErrorLog.WriteLine( ex );
					}
				}

				if ( SandboxGameAssemblyWrapper.IsDebugging )
				{
					Type type = entityToAdd.GetType( );
					Console.WriteLine( type.Name + " '" + entityToAdd.Name + "': Finished adding to scene" );
				}
			}
			catch ( Exception ex )
			{
				LogManager.ErrorLog.WriteLine( ex );
			}
		}

		#endregion "Methods"
	}

	public class SectorManager : BaseObjectManager
	{
		#region "Attributes"

		private SectorEntity _Sector;

		#endregion "Attributes"

		#region "Constructors and Initializers"

		public SectorManager( )
		{
		}

		#endregion "Constructors and Initializers"

		#region "Properties"

		public SectorEntity Sector
		{
			get { return _Sector; }
		}

		#endregion "Properties"

		#region "Methods"

		public void Load( FileInfo fileInfo )
		{
			//Save the file info to the property
			FileInfo = fileInfo;

			//Read in the sector data
			MyObjectBuilder_Sector data = ReadSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( FileInfo.FullName );

			//And instantiate the sector with the data
			_Sector = new SectorEntity( data );
		}

		new public bool Save( )
		{
			return WriteSpaceEngineersFile<MyObjectBuilder_Sector, MyObjectBuilder_SectorSerializer>( _Sector.ObjectBuilder, FileInfo.FullName );
		}

		#endregion "Methods"
	}
}