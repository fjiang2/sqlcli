﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlcli
{
    public enum ActionType
    {
        CompareSchema = 1,
        CompareData = 2,
        CompareRowCount = 3,
        
        Shell = 4,

        GenerateTableRows = 10,
        GenerateScript = 11,
        GenerateSchema = 12,

        Execute = 20
    }
}
