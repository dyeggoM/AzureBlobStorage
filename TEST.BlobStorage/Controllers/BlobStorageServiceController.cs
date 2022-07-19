using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TEST.BlobStorage.Services;

namespace TEST.BlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageServiceController : ControllerBase
    {
        private IBlobStorageStorage _storage;
        public BlobStorageServiceController(IBlobStorageStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Gets all blob names from blob storage.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNames()
        {
            try
            {
                var names = await _storage.GetNames();
                return Ok(names);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Gets all blob names containing spacified name.
        /// </summary>
        /// <param name="name">Name to find in blob names.</param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var names = await _storage.GetNames();
                var response = names.Where(x => x.Contains(System.Web.HttpUtility.UrlDecode(name)));
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Downloads a file from blob storage.
        /// </summary>
        /// <param name="blobName">Blob name to download.</param>
        /// <returns></returns>
        [HttpGet("download/{name}")]
        public async Task<IActionResult> DownloadFile(string blobName)
        {
            try
            {
                var response = await _storage.Download(System.Web.HttpUtility.UrlDecode(blobName));
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Uploads a list of files to the blob storage.
        /// </summary>
        /// <param name="files">List of files to upload to blob storage.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save(List<IFormFile> files)
        {
            try
            {
                if (files.Count < 1)
                    return BadRequest();
                var uris = new List<string>();
                var guid = Guid.NewGuid().ToString("N").ToLower();
                foreach (var file in files)
                {
                    var image = file.OpenReadStream();
                    var blobName = $"{guid}/{file.FileName}";
                    var uri = await _storage.Save(image, blobName);
                    uris.Add(uri);
                }
                return Ok(uris);
            }
            catch (Exception e)
            {
                return StatusCode(500,e.Message);
            }
        }

        /// <summary>
        /// Deletes a file from blob storage.
        /// </summary>
        /// <param name="blobName">Blob name to delete (file path in blob storage).</param>
        /// <returns></returns>
        [HttpDelete("{blobName}")]
        public async Task<IActionResult> Delete(string blobName)
        {
            try
            {
                var result = await _storage.Delete(System.Web.HttpUtility.UrlDecode(blobName));
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
