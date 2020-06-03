
function SetTestItem(inputs, textarea, index, id) {

    inputs[0].setAttribute('name', '_XmlModel.Options[' + index + '].Id')

    inputs[0].setAttribute('value', id);

    inputs[1].setAttribute('name', '_XmlModel.Options[' + index + '].Text');

    if (inputs.length > 2) {
        inputs[2].setAttribute('value', id);
    }
    
    textarea.setAttribute('name', '_XmlModel.Options[' + index + '].Comment');
}

function SetRowItem(inputs, textarea, index) {

    inputs[0].setAttribute('name', '_XmlModel.Rows[' + index + '].Id')

    //inputs[0].setAttribute('value', id);

    inputs[1].setAttribute('name', '_XmlModel.Rows[' + index + '].Text');

    textarea.setAttribute('name', '_XmlModel.Rows[' + index + '].Comment');
}

function SetColumnItem(inputs, index) {

    inputs[0].setAttribute('name', '_XmlModel.Columns[' + index + '].Id')

    //inputs[0].setAttribute('value', id);

    inputs[1].setAttribute('name', '_XmlModel.Columns[' + index + '].Text');
}

function GetBoxSample(row, col) {
    var box = document.getElementById('boxSample').cloneNode(true);
    box.setAttribute('id', row + " " + col);
    box.getElementsByTagName('input')[0].setAttribute('value', row + " " + col);
    return box;
}

function GetColumnSample() {
    var option = document.getElementById('columnMatching').cloneNode(true);
    option.setAttribute('id', "");
    var div = option.getElementsByClassName('toremove')[0];
    if (div == undefined) {
        return option;
    }
    div.classList.remove('toremove');
    div.classList.add('columns_container');
    return option;
}

function GetRowSample() {
    var option = document.getElementById('rowMatching').cloneNode(true);
    option.setAttribute('id', "");
    option.classList.remove('toremove');
    option.classList.add('rowOption');
    return option;
}

function RewriteRowIndexes() {
    var index = 0;

    if (document.getElementById('rowsMatching') == null)
        return;

    var options = document.getElementById('rowsMatching').children;

    if (options.length > 0) {

        for (op of options) {
            var _inputs = op.getElementsByTagName('input');

            var _textarea = op.getElementsByTagName('textarea')[0];

            SetRowItem(_inputs, _textarea, index);

            ++index;
        }
    }

    return index;
}

function RewriteColumnIndexes() {
    var index = 0;

    if (document.getElementById('columnsMatching') == null)
        return;

    var options = document.getElementById('columnsMatching').children;

    if (options.length > 0) {

        for (op of options) {
            var _inputs = op.getElementsByTagName('input');

            SetColumnItem(_inputs, index);

            ++index;
        }
    }

    return index;
}

function RewriteOrderIndexes() {
    var id = 1; var index = 0;

    if (document.getElementById('options') == null) {
        return;
    }

    var options = document.getElementById('options').children;

    if (options.length > 0) {

        for (op of options) {
            var inputs = op.getElementsByTagName('input');

            var textarea = op.getElementsByTagName('textarea')[0];

            SetTestItem(inputs, textarea, index, id);

            ++id; ++index;
        }
    }

    return index;
}

function SetDragable() {
    $("#options").sortable();
    $("#options").disableSelection();

    $(".test").sortable();
    $(".test").disableSelection();

    $(".testSortable").sortable();
    $(".testSortable").disableSelection();

    $("#list").sortable();
    $("#list").disableSelection();
}

$(document).bind('DOMSubtreeModified', function () {
    RewriteRowIndexes();
    RewriteColumnIndexes();
    RewriteOrderIndexes();
    
    SetDragable();
});

var _RowId = 1;
var _ColId = 1;

