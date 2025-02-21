using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Interfaces
{
    public interface IBlob
    {
        string FileName { get; set; }
        string FileDirInStorage { get; set; }
        int BucketId { get; set; }
        Bucket Bucket { get; set; }
    }
}
