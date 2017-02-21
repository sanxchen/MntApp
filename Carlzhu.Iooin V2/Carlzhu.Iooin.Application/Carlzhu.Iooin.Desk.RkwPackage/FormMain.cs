using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

using Carlzhu.Iooin.Entity.mes;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;
using Carlzhu.Iooin.Util.Offices;

namespace Carlzhu.Iooin.Desk.RkwPackage
{
    public partial class FormMain : Form
    {
        #region Console.WriteLabel




        void ConsoleWriteToLabelMsg(string text)
        {
            ShowMessage sm = new ShowMessage(Parameters.Main, "lblMsg");
            sm.ConsoleWriteToLabelMsg(text);
        }

        void ConsolePackageMsg(string text)
        {
            ShowMessage sm = new ShowMessage(Parameters.Main, "lblCurrentBoxQty");
            sm.ConsoleWriteToLabelMsg(text);
        }


        #endregion

        ControlExpress cE = new ControlExpress();

        public FormMain()
        {

            InitializeComponent();

            Parameters.Main = this;
        }


        /// <summary>
        /// 前端显示标签信息
        /// </summary>
        DataTable _dt = new DataTable();

        /// <summary>
        /// 控制是否同MANNO
        /// </summary>
        string ManNo = string.Empty;




        /// <summary>
        /// 整理封箱数据
        /// </summary>
        List<MesRkwPackageView> _mesRkwPackageViews = new List<MesRkwPackageView>();

        #region 包装

        void ShowMsg(string msg)
        {
            ConsoleWriteToLabelMsg(msg);
            txtLabel.SelectAll();
            txtLabel.Focus();
        }

