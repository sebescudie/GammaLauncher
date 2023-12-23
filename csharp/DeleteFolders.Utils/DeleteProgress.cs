using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteFolders.Utils
{
    public class DeleteProgress
    {
        public string CurrentFolder { get; set; }
        public int FilesDeleted { get; set; }
        public int TotalFiles { get; set; }
    }
}
