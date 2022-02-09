using System;

namespace TruePeople.SharePreview.Models
{
    public class SharePreviewContext
    {
        public int NodeId { get; set; }

        public Guid NewestVersionId { get; set; }
    }
}
