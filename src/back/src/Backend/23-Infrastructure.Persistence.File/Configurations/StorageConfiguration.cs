namespace Infrastructure.Persistence.File.Configurations
{
    public class StorageConfiguration
    {
        //public string BlobContainerName { get; set; } = "elector-files";
        // Chemin racine (ex: "C:/Uploads" ou "/var/www/uploads")
        public string RootPath { get; set; } = "C:/Uploads";

        public string RegistrationDocumentsPath { get; set; } = string.Empty;
    }
}
