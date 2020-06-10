using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UglyMug
{
    public class MemoryContainer
    {
        public IMemoryCache _memoryCache { get; set; }
        public MemoryContainer(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
    }
}
