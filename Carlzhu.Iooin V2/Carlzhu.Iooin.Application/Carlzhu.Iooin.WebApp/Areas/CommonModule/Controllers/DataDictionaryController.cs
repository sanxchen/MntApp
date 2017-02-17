
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// �����ֵ�������
    /// </summary>
    public class DataDictionaryController : PublicController<BaseDataDictionaryDetail>
    {
        private readonly BaseDataDictionaryBll _baseDatadictionarybll = new BaseDataDictionaryBll();

        #region �������
        /// <summary>
        /// ���������ͼ
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult SortManage()
        {
            ViewBag.SortCode = BaseFactory.BaseHelper().GetSortCode<BaseDataDictionary>("SortCode").ToString();
            return View();
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSortManage(string KeyValue)
        {
            try
            {
                var Message = "ɾ��ʧ�ܡ�";
                int IsOk = 0;
                if (DataFactory.Database().FindCount<BaseDataDictionary>("ParentId", KeyValue) == 0)
                {
                    IsOk = DataFactory.Database().Delete<BaseDataDictionary>(KeyValue);
                    if (IsOk > 0)
                    {
                        Message = string.Format("�ɹ�ɾ�� {0} ����", 1);
                    }
                }
                else
                {
                    throw new Exception("��ǰ��ѡ���ӽڵ����ݣ�����ɾ����");
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// ����ֵ
        /// </summary>
        /// <param name="KeyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetFormSortManage(string KeyValue)
        {
            BaseDataDictionary entity = DataFactory.Database().FindEntity<BaseDataDictionary>(KeyValue);
            string JsonData = entity.ToJson();
            JsonData = JsonData.Insert(1, "\"ParentName\":\"" + DataFactory.Database().FindEntity<BaseDataDictionary>(entity.ParentId).FullName + "\",");
            return Content(JsonData);
        }
        /// <summary>
        /// �ύ��
        /// </summary>
        /// <param name="entity">ʵ����</param>
        /// <param name="KeyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitFormSortManage(BaseDataDictionary entity, string KeyValue)
        {
            try
            {
                int IsOk = 0;
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    entity.Modify(KeyValue);
                    IsOk = DataFactory.Database().Update(entity);
                }
                else
                {
                    entity.Create();
                    IsOk = DataFactory.Database().Insert(entity);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = KeyValue == "" ? "�����ɹ���" : "�༭�ɹ���" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�����" + ex.Message }.ToString());
            }
        }
        #endregion

        /// <summary>
        /// ��߷����б�������JSON��
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseDataDictionary> list = DataFactory.Database().FindList<BaseDataDictionary>("ORDER BY SortCode ASC");
            List<TreeEntity> treeList = new List<TreeEntity>();
            foreach (BaseDataDictionary item in list)
            {
                string dataDictionaryId = item.DataDictionaryId;
                bool hasChildren = false;
                List<BaseDataDictionary> childnode = list.FindAll(t => t.ParentId == dataDictionaryId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                TreeEntity tree = new TreeEntity
                {
                    id = dataDictionaryId,
                    text = item.FullName,
                    value = item.Code,
                    Attribute = "IsTree",
                    AttributeValue = item.IsTree.ToString(),
                    isexpand = true,
                    complete = true,
                    hasChildren = hasChildren,
                    parentId = item.ParentId
                };
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// �����ֵ��б�������JSON��
        /// </summary>
        /// <param name="dataDictionaryId"></param>
        /// <returns></returns>
        public ActionResult TreeGridListJson(string dataDictionaryId)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(dataDictionaryId))
            {
                List<BaseDataDictionaryDetail> listData = _baseDatadictionarybll.GetDataDictionaryDetailList(dataDictionaryId);
                sb.Append("{ \"rows\": [");
                sb.Append(TreeGridJson(listData, -1));
                sb.Append("]}");
            }
            return Content(sb.ToString());
        }
        int lft = 1, rgt = 1000000;
        public string TreeGridJson(List<BaseDataDictionaryDetail> ListData, int index, string ParentId = "0")
        {
            StringBuilder sb = new StringBuilder();
            List<BaseDataDictionaryDetail> childNodeList = ListData.FindAll(t => t.ParentId == ParentId);
            if (childNodeList.Count > 0) { index++; }
            foreach (BaseDataDictionaryDetail entity in childNodeList)
            {
                string strJson = entity.ToJson();
                strJson = strJson.Insert(1, "\"DataDictionaryDetailName\":\"" + entity.FullName + "\",");
                strJson = strJson.Insert(1, "\"level\":" + index + ",");
                strJson = strJson.Insert(1, "\"isLeaf\":" + (ListData.Count<BaseDataDictionaryDetail>(t => t.ParentId == entity.DataDictionaryDetailId) == 0 ? true : false).ToString().ToLower() + ",");
                strJson = strJson.Insert(1, "\"expanded\":true,");
                strJson = strJson.Insert(1, "\"lft\":" + lft++ + ",");
                strJson = strJson.Insert(1, "\"rgt\":" + rgt-- + ",");
                sb.Append(strJson);
                sb.Append(TreeGridJson(ListData, index, entity.DataDictionaryDetailId));
            }
            return sb.ToString().Replace("}{", "},{");
        }
        /// <summary>
        /// ���ݷ�����롷��ȡ�����ֵ������б�����JSON��
        /// </summary>
        /// <param name="code">�������</param>
        /// <returns></returns>
        public ActionResult BinDingItemsJson(string code)
        {
            List<BaseDataDictionaryDetail> list = _baseDatadictionarybll.GetDataDictionaryDetailListByCode(code);
            return Content(list.ToJson());
        }
        /// <summary>
        /// ���ݷ���Id����ȡ�����ֵ������б�������JSON��
        /// </summary>
        /// <param name="dataDictionaryId">��������</param>
        /// <returns></returns>
        public ActionResult DataDictionaryDetailJson(string dataDictionaryId)
        {
            List<BaseDataDictionaryDetail> list = _baseDatadictionarybll.GetDataDictionaryDetailList(dataDictionaryId);
            List<TreeEntity> treeList = list.Select(item => new TreeEntity
            {
                id = item.DataDictionaryDetailId, text = item.FullName, value = item.Code, isexpand = true, complete = true, hasChildren = false, parentId = item.ParentId
            }).ToList();
            return Content(treeList.TreeToJson());
        }
    }
}