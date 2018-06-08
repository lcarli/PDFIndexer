using System;
using System.Collections.Generic;
using System.Text;

namespace Lucene
{
    class DataRepository
    {
        public static IndexMetadata Get(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id.Equals(id));
        }
    }
}
