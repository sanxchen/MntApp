var acegridFormatter = {};
(function () {
    acegridFormatter.deleteButton = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            primaryKeyName: 'ID'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        var html = [];
        if (aceOpts.allowDelete) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'onclick="acegridFn.delete(\'' + rowData[primaryKeyName] + '\',\'' + options.url + '\',this)" ' +
                  'id="jDeleteButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" title="删除行" ' +
                  '><span class="fa fa-lg fa-trash-o">' +
                  '</span></a>');
        }
        return html.join('');
    }

    acegridFormatter.closeVolnteerButton = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            primaryKeyName: 'ID'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        var html = [];
        if (aceOpts.allowDelete) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'onclick="acegridFn.delete(\'' + rowData[primaryKeyName] + '\',\'' + options.url + '\',this)" ' +
                  'id="jDeleteButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" title="关闭志愿者" ' +
                  '><span class="fa fa-lg fa-trash-o">' +
                  '</span></a>');
        }
        return html.join('');
    }

    acegridFormatter.execlButton = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            primaryKeyName: 'ID'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        var html = [];
        if (aceOpts.allowExecl) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'onclick="acegridFn.execl(\'' + rowData[primaryKeyName] + '\',\'' + options.url + '\',this)" ' +
                  'id="jexeclButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" ' + (options.tooltip ? ('title="' + options.tooltip + '"') : '') +
                  '><span class="fa fa-lg fa-' + (options.icon ? options.icon : '') + '">' +
                  ' </span></a>');





        }
        return html.join('');
    }
    /**
    * 格式化单元格信息，返回一个"编辑窗口的按钮"
    * 选项参数：
    * url: 窗口的URL路径
    * primaryKeyName:默认'ID',用来绑定action的动作的，如 /Detail/ID ,这个"ID"就是它的属性值
    *
    */
    acegridFormatter.editButton = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            primaryKeyName: 'ID'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        var html = [];
        if (aceOpts.allowEdit) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'onclick="acegridFn.edit(\'' + rowData[primaryKeyName] + '\',\'' + options.url + '\')" ' +
                  'id="jEditButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" title="编辑" ' +
                  '><span class="fa fa-lg fa-pencil">' +
                  '</span></a>');
        }

        return html.join('');
    }

    acegridFormatter.sendBackButton = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            action: '',
            primaryKeyName: 'ID',
            confirm: '您确认要执行【退回】操作吗？'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        if (!options.url) {
            options.url = options.action;
        }
        var html = [];
        if (aceOpts.allowEdit) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'onclick="acegridFn.sendback(\'' + rowData[primaryKeyName] + '\',\'' + options.url + '\',\'' + options.confirm + '\',  this)" ' +
                  'id="jEditButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" title="' + '退回' + '" ' +
                  '><span class="fa fa-lg fa-reply">' +
                  '</span></a>');
        }


        return html.join('');

    }
    /**
    * 格式化单元格信息，返回一个"编辑链接"
    * 选项参数：
    * url: 跳转的action的url路径
    * primaryKeyName:默认'ID',用来绑定action的动作的，如 /Detail/ID ,这个"ID"就是它的属性值
    *
    */
    acegridFormatter.editLink = function (i, row, rowData, options) {
        var aceOpts = $('table.acegrid').acegrid('getOptions');
        options = $.extend({}, {
            url: '',
            primaryKeyName: 'ID'
        }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        var html = [];
        if (aceOpts.allowEdit) {
            html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  'href="' + options.url + rowData[primaryKeyName] + '"' +
                  'id="jEditButton_' + row.rowId + '" data-rel="tooltip" class=" ui-pg-div ui-inline-del" ' +
                  'style="float:left;margin-left:5px;" title="编辑" ' +
                  '><span class="fa fa-lg fa-pencil">' +
                  '</span></a>');
        }
        return html.join('');
    }


    acegridFormatter.formatDate = function (i, row, rowData, format) {
        if (typeof format !== 'string') {
            format = "yyyy-MM-dd";
        }
    }
    acegridFormatter.groupAction = function (i, row, rowData, options) {
        options = $.extend({}, { deleteUrl: '', editLink: '', editButton: '', detail: '', primaryKeyName: 'ID' }, options);
        var html = [];
        var aceOptions = $('table.acegrid').acegrid('getOptions');
        if (typeof options.editLink == 'string' && options.editLink.length > 0) {
            html.push(acegridFormatter.editLink(i, row, rowData, { primaryKeyName: options.primaryKeyName, url: options.editLink }));
        }
        if (typeof options.editButton == 'string' && options.editButton.length > 0) {
            html.push(acegridFormatter.editButton(i, row, rowData, { primaryKeyName: options.primaryKeyName, url: options.editButton }));
        }
        if (typeof options.deleteUrl == 'string' && options.deleteUrl.length > 0) {
            html.push(acegridFormatter.deleteButton(i, row, rowData, { primaryKeyName: options.primaryKeyName, url: options.deleteUrl }));
        }
        if (typeof options.detail == 'string' && options.detail.length > 0 && aceOptions.allowViewDetail) {
            html.push(acegridFormatter.customerButton(i, row, rowData, { primaryKeyName: options.primaryKeyName, href: options.detail, text: '详情', tooltip: "查看详情", icon: 'File' }));
        }

        return html.join('');

    }

    /**
    * 自定义按钮
    * 选项参数：
    * icon: 按钮的图标
    * text: 按钮名称，如果使用tooltip就不推荐使用此属性
    * tooltip:当鼠标移动到按钮上时 显示的提示文字，推荐使用
    * primaryKeyName: 默认'ID',用来绑定action的动作的，如 /Detail/ID ,这个"ID"就是它的属性值
    * href: 超链接，也可以写成 javascript:js脚本 
    * onclick:按钮的onclick执行的动作,生成的是这样的 <a onclick="onclick动作"></a>,而不是绑定的function类型
    * action: mvc的action的名称,例如要请求 /Home/Delete/10  这个action,只需要传"/Home/Delete" 就可以，ID是根据primaryKeyName自动传过去
    */
    acegridFormatter.customerButton = function (i, row, rowData, options) {
        options = $.extend({},
                    {
                        icon: undefined,
                        text: undefined,
                        tooltip: undefined,
                        primaryKeyName: 'ID',
                        href: undefined,
                        onclick: undefined,
                        target: undefined,
                        action: '',
                        confirmMsg: undefined,
                        iconColor: undefined,
                        attrs: {}  //特性列表，可以在 按钮上添加的功能 
                    }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        options.icon = options.icon ? options.icon : '';
        options.text = options.text ? options.text : '';
        options.tooltip = options.tooltip ? options.tooltip : '';
        options.href = options.href ? options.href : 'javascript:;';
        options.onclick = options.onclick ? options.onclick : '';
        options.target = options.target ? options.target : '_self';
        var html = [];
        var attrs = [];
        if (typeof options.attrs == 'object') {
            for (var name in options.attrs) {
                attrs.push(name + '="' + options.attrs[name] + '"');
            }
        }
        var onclick = (options.action ? (' onclick="acegridFn.action(\'' + rowData[primaryKeyName] + '\',\'' + options.action + '\',' + (options.confirmMsg ? ('\'' + options.confirmMsg + '\'') : "''") + ',this)" ')
                            : (options.onclick ? ' onclick="' + options.onclick + '"' : ''));
        html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  ' href="' + (options.href) + '" ' +
                  onclick +
                  ' id="jActionButton_' + options.action + '_' + row.rowId + '" ' + (options.tooltip ? 'data-rel="tooltip"' : '') +
                  ' class=" ui-pg-div ui-inline-del" ' +
                  '' + attrs.join(' ') +
                  ' style="float:left;margin-left:5px;text-decoration:none;" ' + (options.tooltip ? ('title="' + options.tooltip + '"') : '') + 'target="' + options.target + '"' +
                  ' ><span class="fa fa-lg fa-' + (options.icon ? options.icon : '') + ' ' + (options.iconColor ? options.iconColor : '') + '">' +
                  ' </span>' + (options.text) + '</a>');
        return html.join('');

    }

    acegridFormatter.titleWithImage = function (i, row, rowData, options) {
        options = $.extend({
            titleField: 'Title',
            thumbField: 'TitleImagePath'
        });
        var html = rowData[options.titleField];
        var thumb = rowData[options.thumbField];
        if (thumb) {
            var img = '<img src=\'' + thumb + '\' alt=\'标题图片\' class=\'online\' style=\'max-width:100px;max-height:100px;\'>';
            html += '<a href="javascript:;" style="margin-left:10px;" data-rel="tooltip" data-placement="top" data-original-title="' + img + '" data-html="true"><i class="fa fa-picture-o"></i></a>';
        }
        return html;
    }

    acegridFormatter.customerButton1 = function (i, row, rowData, options) {
        options = $.extend({},
                    {
                        icon: undefined,
                        text: undefined,
                        tooltip: undefined,
                        primaryKeyName: 'ID',
                        href: undefined,
                        onclick: undefined,
                        action: '',
                        confirmMsg: undefined,
                        iconColor: undefined,
                        attrs: {}  //特性列表，可以在 按钮上添加的功能 
                    }, options);
        primaryKeyName = formatPrimaryKeyName(options.primaryKeyName);
        options.icon = options.icon ? options.icon : '';
        options.text = options.text ? options.text : '';
        options.tooltip = options.tooltip ? options.tooltip : '';
        options.href = options.href ? options.href : 'javascript:;';
        options.onclick = options.onclick ? options.onclick : '';
        var html = [];
        var attrs = [];
        if (typeof options.attrs == 'object') {
            for (var name in options.attrs) {
                attrs.push(name + '="' + options.attrs[name] + '"');
            }
        }
        var onclick = (options.action ? (' onclick="acegridFn.action1(\'' + rowData[primaryKeyName] + '\',\'' + options.action + '\',' + (options.confirmMsg ? ('\'' + options.confirmMsg + '\'') : "''") + ',this)" ')
                            : (options.onclick ? ' onclick="' + options.onclick + '"' : ''));
        html.push('<a onmouseout="jQuery(this).removeClass(\'ui-state-hover\');" ' +
                  'onmouseover="jQuery(this).addClass(\'ui-state-hover\');" ' +
                  ' href="' + (options.href) + '" ' +
                  onclick +
                  ' id="jActionButton_' + options.action + '_' + row.rowId + '" ' + (options.tooltip ? 'data-rel="tooltip"' : '') +
                  ' class=" ui-pg-div ui-inline-del" ' +
                  '' + attrs.join(' ') +
                  ' style="float:left;margin-left:5px;text-decoration:none;" ' + (options.tooltip ? ('title="' + options.tooltip + '"') : '') +
                  ' ><span class="fa fa-lg fa-' + (options.icon ? options.icon : '') + ' ' + (options.iconColor ? options.iconColor : '') + '">' +
                  ' </span>' + (options.text) + '</a>');
        return html.join('');

    }



    function formatPrimaryKeyName(keyName) {
        if (typeof keyName !== 'string') {
            keyName = 'ID';
        }
        return keyName;
    }
})();