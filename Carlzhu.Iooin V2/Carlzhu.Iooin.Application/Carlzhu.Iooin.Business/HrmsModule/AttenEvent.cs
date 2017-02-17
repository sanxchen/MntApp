using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;

namespace Carlzhu.Iooin.Business.HrmsModule
{
    internal class AttenEvent
    {
        internal delegate bool AttAnalysisDelegateHander(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime);
        internal event AttAnalysisDelegateHander AttAnalysisEventHandler;


        public virtual bool OnAttAnalysisEventHandler(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime)
        {
            AttAnalysisDelegateHander hander = AttAnalysisEventHandler;
            return hander != null && hander(baseEmployee, startTime, endTime);
        }
    }
}
