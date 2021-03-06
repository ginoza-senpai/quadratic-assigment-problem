﻿using System;
using System.Collections.Generic;
using Solution;

namespace TestSystem
{
    public abstract partial class ATest
    {
        /// <summary>Write options table for excel output</summary>
        /// <param name="table">export interface</param>
        /// <param name="aOptions">set of options</param>
        public static void WriteOptionsHeader(ITabler table, List<IOptions> aOptions)
        {
            var row = table.AddRow();
            // write names of options columns
            row.AddCells(CTablerExcel.Styles.eStyleGreyBold
                // delete "DEFINE_" from options name and split for cells by ';'
                , aOptions[0].GetValuesNames().Replace("DEFINE_", "").Split(';', StringSplitOptions.RemoveEmptyEntries));
            // write values of options
            foreach(IOptions opt in aOptions)
            {
                row = table.AddRow();
                row.AddCells(CTablerExcel.Styles.eStyleGrey, opt.GetValues().Split(';', StringSplitOptions.RemoveEmptyEntries));
            }

            table.AddRow();
            table.AddRow();
        }
    }
}
