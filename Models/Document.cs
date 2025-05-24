namespace SecureDocumentStorageSystem.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public byte[] Data { get; set; }
        public DateTime UploadDate { get; set; }
        public User User { get; set; }
    }
}