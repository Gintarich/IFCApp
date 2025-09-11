using System;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices
{
    public class TeklaProjectQuery
    {
        public string GetModelName()
        {
            return new Model().GetInfo().ModelName;
        }
        public string GetModelPath()
        {
            return new Model().GetInfo().ModelPath;
        }
    }
}
