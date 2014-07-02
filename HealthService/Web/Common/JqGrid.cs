using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Common
{
    public class JqGrid
    {
        private ArrayList items;
        public JqGrid(ArrayList data)
        {
            items = data;
        }
        public MvcHtmlString js_grid(String tname)
        {
            
            if (items == null || items.Count <=1 )
            {
                return MvcHtmlString.Create("");
            }
            int height = items.Count * 20;
            ArrayList fields = items[0] as ArrayList;
            ArrayList columnlist = new ArrayList();
            String[] fieldlist = new String[fields.Count];
            for(int i=0;i<fields.Count;i++){
                columnlist.Add("{"+String.Format("name :'{0}',index:'{0}'",fields[i])+"}");
                fieldlist[i] ="'"+ fields[i].ToString()+"'";
            }
            ArrayList datas = new ArrayList();
            for (int i = 1; i<items.Count; i++)
            {
                ArrayList tempData = items[i] as ArrayList;
                ArrayList tempItems = new ArrayList();
                for (int j = 0; j < tempData.Count; j++)
                {
                    tempItems.Add(String.Format("{0}:'{1}'", fields[j], tempData[j]));
                }
                datas.Add("{"+String.Join(",", tempItems.ToArray(typeof(String)) as String[])+"}");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append( @"jQuery('"+tname+"').jqGrid({ ");
            sb.Append( @"   datatype: 'local',");
            sb.Append( @"   height: "+Convert.ToString(height)+",");
            sb.Append(@"   colNames: [" + String.Join(",", fieldlist) + "],");
            sb.Append(@"   colModel: [" + String.Join(",", columnlist.ToArray(typeof(String)) as String[]));
            sb.Append( @"   ],");
            sb.Append( @"   multiselect: true,");
            sb.Append( @"   caption: ''");
            sb.Append( @"   });");
            sb.Append(@"   var mydata = [" + String.Join(",", datas.ToArray(typeof(String)) as String[]));
            sb.Append( @"   ];");
            sb.Append( @"   for (var i = 0; i <= mydata.length; i++)");
            sb.Append( @"   jQuery('"+tname+"').jqGrid('addRowData', i + 1, mydata[i]);");            
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}