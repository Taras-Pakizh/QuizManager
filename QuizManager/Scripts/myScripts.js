
$(document).ready(function () {

    $(document).on('click', 'a.addLink', function () {

        var id = 1; var index = 0;

        var options = document.getElementById('options').children;

        if (options.length > 0) {

            for (op of options) {
                var _inputs = op.getElementsByTagName('input');

                _inputs[0].setAttribute('name', '_XmlModel.Options[' + index + '].Id')

                _inputs[0].setAttribute('value', id);

                _inputs[1].setAttribute('name', '_XmlModel.Options[' + index + '].Text');

                if (_inputs.length == 3) {
                    _inputs[2].setAttribute('value', id);
                }

                ++id; ++index;
            }
        }

        var option = document.getElementById('option').cloneNode(true);

        var inputs = option.getElementsByTagName('input');

        inputs[0].setAttribute('name', '_XmlModel.Options[' + index + '].Id')

        inputs[0].setAttribute('value', id);

        inputs[1].setAttribute('name', '_XmlModel.Options[' + index + '].Text');

        if (inputs.length == 3) {
            inputs[2].setAttribute('value', id);
        }

        $('#options').append($(option));
    });

    $(document).on('click', 'a.removeLink', function () {
        
        $(this).closest('.removeContainer').remove();

        var id = 1; var index = 0;

        var options = document.getElementById('options').children;

        if (options.length > 0) {

            for (op of options) {
                var _inputs = op.getElementsByTagName('input');

                _inputs[0].setAttribute('name', '_XmlModel.Options[' + index + '].Id')

                _inputs[0].setAttribute('value', id);

                _inputs[1].setAttribute('name', '_XmlModel.Options[' + index + '].Text');

                if (_inputs.length == 3) {
                    _inputs[2].setAttribute('value', id);
                }

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

        var max = questions[questions.length - 1].getAttribute('value');

        var newMax = parseInt(max) + 1;

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

});
