//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sys.Data
{

    public delegate void RowChangedHandler(object sender, RowChangedEventArgs e);

    public class RowChangedEventArgs : EventArgs
    {
        public readonly RowAdapter adapter;
        public readonly ObjectState state;
        public bool confirmed;
        public bool saved;    //RowChanged fired twice during Adding a record; 
                              //true: after record saved
                              //false: before record saving, give a chance to deny saving


        public RowChangedEventArgs(RowAdapter adapter, ObjectState state, bool saved)
        {
            this.adapter = adapter;
            this.state = state;
            this.confirmed = true;
            this.saved = saved;
        }
    }



}
