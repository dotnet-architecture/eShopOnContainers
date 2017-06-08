namespace Microsoft.eShopOnContainers.BuildingBlocks
{
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    /// <summary>
    /// Key repository that stores XML encrypted keys in a Redis distributed cache.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The values stored in Redis are XML documents that contain encrypted session
    /// keys used for the protection of things like session state. The document contents
    /// are double-encrypted - first with a changing session key; then by a master key.
    /// As such, there's no risk in storing the keys in Redis - even if someone can crack
    /// the master key, they still need to also crack the session key. (Other solutions
    /// for sharing keys across a farm environment include writing them to files
    /// on a file share.)
    /// </para>
    /// <para>
    /// While the repository uses a hash to keep the set of encrypted keys separate, you
    /// can further separate these items from other items in Redis by specifying a unique
    /// database in the connection string.
    /// </para>
    /// <para>
    /// Consumers of the repository are responsible for caching the XML items as needed.
    /// Typically repositories are consumed by things like <see cref="Microsoft.AspNetCore.DataProtection.KeyManagement.KeyRingProvider"/>
    /// which generates <see cref="Microsoft.AspNetCore.DataProtection.KeyManagement.Internal.CacheableKeyRing"/>
    /// values that get cached. The mechanism is already optimized for caching so there's
    /// no need to create a redundant cache.
    /// </para>
    /// </remarks>
    /// <seealso cref="Microsoft.AspNetCore.DataProtection.Repositories.IXmlRepository" />
    /// <seealso cref="System.IDisposable" />
    public class RedisXmlRepository : IXmlRepository, IDisposable
    {
        /// <summary>
        /// The root cache key for XML items stored in Redis
        /// </summary>
        public static readonly string RedisHashKey = "DataProtectionXmlRepository";

        /// <summary>
        /// The connection to the Redis backing store.
        /// </summary>
        private IConnectionMultiplexer _connection;

        /// <summary>
        /// Flag indicating whether the object has been disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisXmlRepository"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The Redis connection string.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger{T}"/> used to log diagnostic messages.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="connectionString" /> or <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public RedisXmlRepository(string connectionString, ILogger<RedisXmlRepository> logger)
            : this(ConnectionMultiplexer.Connect(connectionString), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisXmlRepository"/> class.
        /// </summary>
        /// <param name="connection">
        /// The Redis database connection.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger{T}"/> used to log diagnostic messages.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="connection" /> or <paramref name="logger" /> is <see langword="null" />.
        /// </exception>
        public RedisXmlRepository(IConnectionMultiplexer connection, ILogger<RedisXmlRepository> logger)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._connection = connection;
            this.Logger = logger;

            // Mask the password so it doesn't get logged.
            var configuration = Regex.Replace(this._connection.Configuration, @"password\s*=\s*[^,]*", "password=****", RegexOptions.IgnoreCase);
            this.Logger.LogDebug("Storing data protection keys in Redis: {RedisConfiguration}", configuration);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The <see cref="ILogger{T}"/> used to log diagnostic messages.
        /// </value>
        public ILogger<RedisXmlRepository> Logger { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Gets all top-level XML elements in the repository.
        /// </summary>
        /// <returns>
        /// An <see cref="IReadOnlyCollection{T}"/> with the set of elements
        /// stored in the repository.
        /// </returns>
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var database = this._connection.GetDatabase();
            var hash = database.HashGetAll(RedisHashKey);
            var elements = new List<XElement>();

            if (hash == null || hash.Length == 0)
            {
                return elements.AsReadOnly();
            }

            foreach (var item in hash.ToStringDictionary())
            {
                elements.Add(XElement.Parse(item.Value));
            }

            this.Logger.LogDebug("Read {XmlElementCount} XML elements from Redis.", elements.Count);
            return elements.AsReadOnly();
        }

        /// <summary>
        /// Adds a top-level XML element to the repository.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="friendlyName">
        /// An optional name to be associated with the XML element.
        /// For instance, if this repository stores XML files on disk, the friendly name may
        /// be used as part of the file name. Repository implementations are not required to
        /// observe this parameter even if it has been provided by the caller.
        /// </param>
        /// <remarks>
        /// The <paramref name="friendlyName" /> parameter must be unique if specified.
        /// For instance, it could be the ID of the key being stored.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="element" /> is <see langword="null" />.
        /// </exception>
        public void StoreElement(XElement element, string friendlyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (string.IsNullOrEmpty(friendlyName))
            {
                // The framework always passes in a name, but
                // the contract indicates this may be null or empty.
                friendlyName = Guid.NewGuid().ToString();
            }

            this.Logger.LogDebug("Storing XML element with friendly name {XmlElementFriendlyName}.", friendlyName);

            this._connection.GetDatabase().HashSet(RedisHashKey, friendlyName, element.ToString());
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._connection != null)
                    {
                        this._connection.Close();
                        this._connection.Dispose();
                    }
                }

                this._connection = null;
                this._disposed = true;
            }
        }
    }
}
