using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.HrmsModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.MANAGER;

using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Business.SysModule
{
    public class SystemAccountBll
    {








        #region 用户密码相关问题

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldpwd"></param>
        /// <param name="newpwd"></param>
        /// <returns></returns>
        public static bool ChangePwd(string username, string oldpwd, string newpwd)
        {

            //域用户认证
            ActiveDirectHelper.LoginResult login = ActiveDirectHelper.LoginByAccount(username, oldpwd);
            //本地用户认证








            if (login != ActiveDirectHelper.LoginResult.LoginUserOk) return false;

            //修改成功
            ActiveDirectHelper.SetPasswordByAccount(username, newpwd);
            return true;
        }
        #endregion

        #region 权限检查

        public bool UserValidate(string controllerName, string actionName, string empNo, string systemName)
        {
            SystemControllerBll controller = new SystemControllerBll();
            SystemModelBll model = new SystemModelBll();
            SystemActionBll action = new SystemActionBll();
            SystemPowerBll systemPowerBll = new SystemPowerBll();


            try
            {
                lock (new object())
                {
                    SystemModel modelEntity =
                           (model.Count(c => c.ModelName == systemName) < 1)
                               ?
                               model.AddEntity(new SystemModel()
                               {
                                   ModelName = systemName,
                                   ModelUrl = "~/" + systemName
                               })
                               :
                               model.Single(c => c.ModelName == systemName);

                    Entity.MANAGER.SystemController controllerEntity =
                        (controller.Count(c => c.ControllerName == controllerName) < 1)
                            ?
                            controller.AddEntity(new Entity.MANAGER.SystemController
                            {
                                ModelId = modelEntity.ModelId,
                                ControllerName = controllerName,
                                CreateTime = DateTime.Now,
                                IsDisplay = true,
                            })
                            :
                            controller.Single(c => c.ControllerName == controllerName);

                    SystemAction actionEntity = (
                        action.Count(
                            c => c.ControllerId == controllerEntity.ControllerId && c.ActionName == actionName) < 1)
                            ?
                            action.AddEntity(new SystemAction()
                            {
                                ControllerId = controllerEntity.ControllerId,
                                ActionName = actionName,
                                ActionDetails = "系统自动添加",
                            })
                            :
                            action.Single(
                            c => c.ControllerId == controllerEntity.ControllerId && c.ActionName == actionName);

                    return (systemPowerBll.Count(c => c.ActionId == actionEntity.ActionId && c.EmpNo == empNo) > 0);
                }
            }
            catch
            {
                return false;
            }
        }


        #endregion

    }
}
