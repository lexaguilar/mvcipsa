(function ($) {
    $.fn.loadCatalog = function (settings) {
        var options = $.extend({
            verb: 'get',
            render: null,
            extraParams: {},
            defaultValue: null,
            failCallback: null ,          
        }, settings);

        var that = this;

        return new Promise(resolve => {
            $(that).each(function () {
                var select = $(this);
                
                var $selectize = findSelectize($(select).attr('id'));

                var properties = $(select).getProperties();

                var EjecuteProcess = () => {
                    $(select).loadingShow(true);                   

                    var callback = result => {
                        fillSelect($selectize, result, properties.render, options.defaultValue);
                        $(select).loadingShow(false);
                        resolve(result);                       
                    }

                    var parameters = buildParamters(properties.data);
                    var url = findUrl(properties, options.extraParams);

                    if (url)
                        context.ajax[options.verb](url, parameters, callback, options.failCallback);
                    else
                        $(select).loadingShow(false);
                }

                EjecuteProcess();

                // var hasOptions = hasData($selectize[0].selectize);

                // if (hasOptions && properties.reload) {
                //     EjecuteProcess();
                // } else if (!hasOptions) {
                //     EjecuteProcess();
                // }
            });
        });
    }

    $.fn.getProperties = function () {
        return context.dataConfig[$(this).attr('id')]();        
    }

    
    $.fn.hasValue = function () {
        if ($(this).attr('has-value'))
            return true;
        else
            return false;
    }

    $.fn.clearCatalog = function (settings) {
        var options = $.extend({
            verb: 'get',
            render: null,
            extraParams: {},
            defaultValue: null
        }, settings);

        return $(this).each(function () {
            var select = $(this);           

            var $selectize = findSelectize($(select).attr('id'));           
       
            $selectize[0].selectize.clearOptions();
        });
    }

    $.fn.loadingShow = function (action) {
        $(this).each(function () {
            var select = $(this);
            if (action)
                $(select).parent()
                    .children().eq(2)
                    .children()
                    .append(' <span> <i class="fa fa-spinner fa-pulse sg-color-blue"></i></span>');
            else
                $(select).parent()
                    .children().eq(2).find('span')
                    .remove()
        });
    }

    $.fn.setValueWithAttr = function (newValue) {
        var item = $(this);
        $(item).attr('has-value', newValue);    
        
        

        var SetValueByTag = {           
            INPUT: () => $(item).val(parseFloat(newValue).toFixed(2)).trigger('input'),        
            TD: () => {    
                var formatValue = Formatear(newValue);      
                $(item).text(formatValue);
            }
        } 

        var tagName = $(item).prop("tagName");

        if(SetValueByTag.hasOwnProperty(tagName))
            return SetValueByTag[tagName]();
        else
            TypeError (`El tagName ${tagName} no pertenece al objeto SetValueByTag`);
    }    

    $.fn.getValueWithAttr = function () {
        return $(this).attr('has-value');
    }
}(jQuery));

Selectize.define('infinite_scroll', function (options) {
    var self = this
        , page = 1;

    self.infinitescroll = {
        onScroll: function () {
            var scrollBottom = self.$dropdown_content[0].scrollHeight - (self.$dropdown_content.scrollTop() + self.$dropdown_content.height())
            if (scrollBottom < 300) {
                var query = JSON.stringify({
                    search: self.lastValue,
                    page: page
                })

                self.$dropdown_content.off('scroll')
                self.onSearchChange(query)
            }
        }
    };

    self.onFocus = (function () {
        var original = self.onFocus;

        return function () {
            var query = JSON.stringify({
                search: self.lastValue,
                page: page
            })

            original.apply(self, arguments);
            self.onSearchChange(query)
        };
    })();

    self.onKeyUp = function (e) {
        var self = this;

        if (self.isLocked) return e && e.preventDefault();
        var value = self.$control_input.val() || '';

        if (self.lastValue !== value) {
            var query = JSON.stringify({
                search: value,
                page: page = 1
            });

            self.lastValue = value;
            self.onSearchChange(query);
            self.refreshOptions();
            self.clearOptions();
            self.trigger('type', value);
        }
    };

    self.on('load', function () {
        page++
        self.$dropdown_content.on('scroll', self.infinitescroll.onScroll);
    });

});

Array.prototype.unique = function () {
    return this.filter(function (value, index, self) {
        return self.indexOf(value) === index;
    });
}


String.prototype.pad = function(size) {
    var s = String(this);
    while (s.length < (size || 2)) {s = "0" + s;}
    return s;
  }