$(document).ready(function () {

    $(document).on('click', 'button.btn-empty', function () {

        var position = $('#myarea').prop("selectionStart");

        var myarea = document.getElementById('myarea');

        var text = myarea.value;

        if (text == null) {
            text = "";
        }

        var output = [text.slice(0, position), '(...)', text.slice(position)].join(' ');

        myarea.value = output;
    });

    $(document).on('click', 'button.btn-answers', function () {

        //set primaries
        var buttons = $('.btn-answers');
        for (item of buttons) {
            item.classList.remove("btn-primary");
            item.classList.add("btn-default");
        }
        $(this)[0].classList.remove("btn-default");
        $(this)[0].classList.add("btn-primary");

        var rowContainer = $(this).closest('.removeContainer')[0];

        var rowId = rowContainer.getElementsByTagName('input')[0].getAttribute('value');

        //set hiddens
        var containers = $('.columns_container');
        for (container of containers) {
            //col Id
            var li = $(container).closest('.removeContainer')[0];
            var colId = li.getElementsByTagName('input')[0].getAttribute('value');
            //set all to hidden
            for (hidden of container.children) {
                hidden.classList.add("hidden");
            }
            //set selected to visible
            var boxId = rowId + " " + colId;
            document.getElementById(boxId).classList.remove("hidden");
        }

    });
    $(document).on('click', 'a.addRow', function () {
        var index = RewriteRowIndexes();

        var option = GetRowSample();

        var inputs = option.getElementsByTagName('input');

        var textarea = option.getElementsByTagName('textarea')[0];

        SetRowItem(inputs, textarea, index);

        inputs[0].setAttribute('value', _RowId);

        option.setAttribute("id", "");

        $('#rowsMatching').append($(option));

        //add boxes
        var containers = $('.columns_container');
        for (container of containers) {
            var li = $(container).closest('.removeContainer')[0];
            var colId = li.getElementsByTagName('input')[0].getAttribute('value');

            var box = GetBoxSample(_RowId, colId);

            $(container).append($(box));
        }

        SetDragable();

        ++_RowId;
    });
    $(document).on('click', 'a.addColumn', function () {
        var index = RewriteColumnIndexes();

        var option = GetColumnSample();

        var inputs = option.getElementsByTagName('input');

        SetColumnItem(inputs, index);

        inputs[0].setAttribute('value', _ColId);

        //add boxes
        var containerHiddens = option.getElementsByClassName('columns_container')[0];
        var rows = document.getElementsByClassName("rowOption");
        for (row of rows) {
            var rowId = row.getElementsByTagName('input')[0].getAttribute('value');

            var box = GetBoxSample(rowId, _ColId);

            $(containerHiddens).append($(box));
        }

        option.setAttribute('id', "");

        $('#columnsMatching').append($(option));

        SetDragable();

        ++_ColId;
    });
    $(document).on('click', 'a.removeRowMatching', function () {

        var row = $(this).closest('.removeContainer')[0];
        var rowId = row.getElementsByTagName('input')[0].getAttribute('value');

        var containers = $('.columns_container');
        for (container of containers) {

            for (hidden of container.children) {

                var hiddenId = hidden.getAttribute('id');

                var currectRowId = hiddenId.split(" ")[0];

                if (rowId == currectRowId) {
                    hidden.remove();
                }
            }
        }

        $(this).closest('.removeContainer').remove();

        RewriteRowIndexes();

        SetDragable();
    });
    $(document).on('click', 'a.removeColumnMatching', function () {
        $(this).closest('.removeContainer').remove();

        RewriteColumnIndexes();

        SetDragable();
    });

    $(document).on('click', 'a.addLink', function () {

        var id = 1; var index = 0;

        var options = document.getElementById('options').children;

        if (options.length > 0) {

            for (op of options) {
                var _inputs = op.getElementsByTagName('input');

                var _textarea = op.getElementsByTagName('textarea')[0];

                SetTestItem(_inputs, _textarea, index, id);

                ++id; ++index;
            }
        }

        var option = document.getElementById('option').cloneNode(true);

        var inputs = option.getElementsByTagName('input');

        var textarea = option.getElementsByTagName('textarea')[0];

        SetTestItem(inputs, textarea, index, id);

        $('#options').append($(option));

        SetDragable();
    });
    $(document).on('click', 'a.removeLink', function () {
        
        $(this).closest('.removeContainer').remove();

        var id = 1; var index = 0;

        var options = document.getElementById('options').children;

        if (options.length > 0) {

            for (op of options) {
                var inputs = op.getElementsByTagName('input');

                var textarea = op.getElementsByTagName('textarea')[0];

                SetTestItem(inputs, textarea, index, id);
                
                ++id; ++index;
            }
        }

        SetDragable();
    });

    $(document).on('click', 'input.addquestion', function () {

        var questions = $('.question');

        for (item of questions) {
            item.classList.remove("btn-primary");
            item.classList.add("btn-default");
        }

        var newMax = 1;

        if (questions.length != 0) {
            var max = questions[questions.length - 1].getAttribute('value');

            newMax = parseInt(max) + 1;
        }

        var button = '<input name="value" type="submit" value="' + newMax + '" class="btn btn-primary btn-squared-small question choose" />';

        $('#qGroup').append($(button));

    });
    $(document).on('click', 'input.choose', function () {

        var questions = $('.question');

        for (item of questions) {
            item.classList.remove("btn-primary");
            item.classList.add("btn-default");
        }

        $(this)[0].classList.remove("btn-default");
        $(this)[0].classList.add("btn-primary");

    });

    var IsMoved = true;
    $(document).on('click', 'li.asection', function () {

        if (IsMoved) {
            var list = document.getElementsByClassName("buttons");

            for (item of list) {
                item.classList.add("hidden");
            }

            var div = $(this)[0].getElementsByClassName('buttons');
            for (item of div) {
                item.classList.remove("hidden");
            }  
        }
        IsMoved = true;
    });
    $(document).on('click', 'button.sectionUp', function () {

        var list = document.getElementById('list');
        var aList = list.getElementsByTagName('li');

        var my = $(this).closest('.asection')[0];

        var index = 0;
        for (item of aList) {
            if (item === my) {
                break;
            }
            index++;
        }

        if (index != 0) {
            var div = document.getElementById('saveButton');
            div.classList.remove("hidden");

            var temp = aList[index - 1].innerHTML;
            aList[index - 1].innerHTML = aList[index].innerHTML;
            aList[index].innerHTML = temp;

            IsMoved = false;
            var list = document.getElementsByClassName("buttons");
            for (item of list) {
                item.classList.add("hidden");
            }
            var buttons = aList[index - 1].getElementsByClassName('buttons');
            for (item of buttons) {
                item.classList.remove("hidden");
            }
        }
    });
    $(document).on('click', 'button.sectionDown', function () {

        var list = document.getElementById('list');
        var aList = list.getElementsByTagName('li');

        var my = $(this).closest('.asection')[0];

        var index = 0;
        for (item of aList) {
            if (item === my) {
                break;
            }
            index++;
        }

        if (index != aList.length) {
            var div = document.getElementById('saveButton');
            div.classList.remove("hidden");

            var temp = aList[index + 1].innerHTML;
            aList[index + 1].innerHTML = aList[index].innerHTML;
            aList[index].innerHTML = temp;

            IsMoved = false;
            var list = document.getElementsByClassName("buttons");
            for (item of list) {
                item.classList.add("hidden");
            }
            var buttons = aList[index + 1].getElementsByClassName('buttons');
            for (item of buttons) {
                item.classList.remove("hidden");
            }
        }
    });

});
