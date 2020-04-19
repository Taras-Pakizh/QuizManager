
function SetTestItem(inputs, textarea, index, id) {

    inputs[0].setAttribute('name', '_XmlModel.Options[' + index + '].Id')

    inputs[0].setAttribute('value', id);

    inputs[1].setAttribute('name', '_XmlModel.Options[' + index + '].Text');

    inputs[2].setAttribute('value', id);

    textarea.setAttribute('name', '_XmlModel.Options[' + index + '].Comment');
}

$(document).ready(function () {

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
    });

    $(document).on('click', 'a.removeLink', function () {
        
        $(this).closest('.removeContainer').remove();

        var id = 1; var index = 0;

        var options = document.getElementById('options').children;

        if (options.length > 0) {

            for (op of options) {
                var inputs = op.getElementsByTagName('input');

                var textarea = option.getElementsByTagName('textarea')[0];

                SetTestItem(inputs, textarea, index, id);
                
                ++id; ++index;
            }
        }
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