        /// <summary>
        /// 扫码自动回车拉取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                this.btnLabel_Click(sender, e);//触发button事件  
            }
        }


        /// <summary>
        /// 扫码包装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLabel_Click(object sender, EventArgs e)
        {



            var sn = txtLabel.Text;

            if (!string.IsNullOrEmpty(sn))
            {
                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {
                    //检测标签是否存在
                    var model = context.MesRkwSns.FirstOrDefault(c => c.Sn == sn);

                    //检测标签是否存在
                    if (model == null)
                    {
                        ShowMsg("标签不存在!!!!");
                        return;
                    }

                    //检测是否已封箱
                    if (context.MesRkwPackages.Any(c => c.Sn == model.Sn && c.MesRkwBox.BoxStatus))
                    {
                        ShowMsg("标签已被封箱!!!!");
                        return;
                    }


                    //阻止不同工单混箱
                    if (!string.IsNullOrEmpty(this.ManNo))
                    {
                        if (this.ManNo != model.ManNo)
                        {
                            ShowMsg("不允许不同工单混箱！！！！");
                            return;
                        }

                        //标签是否重复
                        if (_mesRkwPackageViews.Any(c => c.Sn == sn))
                        {
                            ShowMsg("标签重复");
                            return;
                        }
                    }



                    _mesRkwPackageViews.Add(new MesRkwPackageView()
                    {
                        Sn = sn,
                        ManNo = model.MesRkwPo.ManNo,
                        SupplierNo = model.MesRkwPo.SupplierNo,
                        Qty = model.MesRkwPo.Qty,
                        CusName = model.MesRkwPo.CusName,
                        MinicutPoNo = model.MesRkwPo.MinicutPoNo,
                        PartNo = model.MesRkwPo.PartNo,
                        PartName = model.MesRkwPo.PartName,
                        MoNo = model.MesRkwPo.MoNo,
                        HeatNo = model.MesRkwPo.HeatNo,
                        Supplier = model.MesRkwPo.Supplier,
                        RecaroPoNo = model.MesRkwPo.RecaroPoNo,
                        BoxSize = model.MesRkwPo.BoxSize,
                    });

                    object[] strArray =
                    {
                            sn,
                            model.MesRkwPo.ManNo,
                            model.MesRkwPo.SupplierNo,
                            model.MesRkwPo.Qty,
                            model.MesRkwPo.CusName,
                            model.MesRkwPo.MinicutPoNo,
                            model.MesRkwPo.PartNo,
                            model.MesRkwPo.PartName,
                            model.MesRkwPo.MoNo,
                            model.MesRkwPo.HeatNo,
                            model.MesRkwPo.Supplier,
                            model.MesRkwPo.RecaroPoNo,
                        };

                    _dt.Rows.Add(strArray);



                    //选中当前条
                    dgvDataLabels.Rows[dgvDataLabels.RowCount - 1].Selected = true;

                    //将滚动条放在最后一行位置
                    dgvDataLabels.FirstDisplayedScrollingRowIndex = dgvDataLabels.RowCount - 1;

                    //保存当前工单号
                    this.ManNo = model.MesRkwPo.ManNo;

                    ConsoleWriteToLabelMsg($"{sn}:包装中....");
                    ConsolePackageMsg($"{_mesRkwPackageViews.Count}/{model.MesRkwPo.BoxSize}");

                    //达到包装条件，包装
                    if (_dt.Rows.Count == model.MesRkwPo.BoxSize) this.btnPackage_Click(sender, e);//触发button事件






                }
            }

            txtLabel.Text = "";
            txtLabel.Focus();

        }


        /// <summary>
        /// 包装，强制包装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPackage_Click(object sender, EventArgs e)
        {
            if (_dt.Rows.Count == 0)
            {
                ConsoleWriteToLabelMsg("请先扫码！");
                return;
            }
            this.ManNo = string.Empty;
            this.Enabled = false;

            MesRkwBox mesRkwBox;
            List<MesRkwPackage> list;

            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                string date = DateTime.Now.ToString("yyyyMMdd");

                int count = context.MesRkwBoxs.Count(c => c.BoxId.StartsWith(date)) + 1;

                mesRkwBox = new MesRkwBox()
                {
                    BoxId = $"{date}{count.ToString().PadLeft(4, '0')}",
                    CreateTime = DateTime.Now,
                    BoxStatus = true
                };

                //整理封箱
                list = (from object dr in _dt.Rows
                        select new MesRkwPackage()
                        {
                            CreateTime = DateTime.Now,
                            MesRkwBox = mesRkwBox,
                            Sn = (string)((DataRow)dr).ItemArray[0]
                        }).ToList();

                context.MesRkwPackages.AddRange(list);
                context.SaveChanges();

                var sn = _mesRkwPackageViews.First().Sn;
                var mespo = context.MesRkwSns.First(c => c.Sn == sn).MesRkwPo;

                string qty = _dt.Rows.Count.ToString();

                cE.PrintBoxLabel(mespo, mesRkwBox, qty);

                txtLabel.Text = "";
                txtLabel.Focus();
                this.Enabled = true;

            }







            _dt.Clear();
            _mesRkwPackageViews = new List<MesRkwPackageView>();
        }


        /// <summary>
        /// 清除包装数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDataLabels_DoubleClick(object sender, EventArgs e)
        {
            _dt.Clear();
            _mesRkwPackageViews = new List<MesRkwPackageView>();
            this.ManNo = string.Empty;
        }


        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化重印标签类型

            cbbLabelType.Items.AddRange(new string[] { "SN", "Box" });
            cbbLabelType.SelectedIndex = 0;
            cbbLabelType.DropDownStyle = ComboBoxStyle.DropDownList;

            #endregion


            //初始化打印数量
            cbbBoxLabelQty.Items.AddRange(new object[] { "1", "2" });
            cbbBoxLabelQty.SelectedIndex = 0;
            cbbBoxLabelQty.DropDownStyle = ComboBoxStyle.DropDownList;

            //初始化打印机列表
            LocalPrint.GetLocalPrinters().ForEach(c => cbbPrinters.Items.Add(c));
            cbbPrinters.SelectedIndex = 0;
            cbbPrinters.DropDownStyle = ComboBoxStyle.DropDownList;

            //设置默认打印机
            LocalPrint.SetDefaultPrinter(cbbPrinters.Text);


            List<MesRkwPackageView> rkwPackageViews = new List<MesRkwPackageView>();

            _dt = rkwPackageViews.ToDataTable(null);
            _dt.Columns.Remove("IsPrint");
            _dt.Columns.Remove("BoxSize");
            dgvDataLabels.DataSource = _dt;
            dgvDataLabels.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvSnData.EditMode = DataGridViewEditMode.EditProgrammatically;

            // ////Copy Label
            // ////   Console.WriteLine(Base.IsRemote);
            // string remote = @"\\192.168.0.4\Minicut\软件\办公软件\Rkw\label\";
            // string target = Parameters.StartPath + "label\\";

            // //if (Directory.Exists(target))
            // //{
            // //    File.Delete($"{target}*.*");
            // //    Directory.Delete(target);
            // //}




            //Base.CopyFile($"{remote}Package.Lab", $"{target}Package.Lab");
            //Base.CopyFile($"{remote}Shipping.Lab", $"{target}Shipping.Lab");
            //Base.CopyFile($"{remote}sn.Lab", $"{target}sn.Lab");
            //Base.CopyFile($"{remote}TraceableTags.Lab", $"{target}TraceableTags.Lab");


            this.txtCheckWarehouseExcelPath.ReadOnly = true;

        }




        #region 设置打印机


        private void btnSetPrint_Click(object sender, EventArgs e)
        {







            string currentPrint = cbbPrinters.Text;
            ConsoleWriteToLabelMsg(LocalPrint.SetDefaultPrinter(currentPrint) ? "打印机设置成功!" : "打印机设置失败!");
        }



        #endregion

        #region 保存修改工单


        private void btnSavePo_Click(object sender, EventArgs e)
        {
            try
            {
                MesRkwPo rkwPo = new MesRkwPo
                {
                    ManNo = txtManNo.Text,
                    SupplierNo = txtSupplierNo.Text,
                    CusName = txtCUSName.Text,
                    PartNo = txtPartNo.Text,
                    MoNo = txtMoNo.Text,
                    Supplier = txtSupplier.Text,
                    Qty = int.Parse(txtQty.Text),
                    MinicutPoNo = txtMinicutPoNo.Text,
                    PartName = txtPartName.Text,
                    HeatNo = txtHeatNo.Text,
                    RecaroPoNo = txtRecaroPoNo.Text,
                    BoxSize = int.Parse(txtBoxSize.Text),
                    BoxLabelQty = int.Parse(cbbBoxLabelQty.Text)
                };

                ConsoleWriteToLabelMsg("数据保存中....");

                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {
                    var po = context.MesRkwPos.Find(rkwPo.ManNo);
                    if (po == null)
                    {
                        context.MesRkwPos.Add(rkwPo);
                        context.SaveChanges();
                        ConsoleWriteToLabelMsg("数据已保存，可以列印数据....");


                    }
                    else
                    {
                        po.BoxSize = rkwPo.BoxSize;
                        po.Qty = rkwPo.Qty;
                        po.PartNo = rkwPo.PartNo;
                        po.PartName = rkwPo.PartName;
                        po.MoNo = rkwPo.MoNo;
                        po.HeatNo = rkwPo.HeatNo;
                        po.Supplier = rkwPo.Supplier;
                        po.RecaroPoNo = rkwPo.RecaroPoNo;
                        po.BoxLabelQty = rkwPo.BoxLabelQty;

                        context.Set<MesRkwPo>().Attach(po);
                        context.Entry<MesRkwPo>(po).State = EntityState.Modified;
                        context.SaveChanges();
                        ConsoleWriteToLabelMsg("工单已被修改，可以列印数据....");
                        txtManNo.ReadOnly = false;
                    }

                }

            }
            catch (Exception)
            {
                ConsoleWriteToLabelMsg("请确认数据信息是否正确！！！");
            }
        }

        private void txtManNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {
                    if (txtManNo.Text.Length == 10)
                    {
                        var poModel = context.MesRkwPos.Find(txtManNo.Text);
                        if (poModel.IsPrint)
                        {
                            ConsoleWriteToLabelMsg("工单已被打印，不可修改");
                            return;
                        }
                        txtManNo.Text = poModel.ManNo;
                        txtSupplierNo.Text = poModel.SupplierNo;
                        txtCUSName.Text = poModel.CusName;
                        txtPartNo.Text = poModel.PartNo;
                        txtMoNo.Text = poModel.MoNo;
                        txtSupplier.Text = poModel.Supplier;
                        txtQty.Text = poModel.Qty.ToString();
                        txtMinicutPoNo.Text = poModel.MinicutPoNo;
                        txtPartName.Text = poModel.PartName;
                        txtHeatNo.Text = poModel.HeatNo;
                        txtRecaroPoNo.Text = poModel.RecaroPoNo;
                        txtBoxSize.Text = poModel.BoxSize.ToString();
                        cbbBoxLabelQty.Text = poModel.BoxLabelQty.ToString();
                    }
                    txtManNo.ReadOnly = true;

                }

            }
        }


        private void cbbWattingPringMan_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPrintSn.Enabled = true;
        }

        private void btnPrintSn_Click(object sender, EventArgs e)
        {
            btnPrintSn.Enabled = false;
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {

                var poModel = context.MesRkwPos.Find(cbbWattingPringMan.Text);

                List<MesRkwSn> rkwSns = new List<MesRkwSn>();
                for (var i = 1; i <= poModel.Qty; i++)
                {
                    var rkw = new MesRkwSn()
                    {
                        Sn = $"{poModel.ManNo.Substring(2)}{i.ToString().PadLeft(4, '0')}",
                        ManNo = poModel.ManNo
                    };

                    rkwSns.Add(rkw);
                }
                context.MesRkwSns.AddRange(rkwSns);
                cbbWattingPringMan.Items.Remove(cbbWattingPringMan.Text);
                poModel.IsPrint = true;
                context.Set<MesRkwPo>().Attach(poModel);
                context.Entry<MesRkwPo>(poModel).State = EntityState.Modified;
                context.SaveChanges();

                ConsoleWriteToLabelMsg("保存完成，准备打印！！！");

                //打标签
                if (btnPrintMode.Text == @"单列打印")
                {
                    cE.PrintSn(rkwSns);
                }
                else
                {
                    cE.PrintDbSn(rkwSns);
                }
            }

        }


        private void cbbWattingPringMan_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                List<string> mans = context.MesRkwPos.Where(c => !c.IsPrint).Select(k => k.ManNo).ToList();
                cbbWattingPringMan.Items.Clear();
                cbbWattingPringMan.Items.AddRange(mans.ToArray());
                if (cbbWattingPringMan.Items.Count > 0) cbbWattingPringMan.SelectedIndex = 0;
                cbbWattingPringMan.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }
        #endregion

        #region 解包


        /// <summary>
        /// 确认解包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnlock_Click(object sender, EventArgs e)
        {
            string boxid = txtPackageNo.Text;

            if (dgvDataList.Rows.Count > 0)
            {
                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {
                    MesRkwBox box = context.MesRkwBoxs.First(c => c.BoxStatus && c.BoxId == boxid);

                    box.BoxStatus = false;
                    context.Entry(box).State = EntityState.Modified;
                    if (context.SaveChanges() > 0)
                    {
                        ConsoleWriteToLabelMsg("解锁成功!!!");
                        this.txtPackageNo_DoubleClick(sender, e);
                    };
                }
            }
        }

        /// <summary>
        /// 回车拉取解包数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPackageNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string boxid = txtPackageNo.Text;

            if (string.IsNullOrEmpty(boxid))
            {
                ConsoleWriteToLabelMsg("请确认箱号");
                return;
            }

            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                List<string> sns =
                    context.MesRkwPackages.Where(c => c.MesRkwBox.BoxId == boxid && c.MesRkwBox.BoxStatus)
                        .Select(k => k.Sn).ToList();

                var list = context.MesRkwSns.Where(c => sns.Contains(c.Sn)).ToList();

                List<MesRkwPackageView> rkw = new List<MesRkwPackageView>();


                list.ForEach(k =>
                {
                    rkw.Add(new MesRkwPackageView()
                    {
                        Sn = k.Sn,
                        ManNo = k.MesRkwPo.ManNo,
                        SupplierNo = k.MesRkwPo.SupplierNo,
                        Qty = k.MesRkwPo.Qty,
                        CusName = k.MesRkwPo.CusName,
                        MinicutPoNo = k.MesRkwPo.MinicutPoNo,
                        PartNo = k.MesRkwPo.PartNo,
                        PartName = k.MesRkwPo.PartName,
                        MoNo = k.MesRkwPo.MoNo,
                        HeatNo = k.MesRkwPo.HeatNo,
                        Supplier = k.MesRkwPo.Supplier,
                        RecaroPoNo = k.MesRkwPo.RecaroPoNo,
                    });
                });

                if (list.Count > 0)
                {
                    txtPackageNo.ReadOnly = true;
                    btnUnlock.Enabled = true;
                }
                else
                {
                    ConsoleWriteToLabelMsg("箱号不存在，或已被解锁！！！！");
                }

                dgvDataList.DataSource = rkw;
            }
        }

        /// <summary>
        /// 双击清除解包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPackageNo_DoubleClick(object sender, EventArgs e)
        {
            if (txtPackageNo.ReadOnly == true)
            {
                txtPackageNo.ReadOnly = false;
                btnUnlock.Enabled = false;
                dgvDataList.DataSource = null;
            }
        }

        #endregion

        #region 选项卡切换

        /// <summary>
        /// 选项卡切换数据整理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabMain_Selected(object sender, TabControlEventArgs e)
        {
            if (tabMain.SelectedIndex != 2)
            {
                txtBoxSize.Text = "";
                txtManNo.Text = "";
                txtSupplierNo.Text = "16253";
                txtCUSName.Text = "Recaro";
                txtPartNo.Text = "";
                txtMoNo.Text = "";
                txtSupplier.Text = "Giant";
                txtQty.Text = "";
                txtMinicutPoNo.Text = "MJ20150114000001";
                txtPartName.Text = "";
                txtHeatNo.Text = "";
                txtRecaroPoNo.Text = "";
            }
        }


        #endregion

        #region 异常处理

        private void btnSnTry_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                cE.PrintSn(context.MesRkwSns.Take(1).ToList());
            }
        }

        private void btnSingleTry_Click(object sender, EventArgs e)
        {

            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                var mespo = context.MesRkwPos.First();
                mespo.BoxLabelQty = 1;
                cE.PrintBoxLabel(mespo, new MesRkwBox() { BoxId = "201605170001" }, "10");
            }

        }

        private void btnDoubleTry_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                var mespo = context.MesRkwPos.First();
                cE.PrintBoxLabel(mespo, null, "10");

            }
        }


        /// <summary>
        /// 加工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMan_Click(object sender, EventArgs e)
        {



            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                var po = context.MesRkwPos.FirstOrDefault(c => c.ManNo == (txtAddManNO.Text));
                if (po == null || !po.IsPrint)
                {
                    ConsoleWriteToLabelMsg("工单不存在或未被打印");
                    return;
                }

                var maxsn = int.Parse(context.MesRkwSns.Where(c => c.ManNo == (txtAddManNO.Text)).ToList().Last().Sn.Substring(8));
                Console.Write(maxsn);
                //int.Parse(maxsn.)

                List<MesRkwSn> sns = new List<MesRkwSn>();
                int addQty = int.Parse(txtAddManQty.Text);
                for (int i = 1; i <= addQty; i++)
                {
                    sns.Add(new MesRkwSn
                    {
                        Sn = $"{po.ManNo.Substring(2)}{(maxsn + i).ToString().PadLeft(4, '0')}",
                        MesRkwPo = po,
                    });
                }



                po.Qty = po.Qty + addQty;
                context.Set<MesRkwPo>().Attach(po);
                context.Entry<MesRkwPo>(po).State = EntityState.Modified;
                context.SaveChanges();

                context.MesRkwSns.AddRange(sns);
                context.SaveChanges();

                ConsoleWriteToLabelMsg("添加成功,准备打印");

                cE.PrintSn(sns);

                txtAddManNO.Text = string.Empty;
                txtAddManQty.Text = string.Empty;

                ConsoleWriteToLabelMsg("打印成功");
            }

        }

        /// <summary>
        /// 减工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMinusMan_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                var sn = txtMinusMan.Text;
                try
                {
                    if (context.MesRkwPackages.Any(c => c.Sn == sn && c.MesRkwBox.BoxStatus))
                    {
                        ConsoleWriteToLabelMsg("标签已被包装");
                        return;
                    }

                    var model = context.MesRkwSns.Find(txtMinusMan.Text);
                    var man = context.MesRkwPos.Find(model.ManNo);

                    context.Set<MesRkwSn>().Attach(model);
                    context.Entry<MesRkwSn>(model).State = EntityState.Deleted;


                    man.Qty = man.Qty - 1;
                    context.Set<MesRkwPo>().Attach(man);
                    context.Entry<MesRkwPo>(man).State = EntityState.Modified;

                    MessageBox.Show(context.SaveChanges() > 1 ? @"更新成功" : @"更新失败");
                    txtMinusMan.Text = "";
                }
                catch (Exception)
                {

                    MessageBox.Show(@"Error");
                }

            }
        }


        /// <summary>
        /// 查箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQueryBoxSn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {


                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {

                    string sn = txtQueryBoxSn.Text;
                    if (!string.IsNullOrEmpty(sn))
                    {
                        var box = context.MesRkwPackages.FirstOrDefault(c => c.Sn == sn && c.MesRkwBox.BoxStatus);
                        if (box == null)
                        {
                            txtQueryBoxSn.SelectAll();
                            ConsoleWriteToLabelMsg("箱号不存在！");
                        }
                        else
                        {
                            txtQueryBoxSn.ReadOnly = true;
                            txtQueryBoxBoxId.Text = box.BoxId;
                        }

                    }

                }
            }
        }

        /// <summary>
        /// 清空查询结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQueryBoxSn_DoubleClick(object sender, EventArgs e)
        {
            txtQueryBoxSn.ReadOnly = false;
            txtQueryBoxSn.Text = string.Empty;
            txtQueryBoxBoxId.Text = string.Empty;

        }

        #endregion

        #region 重印标签


        private void txtReLabel_KeyDown(object sender, KeyEventArgs e)
        {
            string reLabel = txtReLabel.Text;

            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                if (string.IsNullOrEmpty(reLabel))
                {
                    ConsoleWriteToLabelMsg("先输入信息才可以重印的哦！！！");
                    return;
                }
                using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
                {
                    List<MesRkwPackageView> rkw = new List<MesRkwPackageView>();

                    //确认是箱号还是SN
                    //box

                    List<MesRkwSn> mesRkSn = new List<MesRkwSn>();
                    if (reLabel.StartsWith(DateTime.Now.Year.ToString()))
                    {
                        var sns = context.MesRkwPackages.Where(c => c.BoxId == reLabel && c.MesRkwBox.BoxStatus).Select(k => k.Sn).ToList();
                        mesRkSn = context.MesRkwSns.Where(c => sns.Contains(c.Sn)).ToList();

                        //list.ForEach(ks => mesRkSn.Add(ks.MesRkwSn));
                    }
                    //sn
                    else
                    {
                        mesRkSn.Add(context.MesRkwSns.Find(reLabel));
                    }


                    //整理结果集
                    mesRkSn.ForEach(snModel =>
                    {
                        rkw.Add(new MesRkwPackageView()
                        {
                            Sn = snModel.Sn,
                            ManNo = snModel.MesRkwPo.ManNo,
                            SupplierNo = snModel.MesRkwPo.SupplierNo,
                            Qty = snModel.MesRkwPo.Qty,
                            CusName = snModel.MesRkwPo.CusName,
                            MinicutPoNo = snModel.MesRkwPo.MinicutPoNo,
                            PartNo = snModel.MesRkwPo.PartNo,
                            PartName = snModel.MesRkwPo.PartName,
                            MoNo = snModel.MesRkwPo.MoNo,
                            HeatNo = snModel.MesRkwPo.HeatNo,
                            Supplier = snModel.MesRkwPo.Supplier,
                            RecaroPoNo = snModel.MesRkwPo.RecaroPoNo,
                        });
                    });

                    //显示结果
                    if (rkw.Count > 0)
                    {
                        txtReLabel.ReadOnly = true;
                        btnReLabel.Enabled = true;
                        cbbLabelType.Enabled = true;
                    }
                    else
                    {
                        ConsoleWriteToLabelMsg("此箱号不存在或已被解锁！！！");
                        txtReLabel.Focus();
                        txtReLabel.SelectAll();
                    }

                    dgvSnData.DataSource = rkw;

                }

            }
        }



        private void txtReLabel_DoubleClick(object sender, EventArgs e)
        {
            if (txtReLabel.ReadOnly == true)
            {
                txtReLabel.ReadOnly = false;
                btnReLabel.Enabled = false;
                cbbLabelType.Enabled = false;
                dgvSnData.DataSource = null;
            }
        }

        private void btnReLabel_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                string reType = cbbLabelType.Text;
                var snModel = context.MesRkwSns.Find(dgvSnData.CurrentCell.FormattedValue);
                if (snModel == null)
                {

                    ConsoleWriteToLabelMsg("请选择正确的SN哦！");
                    return;
                }
                if (reType == "SN")
                {
                    List<MesRkwSn> mesRkwSn = new List<MesRkwSn>();
                    mesRkwSn.Add(snModel);
                    cE.PrintSn(mesRkwSn);
                }
                else if (reType == "Box")
                {
                    var mespo = snModel.MesRkwPo;

                    var mesRkwPackage = context.MesRkwPackages.FirstOrDefault(c => c.MesRkwBox.BoxStatus && c.Sn == snModel.Sn);
                    if (mesRkwPackage == null)
                    {
                        ConsoleWriteToLabelMsg("此标签未产生箱号！");
                        return;
                    }


                    var qty = context.MesRkwPackages.Count(c => c.BoxId == mesRkwPackage.MesRkwBox.BoxId).ToString();


                    cE.PrintBoxLabel(mespo, mesRkwPackage.MesRkwBox, qty);

                }

            }
        }














        #endregion



        #region 盘点标签打印



        private void txtCheckWarehouseExcelPath_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = @"Excel文件(*.xls;)|*.xls;",
                ValidateNames = true,
                CheckPathExists = true,
                CheckFileExists = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtCheckWarehouseExcelPath.Text = ofd.FileName;
            }


        }

        private void btnReadExcel_Click(object sender, EventArgs e)
        {
            var path = this.txtCheckWarehouseExcelPath.Text;

            if (!string.IsNullOrEmpty(path))
            {
                DataGridViewCheckBoxColumn newColumn = new DataGridViewCheckBoxColumn { HeaderText = @"Select" };
                dgvCheckWareHouseExcel.Columns.Add(newColumn);
                DataTable dt = ExcelHelper.ExcelImport(path);
                dgvCheckWareHouseExcel.DataSource = dt;
            }
            else
            {
                ConsoleWriteToLabelMsg("请选选择一下可用的excel！");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCheckWareHouseExcel_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex != -1)
            {
                dgvCheckWareHouseExcel.Rows[e.RowIndex].Cells[0].Value =
                (bool)dgvCheckWareHouseExcel.Rows[e.RowIndex].Cells[0].EditedFormattedValue != true;
            }

        }

        private void btnCkWhAllSelect_Click(object sender, EventArgs e)
        {
            if (btnCkWhAllSelect.Text == @"全选")
            {
                for (int i = 0; i < dgvCheckWareHouseExcel.Rows.Count; i++)
                {
                    dgvCheckWareHouseExcel.Rows[i].Cells[0].Value = true;
                }
                btnCkWhAllSelect.Text = @"反选";
            }
            else
            {
                for (int i = 0; i < dgvCheckWareHouseExcel.Rows.Count; i++)
                {
                    dgvCheckWareHouseExcel.Rows[i].Cells[0].Value = (bool)dgvCheckWareHouseExcel.Rows[i].Cells[0].EditedFormattedValue != true;
                }
                btnCkWhAllSelect.Text = @"全选";
            }


        }

        private void btnPrintExcelBySelect_Click(object sender, EventArgs e)
        {
            if (dgvCheckWareHouseExcel.IsCurrentCellDirty)
            {
                dgvCheckWareHouseExcel.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            List<CheckWhare> checkWhares = new List<CheckWhare>();
            //取得选中的行
            for (int i = 0; i < dgvCheckWareHouseExcel.Rows.Count; i++)
            {
                var currVar = dgvCheckWareHouseExcel.Rows[i].Cells[0].Value;
                if (currVar == null) continue;

                bool isSelect;
                bool.TryParse(currVar.ToString(), out isSelect);
                if (isSelect)
                {
                    checkWhares.Add(new CheckWhare()
                    {
                        Location = $"{dgvCheckWareHouseExcel.Rows[i].Cells[4].Value}/{dgvCheckWareHouseExcel.Rows[i].Cells[5].Value}",
                        PartName = dgvCheckWareHouseExcel.Rows[i].Cells[2].Value.ToString(),
                        PartNo = dgvCheckWareHouseExcel.Rows[i].Cells[1].Value.ToString(),
                        Qty = dgvCheckWareHouseExcel.Rows[i].Cells[6].Value.ToString(),
                    });
                }
            }


            ConsoleWriteToLabelMsg("准备打印中....");



            cE.PrintCkWh(checkWhares);


            //整理标签，并打印
        }

        #endregion

        private void btnPrintMode_Click(object sender, EventArgs e)
        {
            btnPrintMode.Text = btnPrintMode.Text == @"单列打印" ? "双列打印" : "单列打印";
        }

        private void btnDSnTry_Click(object sender, EventArgs e)
        {

            Dictionary<string, string> snLabel = new Dictionary<string, string>()
                            {
                                {"PartNo","130-00-280-65+00"},
                                {"Partname","Baggage bar"},
                                {"sn","117201610035"},
                                {"mono","MO1612021361"},

                                {"PartNo1","130-00-180-00+02"},
                                {"Partname1","Beam2"},
                                {"sn1","117201510052"},
                                {"mono1","MO1612050312"},
                            };

            ConsoleWriteToLabelMsg($"打印SN：ok");
            cE.PrintDTryPrint(snLabel);
            //Print(LocalPrint.DefaultPrinter(), Parameters.DbSnLabel, snLabel, 1);
        }

        private void btnCY_Click(object sender, EventArgs e)
        {
            using (var context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper)
            {
                var rkwSns = context.MesRkwSns.Where(c => c.ManNo == "MA11720314").ToList();
                cE.PrintDbSn(rkwSns);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ControlExpress.lbl.Quit();
        }
    }

}
