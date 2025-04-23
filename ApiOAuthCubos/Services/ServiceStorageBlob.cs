using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;

namespace ApiOAuthCubos.Services
{
    public class ServiceStorageBlob
    {
        private BlobServiceClient client;

        public ServiceStorageBlob(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<string>>
           GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach
                (BlobContainerItem item in this.client.GetBlobContainersAsync())
            {
                containers.Add(item.Name);
            }
            return containers;
        }



        public string GetContainerUrl(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);

            return containerClient.Uri.AbsoluteUri;
        }

        public async Task<string> GetBlobUrlAsync
            (string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
