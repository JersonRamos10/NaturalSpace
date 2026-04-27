using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Domain.Entities
{
    public  class File
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; }


        //relaciones

        public Guid UserId { get; set; }

        public User UploadBy { get; set; } = null!;

        public Guid MessageId { get; set; }

        public Message Message { get; set; } = null!;
    }
}
