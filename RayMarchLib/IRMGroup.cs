using System.Collections.Generic;
using System.Xml.Linq;

namespace RayMarchLib
{
    public interface IRMGroup
    {
        public void AddObject(RMObject obj);
        public void DeserializeGroup(XElement elObj, Stack<XElement> descObjects);
    }
}
