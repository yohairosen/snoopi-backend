using System.Collections.Generic;
using System.Web.Caching;
using Snoopi.core.Caching.Providers;

namespace Snoopi.core.Caching.Manager
{
    /// <summary>
    /// The Cache Manager.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class CacheManager<TKey, TValue>
    {

        #region Member Variables

        private static CacheManager<TKey, TValue> _currentCache = null;
        private static readonly object _cacheLock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        private CacheManager()
        {
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <value></value>
        public TValue this[TKey key]
        {
            get
            {
                return GetFromCache(key);
            }
        }

        #endregion

        #region Methods

        #region Public Static

        #region Cache Object Helpers

        /// <summary>
        /// Gets from cache object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static TValue GetFromCache(TKey key)
        {
            return (TValue)GetFromCache(MakeKey(key));
        }

        /// <summary>
        /// Gets from cache object.
        /// This doesn't use any boxing.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static object GetFromCacheObject(TKey key)
        {
            return GetFromCache(MakeKey(key));
        }

        /// <summary>
        /// Gets from cache object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static object GetFromCache(string key)
        {
            return GenericCacheManager.GetInstance().CacheProvider[key];
        }

        #endregion

        /// <summary>
        /// Gets the instance of the cache manager.
        /// </summary>
        /// <returns></returns>
        public static CacheManager<TKey, TValue> GetInstance()
        {
            if (_currentCache == null)
            {
                lock (_cacheLock)
                {
                    if (_currentCache == null)
                    {
                        _currentCache = new CacheManager<TKey, TValue>();
                    }
                }
            }
            return _currentCache;
        }

        #endregion

        #region Public

        /// <summary>
        /// Determines whether the Cache contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the cache contains the key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return GetFromCacheObject(key) != null;
        }

        /// <summary>
        /// Gets the specified key from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            return GetFromCache(key);
        }

        /// <summary>
        /// Inserts the specified key into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(TKey key, TValue value)
        {
            Insert(key, value, CacheHelper.DefaultCacheTime);
        }

        /// <summary>
        /// Inserts the specified key into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cacheDurationInSeconds">The cache duration in seconds.</param>
        public void Insert(TKey key, TValue value, int cacheDurationInSeconds)
        {
            Insert(key, value, cacheDurationInSeconds, CacheItemPriority.Default);
        }

        /// <summary>
        /// Inserts the specified key into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="cacheDurationInSeconds">The cache duration in seconds.</param>
        /// <param name="priority">The priority.</param>
        public void Insert(TKey key, TValue value, int cacheDurationInSeconds, CacheItemPriority priority)
        {
            CacheProvider.Insert(MakeKey(key), value, cacheDurationInSeconds, priority);
        }

        /// <summary>
        /// Removes the specified key from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(TKey key)
        {
            CacheProvider.Remove(MakeKey(key));
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            CacheProvider.ClearCache();
        }

        /// <summary>
        /// Gets the amount of items in the cache.
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return CacheProvider.GetCount();
        }

        #endregion

        #region Private

        /// <summary>
        /// Creates the key used for identifying the cache object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string MakeKey(TKey key)
        {
            return key + "|" + typeof(TKey).ToString() + key.GetHashCode();
        }

        #endregion

        #region internal

        /// <summary>
        /// Gets a list of cached objects.
        /// </summary>
        /// <returns></returns>
        internal IList<string> GetListOfCachedKeys()
        {
            System.Collections.IDictionaryEnumerator itemsInCache = CacheProvider.GetEnumerator();
            IList<string> keysInCache = new List<string>();
            while (itemsInCache.MoveNext())
            {
                keysInCache.Add(itemsInCache.Key.ToString());
            }
            return keysInCache;
        }

        /// <summary>
        /// Removes the specified key from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        internal void RemoveByKey(string key)
        {
            CacheProvider.Remove(key);
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cache provider.
        /// </summary>
        /// <value>The cache provider.</value>
        public ICacheProvider CacheProvider
        {
            get { return GenericCacheManager.GetInstance().CacheProvider; }
        }

        #endregion

    }

    /// <summary>
    /// This is a Generic Cache Manager That doesn't know the type of object that you are using
    /// That way it just loads once the settings that don't have anything with the type.
    /// </summary>
    internal class GenericCacheManager
    {

        #region Member Variables

        private static GenericCacheManager _genericCacheManager;
        private static object _thisLock = new object();

        private ICacheProvider _cacheProvider;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCacheManager"/> class.
        /// </summary>
        private GenericCacheManager()
        {
            SetUseCache(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a instance of this class.
        /// </summary>
        /// <returns></returns>
        public static GenericCacheManager GetInstance()
        {
            if (_genericCacheManager == null)
            {
                lock (_thisLock)
                {
                    if (_genericCacheManager == null)
                    {
                        _genericCacheManager = new GenericCacheManager();
                    }
                }
            }
            return _genericCacheManager;
        }

        /// <summary>
        /// Sets the cache settings.
        /// </summary>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        private void SetUseCache(bool useCache)
        {
            if (useCache)
                _cacheProvider = new HttpRuntimeCache();
            else
                _cacheProvider = new NullCache();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cache provider.
        /// </summary>
        /// <value>The cache provider.</value>
        public ICacheProvider CacheProvider
        {
            get { return _cacheProvider; }
            private set { _cacheProvider = value; }
        }

        #endregion
    }
}
