using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;
using TEST.BlobStorage.CustomEntities;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
//using Azure.Storage;

namespace TEST.BlobStorage.Services
{
    public class BlobStorageService : IBlobStorageStorage
    {
        private readonly AzureStorageConfig storageConfig;

        public BlobStorageService(IOptions<AzureStorageConfig> storageConfig)
        {
            this.storageConfig = storageConfig.Value;
        }

        private async Task<BlobContainerClient> GetCloudBlobContainer()
        {
            var serviceClient = new BlobServiceClient(storageConfig.ConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(storageConfig.FileContainerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        /// <summary>
        /// Uploads a file to the blob storage
        /// </summary>
        /// <param name="fileStream">File data to upload.</param>
        /// <param name="fileName">File Name to upload.</param>
        /// <returns></returns>
        public async Task<string> Save(Stream fileStream, string fileName)
        {
            var containerClient = await GetCloudBlobContainer();
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            var uri = blobClient.Uri;
            return uri.ToString();
        }

        /// <summary>
        /// Gets all blob names from blob storage.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetNames()
        {
            var containerClient = await GetCloudBlobContainer();
            var results = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var url = $"{containerClient.Uri.AbsoluteUri}/{blobItem.Name}";
                results.Add(url);
            }
            return results;
        }

        /// <summary>
        /// Deletes a file from blob storage
        /// </summary>
        /// <param name="blobName">Blob name to delete.</param>
        /// <returns></returns>
        public async Task<bool> Delete(string fileName)
        {
            var blobClient = await GetCloudBlobContainer();
            var result = await blobClient.GetBlobClient(fileName).DeleteIfExistsAsync();
            return result;
        }

        /// <summary>
        /// Downloads a file from blob storage
        /// </summary>
        /// <param name="blobName">Blob name to download.</param>
        /// <returns></returns>
        public async Task<FileStream> Download(string blobName)
        {
            var containerClient = await GetCloudBlobContainer();
            var blobClient = containerClient.GetBlobClient(blobName);
            string downloadFilePath = Path.Combine(Directory.GetCurrentDirectory(),"Files", blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
            var file = new FileStream(downloadFilePath,FileMode.Open);
            return file;
        }
    }
}
