using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Off_EE
{
	public delegate void OnClean();
	public static class CacheHandler
	{
		public static event OnClean OnClean;
		private static Dictionary<int, Dictionary<object, object>> cache = new Dictionary<int, Dictionary<object, object>>();
		private static int _next = int.MinValue;

		/// <summary>
		/// Clean all of the caches
		/// </summary>
		public static void Clean()
		{
			_next = int.MinValue;
			cache.Clear();
			cache = new Dictionary<int, Dictionary<object, object>>();
			if (CacheHandler.OnClean != null)
			{
				OnClean();
			}
		}

		/// <summary>
		/// Clean all of the items in a cache
		/// </summary>
		/// <param name="CacheId">The id of the cache to clean</param>
		public static void Clean(int CacheId)
		{
			if (cache.ContainsKey(CacheId))
			{
				cache[CacheId].Clear();
				cache[CacheId] = new Dictionary<object, object>();
			}
		}

		/// <summary>
		/// Get a valid key that no other cache uses
		/// </summary>
		public static int NextCacheId
		{
			get
			{
				cache.Add(_next, new Dictionary<object, object>());
				return _next++;
			}
			set
			{
				_next = value;
			}
		}

		/// <summary>
		/// Get the amount of objects in a cache
		/// </summary>
		/// <param name="CacheId">The id of the cache</param>
		/// <returns>-1 if the cache doesn't exist, otherwise returns the amount of items in the cache</returns>
		public static int GetCacheAmount(int CacheId)
		{
			if(cache.ContainsKey(CacheId))
				return cache[CacheId].Count;
			return -1;
		}

		/// <summary>
		/// Store an object in the cache
		/// </summary>
		/// <param name="CacheId">The id of the cache to store the object in</param>
		/// <param name="Key">The key to store the object with</param>
		/// <param name="Store">The object to store</param>
		public static void StoreCacheObject(int CacheId, object Key, object Store)
		{
			if (cache.ContainsKey(CacheId))
				cache[CacheId][Key] = Store;
		}

		/// <summary>
		/// Remove an object in the cache
		/// </summary>
		/// <param name="CacheId">The cache id of the cache</param>
		/// <param name="Key">The key of the object to remove</param>
		public static void CleanCacheObject(int CacheId, object Key)
		{
			if (cache.ContainsKey(CacheId))
				if (cache[CacheId].ContainsKey(Key))
					cache[CacheId].Remove(Key);
		}

		/// <summary>
		/// Get a cache object
		/// </summary>
		/// <param name="CacheId">The Id of the cache</param>
		/// <param name="Key">The object key</param>
		/// <returns>null if the cache doesn't exist or the key in the cache doesn't exist</returns>
		public static object GetCacheObject(int CacheId, object Key)
		{
			if (cache.ContainsKey(CacheId))
				if (cache[CacheId].ContainsKey(Key))
					return cache[CacheId][Key];
			return null;
		}

		/// <summary>
		/// Check if a cache contains the cache id
		/// </summary>
		/// <param name="CacheId">The cache id</param>
		/// <param name="Key">The object key to check</param>
		/// <returns>true if the cache exists and the key is in there, and false if otherwise.</returns>
		public static bool CacheContainsKey(int CacheId, object Key)
		{
			if (cache.ContainsKey(CacheId))
				if (cache[CacheId].ContainsKey(Key))
					return true;
			return false;
		}

		/// <summary>
		/// Get all of the keys in a cache
		/// </summary>
		/// <param name="CacheId">The id of the cache</param>
		/// <returns>null if the cache doesn't exist, otherwise returns the keys of the cache.</returns>
		public static Dictionary<object, object>.KeyCollection CacheKeys(int CacheId)
		{
			if (cache.ContainsKey(CacheId))
				return cache[CacheId].Keys;
			return null;
		}

		/// <summary>
		/// Get all of the keys in a cache
		/// </summary>
		/// <param name="CacheId">The id of the cache</param>
		/// <returns>null if the cache doesn't exist, otherwise returns the keys of the cache.</returns>
		public static Dictionary<object, object>.ValueCollection CacheValues(int CacheId)
		{
			if (cache.ContainsKey(CacheId))
				return cache[CacheId].Values;
			return null;
		}
	}

	/// <summary>
	/// A Cache to store items
	/// </summary>
	public class Cache
	{
		private int _key;
		private int _capacity;

		#region Event Handler
		/// <summary>
		/// Get a new cache key when cleaning happens
		/// </summary>
		private void CacheHandler_OnClean()
		{
			_key = CacheHandler.NextCacheId;
		}
		#endregion

		#region Constructors
		
		/// <summary>
		/// Create a new cache
		/// </summary>
		/// <param name="MaxAmount">The maximum capacity of the dictionary</param>
		public Cache(int MaxAmount)
		{
			CacheHandler.OnClean += CacheHandler_OnClean;
			_capacity = MaxAmount;
			_key = CacheHandler.NextCacheId;
		}
		#endregion

		#region Cache Handlers
		/// <summary>
		/// Store an object in the cache
		/// </summary>
		/// <param name="key">The key of it</param>
		/// <param name="store">The object to store</param>
		public void Store(object key, object store)
		{
			CacheHandler.StoreCacheObject(_key, key, store);
			if (_capacity != -1)
			{
				if (CacheHandler.GetCacheAmount(_key) > _capacity)
				{
					CacheHandler.CleanCacheObject(_key, GetKey(0));
				}
			}
		}

		/// <summary>
		/// Get an object
		/// </summary>
		/// <param name="key">The key to get</param>
		/// <returns></returns>
		public object Get(object key)
		{
			return CacheHandler.GetCacheObject(_key, key);
		}
		
		/// <summary>
		/// Get the key of an object at a certain index
		/// </summary>
		/// <param name="index">The index to get the object from</param>
		/// <returns>null if the index was greater than the count.</returns>
		public object GetKey(int index)
		{
			if (index > _capacity)
				return null;

			int _c = 0;
			foreach(var i in CacheHandler.CacheKeys(_key))
			{
				if (_c == index)
					return i;
				_c++;
			}
			return null;
		}

		/// <summary>
		/// Get the key of an object at a certain index
		/// </summary>
		/// <param name="index">The index to get the object from</param>
		/// <returns>null if the index was greater than the count.</returns>
		public object GetValue(int index)
		{
			if (index > _capacity)
				return null;

			int _c = 0;
			foreach (var i in CacheHandler.CacheValues(_key))
			{
				if (_c == index)
					return i;
				_c++;
			}
			return null;
		}

		/// <summary>
		/// Get a list of keys for the cache
		/// </summary>
		/// <returns>The keys of the cache</returns>
		public Dictionary<object, object>.KeyCollection GetKeys()
		{
			return CacheHandler.CacheKeys(_key);
		}

		/// <summary>
		/// Get a list of values for the cache
		/// </summary>
		/// <returns>The values of the cache</returns>
		public Dictionary<object, object>.ValueCollection GetValues()
		{
			return CacheHandler.CacheValues(_key);
		}

		/// <summary>
		/// Check if the cache contains the key specified
		/// </summary>
		/// <param name="key">The key id</param>
		/// <returns>If the object exists in the cache</returns>
		public bool ContainsKey(object key)
		{
			return CacheHandler.CacheContainsKey(_key, key);
		}

		/// <summary>
		/// Clean the cache
		/// </summary>
		public void Clean()
		{
			CacheHandler.Clean(_key);
		}

		/// <summary>
		/// Remove an object from the cache
		/// </summary>
		/// <param name="key">The key of the object to remove</param>
		public void Remove(object key)
		{
			CacheHandler.CleanCacheObject(_key, key);
		}
		#endregion

		/// <summary>
		/// Clone the cache
		/// </summary>
		/// <returns>A clone of this cache.</returns>
		public Cache Clone()
		{
			var clone = new Cache(_capacity);
			clone._key = _key;
			return clone;
		}

		public int Count
		{
			get
			{
				return CacheHandler.GetCacheAmount(_key);
			}
			set
			{

			}
		}
		public int Length
		{
			get
			{
				return Count;
			}
			set
			{
				Count = value;
			}
		}
	}
}
