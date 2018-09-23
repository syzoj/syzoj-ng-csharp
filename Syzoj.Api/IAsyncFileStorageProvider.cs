using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syzoj.Api
{
    /// <summary>
    /// This interface defines a storage provider. A storage provider
    /// offers file download and upload service directly to the users.
    /// </summary>
    public interface IAsyncFileStorageProvider
    {
        /// <summary>
        /// Get the list of filenames (without directories) under a path.
        /// </summary>
        Task<IEnumerable<string>> GetFiles(string path);

        /// <summary>
        /// Generates a link to allow the user to upload the file.
        /// The link is expected to expire quickly so it should be generated
        /// on demand.
        /// </summary>
        Task<string> GenerateUploadLink(string path);

        /// <summary>
        /// Generates a link to allow the user to download the file.
        /// Should return null if the file's existence can be determined
        /// immediately and it doesn't exist.
        /// The link is expected to expire quickly so it should be generated
        /// on demand.
        /// </summary>
        Task<string> GenerateDownloadLink(string path, string fileName);
    }
}