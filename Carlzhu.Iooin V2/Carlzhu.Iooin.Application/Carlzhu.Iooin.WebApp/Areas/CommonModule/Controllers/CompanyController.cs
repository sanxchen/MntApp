
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// ��˾���������
    /// </summary>
    public class CompanyController : PublicController<BaseCompany>
    {
        readonly BaseCompanyBll _baseCompanybll = new BaseCompanyBll();
        /// <summary>
        /// ����˾����������JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseCompany> list = _baseCompanybll.GetList();
            List<TreeEntity> treeList = new List<TreeEntity>();
            foreach (BaseCompany item in list)
            {
                bool hasChildren = false;
                IList childnode = list.FindAll(t => t.ParentId == item.CompanyId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                TreeEntity tree = new TreeEntity
                {
                    id = item.CompanyId,
                    text = item.FullName,
                    value = item.CompanyId,
                    Attribute = "Category",
                    AttributeValue = item.Category,
                    isexpand = true,
                    complete = true,
                    hasChildren = hasChildren,
                    parentId = item.ParentId
                };
                if (item.ParentId == "0")
                {
                    tree.img = "/Content/Images/Icon16/molecule.png";
                }
                else
                {
                    tree.img = "/Content/Images/Icon16/hostname.png";
                }
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// ����˾�������ع�˾�б�JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeGridListJson()
        {
            List<BaseCompany> listData = _baseCompanybll.GetList();
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"rows\": [");
            sb.Append(TreeGridJson(listData, -1));
            sb.Append("]}");
            return Content(sb.ToString());
        }
        int lft = 1, rgt = 1000000;
        public string TreeGridJson(List<BaseCompany> listData, int index, string parentId = "0")
        {
            StringBuilder sb = new StringBuilder();
            List<BaseCompany> childNodeList = listData.FindAll(t => t.ParentId == parentId);
            if (childNodeList.Count > 0) { index++; }
            foreach (BaseCompany entity in childNodeList)
            {
                string strJson = Util.Json.ToJson(entity);
                strJson = strJson.Insert(1, "\"level\":" + index + ",");
                strJson = strJson.Insert(1, "\"isLeaf\":" + (listData.Count<BaseCompany>(t => t.ParentId == entity.CompanyId) == 0 ? true : false).ToString().ToLower() + ",");
                strJson = strJson.Insert(1, "\"expanded\":true,");
                strJson = strJson.Insert(1, "\"lft\":" + lft++ + ",");
                strJson = strJson.Insert(1, "\"rgt\":" + rgt-- + ",");
                sb.Append(strJson);
                sb.Append(TreeGridJson(listData, index, entity.CompanyId));
            }
            return sb.ToString().Replace("}{", "},{");
        }
        /// <summary>
        /// ����˾���������б�JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult ListJson(string parentId)
        {
            List<BaseCompany> list = _baseCompanybll.GetList();
            if (!string.IsNullOrEmpty(parentId))
            {
                list = list.FindAll(t => t.ParentId == parentId);
            }
            return Content(Util.Json.ToJson(list));
        }
        /// <summary>
        /// ����˾����ɾ������
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult DeleteCompany(string keyValue)
        {
            try
            {
                var message = "ɾ��ʧ�ܡ�";
                int isOk = 0;
                int departmentCount = DataFactory.Database().FindCount<BaseDepartment>("CompanyId", keyValue);
                if (departmentCount == 0)
                {
                    isOk = Repositoryfactory.Repository().Delete(keyValue);
                    if (isOk > 0)
                    {
                        message = "ɾ���ɹ���";
                    }
                }
                else
                {
                    message = "��˾���в��ţ�����ɾ����";
                }
                WriteLog(isOk, keyValue, message);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                WriteLog(-1, keyValue, "����ʧ�ܣ�" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// ����˾��������ֵ
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult SetCompanyForm(string keyValue)
        {
            BaseCompany entity = Repositoryfactory.Repository().FindEntity(keyValue);
            string strJson = Util.Json.ToJson(entity);
            //�Զ���
            strJson = strJson.Insert(1, BaseFormAttributeBll.Instance.GetBuildForm(keyValue));
            return Content(strJson);
        }
        /// <summary>
        /// ����˾�����ύ��
        /// </summary>
        /// <param name="entity">ʵ�����</param>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="buildFormJson">�Զ����</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult SubmitCompanyForm(BaseCompany entity, string keyValue, string buildFormJson)
        {
            string moduleId = DESEncrypt.Decrypt(WebHelper.GetCookie("ModuleId"));
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                int isOk = 0;
                string message = keyValue == "" ? "�����ɹ���" : "�༭�ɹ���";
                if (!string.IsNullOrEmpty(keyValue))
                {
                    BaseCompany oldentity = Repositoryfactory.Repository().FindEntity(keyValue);//��ȡû����֮ǰʵ�����
                    entity.Modify(keyValue);
                    isOk = database.Update(entity, isOpenTrans);
                    this.WriteLog(isOk, entity, oldentity, keyValue, message);
                }
                else
                {
                    entity.Create();
                    isOk = database.Insert(entity, isOpenTrans);
                    this.WriteLog(isOk, entity, null, keyValue, message);
                }
                BaseFormAttributeBll.Instance.SaveBuildForm(buildFormJson, entity.CompanyId, moduleId, isOpenTrans);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, keyValue, "����ʧ�ܣ�" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
    }
}