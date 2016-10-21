using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP.Mapper
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();
    }
}
