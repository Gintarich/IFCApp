using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.IFCServices.Utils;

public static class VUGDCoordinateSystems
{
    public static Matrix4d InverseKul = new Matrix4d(new double[,]
        {
            {0.515038078489869 ,   0.857167298551142,    0, -463783847.754614 },
            {-0.857167298551142,   0.515038078489869,    0, 159621882.973248 },
            {0,                    0,                    1, -32200    },
            {0,                    0,                    0, 1        }
        });

    public static Matrix4d InverseBol = new Matrix4d(new double[,]
        {
            {-0.743495091901685,0.668741391210462,0,157585827.05401},
            {-0.668741391210462,-0.743495091901685,0,573888032.169514},
            {0,0,1,-2800},
            {0,0,0,1},
        });

    public static Matrix4d InverseDzin = new Matrix4d(new double[,]
    {
    {0.928420912597526867 , 0.3715299650237975627 , 0 , -571324486.24695154022 },
    {-0.3715299650237975627 , 0.928420912597526867 , 0 , -110466559.48612762342 },
    {0,0,1,-4000},
    {0,0,0,1},
    });
}
