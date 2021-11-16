using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;

namespace CustomConverterSampleApp.Converters
{
    public interface IMyBlobService
    {
        CloudBlob GetCloudBlob(Uri blobUri);
    }

    public class MyBlobService : IMyBlobService
    {
        public MyBlobService()
        {
            ValidateConnection();
        }

        private void ValidateConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            // Check whether the connection string can be parsed.
            if (!CloudStorageAccount.TryParse(connectionString, out _))
            {
                throw new InvalidOperationException(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'AZURE_STORAGE_CONNECTION_STRING' with your storage " +
                    "connection string as a value.");
            }
        }

        public CloudBlob GetCloudBlob(Uri blobUri)
        {
            return new CloudBlob(blobUri);
        }
    }
}