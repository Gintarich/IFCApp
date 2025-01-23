using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services
{
    public static class ModelAttributeServer
    {
        public static string GetModelName()
        {
            var model = new Model();
            return model.GetInfo().ModelName.Split('.')[0];
        }
        public static string GetFilePath()
        {
            var model = new Model();
            return model.GetInfo().ModelPath;
        }
    }
}
