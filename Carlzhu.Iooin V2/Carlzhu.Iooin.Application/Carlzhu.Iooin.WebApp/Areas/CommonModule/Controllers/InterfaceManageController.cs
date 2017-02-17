
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// �ӿڹ��������
    /// </summary>
    public class InterfaceManageController : PublicController<BaseInterfaceManage>
    {
        readonly BaseInterfaceManageBll _baseinterfacemanagebll = new BaseInterfaceManageBll();
        /// <summary>
        /// ���ӿڹ������ؽӿ��б�JSON
        /// </summary>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(Pagination jqgridparam)
        {
            Stopwatch watch = CommonHelper.TimerStart();
            List<BaseInterfaceManage> listData = _baseinterfacemanagebll.GetPageList(ref jqgridparam);
            var jsonData = new
            {
                total = jqgridparam.total,
                page = jqgridparam.page,
                records = jqgridparam.records,
                costtime = CommonHelper.TimerEnd(watch),
                rows = listData,
            };
            return Content(Util.Json.ToJson(jsonData));
        }
        /// <summary>
        /// ���ӿڹ������ؽӿڲ����б�JSON
        /// </summary>
        /// <param name="InterfaceId"></param>
        /// <returns></returns>
        public ActionResult GridInterfaceParameterListJson(string InterfaceId)
        {
            List<BaseInterfaceManageParameter> listData = _baseinterfacemanagebll.GetInterfaceParameterList(InterfaceId);
            var jsonData = new
            {
                rows = listData,
            };
            return Content(Util.Json.ToJson(jsonData));
        }
        /// <summary>
        /// �ύ�ӿڱ����������༭��
        /// </summary>
        /// <param name="keyValue">����</param>
        /// <param name="entity">�ӿڶ���</param>
        /// <param name="parameterJson">�ӿڲ���</param>
        /// <returns></returns>
        public ActionResult SubmitInterfaceForm(string keyValue, BaseInterfaceManage entity, string parameterJson)
        {
            try
            {
                string message = keyValue == "" ? "�����ɹ���" : "�༭�ɹ���";
                int isOk = _baseinterfacemanagebll.SubmitInterfaceForm(keyValue, entity, parameterJson);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
    }
}