using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public class BlobStorage
    {
        private static CloudBlobContainer blobContainer;
        public static CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["AppCnn"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference("my-images");
            if (blobContainer.CreateIfNotExists())
            {
                blobContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return blobContainer;
        }

        public static string GetBlobPathWithSas(string blobName)
        {
            CloudBlobContainer _container = BlobStorage.GetCloudBlobContainer();
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobName);
            return (blob.Uri.ToString());
        }

        public static bool CheckFileType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".gif") || ext.Equals(".tiff") || ext.Equals("jpge") || ext.Equals(".CR2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
