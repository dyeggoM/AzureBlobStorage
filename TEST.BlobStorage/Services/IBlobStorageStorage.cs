using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TEST.BlobStorage.Services
{
    public interface IBlobStorageStorage
    {
        /// <summary>
        /// Uploads a file to the blob storage
        /// </summary>
        /// <param name="fileStream">File data to upload.</param>
        /// <param name="fileName">File Name to upload.</param>
        /// <returns></returns>
        Task<string> Save(Stream fileStream, string fileName);
        /// <summary>
        /// Gets all blob names from blob storage.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetNames();
        /// <summary>
        /// Deletes a file from blob storage
        /// </summary>
        /// <param name="blobName">Blob name to delete.</param>
        /// <returns></returns>
        Task<bool> Delete(string blobName);
        /// <summary>
        /// Downloads a file from blob storage
        /// </summary>
        /// <param name="blobName">Blob name to download.</param>
        /// <returns></returns>
        Task<FileStream> Download(string blobName);
    }
}
