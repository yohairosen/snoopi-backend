﻿using System;
using System.Text.RegularExpressions;
using System.Web.Caching;

namespace Snoopi.core.Caching.Manager
{
    public static class CacheHelper
    {
        #region Delegates

        /// <summary>
        /// Represents the method that performs an action.
        /// Used when the object is not in the cache.
        /// </summary>
        public delegate T AddToCacheAction<T>();

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Caches the object of T.
        /// If the object is in the cache then it will return that object
        /// or else it will insert <see cref="addToCache"/> to the cache.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="addToCache">This will run if the object is not in the cache.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>The object from the cache.</returns>
        public static T CacheObject<T>(AddToCacheAction<T> addToCache, string cacheKey)
        {
            return CacheObject<T>(addToCache, cacheKey, DefaultCacheTime);
        }


        /// <summary>
        /// Caches the object of T.
        /// If the object is in the cache then it will return that object
        /// or else it will insert <see cref="addToCache"/> to the cache.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="addToCache">This will run if the object is not in the cache.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheDurationInSeconds">The cache duration in seconds.</param>
        /// <returns>The object from the cache.</returns>
        public static T CacheObject<T>(AddToCacheAction<T> addToCache, string cacheKey, int cacheDurationInSeconds)
        {
            return CacheObject<T>(addToCache, cacheKey, cacheDurationInSeconds, CacheItemPriority.Normal);
        }


        /// <summary>
        /// Caches the object of T.
        /// If the object is in the cache then it will return that object
        /// or else it will insert <see cref="addToCache"/> to the cache.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="addToCache">This will run if the object is not in the cache.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheDurationInSeconds">The cache duration in seconds.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The object from the cache.</returns>
        public static T CacheObject<T>(AddToCacheAction<T> addToCache, string cacheKey, int cacheDurationInSeconds, CacheItemPriority priority)
        {
            CacheManager<string, T> cacheManager = CacheManager<string, T>.GetInstance();

            T cachedObject = cacheManager[cacheKey];
            if (cachedObject == null)
            {
                cachedObject = addToCache();
                if (cachedObject != null)
                    cacheManager.Insert(cacheKey, cachedObject, cacheDurationInSeconds, priority);
            }
            return cachedObject;
        }


        /// <summary>
        /// Removes the cache object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        public static void RemoveCacheObject<T>(string cacheKey)
        {
            CacheManager<string, T>.GetInstance().Remove(cacheKey);
        }

        /// <summary>
        /// Removes all the objects from the cache that start with <see cref="keyStartsWith"/>.
        /// </summary>
        /// <param name="keyStartsWith">The starts with.</param>
        public static void RemoveAllCacheObjectStartWith(string keyStartsWith)
        {
            RemoveAll(delegate(string item)
            {
                return item.StartsWith(keyStartsWith);
            });
        }

        /// <summary>
        /// Removes all cache object that match the Regular Expression pattern.
        /// </summary>
        /// <param name="regxPattern">The regx.</param>
        public static void RemoveAllCacheObjectByPattern(string regxPattern)
        {
            Regex regex = new Regex(regxPattern);
            RemoveAll(delegate(string item)
            {
                return regex.Match(item).Success;
            });
        }

        /// <summary>
        /// Removes all Objects from the cache that match the condition.
        /// </summary>
        /// <param name="match">The Predicate<> delegate that defines the conditions of the elements to search for.</param>
        public static void RemoveAll(System.Predicate<string> match)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            CacheManager<string, string> cacheManager = CacheManager<string, string>.GetInstance();
            foreach (string item in cacheManager.GetListOfCachedKeys())
            {
                if (match(item.Contains("|") ? item.Substring(0, item.LastIndexOf("|")) : item))//this will remove the extra information from the end of the key.
                    cacheManager.RemoveByKey(item);
            }
        }

        /// <summary>
        /// Gets the get amount of items in cache.
        /// </summary>
        /// <value>The get amount of items in cache.</value>
        public static int GetAmountOfItemsInCache
        {
            get
            {
                return CacheManager<string, string>.GetInstance().GetCount();
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the get short cache time.
        /// </summary>
        /// <value>The get short cache time.</value>
        public static int ShortCacheTime
        {
            get
            {
                return 30;
            }
        }

        /// <summary>
        /// Gets the get normal cache time.
        /// </summary>
        /// <value>The get normal cache time.</value>
        public static int NormalCacheTime
        {
            get
            {
                return 360;
            }
        }

        /// <summary>
        /// Gets the get long cache time.
        /// </summary>
        /// <value>The get long cache time.</value>
        public static int LongCacheTime
        {
            get
            {
                return 3600;
            }
        }

        /// <summary>
        /// Gets the get default cache time.
        /// </summary>
        /// <value>The get default cache time.</value>
        public static int DefaultCacheTime
        {
            get
            {
                return NormalCacheTime;
            }
        }

        #endregion
    }
}
