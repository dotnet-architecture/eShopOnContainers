// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Microsoft.Extensions.HealthChecks
{
    // REVIEW: Do we want these to continue to use default parameters?
    // REVIEW: What are the appropriate guards for these functions?

    public static class AzureHealthCheckBuilderExtensions
    {
        public static HealthCheckBuilder AddAzureBlobStorageCheck(this HealthCheckBuilder builder, string accountName, string accountKey, string containerName = null, TimeSpan? cacheDuration = null)
        {
            var credentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(credentials, true);
            return AddAzureBlobStorageCheck(builder, storageAccount, containerName, cacheDuration);
        }

        public static HealthCheckBuilder AddAzureBlobStorageCheck(HealthCheckBuilder builder, CloudStorageAccount storageAccount, string containerName = null, TimeSpan? cacheDuration = null)
        {
            builder.AddCheck($"AzureBlobStorageCheck {storageAccount.BlobStorageUri} {containerName}", async () =>
            {
                bool result;
                try
                {
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    var properties = await blobClient.GetServicePropertiesAsync().ConfigureAwait(false);

                    if (!String.IsNullOrWhiteSpace(containerName))
                    {
                        var container = blobClient.GetContainerReference(containerName);

                        result = await container.ExistsAsync();
                    }

                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }

                return result
                    ? HealthCheckResult.Healthy($"AzureBlobStorage {storageAccount.BlobStorageUri} is available")
                    : HealthCheckResult.Unhealthy($"AzureBlobStorage {storageAccount.BlobStorageUri} is unavailable");
            }, cacheDuration ?? builder.DefaultCacheDuration);

            return builder;
        }

        public static HealthCheckBuilder AddAzureTableStorageCheck(this HealthCheckBuilder builder, string accountName, string accountKey, string tableName = null, TimeSpan? cacheDuration = null)
        {
            var credentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(credentials, true);
            return AddAzureTableStorageCheck(builder, storageAccount, tableName, cacheDuration);
        }

        public static HealthCheckBuilder AddAzureTableStorageCheck(HealthCheckBuilder builder, CloudStorageAccount storageAccount, string tableName = null, TimeSpan? cacheDuration = null)
        {
            builder.AddCheck($"AzureTableStorageCheck {storageAccount.TableStorageUri} {tableName}", async () =>
            {
                bool result;
                try
                {
                    var tableClient = storageAccount.CreateCloudTableClient();

                    var properties = await tableClient.GetServicePropertiesAsync().ConfigureAwait(false);

                    if (!String.IsNullOrWhiteSpace(tableName))
                    {
                        var table = tableClient.GetTableReference(tableName);

                        result = await table.ExistsAsync();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }

                return result
                    ? HealthCheckResult.Healthy($"AzureTableStorage {storageAccount.BlobStorageUri} is available")
                    : HealthCheckResult.Unhealthy($"AzureTableStorage {storageAccount.BlobStorageUri} is unavailable");

            }, cacheDuration ?? builder.DefaultCacheDuration);

            return builder;
        }

        public static HealthCheckBuilder AddAzureFileStorageCheck(this HealthCheckBuilder builder, string accountName, string accountKey, string shareName = null, TimeSpan? cacheDuration = null)
        {
            var credentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(credentials, true);
            return AddAzureFileStorageCheck(builder, storageAccount, shareName, cacheDuration);
        }

        public static HealthCheckBuilder AddAzureFileStorageCheck(HealthCheckBuilder builder, CloudStorageAccount storageAccount, string shareName = null, TimeSpan? cacheDuration = null)
        {
            builder.AddCheck($"AzureFileStorageCheck {storageAccount.FileStorageUri} {shareName}", async () =>
            {
                bool result;
                try
                {
                    var fileClient = storageAccount.CreateCloudFileClient();

                    var properties = await fileClient.GetServicePropertiesAsync().ConfigureAwait(false);

                    if (!String.IsNullOrWhiteSpace(shareName))
                    {
                        var share = fileClient.GetShareReference(shareName);

                        result = await share.ExistsAsync();
                    }

                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }

                return result
                    ? HealthCheckResult.Healthy($"AzureFileStorage {storageAccount.BlobStorageUri} is available")
                    : HealthCheckResult.Unhealthy($"AzureFileStorage {storageAccount.BlobStorageUri} is unavailable");
            }, cacheDuration ?? builder.DefaultCacheDuration);

            return builder;
        }

        public static HealthCheckBuilder AddAzureQueueStorageCheck(this HealthCheckBuilder builder, string accountName, string accountKey, string queueName = null, TimeSpan? cacheDuration = null)
        {
            var credentials = new StorageCredentials(accountName, accountKey);
            var storageAccount = new CloudStorageAccount(credentials, true);
            return AddAzureQueueStorageCheck(builder, storageAccount, queueName, cacheDuration);
        }

        public static HealthCheckBuilder AddAzureQueueStorageCheck(HealthCheckBuilder builder, CloudStorageAccount storageAccount, string queueName = null, TimeSpan? cacheDuration = null)
        {
            builder.AddCheck($"AzureQueueStorageCheck {storageAccount.QueueStorageUri} {queueName}", async () =>
            {
                bool result;
                try
                {
                    var queueClient = storageAccount.CreateCloudQueueClient();

                    var properties = await queueClient.GetServicePropertiesAsync().ConfigureAwait(false);

                    if (!String.IsNullOrWhiteSpace(queueName))
                    {
                        var queue = queueClient.GetQueueReference(queueName);

                        result = await queue.ExistsAsync();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }

                return result
                    ? HealthCheckResult.Healthy($"AzureFileStorage {storageAccount.BlobStorageUri} is available")
                    : HealthCheckResult.Unhealthy($"AzureFileStorage {storageAccount.BlobStorageUri} is unavailable");

            }, cacheDuration ?? builder.DefaultCacheDuration);

            return builder;
        }
    }
}
