using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.Utils
{
    public static class TmpAppData
    {
        public static DateTime previousOffsetSave { get; set; }
        public static bool nowChangingOffset { get; set; }
    }
}
